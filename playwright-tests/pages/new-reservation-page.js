const { expect } = require('@playwright/test');

class NewReservationPage {
    constructor(page) {
        this.page = page;
        this.dateInInput = page.getByTestId('date-in');
        this.dateOutInput = page.getByTestId('date-out');

        this.capacityInput = page.getByTestId('capacity');
        this.roomTypeSelect = page.getByTestId('room-type-select');
        this.roomSelect = page.getByTestId('room-select');

        this.guestSelect = page.getByTestId('guest-select');

        this.depositInput = page.getByTestId('deposit-input');
        this.depositSelect = page.getByTestId('deposit-select');
        this.addDepositButton = page.getByTestId('add-deposit-btn');

        this.serviceSelect = page.getByTestId('service-select');
        this.addServiceButton = page.getByRole('button', { name: 'Add' });

        this.submitButton = page.getByTestId('submit-button');
        this.cancelButton = page.getByTestId('cancel-button');
    }

    async fillReservationForm(inDate, outDate, capacity, roomType, room, guest, depositSum, depositType, service) {
        await this.dateInInput.fill(inDate);
        await this.dateOutInput.fill(outDate);
        await this.capacityInput.fill(capacity.toString());

        await this.roomTypeSelect.selectOption(roomType);
        await this.roomSelect.selectOption(room);

        await this.guestSelect.selectOption(guest);

        if (depositSum && depositType) {
            await this.addDepositButton.click();
            await this.depositInput.fill(depositSum.toString());
            await this.depositSelect.selectOption(depositType);
        }

        if (service) {
            await this.serviceSelect.selectOption(service);
            await this.addServiceButton.click();
        }
    }


    async submitForm() {
        await this.submitButton.click();
    }

    async cancelForm() {
        await this.cancelButton.click();
    }



}
module.exports = NewReservationPage;
