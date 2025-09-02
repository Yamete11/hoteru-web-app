import http from '@/lib/http';
import { ENDPOINTS } from '@/config/api';

export async function getByName(userName) {
    const { data } = await http.get(ENDPOINTS.USER.BY_NAME(userName));
    return data;
}

export async function list() {
    const { data } = await http.get(ENDPOINTS.USER.ROOT);
    return data;
}
