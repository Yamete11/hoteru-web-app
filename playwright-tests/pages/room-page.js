const { expect } = require('@playwright/test');

class RoomPage {
    constructor(page) {
        this.page = page;

        this.newRoomButton = page.getByTestId('new-room-button');
    }

    async openNewRoom() {
        await this.newRoomButton.click();
    }


}

module.exports = RoomPage;
