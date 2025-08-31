import { createRouter, createWebHistory } from 'vue-router'
import { me } from '@/api/auth'

const Login = () => import('@/pages/Login.vue')
const Reservation = () => import('@/pages/Reservation.vue')
const Arrival = () => import('@/pages/Arrival.vue')
const Guest = () => import('@/pages/Guest.vue')
const Room = () => import('@/pages/Room.vue')
const Service = () => import('@/pages/Service.vue')
const History = () => import('@/pages/History.vue')
const Registration = () => import('@/pages/Registration.vue')
const HistoryDetails = () => import('@/pages/forms/HistoryDetails.vue')
const ListOfEmployees = () => import('@/pages/ListOfEmployees.vue')
const MyAccount = () => import('@/pages/MyAccount.vue')
const ServiceForm = () => import('@/pages/forms/ServiceForm.vue')
const RoomForm = () => import('@/pages/forms/RoomForm.vue')
const GuestForm = () => import('@/pages/forms/GuestForm.vue')
const ReservationForm = () => import('@/pages/forms/ReservationForm.vue')
const HotelSettings = () => import('@/pages/HotelSettings.vue')

const routes = [
    { path: '/', component: Login, meta: { requiresAuth: false } },
    { path: '/registration', component: Registration, meta: { requiresAuth: false } },

    { path: '/reservations/:idReservation', component: ReservationForm, name: 'ReservationDetails', meta: { requiresAuth: true }, props: r => ({ idReservation: Number(r.params.idReservation), detailsType: 'Reservation' }) },
    { path: '/reservations', component: Reservation, meta: { requiresAuth: true } },
    { path: '/new-reservation', component: ReservationForm, name: 'NewReservation', meta: { requiresAuth: true } },

    { path: '/arrivals/:idReservation', component: ReservationForm, name: 'ArrivalDetails', meta: { requiresAuth: true }, props: r => ({ idReservation: Number(r.params.idReservation), detailsType: 'Arrival' }) },
    { path: '/arrivals', component: Arrival, meta: { requiresAuth: true } },

    { path: '/guests/:idPerson', component: GuestForm, name: 'GuestDetails', meta: { requiresAuth: true }, props: route => ({ idPerson: Number(route.params.idPerson) }) },
    { path: '/guests', component: Guest, name: 'Guests', meta: { requiresAuth: true } },
    { path: '/new-guest', component: GuestForm, name: 'NewGuest', meta: { requiresAuth: true } },

    { path: '/rooms/:idRoom', component: RoomForm, name: 'RoomDetails', meta: { requiresAuth: true }, props: route => ({ idRoom: Number(route.params.idRoom) }) },
    { path: '/rooms', component: Room, name: 'Rooms', meta: { requiresAuth: true } },
    { path: '/new-room', component: RoomForm, name: 'NewRoom', meta: { requiresAuth: true } },

    { path: '/services/:idService', component: ServiceForm, name: 'ServiceDetails', meta: { requiresAuth: true }, props: route => ({ idService: Number(route.params.idService) }) },
    { path: '/services', component: Service, name: 'Services', meta: { requiresAuth: true } },
    { path: '/new-service', component: ServiceForm, name: 'NewService', meta: { requiresAuth: true } },

    { path: '/history/:idReservation', component: HistoryDetails, name: 'HistoryDetails', meta: { requiresAuth: true }, props: true },
    { path: '/history', component: History, meta: { requiresAuth: true } },

    { path: '/my-account', component: MyAccount, meta: { requiresAuth: true } },

    { path: '/employees', component: ListOfEmployees, meta: { requiresAuth: true, roles: ['Admin', 'Superadmin'] } },
    { path: '/hotel-settings', name: 'HotelSettings', component: HotelSettings, meta: { requiresAuth: true, roles: ['Superadmin'] } },

    { path: '/user/:id', name: 'UserDetails', component: () => import('@/pages/MyAccount.vue'), props: true }
]

const router = createRouter({
    history: createWebHistory('/'),
    routes,
})

router.beforeEach(async (to, from, next) => {
    const requiresAuth = to.matched.some(r => r.meta?.requiresAuth)
    if (!requiresAuth) return next()

    try {
        const u = await me()
        const allowed = to.meta?.roles
        if (Array.isArray(allowed) && allowed.length > 0) {
            if (!u?.role || !allowed.includes(u.role)) return next('/arrivals')
        }
        return next()
    } catch {
        return next({ path: '/' })
    }
})


export default router
