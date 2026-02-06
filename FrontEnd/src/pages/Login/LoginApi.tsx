import { authApi } from "@/core/api/Client";

export async function loginApi(username: string, password: string) {
    const res = await authApi.post('api/Auth/login', {username, password}, { withCredentials: true });

    return res.data;
}

export async function logoutApi() {
    try {
        await authApi.post('api/Auth/logout', {}, { withCredentials: true });
    }
    catch (error) {
        console.error("Error during logout:", error);
    }

}