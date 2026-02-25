import { requestApi } from "@/core/api/Client";
import type { CreateLeaveRequest, GetRequestQuery, LeaveRequestResponse, UpdateLeaveRequest } from "@/pages/Requests/LeaveRequestsType";
import type { PagedResult } from "@/components/common/PageResultType";
import axios from "axios";

export interface RequestResponse<T> {
    isSuccess: boolean;
    message: string;
    data?: T;
}

export async function getRequest(queryData?: GetRequestQuery) {
    try {
        const res = await requestApi.get<PagedResult<LeaveRequestResponse>>('Request', { params: queryData });
        console.log("Fetched leave requests:", res.data);
        return res.data;
    }
    catch (error: any) {
        console.error("Error fetching leave requests:", error);
        return null;
    }
};  

export async function createRequest(userId: number, formData: FormData): Promise<RequestResponse<LeaveRequestResponse> | null> {
    const data = Object.fromEntries(formData.entries());
    const newRequest: CreateLeaveRequest = {
        userId: userId,
        type: Number(data.type),
        startDate: data.startDate as string,
        endDate: data.endDate as string,
        isHalfDayOff: Number(data.isHalfDayOff) === 1 ? true : false,
        reason: data.reason as string
    };

    try {
        const res = await requestApi.post<LeaveRequestResponse>('Request/createRequest', newRequest);
        return {
            isSuccess: true,
            message: 'Leave request created successfully',
            data: res.data
        };
    } catch (error: any) {
        if (axios.isAxiosError(error)) {
            // Lấy error message từ response body của BadRequest
            const errorMessage = error.response?.data || error.message;

            console.error("Create request error:", {
                status: error.response?.status,
                message: errorMessage
            });
            // Throw error với message từ backend
            throw new Error(errorMessage);
        }
        console.error("Error creating leave request:", error);
        return {
            isSuccess: false,
            message: error.response?.data || 'Failed to create leave request'
        };
    }
}

export async function updateRequest(userId: number, selectedRequest: LeaveRequestResponse, formData?: FormData): Promise<RequestResponse<LeaveRequestResponse> | null> {
    const data = formData ? Object.fromEntries(formData.entries()) : null;

    const updatingRequest: UpdateLeaveRequest = {
        requestId: Number(selectedRequest?.requestId),
        userId: userId,
        type: data?.type ? Number(data.type) : selectedRequest.type,
        startDate: data?.startDate ? data.startDate as string : selectedRequest.startDate,
        endDate: data?.endDate ? data.endDate as string : selectedRequest.endDate,
        isHalfDayOff: Number(data?.isHalfDayOff ? data.isHalfDayOff : selectedRequest.isHalfDayOff) === 1 ? true : false,
        reason: data?.reason ? data.reason as string : selectedRequest.reason,
        status: data?.status ? Number(data.status) : selectedRequest.status
    };

    try {
        console.log("Updating request with data:", updatingRequest);
        const res = await requestApi.put<LeaveRequestResponse>('Request/updateRequest', updatingRequest);
        return {
            isSuccess: true,
            message: 'Leave request updated successfully',
            data: res.data
        };
    } catch (error: any) {
        if (axios.isAxiosError(error)) {
            // Lấy error message từ response body của BadRequest
            const errorMessage = error.response?.data || error.message;

            console.error("Update request error:", {
                status: error.response?.status,
                message: errorMessage
            });
            // Throw error với message từ backend
            throw new Error(errorMessage);
        }
        console.error("Error updating leave request:", error);
        return {
            isSuccess: false,
            message: error.response?.data || 'Failed to update leave request'
        };
    }
}

export async function deleteRequest(requestId: number): Promise<RequestResponse<boolean> | null> {
    try {
        const res = await requestApi.patch<boolean>(`Request/deleteRequest/${requestId}`);
        return {
            isSuccess: true,
            message: 'Leave request deleted successfully',
            data: res.data
        };
    } catch (error: any) {
        if (axios.isAxiosError(error)) {
            // Lấy error message từ response body của BadRequest
            const errorMessage = error.response?.data || error.message;

            console.error("Delete request error:", {
                status: error.response?.status,
                message: errorMessage
            });
            // Throw error với message từ backend
            throw new Error(errorMessage);
        }
        console.error("Error deleting leave request:", error);
        return {
            isSuccess: false,
            message: error.response?.data || 'Failed to delete leave request'
        };
    }
}

// export async function createBalance(balanceRequest: LeaveBalanceRequest): Promise<RequestResponse<any> | null> {
//     try {
//         const res = await requestApi.post('Request/createBalance', balanceRequest);
//         return {
//             isSuccess: true,
//             message: 'Leave balance created successfully',
//             data: res.data
//         };
//     } catch (error: any) {
//         console.error("Error creating leave balances:", error);
//         return {
//             isSuccess: false,
//             message: error.response?.data || 'Failed to create leave balance'
//         };
//     }
// }