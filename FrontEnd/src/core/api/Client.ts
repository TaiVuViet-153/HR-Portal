import { AUTH_BASE_URL, EMPLOYEE_BASE_URL, REQUEST_BASE_URL } from "@/config";
import { createApiClient } from "@/core/api/CreateApiClient";

export const authApi = createApiClient(AUTH_BASE_URL);
export const requestApi = createApiClient(REQUEST_BASE_URL);
export const employeeApi = createApiClient(EMPLOYEE_BASE_URL);
