import axios from 'axios';
import { LOGIN_URL } from '@/router/urls.js';
import router from '@/router/index.js';

const createApiClient = (baseUrl) => {
    const apiClient = axios.create({
        baseURL: baseUrl,
        headers: {
            'Content-Type': 'application/json',
        },
    });

    // Add a request interceptor to add the token to the request headers.
    apiClient.interceptors.request.use(
        config => {
            const token = localStorage.getItem('token');
            if (token) {
                config.headers.Authorization = `Bearer ${token}`;
            }
            return config;
        },
        error => Promise.reject(error)
    );

    // Add a response interceptor to handle 401 errors.
    apiClient.interceptors.response.use(
        response => response,
        error => {
            if (error.response && error.response.status === 401) {
                localStorage.removeItem('token');
                router.push(LOGIN_URL);
            }
            return Promise.reject(error);
        }
    );

    return apiClient;
}

export default createApiClient;
