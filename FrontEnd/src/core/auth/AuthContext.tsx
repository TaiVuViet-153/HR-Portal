import type { AuthAction, AuthContextValue, AuthState, LoginPayload } from "@/core/auth/AuthType";
import { createContext, useContext, useEffect, useMemo, useReducer, useRef } from "react";
import { authApi, requestApi } from "@/core/api/Client";
import { setupInterceptors } from "@/core/api/SetupInterceptors";
import Loading from "@/components/common/Loading";

const initialState: AuthState = {
    accessToken: null,
    user: undefined,
    isAuthenticated: false,
    isInitializing: true
};

function authReducer(state: AuthState, action: AuthAction) : AuthState {
    switch (action.type) {
        case "LOGIN":
            return {
                ...state,
                accessToken: action.payload.accessToken,
                user: action.payload.user,
                isAuthenticated: true,
                isInitializing: false
            };
        case "SET_ACCESS_TOKEN":
            return {
                ...state,
                accessToken: action.payload.accessToken,
                user: action.payload.user,
                isAuthenticated: !!action.payload.accessToken,
                isInitializing: false
            };
        case "LOGOUT":
            return {
                ...initialState,
                isInitializing: false
            }
        case "FINISH_INIT":
            return {
                ...state,
                isInitializing: false
            }
        default:
            return state;
    };
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function useAuth() {
    const ctx = useContext(AuthContext);
    if(!ctx) throw new Error("useAuth must be used within AuthProvider")
    
    return ctx;
}

export function AuthProvider({ children }: React.PropsWithChildren) {
    const [state, dispatch] = useReducer(authReducer, initialState);
    const refreshInProgress = useRef(false);
    const refreshPromise = useRef<Promise<LoginPayload | null> | null>(null);
    const accessTokenRef = useRef<string | null>(state.accessToken);

    // keep ref in sync with state so interceptors can always read latest token without re-registering
    useEffect(() => { accessTokenRef.current = state.accessToken; }, [state.accessToken]);

    const getAccessToken = () => accessTokenRef.current;

    const onRefreshFail = () => {
        dispatch({ type: 'LOGOUT' });
    };

    const requestRefresh = () => {
        if (refreshInProgress.current && refreshPromise.current) {
            return refreshPromise.current;
        }

        refreshInProgress.current = true;
        const p = (async () => {
            try {
                const res = await authApi.post('Auth/refresh', undefined, { withCredentials: true });
                const data: LoginPayload = res.data;
                
                return data;
            } catch (err) {
                // console.debug('[Auth] refresh failed', err);
                onRefreshFail();
                return null;
            } finally {
                refreshInProgress.current = false;
                refreshPromise.current = null;
            }
        })();

        refreshPromise.current = p;
        return p;
    };

    // register interceptors once on mount; they will call getAccessToken and requestRefresh when needed
    useEffect(() => {
        const cleanUp1 = setupInterceptors(authApi, getAccessToken, requestRefresh, onRefreshFail);
        const cleanUp2 = setupInterceptors(requestApi, getAccessToken, requestRefresh, onRefreshFail);

        return () => {
            cleanUp1();
            cleanUp2();
        };
    }, []);

    // Silent refresh: attempt on mount so a hard reload (F5) will restore session
    useEffect(() => {
        // use the shared requestRefresh so callers can await the same promise
        requestRefresh().then((data) => {
            if(data === null)
                dispatch({ type: 'FINISH_INIT' });
            else
                dispatch({ type: 'SET_ACCESS_TOKEN', payload: data});
        }).catch(() => {
            dispatch({ type: 'FINISH_INIT' });
        });
    }, []);

    const value = useMemo<AuthContextValue>(() => ({
        state,
        login: (payload) => {
            dispatch({ type: "LOGIN", payload});
        },
        logout: () => dispatch({ type: "LOGOUT" }),
        setAccessToken: (data) => dispatch({ type: "SET_ACCESS_TOKEN", payload: data })
    }), [state]);

    return (
        <AuthContext.Provider value={value}>
            {state.isInitializing ? (
                <Loading />
            ) : (
                children
            )}
        </AuthContext.Provider>
    );
};

