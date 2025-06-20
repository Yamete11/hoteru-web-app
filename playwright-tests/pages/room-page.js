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

    async deleteRoom(){
        await this.deleteRoomButton.click();
    }

    async openDetails(){
        await this.roomItemDetailsButton.last().click()
    }
}

module.exports = RoomPage;
