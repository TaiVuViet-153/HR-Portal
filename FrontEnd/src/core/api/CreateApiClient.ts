import axios from "axios"

export const createApiClient = (baseUrl: string) => {
    return axios.create({
        baseURL: baseUrl,
        timeout: 5000,
        withCredentials: true
    });
}