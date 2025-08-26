import { createRouter, createWebHistory } from 'vue-router';
import store from '@/store';
import { parseJwt } from '@/utils/jwt';

const Login = () => import('@/pages/Login.vue');
const Reservation = () => import('@/pages/Reservation.vue');
const Arrival = () => import('@/pages/Arrival.vue');
const Guest = () => import('@/pages/Guest.vue');
const Room = () => import('@/pages/Room.vue');
const Service = () => import('@/pages/Service.vue');
const History = () => import('@/pages/History.vue');
const Registration = () => import('@/pages/Registration.vue');
const HistoryDetails = () => import('@/pages/forms/HistoryDetails.vue');
const ListOfEmployees = () => import('@/pages/ListOfEmployees.vue');
const MyAccount = () => import('@/pages/MyAccount.vue');
const ServiceForm = () => import('@/pages/forms/ServiceForm.vue');
const RoomForm = () => import('@/pages/forms/RoomForm.vue');
const GuestForm = () => import('@/pages/forms/GuestForm.vue');
const ReservationForm = () => import('@/pages/forms/ReservationForm.vue');
const HotelSettings = () => import('@/pages/HotelSettings.vue');


const routes = [
    {
        path: '/', component: Login,
        meta: { requiresAuth: false } },
    {
        path: '/registration',
        component: Registration,
        meta: { requiresAuth: false } },

    {
        path: '/reservations/:idReservation',
        component: ReservationForm,
        name: 'ReservationDetails',
        meta: { requiresAuth: true },
        props: r => ({ idReservation: Number(r.params.idReservation), detailsType: 'Reservation' }),
    },
    {
        path: '/reservations',
        component: Reservation,
        meta: { requiresAuth: true } },

    {
        path: '/new-reservation',
        component: ReservationForm,
        name: 'NewReservation',
        meta: { requiresAuth: true } },

    {
        path: '/arrivals/:idReservation',
        component: ReservationForm,
        name: 'ArrivalDetails',
        meta: { requiresAuth: true },
        props: r => ({ idReservation: Number(r.params.idReservation), detailsType: 'Arrival' }),
    },
    {
        path: '/arrivals',
        component: Arrival,
        meta: { requiresAuth: true } },

    {
        path: '/guests/:idPerson',
        component: GuestForm,
        name: 'GuestDetails',
        meta: { requiresAuth: true },
        props: route => ({ idPerson: Number(route.params.idPerson) }),
    },
    {
        path: '/guests',
        component: Guest,
        name: 'Guests',
        meta: { requiresAuth: true } },
    {
        path: '/new-guest',
        component: GuestForm,
        name: 'NewGuest',
        meta: { requiresAuth: true } },

    {
        path: '/rooms/:idRoom',
        component: RoomForm,
        name: 'RoomDetails',
        meta: { requiresAuth: true },
        props: route => ({ idRoom: Number(route.params.idRoom) }),
    },
    {
        path: '/rooms',
        component: Room,
        name: 'Rooms',
        meta: { requiresAuth: true } },
    {
        path: '/new-room',
        component: RoomForm,
        name: 'NewRoom',
        meta: { requiresAuth: true } },

    {
        path: '/services/:idService',
        component: ServiceForm,
        name: 'ServiceDetails',
        meta: { requiresAuth: true },
        props: route => ({ idService: Number(route.params.idService) }),
    },
    {
        path: '/services',
        component: Service,
        name: 'Services',
        meta: { requiresAuth: true } },
    {
        path: '/new-service',
        component: ServiceForm,
        name: 'NewService',
        meta: { requiresAuth: true } },

    {
        path: '/history/:idReservation',
        component: HistoryDetails,
        name: 'HistoryDetails',
        meta: { requiresAuth: true },
        props: true
    },
    {
        path: '/history',
        component: History,
        meta: { requiresAuth: true }
    },
    {
        path: '/my-account',
        component: MyAccount,
        meta: { requiresAuth: true }
    },

    {
        path: '/employees',
        component: ListOfEmployees,
        meta: { requiresAuth: true, roles: ['Admin', 'Superadmin'] }
    },
    {
        path: '/hotel-settings',
        name: 'HotelSettings',
        component: HotelSettings,
        meta: { roles: ['Superadmin'] },
    },
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
