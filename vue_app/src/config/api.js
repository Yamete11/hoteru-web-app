const API_BASE_URL = "http://localhost:8080";

export default {
    BASE_URL: API_BASE_URL,
    REGISTRATION: {
        HOTEL: `${API_BASE_URL}/api/Hotel`
    },
    RESERVATION: {
        RESERVATION: `${API_BASE_URL}/api/Reservation`,
        RESERVATION_BY_ID: (id) => `${API_BASE_URL}/api/Reservation/${id}`,
        HISTORY: `${API_BASE_URL}/api/Reservation/history`,
        HISTORY_ID: (id) => `${API_BASE_URL}/api/Reservation/history/${id}`,
        ARRIVALS: `${API_BASE_URL}/api/Reservation/arrivals`,
        ARRIVAL_BY_ID: (id) => `${API_BASE_URL}/api/Reservation/arrival/${id}`,
    },
    CONFIRM: (id) => `${API_BASE_URL}/api/confirm/${id}`,
    LOGIN: `${API_BASE_URL}/api/Login`,
    USER: {
        USER: `${API_BASE_URL}/api/User`,
        USER_BY_NAME: (userName) => `${API_BASE_URL}/api/User/${userName}`,
        PERSON: (id) => `${API_BASE_URL}/api/User/${id}`,
        FULL_USER: (id) => `${API_BASE_URL}/api/User/fullUser/${id}`,
    },

    SERVICE: `${API_BASE_URL}/api/Service`,
    SERVICE_ID: (id) => `${API_BASE_URL}/api/Service/${id}`,

    ROOM: `${API_BASE_URL}/api/Room`,
    ROOM_ID: (id) => `${API_BASE_URL}/api/Room/${id}`,
    FREE_ROOM: `${API_BASE_URL}/api/Room/freeRooms`,
    FREE_ROOM_BY_ID: (idRoom) => `${API_BASE_URL}/api/Room/freeRooms?idRoom=${idRoom}`,
    ROOM_TYPE: `${API_BASE_URL}/api/RoomType`,
    ROOM_STATUS: `${API_BASE_URL}/api/RoomStatus`,

    GUEST: `${API_BASE_URL}/api/Guest`,
    GUEST_ID: (id) => `${API_BASE_URL}/api/Guest/${id}`,
    GUEST_STATUS: `${API_BASE_URL}/api/GuestStatus`,

    USER_TYPE: `${API_BASE_URL}/api/UserType`,
    DEPOSIT_TYPE: `${API_BASE_URL}/api/DepositType`,
};
