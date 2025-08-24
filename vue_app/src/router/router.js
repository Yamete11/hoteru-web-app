import { createRouter, createWebHistory } from 'vue-router';
import Login from '@/pages/Login.vue';
import Reservation from '@/pages/Reservation.vue';
import Arrival from '@/pages/Arrival.vue';
import Guest from '@/pages/Guest.vue';
import Room from '@/pages/Room.vue';
import Service from '@/pages/Service.vue';
import History from '@/pages/History.vue';
import Registration from '@/pages/Registration.vue';
import NewRoom from '@/pages/new/NewRoom.vue';
import NewService from '@/pages/new/NewService.vue';
import RoomDetails from '@/pages/details/RoomDetails.vue';
import ServiceDetails from '@/pages/details/ServiceDetails.vue';
import GuestDetails from '@/pages/details/GuestDetails.vue';
import NewGuest from '@/pages/new/NewGuest.vue';
import HistoryDetails from '@/pages/details/HistoryDetails.vue';
import ListOfEmployees from '@/pages/ListOfEmployees.vue';
import ArrivalDetails from '@/pages/details/ArrivalDetails.vue';
import NewReservation from '@/pages/new/NewReservation.vue';
import MyAccount from '@/pages/MyAccount.vue';

import store from '@/store';
import { parseJwt } from '@/utils/jwt';

const routes = [
    { path: '/', component: Login, meta: { requiresAuth: false } },
    { path: '/registration', component: Registration, meta: { requiresAuth: false } },

    { path: '/reservations/:idReservation', component: ArrivalDetails, name: 'ReservationDetails',
        meta: { requiresAuth: true }, props: r => ({ idReservation: Number(r.params.idReservation), detailsType: 'Reservation' }) },
    { path: '/reservations', component: Reservation, meta: { requiresAuth: true } },
    { path: '/new-reservation', component: NewReservation, meta: { requiresAuth: true } },

    { path: '/arrivals/:idReservation', component: ArrivalDetails, name: 'ArrivalDetails',
        meta: { requiresAuth: true }, props: r => ({ idReservation: Number(r.params.idReservation), detailsType: 'Arrival' }) },
    { path: '/arrivals', component: Arrival, meta: { requiresAuth: true } },

    { path: '/guests/:idPerson', component: GuestDetails, name: 'GuestDetails',
        meta: { requiresAuth: true }, props: route => ({ idPerson: Number(route.params.idPerson) }) },
    { path: '/guests', component: Guest, meta: { requiresAuth: true } },
    { path: '/new-guest', component: NewGuest, meta: { requiresAuth: true } },

    { path: '/rooms/:idRoom', component: RoomDetails, name: 'RoomDetails',
        meta: { requiresAuth: true }, props: route => ({ idRoom: Number(route.params.idRoom) }) },
    { path: '/rooms', component: Room, name: 'Rooms', meta: { requiresAuth: true } },
    { path: '/new-room', component: NewRoom, meta: { requiresAuth: true } },

    { path: '/services/:idService', component: ServiceDetails, name: 'ServiceDetails',
        meta: { requiresAuth: true }, props: route => ({ idService: Number(route.params.idService) })},
    { path: '/services', component: Service, name: 'Services', meta: { requiresAuth: true } },
    { path: '/new-service', component: NewService, meta: { requiresAuth: true } },

    { path: '/history/:idReservation', component: HistoryDetails, name: 'HistoryDetails',
        meta: { requiresAuth: true }, props: true },
    { path: '/history', component: History, meta: { requiresAuth: true } },

    { path: '/my-account', component: MyAccount, meta: { requiresAuth: true } },

    { path: '/employees', component: ListOfEmployees,
        meta: { requiresAuth: true, roles: ['Admin', 'Superadmin'] } },
];

const router = createRouter({
    history: createWebHistory('/'),
    routes,
});

function getToken() {
    return store.getters.getToken || localStorage.getItem('token') || null;
}

function isTokenExpired(token) {
    try {
        const payload = parseJwt(token);
        if (!payload?.exp) return true;
        return Math.floor(Date.now() / 1000) >= payload.exp;
    } catch {
        return true;
    }
}

function getUserRole(token) {
    try {
        const payload = parseJwt(token);
        return payload?.role ?? null;
    } catch {
        return null;
    }
}

router.beforeEach((to, from, next) => {
    const token = getToken();
    const requiresAuth = to.matched.some(r => r.meta?.requiresAuth);

    if (!requiresAuth) {
        if (token && !isTokenExpired(token) && (to.path === '/' || to.path === '/registration')) {
            return next('/arrivals');
        }
        return next();
    }

    if (!token || isTokenExpired(token)) {
        store.dispatch?.('logout');
        return next({ path: '/' });
    }

    const allowedRoles = to.meta?.roles;
    if (Array.isArray(allowedRoles) && allowedRoles.length > 0) {
        const role = getUserRole(token);
        if (!role || !allowedRoles.includes(role)) {
            return next('/arrivals');
        }
    }

    return next();
});

export default router;