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
        path: '/rooms',
        component: Room,
    },
    {
        path: '/new-room',
        component: NewRoom
    },
    {
        path: '/services',
        component: Service
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