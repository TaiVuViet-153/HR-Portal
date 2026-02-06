import { Navigate } from "react-router-dom";
import { useAuth } from "@/core/auth/AuthContext";
import type { UserRoleValue } from "@/core/auth/AuthType";

type RequireRoleProps = {
    roles: readonly UserRoleValue[];
    children: React.ReactNode;
};

function hasAccess(userRoles: string | string[] | undefined, allowedRoles: readonly UserRoleValue[]): boolean {
    if (!userRoles) return false;
    
    // Handle single role (string)
    if (typeof userRoles === 'string') {
        return allowedRoles.includes(userRoles as UserRoleValue);
    }
    
    // Handle multiple roles (array)
    if (Array.isArray(userRoles)) {
        return userRoles.some(role => allowedRoles.includes(role as UserRoleValue));
    }
    
    return false;
}

export function RequireRole({ roles, children }: RequireRoleProps) {
    const { state } = useAuth();
    const userRoles = state.user?.roles;

    if (!hasAccess(userRoles, roles)) {
        return <Navigate to="/403" replace />;
    }

    return <>{children}</>;
}
