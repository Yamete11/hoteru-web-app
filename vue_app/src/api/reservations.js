import http from '@/lib/http';
import { ENDPOINTS } from '@/config/api';

const clean = (obj = {}) =>
    Object.fromEntries(Object.entries(obj).filter(([, v]) => v !== undefined && v !== ''));

export async function arrivals({ page = 1, limit = 15, searchQuery = '', searchField = '' } = {}) {
    const { data } = await http.get(ENDPOINTS.RESERVATION.ARRIVALS, {
        params: clean({ page, limit, searchQuery, searchField }),
    });
    return data;
}

export async function list({ page = 1, limit = 15, searchQuery = '', searchField = '' } = {}) {
    const { data } = await http.get(ENDPOINTS.RESERVATION.ROOT, {
        params: clean({ page, limit, searchQuery, searchField }),
    });
    return data;
}

export async function history({ page = 1, limit = 15, searchQuery = '', searchField = '' } = {}) {
    const { data } = await http.get(ENDPOINTS.RESERVATION.HISTORY, {
        params: clean({ page, limit, searchQuery, searchField }),
    });
    return data;
}

export async function getArrival(id) {
    const { data } = await http.get(ENDPOINTS.RESERVATION.ARRIVAL_BY_ID(id));
    return data;
}

export async function getHistory(id) {
    const { data } = await http.get(ENDPOINTS.RESERVATION.HISTORY_BY_ID(id));
    return data;
}

export async function confirm(id) {
    const { data } = await http.put(ENDPOINTS.RESERVATION.CONFIRM(id));
    return data;
}

export async function create(dto) {
    const { data } = await http.post(ENDPOINTS.RESERVATION.ROOT, dto);
    return data;
}

export async function update(dto) {
    const { data } = await http.put(ENDPOINTS.RESERVATION.ROOT, dto);
    return data;
}

export async function remove(id) {
    const { data } = await http.delete(ENDPOINTS.RESERVATION.BY_ID(id));
    return data;
}
