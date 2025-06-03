const { test, expect } = require('@playwright/test');
const LoginPage = require('../../pages/login-page');
const testData = require('../test-data/user-data');

test('Login with valid credentials', async ({ page }) => {
    const loginPage = new LoginPage(page);

    await loginPage.goto();
    await loginPage.fillLoginForm(testData.validUsername, testData.validPassword);
    await loginPage.submitLoginForm();
    await loginPage.assertRedirectToArrivals();
});

test('Login with invalid credentials shows error message', async ({ page }) => {
    const loginPage = new LoginPage(page);

    await loginPage.goto();
    await loginPage.fillLoginForm(testData.invalidUsername, testData.invalidPassword);
    await loginPage.submitLoginForm();

    const error = await loginPage.getErrorMessage();
    expect(error).toContain('Validation failed');
});

test('Login with empty fields does not proceed and shows error', async ({ page }) => {
    const loginPage = new LoginPage(page);

    await loginPage.goto();
    await loginPage.fillLoginForm('', '');
    await loginPage.submitLoginForm();

    const error = await loginPage.getErrorMessage();
    expect(error).toContain('Validation failed');
});

test('Only password entered shows validation error', async ({ page }) => {
    const loginPage = new LoginPage(page);

    await loginPage.goto();
    await loginPage.fillLoginForm('', testData.validPassword);
    await loginPage.submitLoginForm();

    const error = await loginPage.getErrorMessage();
    expect(error).toContain('Validation failed');
});



