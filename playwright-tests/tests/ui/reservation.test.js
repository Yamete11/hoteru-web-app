const { test, expect} = require('@playwright/test');
const LoginPage = require('../../pages/login-page');
const ArrivalPage = require('../../pages/arrival-page');
const RoomPage = require('../../pages/room-page');
const RoomDetailsPage = require('../../pages/details/room-details-page');
const NewReservationPage = require('../../pages/new/new-reservation-page');
const testData = require('../test-data/user-data');
const Navbar = require("../../components/navbar");
const Sidebar = require("../../components/sidebar");



test.beforeEach(async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.goto();
    await loginPage.fillLoginForm(testData.validUsername, testData.validPassword);
    await loginPage.submitLoginForm();
});


test('Create new Reservation', async ({ page }) => {
    const navbar = new Navbar(page);
    const newReservationPage = new NewReservationPage(page)
    const arrivalPage = new ArrivalPage(page);

    await navbar.openNewReservation();

    await newReservationPage.fillReservationForm(
        testData.reservationInDate,
        testData.reservationOutDate,
        testData.reservationCapacity,
        testData.reservationRoomType,
        testData.reservationRoom,
        testData.reservationGuest,
        testData.reservationDepositSum,
        testData.reservationDepositType,
        testData.reservationService
    )

    await newReservationPage.submitForm()
    await arrivalPage.search("Room", "104")
    await arrivalPage.deleteReservation()

});

test.afterEach(async ({ page }) => {
    const sidebar = new Sidebar(page)
    const roomPage = new RoomPage(page)
    const roomDetailsPage = new RoomDetailsPage(page)
    await sidebar.clickButton("rooms")

    await roomPage.fillSearchInput("104")
    await roomPage.openDetails();
    await roomDetailsPage.clickSubmit()
    await roomDetailsPage.changeStatus('3')
    await roomDetailsPage.clickSubmit()
});

