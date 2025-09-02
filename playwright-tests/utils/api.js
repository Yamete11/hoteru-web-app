import { request } from '@playwright/test';

export async function deleteHotelByTitle(hotelTitle) {
    const context = await request.newContext({
        baseURL: 'https://localhost:44384',
        ignoreHTTPSErrors: true
    });

    const response = await context.delete('/api/hotel', {
        data: { hotelTitle }
    });

    if (!response.ok()) {
        throw new Error(`Failed to delete hotel "${hotelTitle}". Status: ${response.status()}`);
    }

    await context.dispose();
}
