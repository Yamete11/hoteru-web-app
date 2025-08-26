import http from '@/lib/http';
import { ENDPOINTS } from '@/config/api';

const clean = (obj = {}) =>
    Object.fromEntries(
        Object.entries(obj).filter(([, v]) => v !== undefined && v !== '' && v !== null)
    );


export async function list({ page = 1, limit = 15, searchQuery = '', searchField = 'number' } = {}) {
    const { data } = await http.get(ENDPOINTS.ROOM.ROOT, {
        params: clean({ page, limit, searchQuery, searchField }),
    });
    return data;
}

export async function get(idRoom) {
    const { data } = await http.get(ENDPOINTS.ROOM.BY_ID(idRoom));
    return data;
}

export async function free(idRoom = 0) {
    const url = idRoom ? ENDPOINTS.ROOM.FREE_BY_ID(idRoom) : ENDPOINTS.ROOM.FREE;
    const { data } = await http.get(url);
    return data;
}

export async function create(dto) {
    const { data } = await http.post(ENDPOINTS.ROOM.ROOT, dto);
    return data;
}

export async function update(dto) {
    const { data } = await http.put(ENDPOINTS.ROOM.ROOT, dto);
    return data;
}

export async function remove(idRoom) {
    const { data } = await http.delete(ENDPOINTS.ROOM.BY_ID(idRoom));
    return data;
}

export async function types() {
    const { data } = await http.get(ENDPOINTS.ROOM.TYPE);
    return data;
}

export async function statuses() {
    const { data } = await http.get(ENDPOINTS.ROOM.STATUS);
    return data;
}
