const { expect } = require('@playwright/test');

class RoomPage {
    constructor(page) {
        this.page = page;

        this.newRoomButton = page.getByTestId('new-room-button');
        this.searchInputField = page.getByTestId('search-input');

        this.roomNumber = page.getByTestId('room-number');
        this.roomCapacity = page.getByTestId('room-capacity');
        this.roomType = page.getByTestId('room-type');
        this.roomStatus = page.getByTestId('room-status');
        this.deleteRoomButton = page.getByTestId('delete-room-button');
        this.roomItemDetailsButton = page.getByTestId('room-item-details-button');

    }

    async openNewRoom() {
        await this.newRoomButton.click();
    }

    async fillSearchInput(text) {
        await this.searchInputField.fill(text);
    }

    async assertValues(number, capacity){
        await expect(this.roomNumber.last()).toHaveText(number);
        await expect(this.roomCapacity.last()).toHaveText(String(capacity));
    }

    async deleteRoomByNumber(number) {
        await this.fillSearchInput(String(number));
        await this.page.waitForTimeout(400);

        const row = this.page
            .locator('.item-div')
            .filter({ has: this.page.getByTestId('room-number').filter({ hasText: String(number) }) });

        await row.first().waitFor({ state: 'visible', timeout: 3000 });
        await row.getByTestId('delete-room-button').click();

        await row.first().waitFor({ state: 'detached', timeout: 3000 }).catch(() => {});
    }

    async openDetails(){
        await this.roomItemDetailsButton.last().click()
    }
}

module.exports = RoomPage;
