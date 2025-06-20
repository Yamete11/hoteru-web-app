const { expect } = require('@playwright/test');

class RoomDetailsPage {
    constructor(page) {
        this.page = page;
        this.numberInput = page.getByTestId('input-number');
        this.capacityInput = page.getByTestId('input-capacity');
        this.priceInput = page.getByTestId('input-price');
        this.typeSelect = page.getByTestId('room-type-select');
        this.statusSelect = page.getByTestId('room-status-select');
        this.backButton = page.getByTestId('btn-back');
        this.submitButton = page.getByTestId('btn-submit');

    }

    async changeStatus(status) {
        await this.statusSelect.selectOption(status);
    }


    async clickSubmit() {
        await this.submitButton.click();
    }

    async clickBack() {
        await this.backButton.click();
    }

}

module.exports = RoomDetailsPage;
