import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from '@/core/auth/AuthContext';
import Loading from "@/components/common/Loading";

export function RequireAuth() {
    const { state } = useAuth();

    if (state.isInitializing) {
        return <Loading />;
    }

    if (!state.isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    return <Outlet />;
}
