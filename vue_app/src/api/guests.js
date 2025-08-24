import http from '@/lib/http';
import { ENDPOINTS } from '@/config/api';

const clean = (obj = {}) =>
    Object.fromEntries(
        Object.entries(obj).filter(([,v]) => v !== undefined && v !== '')
);

export async function list({ page=1, limit=15, searchQuery='', searchField='' } = {}) {
    const { data } = await http.get(ENDPOINTS.GUEST.ROOT, {
        params: clean({ page, limit, searchQuery, searchField }) });
    return data;
}

export async function getPerson(idPerson) {
    const { data } = await http.get(ENDPOINTS.GUEST.BY_ID(idPerson));
    return data;
}

export async function remove(idPerson) {
    const { data } = await http.delete(ENDPOINTS.GUEST.BY_ID(idPerson));
    return data;
}

export async function update(dto) {
    const { data } = await http.put(ENDPOINTS.GUEST.ROOT, dto);
    return data;
}

export async function create(dto) {
    const { data } = await http.post(ENDPOINTS.GUEST.ROOT, dto);
    return data;
}

export async function status() {
    const { data } = await http.get(ENDPOINTS.GUEST.STATUS);
    return data;
}