import http from '@/lib/http';
import { ENDPOINTS } from '@/config/api';

export async function list() {
    const { data } = await http.get(ENDPOINTS.DEPOSIT_TYPE);
    return data;
}
