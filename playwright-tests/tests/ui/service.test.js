const { test } = require('@playwright/test');
const LoginPage = require('../../pages/login-page');
const ServicePage = require('../../pages/service-page');
const NewServicePage = require('../../pages/new/new-service-page');
const SideBar = require('../../components/sidebar');
const testData = require('../test-data/user-data');
const ServiceDetailsPage = require("../../pages/details/service-details-page");

test.beforeEach(async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.goto();
    await loginPage.fillLoginForm(testData.validUsername, testData.validPassword);
    await loginPage.submitLoginForm();
});


test('Create new Service', async ({ page }) => {
    const sidebar = new SideBar(page);
    const servicePage = new ServicePage(page);
    const newServicePage = new NewServicePage(page);

    await sidebar.clickButton('services');
    await servicePage.openNewService();
    await newServicePage.fillForm(
        testData.serviceTitle,
        testData.servicePrice,
        testData.serviceDescription
    );
    await newServicePage.confirmForm();

    await servicePage.enterSearch(testData.serviceTitle)

    await servicePage.assertValues(
        testData.serviceTitle,
        testData.servicePrice,
        testData.serviceDescription
    );
});

test('Edit a service', async ({ page }) => {
    const sidebar = new SideBar(page);
    const servicePage = new ServicePage(page);
    const serviceDetailsPage = new ServiceDetailsPage(page);

    await sidebar.clickButton('services');

    await servicePage.openDetails();
    await serviceDetailsPage.saveForm();
    await serviceDetailsPage.fillForm(testData.editServiceTitle, testData.editServicePrice, testData.editServiceDescription)
    await serviceDetailsPage.saveForm();

    await sidebar.clickButton('services');

    await servicePage.assertValues(
        testData.editServiceTitle,
        testData.editServicePrice,
        testData.editServiceDescription
    );
    await servicePage.enterSearch(testData.editServiceTitle)
    await servicePage.deleteService()
});

