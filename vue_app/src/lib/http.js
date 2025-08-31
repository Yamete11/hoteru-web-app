import axios from "axios";

const base = (import.meta.env.VITE_API_BASE_URL || '').replace(/\/+$/, '')

const http = axios.create({
    baseURL: base || '/api',
    timeout: 15000,
    withCredentials: true,
})
export function setAuthToken(token) {
    if (token) http.defaults.headers.common.Authorization = `Bearer ${token}`
    else delete http.defaults.headers.common.Authorization
}


http.interceptors.request.use((config) => {
    const url = String(config.url || '').toLowerCase();
    const isLogin = url.endsWith('/api/login') || url.includes('/api/login?');
    if (!isLogin && !config.headers?.Authorization) {
        const t = localStorage.getItem('token');
        if (t) config.headers.Authorization = `Bearer ${t}`;
    }
    return config;
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
    (err) => {
        const norm = normalizeError(err);
        if (norm.status === 401) localStorage.removeItem('token');
        return Promise.reject(norm);
    }
);

export default http;
