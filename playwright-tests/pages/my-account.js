class MyAccount {
    constructor(page) {
        this.page = page;
        this.inputName = page.getByTestId('input-name');
        this.inputSurname = page.getByTestId('input-surname');
        this.inputEmail = page.getByTestId('input-email');
        this.inputLogin = page.getByTestId('input-login');
        this.inputTypeReadonly = page.getByTestId('input-type-readonly');
        this.selectUserType = page.getByTestId('select-user-type');
        this.backButton = page.getByTestId('button-back');
        this.submitButton = page.getByTestId('button-submit');
    }

    async selectUserType(value) {
        await this.selectUserType.selectOption(value);
    }


    async getLoginValue() {
        return await this.inputLogin.inputValue();
    }

}

module.exports = MyAccount;
