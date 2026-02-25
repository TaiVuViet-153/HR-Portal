import type { PagedResult } from "@/components/common/PageResultType";
import { requestApi } from "@/core/api/Client";
import type { 
    CreateBalanceRequest,
    GetBalancesQuery,
    GetBalancesResponse,
    UpdateBalanceRequest,
    SelectedBalance
} from "@/pages/Balances/BalancesType";

export interface BalanceResponse<T> {
    success: boolean;
    message: string;
    data?: T;
}

export async function getBalances(query?: GetBalancesQuery) {
    try {
        const params = new URLSearchParams();
        if (query?.page) params.append('page', String(query.page));
        if (query?.pageSize) params.append('pageSize', String(query.pageSize));
        if (query?.search) params.append('search', query.search);
        if (query?.type !== undefined) params.append('type', String(query.type));
        if (query?.createdYear) params.append('year', String(query.createdYear));
        if (query?.sortBy) params.append('sortBy', query.sortBy);
        if (query?.sortDir !== undefined) params.append('sortDir', String(query.sortDir));
        
        const res = await requestApi.get<PagedResult<GetBalancesResponse>>('Balance', { params });

        console.log("Fetched balances:", res.data);
        return res.data;
    }
    catch (error: any) {
        console.error("Error fetching balances:", error);
        return null;
    }
}

// export async function getBalanceById(id: number) {
//     try {
//         const res = await requestApi.get<GetBalancesResponse>(`Balance/${id}`);
//         return res.data;
//     } catch (error: any) {
//         console.error("Error fetching balance:", error);
//         return null;
//     }
// }

export async function createBalance(formData: FormData): Promise<BalanceResponse<boolean> | null> {
    const data = formData ? Object.fromEntries(formData.entries()) : null;

    const creatingBalance: CreateBalanceRequest = {
        userName: data?.userName ? data.userName as string : '',
        type: data?.type ? Number(data.type) : 0,
        balance: data?.balance ? Number(data.balance) : 0
    }

    try {
        const res = await requestApi.post<BalanceResponse<boolean>>('Balance/createBalance', creatingBalance);
        return {
            success: res.data.success,
            message: res.data.message
        };
    } catch (error: any) {
        return {
            success: false,
            message: error.response?.data
        };
    }
}

export async function updateBalance(selectedBalance: SelectedBalance, formData: FormData): Promise<BalanceResponse<boolean> | null> {
    const data = formData ? Object.fromEntries(formData.entries()) : null;

    const updatingBalance: UpdateBalanceRequest = {
        userID: selectedBalance.user.userID,
        type: data?.type ? Number(data.type) : selectedBalance.balanceItem.leaveType,
        year: data?.year ? Number(data.year) : selectedBalance.balanceItem.year,
        balance: data?.balance ? Number(data.balance) : selectedBalance.balanceItem.balance
    }
    console.log("Updating balance with data:", updatingBalance);

    try {
        const res = await requestApi.put<BalanceResponse<boolean>>('Balance/updateBalance', updatingBalance);

        return {
            success: res.data.success,
            message: res.data.message
        };
    } catch (error: any) {
        return {
            success: false,
            message: error.response?.data
        };
    }
}

export async function deleteBalance(userID: number, type: number, year: number): Promise<BalanceResponse<boolean> | null> {
    try {
        const res = await requestApi.delete<BalanceResponse<boolean>>(`Balance/deleteBalance/${userID}&${type}&${year}`);
        
        return {
            success: res.data.success,
            message: res.data.message
        };
    } catch (error: any) {
        return {
            success: false,
            message: error.response?.data
        };
    }
}
