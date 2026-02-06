export interface LeaveRequest {
  id: string;
  employeeId: string;
  employeeName: string;
  startDate: string;
  endDate: string;
  type: 'Annual' | 'Sick' | 'Personal';
  status: 'Pending' | 'Approved' | 'Rejected';
  reason: string;
}

export interface LeaveRequestResponse {
  requestId: number;
  userId: number;
  userName: string;
  type: string;
  startDate: string;
  endDate: string;
  isHalfDayOff: boolean;
  reason: string;
  status: string;
  createdAt: string;
}

export interface CreateLeaveRequest {
  userId: number;
  type: number;
  startDate: string;
  endDate: string;
  isHalfDayOff: boolean;
  reason: string;
}

export interface UpdateLeaveRequest {
  requestId: number;
  userId: number;
  type?: number;
  startDate?: string;
  endDate?: string;
  isHalfDayOff?: boolean;
  reason?: string;
  status?: number;
}

export interface CreateRequestResponse {
  success: boolean;
  message: string;
}

export type GetRequestQuery = {
  userId?: number;
  type?: number;
  startDate?: string;
  endDate?: string;
  isHalfDayOff?: boolean;
  reason?: string;
  status?: number;
  page?: number;
  pageSize?: number;
};

export interface LeaveBalanceRequest {
  userId: number;
  type: number;
  balance: number;
}