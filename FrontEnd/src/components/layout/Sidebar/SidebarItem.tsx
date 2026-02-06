import { useAuth } from "@/core/auth/AuthContext";
import { Link, useLocation } from "react-router-dom";
import { type LucideIcon } from 'lucide-react';
import type { UserRoleValue } from "@/core/auth/AuthType";


type SidebarItemProps = {
  to: string;
  icon: LucideIcon;
  label: string;
  roles: readonly UserRoleValue[];
  collapsed: boolean;
};


function hasAccess(userRoles: UserRoleValue | UserRoleValue[] | undefined, allowedRoles: readonly UserRoleValue[]): boolean {
  if (!userRoles) return false;
  
  // Handle single role (string)
  if (typeof userRoles === 'string') {
    return allowedRoles.includes(userRoles);
  }
  
  // Handle multiple roles (array)
  return userRoles.some(role => allowedRoles.includes(role));
}


export default function SidebarItem({ to, icon: Icon, label, roles, collapsed }: SidebarItemProps) {
  const { state } = useAuth();
  const location = useLocation();
  const isActive = location.pathname === to;

  const userRoles = state.user?.roles as UserRoleValue | UserRoleValue[] | undefined;
  
  if (!hasAccess(userRoles, roles)) {
    return null;
  }

  return (
    <Link
      to={to}
      title={collapsed ? label : undefined}
      className={`flex items-center gap-3 px-4 py-3 rounded-xl transition-all duration-200 mb-1 ${
        isActive 
          ? 'bg-blue-600 text-white shadow-lg shadow-blue-200' 
          : 'text-gray-500 hover:bg-gray-100'
      }`}
    >
      <Icon size={20} className="shrink-0" />
      {!collapsed && <span className="font-semibold whitespace-nowrap">{label}</span>}
    </Link>
  );
};
