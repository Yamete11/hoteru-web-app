class LoginPage {
    constructor(page) {
        this.page = page;
        this.loginInput = page.getByTestId('login-input');
        this.passwordInput = page.getByTestId('password-input');
        this.loginButton = page.getByTestId('login-button');
        this.errorMessage = page.getByTestId('error-message');
    }

    async goto() {
        await this.page.goto('http://localhost:5173/');
    }

    async fillLoginForm(username, password) {
        await this.loginInput.fill(username);
        await this.passwordInput.fill(password);
    }

    async submitLoginForm() {
        await this.loginButton.click();
    }

    async getErrorMessage() {
        return await this.errorMessage.textContent();
    }

    async assertRedirectToArrivals() {
        await this.page.waitForURL(/\/arrivals/);
    }

    async isLoginButtonVisible() {
        return await this.loginButton.isVisible();
    }
}

module.exports = LoginPage;
