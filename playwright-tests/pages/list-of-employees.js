class ListOfEmployees {
    constructor(page) {
        this.page = page;

        this.nameInput = page.getByTestId('input-name');
        this.surnameInput = page.getByTestId('input-surname');
        this.emailInput = page.getByTestId('input-email');
        this.loginInput = page.getByTestId('input-login');
        this.passwordInput = page.getByTestId('input-password');
        this.userTypeSelect = page.getByTestId('select-user-type');

        this.cleanButton = page.getByTestId('clean-button');
        this.addUserButton = page.getByTestId('add-user-button');

        this.userList = page.getByTestId('user-list');
    }

    async fillNewUserForm(user) {
        await this.nameInput.fill(user.name);
        await this.surnameInput.fill(user.surname);
        await this.emailInput.fill(user.email);
        await this.loginInput.fill(user.login);
        await this.passwordInput.fill(user.password);
        await this.userTypeSelect.selectOption({ label: user.userType });
    }

    async submitForm() {
        await this.addUserButton.click();
    }

    async cleanForm() {
        await this.cleanButton.click();
    }

    async deleteUserByLogin(login) {
        const userLocator = this.page.getByTestId(`user-item-${login}`);
        const removeBtn = userLocator.getByTestId(`delete-user-${login}`);
        if (await removeBtn.isVisible()) {
            await removeBtn.click();
        }
    }

    async isUserPresent(login) {
        const user = this.page.getByTestId(`user-item-${login}`);
        try {
            await user.waitFor({ state: 'visible', timeout: 5000 });
            return true;
        } catch {
            return false;
        }
    }
}

module.exports = ListOfEmployees;
