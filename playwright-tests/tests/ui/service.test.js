const { test } = require('@playwright/test');
const LoginPage = require('../../pages/login-page');
const ServicePage = require('../../pages/service-page');
const NewServicePage = require('../../pages/new-service-page');
const SideBar = require('../../components/sidebar');
const testData = require('../test-data/user-data');

test('Create new Service', async ({ page }) => {
    const loginPage = new LoginPage(page);
    const sidebar = new SideBar(page);
    const servicePage = new ServicePage(page);
    const newServicePage = new NewServicePage(page);

    await loginPage.goto();
    await loginPage.fillLoginForm(testData.validUsername, testData.validPassword);
    await loginPage.submitLoginForm();

    await sidebar.clickButton('services');
    await servicePage.openNewService();
    await newServicePage.fillForm(
        testData.serviceTitle,
        testData.servicePrice,
        testData.serviceDescription
    );
    await newServicePage.confirmForm();

    await servicePage.assertValues(
        testData.serviceTitle,
        testData.servicePrice,
        testData.serviceDescription
    );
});
