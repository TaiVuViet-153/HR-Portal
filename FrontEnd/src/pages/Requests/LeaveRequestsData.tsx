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
export const isPending = (status: number): boolean => {
    return status === RequestStatuses.Pending;
};

export const isApproved = (status: number): boolean => {
    return status === RequestStatuses.Approved;
};

export const isRejected = (status: number): boolean => {
    return status === RequestStatuses.Rejected;
};

export const isCancelled = (status: number): boolean => {
    return status === RequestStatuses.Cancelled;
};

// Helper: kiểm tra type
export const isUnpaid = (type: number): boolean => {
    return type === RequestTypes.Unpaid;
}

export const isPaid = (type: number): boolean => {
    return type === RequestTypes.Paid;
}

export const isMaternity = (type: number): boolean => {
    return type === RequestTypes.Maternity;
}

export const isWedding = (type: number): boolean => {
    return type === RequestTypes.Wedding;
}

export const isBereavement = (type: number): boolean => {
    return type === RequestTypes.Bereavement;
}
