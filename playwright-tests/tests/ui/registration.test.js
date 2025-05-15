const { test, expect } = require('@playwright/test');
const RegistrationPage = require('../../pages/registration-page');
const LoginPage = require('../../pages/login-page');

/*test('Successful registration redirects to login page', async ({ page }) => {
    const registrationPage = new RegistrationPage(page);
    const loginPage = new LoginPage(page);

    await registrationPage.goto();

    const userData = {
        firstName: 'Jack',
        lastName: 'Mathews',
        email: 'glebasher11@gmail.com',
        login: 'zxc',
        password: 'zxc',
        companyName: 'TestCorp',
        country: 'USA',
        city: 'California',
        street: '5thAvenue',
        postcode: '10001'
    };

    await registrationPage.fillRegistrationForm(userData);
    await registrationPage.submitForm();

    await registrationPage.assertRedirectToLogin();
    expect(await loginPage.isLoginButtonVisible()).toBeTruthy();
});*/

test('Submitting empty registration form shows error', async ({ page }) => {
    const registrationPage = new RegistrationPage(page);

    await registrationPage.goto();
    await registrationPage.submitForm();

    const errorMessages = await registrationPage.getErrorMessages();

    for (const message of errorMessages) {
        expect(message).toContain('This field is required');
    }
});


test('Cancel button redirects to login page', async ({ page }) => {
    const registrationPage = new RegistrationPage(page);
    const loginPage = new LoginPage(page);

    await registrationPage.goto();
    await registrationPage.cancelForm();

    await registrationPage.assertRedirectToLogin();
    expect(await loginPage.isLoginButtonVisible()).toBeTruthy();
});

