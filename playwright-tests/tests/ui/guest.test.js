const { test } = require('@playwright/test');
const LoginPage = require('../../pages/login-page');
const GuestPage = require('../../pages/guest-page');
const NewGuestPage = require('../../pages/new-guest-page');
const SideBar = require('../../components/sidebar');
const testData = require('../test-data/user-data');

test.beforeEach(async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.goto();
    await loginPage.fillLoginForm(testData.validUsername, testData.validPassword);
    await loginPage.submitLoginForm();
});

test('Create new Guest', async ({ page }) => {
    const sidebar = new SideBar(page);
    const guestPage = new GuestPage(page);
    const newGuestPage = new NewGuestPage(page);

    await sidebar.clickButton('guests');
    await guestPage.openNewGuest();
    await newGuestPage.fillForm(
        testData.guestName,
        testData.guestSurname,
        testData.guestEmail,
        testData.guestTelNumber,
        testData.guestPassport,
        testData.guestStatus
    );
    await newGuestPage.submitForm();

    /*await guestPage.assertValues(
        testData.guestName,
        testData.guestSurname,
        testData.guestEmail,
        testData.guestTelNumber,
    );*/
});
