export interface User {
  id: number;
  userDetail: string | null;
  createdAt: string;
}

export interface UserDetail {
  FirstName?: string;
  LastName?: string;
  FullName?: string;
  Email?: string;
  Phone?: string;
  PhoneNumber?: string;
  BirthDate?: string;
  Address?: string;
  Location?: string;
  TimeZone?: string;
  UserName?: string;
  UserPhoto?: string | null;
  BranchID?: number | null;
  Type2Fa?: string | null;
  TahoUserID?: number;
}

export interface UserWithDetail {
  userID: number;
  userName: string | null;
  detail?: UserDetail | null;
  createdAt: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  phoneNumber?: string;
  location?: string;
  timeZone?: string;
  birthDate?: string;
  address?: string;
  leaveBalances?: LeaveBalance[];
  roles?: string[];
  status: number;
}

export interface LeaveBalance {
  leaveType: number;
  year: number;
  balance: number;
}

export interface GetUsersQuery {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: number;
  createdAfter?: Date;
  createdBefore?: Date;
  sortBy?: string;
  sortDir?: number;
}

// Request DTOs matching backend
export interface CreateUserRequest {
  userName: string;
  email: string;
}

export interface UpdateUserRequest {
  userID: number;
  email?: string;
  currentPassword?: string;
  newPassword?: string;
  detail?: string;
  status?: number;
}

// Response DTOs matching backend
export interface CreateUserResponse {
  userID: number;
  userName: string;
  email: string;
  password: string;
}

export interface UpdateUserResponse {
  userID: number;
  userName: string;
  email: string;
  updatedDate: string;
}

export interface UserResponse<T> {
  isSuccess: boolean;
  message: string;
  data?: T;
}

export interface ResetPasswordResponse {
  newPassword: string;
}