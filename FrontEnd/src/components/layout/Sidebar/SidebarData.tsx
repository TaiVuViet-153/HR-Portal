import { CalendarDays, Users, Archive } from 'lucide-react';
import type { UserRoleValue } from "@/core/auth/AuthType";
import type { LucideIcon } from 'lucide-react';

export type SidebarMenuItem = {
    to: string;
    icon: LucideIcon;
    label: string;
    roles: readonly UserRoleValue[];
};

export const SidebarData: SidebarMenuItem[] = [
    {
        to: "/users",
        icon: Users,
        label: "Employees",
        roles: ['ADMIN', 'USER']
    },
    {
        to: "/requests",
        icon: CalendarDays,
        label: "Leave Requests",
        roles: ['ADMIN', 'USER']
    },
    {
        to: "/balances",
        icon: Archive,
        label: "Leave Balances",
        roles: ['ADMIN', 'USER']
    }
];