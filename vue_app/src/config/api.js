const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL || "").replace(/\/+$/, "");
const withBase = (path) => `${API_BASE_URL}${path}`;

export const ENDPOINTS = {
    BASE_URL: API_BASE_URL,

    REGISTRATION: { HOTEL: withBase("/api/Hotel") },

    HOTEL: {
        ROOT: withBase("/api/Hotel"),
    },

    RESERVATION: {
        ROOT: withBase("/api/Reservation"),
        BY_ID: (id) => withBase(`/api/Reservation/${id}`),
        HISTORY: withBase("/api/Reservation/history"),
        HISTORY_BY_ID: (id) => withBase(`/api/Reservation/history/${id}`),
        ARRIVALS: withBase("/api/Reservation/arrivals"),
        ARRIVAL_BY_ID: (id) => withBase(`/api/Reservation/arrival/${id}`),
        CONFIRM: (id) => withBase(`/api/Reservation/confirm/${id}`),
    },

    AUTH: { LOGIN: withBase("/api/Login") },

    USER: {
        ROOT: withBase("/api/User"),
        BY_ID: (id) => withBase(`/api/User/${id}`),
        BY_NAME: (userName) => withBase(`/api/User/${encodeURIComponent(userName)}`),
        FULL: (id) => withBase(`/api/User/fullUser/${id}`),
    },

    SERVICE: {
        ROOT: withBase("/api/Service"),
        BY_ID: (id) => withBase(`/api/Service/${id}`)
    },

    ROOM: {
        ROOT: withBase("/api/Room"),
        BY_ID: (id) => withBase(`/api/Room/${id}`),
        FREE: withBase("/api/Room/freeRooms"),
        FREE_BY_ID: (idRoom) => withBase(`/api/Room/freeRooms?idRoom=${encodeURIComponent(idRoom)}`),
        TYPE: withBase("/api/RoomType"),
        STATUS: withBase("/api/RoomStatus"),
    },

    GUEST: {
        ROOT: withBase("/api/Guest"),
        BY_ID: (id) => withBase(`/api/Guest/${id}`),
        STATUS: withBase("/api/GuestStatus"),
    },

    USER_TYPE: withBase("/api/UserType"),
    DEPOSIT_TYPE: withBase("/api/DepositType"),
};
