import axios from "axios"

export const createApiClient = (baseUrl: string) => {
    return axios.create({
        baseURL: baseUrl,
        timeout: 30000,
        withCredentials: true
    });
}