export const LeaveTypes = [
    { label: 'Unpaid', value: '0' },
    { label: 'Paid', value: '1' },
    { label: 'Maternity', value: '2' },
    { label: 'Wedding', value: '3' },
    { label: 'Bereavement', value: '4' }
];

export const LeaveTypeOptions = [
    { label: 'Paid', value: '1' },
    { label: 'Maternity', value: '2' },
    { label: 'Wedding', value: '3' },
    { label: 'Bereavement', value: '4' }
]

export const LeaveTypeEnum: Record<string, number> = {
    Unpaid: 0,
    Paid: 1,
    Maternity: 2,
    Wedding: 3,
    Bereavement: 4
};

// Helper: get leave type key from value
export const getLeaveTypeKey = (value: number): string => {
    return Object.keys(LeaveTypeEnum).find(key => LeaveTypeEnum[key] === value) ?? 'Unpaid';
};

export const getLeaveTypeColor = (leaveType: number | string) => {
    const typeNum = typeof leaveType === 'string' ? Number(leaveType) : leaveType;
    const typeKey = getLeaveTypeKey(typeNum);
    
    const typeColors: Record<string, { bg: string; text: string; badge: string }> = {
        Unpaid: { bg: 'bg-gray-50', text: 'text-gray-600', badge: 'bg-gray-100' },
        Paid: { bg: 'bg-blue-50', text: 'text-blue-600', badge: 'bg-blue-100' },
        Maternity: { bg: 'bg-pink-50', text: 'text-pink-600', badge: 'bg-pink-100' },
        Wedding: { bg: 'bg-purple-50', text: 'text-purple-600', badge: 'bg-purple-100' },
        Bereavement: { bg: 'bg-red-50', text: 'text-red-600', badge: 'bg-red-100' }
    };
    return typeColors[typeKey] || { bg: 'bg-green-50', text: 'text-green-600', badge: 'bg-green-100' };
};

export const SortByOptions = [
    { label: 'Create Date', value: 'createdAt' },
    { label: 'Username', value: 'userName' },
    { label: 'Balance', value: 'balance' },
    { label: 'Leave Type', value: 'type' }
];

export const SortDirOptions = [
    { label: 'Ascending', value: '1' },
    { label: 'Descending', value: '0' }
];

export const DETAIL_FIELD_MAP: Record<string, string> = {};
