const { expect } = require('@playwright/test');

class ServicePage {
    constructor(page) {
        this.page = page;

        this.newServiceButton = page.getByTestId('new-service-button');
        this.serviceItemTitle = page.getByTestId('service-item-title');
        this.serviceItemPrice = page.getByTestId('service-item-price');
        this.serviceItemDescription = page.getByTestId('service-item-description');
        this.serviceItemDetailsButton = page.getByTestId('service-item-details-button');
    }

    async openNewService() {
        await this.newServiceButton.click();
    }

    async openDetails(){
        await this.serviceItemDetailsButton.last().click()
    }

    async assertValues(title, price, description) {
        await expect(this.serviceItemTitle.last()).toHaveText(title);
        await expect(this.serviceItemPrice.last()).toHaveText(String(price));
        await expect(this.serviceItemDescription.last()).toHaveText(description);
    }
}

module.exports = ServicePage;
