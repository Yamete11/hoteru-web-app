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

export function mapCreatePayload(ui, idUser) {
    return {
        In: ui.in,
        Out: ui.out,
        Capacity: Number(ui.capacity),
        Price: Number(ui.price),
        IdRoom: ui.idRoom,
        Confirmed: !!ui.confirmed,
        Sum: ui.depositSum ? Number(ui.depositSum) : 0,
        IdDepositType: ui.depositSum ? ui.idDepositType : 0,
        idPerson: ui.idGuest,
        services: Array.isArray(ui.services) ? ui.services : [],
        idUser,
    };
}

export function mapUpdatePayload(idReservation, ui) {
    return {
        idReservation,
        in: ui.in,
        out: ui.out,
        capacity: Number(ui.capacity),
        idRoom: ui.idRoom,
        idRoomType: ui.idRoomType,
        idGuest: ui.idGuest,
        price: Number(ui.price),
        confirmed: !!ui.confirmed,
        depositSum: ui.depositSum ? Number(ui.depositSum) : 0,
        idDepositType: ui.depositSum ? ui.idDepositType : 0,
        services: Array.isArray(ui.services) ? ui.services : [],
    };
}
