const { expect } = require('@playwright/test');

class NewRoomPage {
    constructor(page) {
        this.page = page;
        this.numberInput = page.getByTestId('number-input');
        this.capacityInput = page.getByTestId('capacity-input');
        this.priceInput = page.getByTestId('price-input');
        this.typeSelect = page.getByTestId('type-select');
        this.statusSelect = page.getByTestId('status-select');

        this.cancelButton = page.getByTestId('cancel-button');
        this.submitButton = page.getByTestId('submit-button');
    }


    async fillForm(number, capacity, price, type, status) {
        await this.numberInput.fill(number);
        await this.capacityInput.fill(capacity);
        await this.priceInput.fill(price);
        await this.typeSelect.selectOption(type);
        await this.statusSelect.selectOption(status);
    }

    async submitForm() {
        await this.submitButton.click();
    }

    async cancelForm() {
        await this.cancelButton.click();
    }

    async assertValues(number, capacity, price) {
        await expect(this.roomNumberCell).toHaveText(number);
        await expect(this.roomCapacityCell).toHaveText(String(capacity));
        await expect(this.roomPriceCell).toHaveText(String(price));
    }
}

module.exports = NewRoomPage;
