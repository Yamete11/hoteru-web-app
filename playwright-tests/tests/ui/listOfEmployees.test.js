const { test, expect } = require('@playwright/test');
const LoginPage = require("../../pages/login-page");
const Navbar = require("../../components/navbar");
const testData = require("../test-data/user-data");
const MyAccount = require("../../pages/my-account");
const ListOfEmployees = require("../../pages/list-of-employees");

test.beforeEach(async ({ browser }) => {
    const context = await browser.newContext();
    const page = await context.newPage();

    const loginPage = new LoginPage(page);

    await loginPage.goto();
    await loginPage.fillLoginForm(testData.validUsername, testData.validPassword);
    await loginPage.submitLoginForm();

    test.context = context;
    test.page = page;
});

test.afterEach(async () => {
    await test.context.close();
});

/*test('Check my account login', async () => {
    const page = test.page;
    const navbar = new Navbar(page);
    const myAccount = new MyAccount(page);

    await navbar.openMyAccount();

    await expect(await myAccount.getLoginValue()).toBe(testData.validUsername);
});*/

test('Creating new user', async () => {
    const page = test.page;
    const navbar = new Navbar(page);
    const listOfEmployees = new ListOfEmployees(page);

    await navbar.openListOfEmployees();

    const newUser = {
        name: 'Volodya',
        surname: 'Box',
        email: 'volodya@example.com',
        login: 'checkthisout',
        password: 'password123',
        userType: 'Employee'
    };

    await listOfEmployees.fillNewUserForm(newUser);
    await listOfEmployees.submitForm()

    await navbar.openListOfEmployees();

    const isUserPresent = await listOfEmployees.isUserPresent(newUser.login);
    expect(isUserPresent).toBe(true);

    await listOfEmployees.deleteUserByLogin(newUser.login)
});

