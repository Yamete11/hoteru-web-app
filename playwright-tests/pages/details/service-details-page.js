const { expect } = require('@playwright/test');

class ServiceDetailsPage {
    constructor(page) {
        this.page = page;

        this.titleInput = page.getByTestId('input-title');
        this.priceInput = page.getByTestId('input-price');
        this.descriptionInput = page.getByTestId('input-description');
        this.backButton = page.getByTestId('button-back');
        this.editSaveButton = page.getByTestId('button-edit-save');
    }

    async fillForm(title, price, description){
        await this.titleInput.fill(title);
        await this.priceInput.fill(price);
        await this.descriptionInput.fill(description);
    }

    async saveForm(){
        await this.editSaveButton.click();
    }

}

module.exports = ServiceDetailsPage;
