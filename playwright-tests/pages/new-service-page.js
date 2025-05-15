class NewServicePage {
    constructor(page) {
        this.page = page;
        this.titleInput = page.getByTestId('service-title');
        this.priceInput = page.getByTestId('service-price');
        this.descriptionInput = page.getByTestId('service-description');
        this.confirmButton = page.getByTestId('service-confirm-button');
    }

    async fillForm(title, price, description){
        await this.titleInput.fill(title)
        await this.priceInput.fill(String(price))
        await this.descriptionInput.fill(description)
    }

    async confirmForm(){
        await this.confirmButton.click()
    }
}

module.exports = NewServicePage;
