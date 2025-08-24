import http from '@/lib/http';
import { ENDPOINTS } from '@/config/api';

const clean = (obj = {}) =>
    Object.fromEntries(
        Object.entries(obj).filter(([, v]) => v !== undefined && v !== '' && v !== null)
    );

export async function list({ page = 1, limit = 15, searchQuery = '', searchField = '' } = {}) {
    const { data } = await http.get(ENDPOINTS.SERVICE.ROOT, {
        params: clean({ page, limit, searchQuery, searchField }),
    });
    return data;
}

export async function get(id) {
    const { data } = await http.get(ENDPOINTS.SERVICE.BY_ID(id));
    return data;
}

export async function create(dto) {
    const { data } = await http.post(ENDPOINTS.SERVICE.ROOT, dto);
    return data;
}

export async function update(dto) {
    const { data } = await http.put(ENDPOINTS.SERVICE.BY_ID(dto.idService), dto);
    return data;
}

export async function remove(id) {
    const { data } = await http.delete(ENDPOINTS.SERVICE.BY_ID(id));
    return data;
}
