import axios from "axios";

const base = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/+$/, '');

const http = axios.create({
    baseURL: base || '/api',
    timeout: 15000,
    withCredentials: true,
});

function normalizeError(err) {
    if (!err.response) return { status: 0, message: 'Network error', details: null };
    const { status, data } = err.response;
    const message = data?.message || data?.Message || 'Request failed';
    const details = data?.errors || data || null;
    return { status, message, details };
}

http.interceptors.response.use(
    (res) => res,
    (err) => Promise.reject(normalizeError(err))
);

export default http;
