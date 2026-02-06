export const RequestTypes: Record<string, number> = {
    Unpaid: 0, 
    Paid: 1, 
    Maternity: 2, 
    Wedding: 3, 
    Bereavement: 4
};

export const RequestStatuses: Record<string, number> = {
    Pending: 0, 
    Approved: 1, 
    Rejected: 2, 
    Cancelled: 3
};

// Helper: lấy key từ value
export const getStatusKey = (value: number): string => {
    return Object.keys(RequestStatuses).find(key => RequestStatuses[key] === value) ?? 'Pending';
};

export const getTypeKey = (value: number): string => {
    return Object.keys(RequestTypes).find(key => RequestTypes[key] === value) ?? 'Unpaid';
};

// Helper: kiểm tra status
export const isPending = (status: string | number): boolean => {
    if (typeof status === 'number') return status === RequestStatuses.Pending;
    return status === 'Pending';
};

export const isApproved = (status: string | number): boolean => {
    if (typeof status === 'number') return status === RequestStatuses.Approved;
    return status === 'Approved';
};

export const isRejected = (status: string | number): boolean => {
    if (typeof status === 'number') return status === RequestStatuses.Rejected;
    return status === 'Rejected';
};

export const isCancelled = (status: string | number): boolean => {
    if (typeof status === 'number') return status === RequestStatuses.Cancelled;
    return status === 'Cancelled';
};
