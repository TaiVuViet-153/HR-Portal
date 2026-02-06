import { requestApi } from "@/core/api/Client";
import type { CreateLeaveRequest, GetRequestQuery, LeaveBalanceRequest, LeaveRequestResponse, UpdateLeaveRequest } from "@/pages/Requests/LeaveRequestsType";
import { RequestStatuses, RequestTypes } from "@/pages/Requests/LeaveRequestsData";
import type { PagedResult } from "@/components/common/PageResultType";

export async function getRequest(queryData?: GetRequestQuery) {
    try {
        const res = await requestApi.get<PagedResult<LeaveRequestResponse>>('/api/Request/filteredRequests', { params: queryData });
        return res.data;
    }
    catch (error: any) {
        console.error("Error fetching leave requests:", error);
    }
};  

export async function createRequest(userId: number, formData: FormData) {
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
        const res = await requestApi.post('/api/Request/createRequest', newRequest);
        return res.data;
    } catch (error: any) {
        console.error("Error creating leave request:", error);
    }
}

export async function updateRequest(userId: number, selectedRequest: LeaveRequestResponse, formData?: FormData) {
    const data = formData ? Object.fromEntries(formData.entries()) : null;

    const updatingRequest: UpdateLeaveRequest = {
        requestId: Number(selectedRequest?.requestId),
        userId: userId,
        type: Number(data?.type ? data.type : RequestTypes[selectedRequest.type]),
        startDate: data?.startDate ? data.startDate as string : selectedRequest.startDate,
        endDate: data?.endDate ? data.endDate as string : selectedRequest.endDate,
        isHalfDayOff: Number(data?.isHalfDayOff ? data.isHalfDayOff : selectedRequest.isHalfDayOff) === 1 ? true : false,
        reason: data?.reason ? data.reason as string : selectedRequest.reason,
        status: Number(RequestStatuses[selectedRequest?.status])
    };

    try {
        const res = await requestApi.put('/api/Request/updateRequest', updatingRequest);
        return res.data;
    } catch (error: any) {
        console.error("Error updating leave request:", error);
    }
}

export async function deleteRequest(requestId: number) {
    try {
        const res = await requestApi.patch(`/api/Request/deleteRequest/${requestId}`);
        return res.data;
    } catch (error: any) {
        console.error("Error deleting leave request:", error);
    }
}

export async function createBalance(balanceRequest: LeaveBalanceRequest) {
    try {
        const res = await requestApi.post('/api/Request/createBalance', balanceRequest);
        return res.data;
    } catch (error: any) {
        console.error("Error creating leave balances:", error);
    }
}