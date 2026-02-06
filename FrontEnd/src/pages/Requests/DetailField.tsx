import React from 'react';
import { isPending, isApproved, isRejected, isCancelled } from '@/pages/Requests/LeaveRequestsData';

type DetailFieldProps = {
    label: string;
    value: React.ReactNode;
    icon?: React.ReactNode;
};

export function DetailField({ label, value, icon }: DetailFieldProps) {
    return (
        <div className="flex items-start gap-3 py-3 border-b border-gray-50 last:border-0">
            {icon && (
                <div className="w-10 h-10 bg-blue-50 rounded-xl flex items-center justify-center text-blue-500 shrink-0">
                    {icon}
                </div>
            )}
            <div className="flex-1 min-w-0">
                <p className="text-sm text-gray-500 mb-0.5">{label}</p>
                <div className="text-gray-800 font-medium">{value || '-'}</div>
            </div>
        </div>
    );
}

type StatusBadgeProps = {
    status: string | number;
};

export function StatusBadge({ status }: StatusBadgeProps) {
    const getStyles = () => {
        if (isPending(status)) return { bg: 'bg-amber-100 text-amber-700', dot: 'bg-amber-500' };
        if (isApproved(status)) return { bg: 'bg-green-100 text-green-700', dot: 'bg-green-500' };
        if (isRejected(status)) return { bg: 'bg-red-100 text-red-700', dot: 'bg-red-500' };
        if (isCancelled(status)) return { bg: 'bg-gray-100 text-gray-600', dot: 'bg-gray-500' };
        return { bg: 'bg-gray-100 text-gray-500', dot: 'bg-gray-400' };
    };

    const styles = getStyles();
    const displayStatus = typeof status === 'string' ? status : String(status);

    return (
        <span className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-medium ${styles.bg}`}>
            <span className={`w-1.5 h-1.5 rounded-full mr-2 ${styles.dot}`} />
            {displayStatus.charAt(0).toUpperCase() + displayStatus.slice(1).toLowerCase()}
        </span>
    );
}

type DetailSectionProps = {
    title: string;
    children: React.ReactNode;
};

export function DetailSection({ title, children }: DetailSectionProps) {
    return (
        <div className="mb-6 last:mb-0">
            <h4 className="text-sm font-semibold text-gray-400 uppercase tracking-wider mb-3">{title}</h4>
            <div className="bg-gray-50 rounded-xl p-4">
                {children}
            </div>
        </div>
    );
}
