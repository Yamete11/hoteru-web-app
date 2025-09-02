const { expect } = require('@playwright/test');

class NewReservationPage {
    constructor(page) {
        this.page = page;
        this.dateInInput = page.getByTestId('date-in');
        this.dateOutInput = page.getByTestId('date-out');

        this.capacityInput = page.getByTestId('capacity-input');
        this.roomTypeSelect = page.getByTestId('room-type-select');
        this.roomSelect = page.getByTestId('room-select');
        this.addRoomButton = page.getByTestId('add-room-btn');

        this.guestSelect = page.getByTestId('guest-select');
        this.addGuestButton = page.getByTestId('add-guest-btn');

        this.depositInput = page.getByTestId('deposit-input');
        this.depositSelect = page.getByTestId('deposit-select');
        this.addDepositButton = page.getByTestId('add-deposit-btn');

        this.serviceSelect = page.getByTestId('service-select');
        this.addServiceButton = page.getByTestId('add-service-btn');

        this.submitButton = page.getByTestId('submit-button');
        this.cancelButton = page.getByTestId('cancel-button');
    }

    async fillReservationForm(capacity, roomType, room, guest, depositSum, depositType, service) {

        await this.capacityInput.waitFor({ state: 'visible' });
        await this.capacityInput.click({ clickCount: 2 });
        await this.capacityInput.press('Backspace');
        await this.capacityInput.type(String(capacity));

        await this.roomTypeSelect.selectOption({ label: roomType });
        await this.roomSelect.waitFor({ state: 'visible' });
        await this.roomSelect.selectOption({ label: room });
        await this.addRoomButton.click();


        await this.guestSelect.selectOption({ label: guest });
        await this.addGuestButton.click();

        await this.addDepositButton.click();

        await this.depositInput.fill(String(depositSum));
        await this.depositSelect.selectOption({ label: depositType });

        await this.serviceSelect.selectOption({ label:  service });
        await this.addServiceButton.click();

    }


    async submitForm() {
        await this.submitButton.waitFor({ state: 'visible' });
        await this.submitButton.click();
    }

    async cancelForm() {
        await this.cancelButton.click();
    }



}
module.exports = NewReservationPage;
