import axios from "axios";
import { authApi } from "@/core/api/Client";

export async function loginApi(username: string, password: string) {
    try {
        const res = await authApi.post('Auth/login', {username, password}, { withCredentials: true });
        console.log("Login response:", res.data);
        return res.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            // Lấy error message từ response body của BadRequest
            const errorMessage = error.response?.data || error.message;
            console.error("Login error:", {
                status: error.response?.status,
                message: errorMessage
            });
            // Throw error với message từ backend
            throw new Error(errorMessage);
        }
        console.error("Error during login:", error);
        throw error;
    }
}

export async function logoutApi() {
    try {
        await authApi.post('Auth/logout', {}, { withCredentials: true });
    }
    catch (error) {
        console.error("Error during logout:", error);
    }

}