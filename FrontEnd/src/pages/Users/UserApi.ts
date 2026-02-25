import { apply } from "json-merge-patch";
import type { PagedResult } from "@/components/common/PageResultType";
import { employeeApi } from "@/core/api/Client";
import type { 
    UserWithDetail, 
    GetUsersQuery, 
    CreateUserRequest, 
    UpdateUserRequest,
    CreateUserResponse,
    UpdateUserResponse,
    UserResponse
} from "@/pages/Users/UsersType";
import { DETAIL_FIELD_MAP } from "./UsersData";

export async function getUsers(query?: GetUsersQuery) {
    try {
        const params = new URLSearchParams();
        if (query?.page) params.append('page', String(query.page));
        if (query?.pageSize) params.append('pageSize', String(query.pageSize));
        if (query?.search) params.append('search', query.search);
        if (query?.status !== undefined) params.append('status', String(query.status));
        if (query?.createdAfter) params.append('createdAfter', query.createdAfter.toISOString());
        if (query?.createdBefore) params.append('createdBefore', query.createdBefore.toISOString());
        if (query?.sortBy) params.append('sortBy', query.sortBy);
        if (query?.sortDir !== undefined) params.append('sortDir', String(query.sortDir));

        const res = await employeeApi.get<PagedResult<UserWithDetail>>('User/', { params });

        console.log("Fetched users:", res.data);
        return res.data;
    }
    catch (error: any) {
        console.error("Error fetching users:", error);
        return null;
    }
}

export async function getUserById(id: number) {
    try {
        const res = await employeeApi.get<UserWithDetail>(`User/${id}`);
        return res.data;
    } catch (error: any) {
        console.error("Error fetching user:", error);
        return null;
    }
}

export async function createUser(formData: FormData): Promise<UserResponse<CreateUserResponse> | null> {
    const data = formData ? Object.fromEntries(formData.entries()) : null;

    const creatingUser: CreateUserRequest = {
        userName: data?.userName ? data.userName as string : '',
        email: data?.email ? data.email as string : ''
    }

    try {
        const res = await employeeApi.post<CreateUserResponse>('User/', creatingUser);
        return {
            isSuccess: true,
            message: 'User created successfully',
            data: res.data
        };
    } catch (error: any) {
        return {
            isSuccess: false,
            message: error.response?.data || 'Failed to create user'
        };
    }
}

export async function updateUser(selectedUser: UserWithDetail, formData: FormData): Promise<UserResponse<UpdateUserResponse> | null> {
    
    const data = formData ? Object.fromEntries(formData.entries()) : null;
    const detailPatch = updatingUserDetail(formData);
    const mergedDetail = selectedUser.detail ? apply(JSON.parse(selectedUser.detail as string), JSON.parse(detailPatch)) : JSON.parse(detailPatch);
    
    const updatingUser: UpdateUserRequest = {
        userID: selectedUser.userID,
        email: data?.email ? data.email as string : selectedUser.email,
        currentPassword: data?.currentPassword ? data.currentPassword as string : undefined,
        newPassword: data?.newPassword ? data.newPassword as string : undefined,
        detail: JSON.stringify(mergedDetail),
        status: data?.status ? Number(data.status) : selectedUser.status
    }

    try {
        const res = await employeeApi.put<UpdateUserResponse>(`User/${updatingUser.userID}`, updatingUser);

        return {
            isSuccess: true,
            message: 'User updated successfully',
            data: res.data
        };
    } catch (error: any) {
        return {
            isSuccess: false,
            message: error?.response?.data || 'Failed to update user'
        };
    }
}

export async function deleteUser(id: number): Promise<UserResponse<boolean> | null> {
    try {
        const res = await employeeApi.delete<boolean>(`User/${id}`);
        return {
            isSuccess: true,
            message: 'User deleted successfully',
            data: res.data
        };
    } catch (error: any) {
        console.error("Error deleting user:", error);
        return {
            isSuccess: false,
            message: error.response?.data || 'Failed to delete user'
        };
    }
}

export async function resetPassword(id: number): Promise<UserResponse<string> | null> {
    try {
        const res = await employeeApi.put<string>(`User/${id}/reset-password`);
        return {
            isSuccess: true,
            message: 'Password reset successfully',
            data: res.data
        };
    } catch (error: any) {
        console.error("Error resetting password:", error);
        return {
            isSuccess: false,
            message: error.response?.data || 'Failed to reset password'
        };
    }
}

export async function lockUser(id: number): Promise<UserResponse<boolean> | null> {
    try {
        const res = await employeeApi.put<boolean>(`User/${id}/lock-user`);
        return {
            isSuccess: true,
            message: 'User locked successfully',
            data: res.data
        };
    } catch (error: any) {
        console.error("Error locking user:", error);
        return {
            isSuccess: false,
            message: error.response?.data || 'Failed to lock user'
        };
    }
}

// export async function incrementFailedLogin(id: number): Promise<UserResponse<number> | null> {
//     try {
//         const res = await employeeApi.put<number>(`User/${id}/increment-failed-login`);
//         return {
//             isSuccess: true,
//             message: 'Failed login count incremented',
//             data: res.data
//         };
//     } catch (error: any) {
//         console.error("Error incrementing failed login:", error);
//         return {
//             isSuccess: false,
//             message: error.response?.data || 'Failed to increment failed login count'
//         };
//     }
// }

function updatingUserDetail(formData: FormData | null) {
  if (!formData) return JSON.stringify({});

  const data = Object.fromEntries(formData.entries());

  const patchedDetail: Record<string, any> = {};

  for (const [formKey, detailKey] of Object.entries(DETAIL_FIELD_MAP)) {
    const value = data[formKey];

    if (
      value === undefined ||
      value === null ||
      (typeof value === "string" && value.trim() === "")
    ) {
      continue;
    }

    patchedDetail[detailKey] = value;
  }

  return JSON.stringify(patchedDetail);
}


