const { test, expect } = require('@playwright/test');
const RegistrationPage = require('../../pages/registration-page');
const LoginPage = require('../../pages/login-page');
const testData = require('../test-data/user-data');
import { deleteHotelByTitle } from '../../utils/api';


test('Successful registration redirects to login page', async ({ page }) => {
    const registrationPage = new RegistrationPage(page);
    const loginPage = new LoginPage(page);

    await registrationPage.goto();

    await registrationPage.fillRegistrationForm(testData.newRegistrationUser);
    await registrationPage.submitForm();

    await registrationPage.assertRedirectToLogin();
    expect(await loginPage.isLoginButtonVisible()).toBeTruthy();

    await deleteHotelByTitle(testData.newRegistrationUser.companyName);
});

test('Submitting empty registration form shows error', async ({ page }) => {
    const registrationPage = new RegistrationPage(page);

    await registrationPage.goto();
    await registrationPage.submitForm();

    const errorMessages = await registrationPage.getErrorMessages();

    for(const message of errorMessages) {
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

