const { test } = require('@playwright/test');
const LoginPage = require('../../pages/login-page');
const RoomPage = require('../../pages/room-page');
const NewRoomPage = require('../../pages/new-room-page');
const SideBar = require('../../components/sidebar');
const testData = require('../test-data/user-data');

test.beforeEach(async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.goto();
    await loginPage.fillLoginForm(testData.validUsername, testData.validPassword);
    await loginPage.submitLoginForm();
});


test('Create new Room', async ({ page }) => {
    const sidebar = new SideBar(page);
    const roomPage = new RoomPage(page);
    const newRoomPage = new NewRoomPage(page);

    await sidebar.clickButton('rooms');
    await roomPage.openNewRoom();
    await newRoomPage.fillForm(
        testData.roomNumber,
        testData.roomCapacity,
        testData.roomPrice,
        testData.roomType,
        testData.roomStatus
    );
    await newRoomPage.submitForm();

    await roomPage.fillSearchInput(testData.roomNumber);

    await roomPage.assertValues(
        testData.roomNumber,
        testData.roomCapacity,
    );

    await roomPage.deleteRoom();
});

