class Navbar {
    constructor(page) {
        this.page = page;

        this.newReservation = page.getByTestId('new-reservation-button');
        this.settings = page.getByTestId('settings-button');
        this.listOfEmployees = page.getByTestId('employees-link');
        this.myAccount = page.getByTestId('my-account-link');
        this.logout = page.getByTestId('log-out');
    }

    async openNewReservation() {
        await this.newReservation.click();
    }

    async openSettingsDropdown() {
        await this.settings.hover();
    }

    async openMyAccount() {
        await this.openSettingsDropdown();
        await this.myAccount.click();
    }

    async openListOfEmployees() {
        await this.openSettingsDropdown();
        await this.listOfEmployees.click();
    }

    async logoutUser() {
        await this.logout.click();
    }
}

module.exports = Navbar;

