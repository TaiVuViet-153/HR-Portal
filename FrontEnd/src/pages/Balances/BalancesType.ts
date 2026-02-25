export interface CreateBalanceRequest {
    userName: string;
    type: number;
    balance: number;
}

export interface GetBalancesQuery {
    page?: number;
    pageSize?: number;
    search?: string;
    type?: number;
    createdYear?: number;
    sortBy?: string;
    sortDir?: number;
}

// export interface CreateBalanceResponse {
//     userId: number;
//     userName: string;
//     type: number;
//     balance: number;
//     year: number;
// }

// Nested leave balance item
export interface LeaveBalance {
    leaveType: number;
    year: number;
    balance: number;
}

// User with their leave balances - matches API response
export interface GetBalancesResponse {
    userID: number;
    userName?: string;
    detail?: string;
    firstName?: string;
    lastName?: string;
    email?: string;
    leaveBalances: LeaveBalance[];
    createdAt: string;
}

export interface UpdateBalanceRequest {
    userID: number;
    type?: number;
    year?: number;
    balance?: number;
}

// Type for selected balance when editing
export interface SelectedBalance {
    user: GetBalancesResponse;
    balanceItem: LeaveBalance;
}

export interface BalanceResponse<T> {
    isSuccess: boolean;
    message: string;
    data?: T;
}