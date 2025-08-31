import http from '@/lib/http'
import { ENDPOINTS } from '@/config/api'

export async function login({ login, password }) {
    const { data } = await http.post(ENDPOINTS.AUTH.LOGIN, { login, password })
    return data
}
export async function refresh() {
    const { data } = await http.post(`${ENDPOINTS.AUTH.LOGIN}/refresh`)
    return data
}
export async function logout() {
    const { data } = await http.post(`${ENDPOINTS.AUTH.LOGIN}/logout`)
    return data
}
export async function me() {
    const { data } = await http.get(`${ENDPOINTS.AUTH.LOGIN}/me`)
    return data
}
