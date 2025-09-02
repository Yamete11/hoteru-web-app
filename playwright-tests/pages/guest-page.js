const { expect } = require('@playwright/test');

class GuestPage {
    constructor(page) {
        this.page = page;

        this.newGuestButton = page.getByTestId('new-guest-button');
        this.guestItemName = page.getByTestId('guest-name');
        this.guestItemSurname = page.getByTestId('guest-surname');
        this.guestItemTelNumber = page.getByTestId('guest-telNumber');
        this.guestItemEmail = page.getByTestId('guest-email');
        this.searchInput = page.getByTestId('guest-search-input');
        this.deleteGuestButton = page.getByTestId('delete-guest-button');
    }

    async openNewGuest() {
        await this.newGuestButton.click();
    }

    async assertValues(name, surname, telNumber, email) {
        await expect(this.guestItemName.last()).toHaveText(name);
        await expect(this.guestItemSurname.last()).toHaveText(surname);
        await expect(this.guestItemTelNumber.last()).toHaveText(telNumber);
        await expect(this.guestItemEmail.last()).toHaveText(email);
    }

    async enterSearchQuery(query) {
        await this.searchInput.fill(query);
        await this.page.waitForTimeout(400);
    }

    async deleteGuestByName(name) {
        await this.enterSearchQuery(name);

        await this.guestItemName.last().waitFor({ state: 'visible', timeout: 3000 });

        await this.deleteGuestButton.last().click();

        await this.guestItemName.last().waitFor({ state: 'detached', timeout: 3000 }).catch(() => {});
    }

}

module.exports = GuestPage;
