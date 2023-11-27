import Login from "../pages/Login.vue"
import {createRouter, createWebHistory} from "vue-router";
import Reservation from "../pages/Reservation.vue";
import Arrival from "../pages/Arrival.vue";
import Guest from "../pages/Guest.vue";
import Room from "../pages/Room.vue";
import Service from "../pages/Service.vue";
import History from "../pages/History.vue";
import Registration from "../pages/Registration.vue";
import NewRoom from "../pages/NewRoom.vue";
import NewService from "../pages/NewService.vue";
import RoomDetails from "../pages/RoomDetails.vue";
import ServiceDetails from "../pages/ServiceDetails.vue";


const routes = [
    {
        path: '/',
        component: Login
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
        path: '/arrivals',
        component: Arrival
    },
    {
        path: '/guests',
        component: Guest
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
        component: Service
    },
    {
        path: '/new-service',
        component: NewService
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