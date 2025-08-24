import http from '@/lib/http';
import { ENDPOINTS } from '@/config/api';

export async function login({ login, password }) {
    const { data } = await http.post(ENDPOINTS.AUTH.LOGIN, { login, password });
    return data;
}
