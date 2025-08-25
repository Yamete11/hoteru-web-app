import http from '@/lib/http';
import { ENDPOINTS } from '@/config/api';

export async function login({ login, password }) {
    const { data } = await http.post(ENDPOINTS.AUTH.LOGIN, { login, password });
    return data;
}

/*export const auth = {
    login(payload) {
        return http.post(ENDPOINTS.AUTH.LOGIN, payload);
    },
    refresh() {
        return http.post(`${ENDPOINTS.AUTH.LOGIN}/refresh`);
    },
    logout() {
        return http.post(`${ENDPOINTS.AUTH.LOGIN}/logout`);
    },
};*/

