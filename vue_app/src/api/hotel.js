import http from '@/lib/http'
import { ENDPOINTS } from '@/config/api'

export async function get() {
    const { data } = await http.get(ENDPOINTS.HOTEL.ROOT)
    return data
}

export async function update(dto) {
    const { data } = await http.put(ENDPOINTS.HOTEL.ROOT, dto)
    return data
}
