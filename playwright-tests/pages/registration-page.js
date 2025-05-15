class RegistrationPage {
    constructor(page) {
        this.page = page;

        this.firstNameInput = page.getByTestId('registration-first-name');
        this.lastNameInput = page.getByTestId('registration-last-name');
        this.emailInput = page.getByTestId('registration-email');
        this.loginInput = page.getByTestId('registration-login');
        this.passwordInput = page.getByTestId('registration-password');
        this.companyNameInput = page.getByTestId('registration-company-name');
        this.countryInput = page.getByTestId('registration-country');
        this.cityInput = page.getByTestId('registration-city');
        this.streetInput = page.getByTestId('registration-street');
        this.postcodeInput = page.getByTestId('registration-postcode');

        this.cancelButton = page.getByTestId('registration-cancel');
        this.submitButton = page.getByTestId('registration-submit');
        this.errorMessages = page.getByTestId('error-message');
    }

    async goto() {
        await this.page.goto('http://localhost:5173/registration');
    }

    async fillRegistrationForm({
                                   firstName,
                                   lastName,
                                   email,
                                   login,
                                   password,
                                   companyName,
                                   country,
                                   city,
                                   street,
                                   postcode
                               }) {
        await this.firstNameInput.fill(firstName);
        await this.lastNameInput.fill(lastName);
        await this.emailInput.fill(email);
        await this.loginInput.fill(login);
        await this.passwordInput.fill(password);
        await this.companyNameInput.fill(companyName);
        await this.countryInput.fill(country);
        await this.cityInput.fill(city);
        await this.streetInput.fill(street);
        await this.postcodeInput.fill(postcode);
    }

    async submitForm() {
        await this.submitButton.click();
    }

    async cancelForm() {
        await this.cancelButton.click();
    }

    async assertRedirectToLogin() {
        await this.page.waitForURL('http://localhost:5173/');
    }

    async getErrorMessages() {
        const count = await this.errorMessages.count();
        const messages = [];
        for (let i = 0; i < count; i++) {
            messages.push(await this.errorMessages.nth(i).innerText());
        }
        return messages;
    }
}

module.exports = RegistrationPage;
