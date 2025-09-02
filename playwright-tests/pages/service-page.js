const { expect } = require('@playwright/test');

class ServicePage {
    constructor(page) {
        this.page = page;

        this.newServiceButton = page.getByTestId('new-service-button');
        this.serviceItemTitle = page.getByTestId('service-item-title');
        this.serviceItemPrice = page.getByTestId('service-item-price');
        this.searchInput = page.getByTestId('search-input');
        this.serviceItemDescription = page.getByTestId('service-item-description');
        this.serviceItemDetailsButton = page.getByTestId('service-item-details-button');
        this.serviceItemDeleteButton = page.getByTestId('service-item-delete-button');
    }

    async openNewService() {
        await this.newServiceButton.click();
    }

    async openDetails(){
        await this.serviceItemDetailsButton.last().click();
    }

    async assertValues(title, price, description) {
        await expect(this.serviceItemTitle.last()).toHaveText(title);
        await expect(this.serviceItemPrice.last()).toHaveText(String(price));
        await expect(this.serviceItemDescription.last()).toHaveText(description);
    }

    async enterSearch(text){
        await this.searchInput.fill(text);
    }


    async deleteServiceByTitle(title) {
        await this.enterSearch(title);
        const row = this.page.locator('.item-div').filter({ has: this.page.getByTestId('service-item-title').filter({ hasText: title }) });
        await expect(row).toBeVisible({ timeout: 5000 });

        await this.serviceItemDeleteButton.click();
        await expect(row).toHaveCount(0, { timeout: 5000 });
    }
}

module.exports = ServicePage;
