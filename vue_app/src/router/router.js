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
import ReservationDetails from "../pages/details/ReservationDetails.vue";
import ArrivalDetails from "../pages/details/ArrivalDetails.vue";
import NewReservation from "../pages/new/NewReservation.vue";


const routes = [
    {
        path: '/',
        component: Login
    },
    {
        path: '/new-reservation',
        component: NewReservation
    },
    {
        path: '/reservations/:idReservation',
        component: ReservationDetails,
        name: 'ReservationDetails',
        props: true
    },
    {
        path: '/reservations',
        component: Reservation
    },
    {
        path: '/registration',
        component: Registration
    },
    {
        path: '/arrivals/:idReservation',
        component: ArrivalDetails,
        name: 'ArrivalDetails',
        props: true
    },
    {
        path: '/arrivals',
        component: Arrival
    },
    {
        path: '/guests/:idPerson',
        component: GuestDetails,
        name: 'GuestDetails',
        props: true
    },
    {
        path: '/guests',
        component: Guest
    },
    {
        path: '/new-guest',
        component: NewGuest
    },
    {
        path: '/rooms/:idRoom',
        component: RoomDetails,
        name: 'RoomDetails',
        props: true
    },
    {
        path: '/rooms',
        component: Room,
        name: 'Rooms'
    },
    {
        path: '/new-room',
        component: NewRoom
    },
    {
        path: '/services/:idService',
        component: ServiceDetails,
        name: 'ServiceDetails',
        props: true
    },
    {
        path: '/services',
        component: Service,
        name: "Services"
    },
    {
        path: '/new-service',
        component: NewService
    },
    {
        path: '/history/:idReservation',
        component: HistoryDetails,
        name: 'HistoryDetails',
        props: true
    },
    {
        path: '/history',
        component: History
    }
]

const router = createRouter({
    routes,
    history: createWebHistory("/")
})

export default router;