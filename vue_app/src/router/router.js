import Login from "../pages/Login.vue"
import {createRouter, createWebHistory} from "vue-router";
import Reservation from "../pages/Reservation.vue";
import Arrival from "../pages/Arrival.vue";
import Guest from "../pages/Guest.vue";
import Room from "../pages/Room.vue";
import Service from "../pages/Service.vue";
import History from "../pages/History.vue";
import Registration from "../pages/Registration.vue";
import NewRoom from "../pages/new/NewRoom.vue";
import NewService from "../pages/new/NewService.vue";
import RoomDetails from "../pages/details/RoomDetails.vue";
import ServiceDetails from "../pages/details/ServiceDetails.vue";
import GuestDetails from "../pages/details/GuestDetails.vue";
import NewGuest from "../pages/new/NewGuest.vue";
import HistoryDetails from "../pages/details/HistoryDetails.vue";
import ListOfEmployees from "../pages/ListOfEmployees.vue";
import ArrivalDetails from "../pages/details/ArrivalDetails.vue";
import NewReservation from "../pages/new/NewReservation.vue";
import MyAccount from "../pages/MyAccount.vue";


const routes = [
    {
        path: '/',
        component: Login
    },
    {
        path: '/reservations/:idReservation',
        component: ArrivalDetails,
        name: 'ReservationDetails',
        meta: { requiresAuth: true },
        props: route => ({ idReservation: Number(route.params.idReservation), detailsType: 'Reservation' })
    },
    {
        path: '/reservations',
        component: Reservation,
        meta: { requiresAuth: true }
    },
    {
        path: '/new-reservation',
        component: NewReservation,
        meta: { requiresAuth: true }
    },
    {
        path: '/registration',
        component: Registration
    },
    {
        path: '/arrivals/:idReservation',
        component: ArrivalDetails,
        name: 'ArrivalDetails',
        meta: { requiresAuth: true },
        props: route => ({ idReservation: Number(route.params.idReservation), detailsType: 'Arrival' })
    },
    {
        path: '/arrivals',
        component: Arrival,
        meta: { requiresAuth: true }
    },
    {
        path: '/guests/:idPerson',
        component: GuestDetails,
        name: 'GuestDetails',
        meta: { requiresAuth: true },
        props: true
    },
    {
        path: '/guests',
        component: Guest,
        meta: { requiresAuth: true }
    },
    {
        path: '/new-guest',
        component: NewGuest,
        meta: { requiresAuth: true }
    },
    {
        path: '/rooms/:idRoom',
        component: RoomDetails,
        name: 'RoomDetails',
        meta: { requiresAuth: true },
        props: true
    },
    {
        path: '/rooms',
        component: Room,
        name: 'Rooms',
        meta: { requiresAuth: true }
    },
    {
        path: '/new-room',
        component: NewRoom,
        meta: { requiresAuth: true }
    },
    {
        path: '/services/:idService',
        component: ServiceDetails,
        name: 'ServiceDetails',
        meta: { requiresAuth: true },
        props: true
    },
    {
        path: '/services',
        component: Service,
        name: "Services",
        meta: { requiresAuth: true }
    },
    {
        path: '/new-service',
        component: NewService,
        meta: { requiresAuth: true }
    },
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
        meta: { requiresAuth: true }
    }
]



const router = createRouter({
    routes,
    history: createWebHistory("/")
})

router.beforeEach((to, from, next) => {
    const isAuthenticated = !!localStorage.getItem('token');

    if (to.matched.some(record => record.meta.requiresAuth) && !isAuthenticated) {
        next('/');
    } else if ((to.path === '/' || to.path === '/registration') && isAuthenticated) {
        next('/arrivals');
    } else {
        next();
    }
});


export default router;

