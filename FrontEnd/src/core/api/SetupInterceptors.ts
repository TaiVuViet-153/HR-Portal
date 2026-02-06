import {type AxiosInstance, type InternalAxiosRequestConfig } from "axios";
import type { LoginPayload } from "@/core/auth/AuthType";

export const setupInterceptors = (
    api: AxiosInstance,
    getAccessToken: () => string | null,
    requestRefresh: () => Promise<LoginPayload | null>,
    onRefreshFail: () => void
) => {
    
    // Request: needed Authorization with token
    const requestInterceptor = api.interceptors.request.use(async (config: InternalAxiosRequestConfig) => {
        const accessToken = getAccessToken();
        
        // If we don't have a token, attempt to refresh before proceeding (but skip if this is the refresh endpoint)
        const refreshEndpoint = '/api/Auth/refresh';
        
        if (!accessToken && config.url && !config.url.includes(refreshEndpoint)) {
            try {
                const newToken = await requestRefresh();
                if (newToken) {
                    config.headers = config.headers || {};
                    config.headers.Authorization = `Bearer ${newToken.accessToken}`;
                }
            } catch (err) {
                // let the request continue and fail — response interceptor will handle
            }
        } else if (accessToken) {
            config.headers = config.headers || {};
            config.headers.Authorization = `Bearer ${accessToken}`;
        }

        return config;
    });

    // Response: handle 401 to refresh token
    let isRefreshing = false;
    const failedQueue: Array<{
        resolve: (value?: any) => void;
        reject: (error: any) => void;
        config: InternalAxiosRequestConfig;
    }> = [];

    const processQueue = (error: any, token: string | null) => {
        failedQueue.forEach(({ resolve, reject, config }) => {
            if (error) {
                reject(error);
            } else {
                if (token) config.headers.Authorization = `Bearer ${token}`;
                resolve(api(config));
            }
        });
        failedQueue.length = 0;
    };

    const debug = (msg: string, ...args: any[]) => {
        try { console.debug('[Interceptor]', msg, ...args); } catch {}
    }

    const responseInterceptor = api.interceptors.response.use(
        (res) => res,
        async (error) => {
            const originalRequest = error.config as any;

            const refreshEndpoint = '/api/Auth/refresh';
            if (originalRequest?.url?.includes && originalRequest.url.includes(refreshEndpoint)) {
                onRefreshFail();
                return Promise.reject(error);
            }

            if (error.response?.status === 401 && !originalRequest._retry) {
                originalRequest._retry = true;

                if (isRefreshing) {
                    debug('queuing request while refreshing', originalRequest.url);
                    return new Promise((resolve, reject) => {
                        failedQueue.push({ resolve, reject, config: originalRequest });
                    });
                }

                isRefreshing = true;
                try {
                    const res = await requestRefresh();
                    const newToken = res ? res.accessToken : null;
                    isRefreshing = false;
                    debug('refresh finished, processing queue with token length', newToken ? newToken.length : 0);
                    processQueue(null, newToken);
                    if (newToken) {
                        originalRequest.headers.Authorization = `Bearer ${newToken}`;
                    }
                    return api(originalRequest);
                } catch (err) {
                    isRefreshing = false;
                    processQueue(err, null);
                    onRefreshFail();
                    return Promise.reject(err || error);
                }
            }

            return Promise.reject(error);
    });

    return () => {
        api.interceptors.request.eject(requestInterceptor);
        api.interceptors.response.eject(responseInterceptor);
    }
}