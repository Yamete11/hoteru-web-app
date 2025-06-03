const { test } = require('@playwright/test');
const LoginPage = require('../../pages/login-page');
const testData = require('../test-data/user-data');
const Navbar = require("../../components/navbar");



test.beforeEach(async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.goto();
    await loginPage.fillLoginForm(testData.validUsername, testData.validPassword);
    await loginPage.submitLoginForm();
});


test('Create new Reservation', async ({ page }) => {
    const navbar = new Navbar(page);
    await navbar.openNewReservation();
});

