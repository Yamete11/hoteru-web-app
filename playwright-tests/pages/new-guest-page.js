const { expect } = require('@playwright/test');

class NewGuestPage {
    constructor(page) {
        this.page = page;
        this.nameInput = page.getByTestId('name-input');
        this.surnameInput = page.getByTestId('surname-input');
        this.emailInput = page.getByTestId('email-input');
        this.telInput = page.getByTestId('tel-input');
        this.passportInput = page.getByTestId('passport-input');
        this.statusSelect = page.getByTestId('status-select');

        this.cancelButton = page.getByTestId('cancel-button');
        this.confirmButton = page.getByTestId('confirm-button');
    }

    async fillForm(name, surname, email, tel, passport, status) {
        await this.nameInput.fill(name);
        await this.surnameInput.fill(surname);
        await this.emailInput.fill(email);
        await this.telInput.fill(tel);
        await this.passportInput.fill(passport);
        await this.statusSelect.selectOption(status);
    }

    async submitForm() {
        await this.confirmButton.click();
    }

    async cancelForm() {
        await this.cancelButton.click();
    }

}
module.exports = NewGuestPage;
