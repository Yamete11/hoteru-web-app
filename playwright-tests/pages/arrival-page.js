class ArrivalPage {
    constructor(page) {
        this.page = page;

        this.selectSearchType = page.getByTestId("arrival-search-select");
        this.searchInput = page.getByTestId("arrival-search-input");
        this.deleteReservationButton = page.getByTestId("delete-reservation-button");
    }

    async selectSearchField(fieldType) {
        await this.selectSearchType.selectOption(fieldType);
    }

    async enterSearchQuery(query) {
        await this.searchInput.fill(query);
    }

    async search(fieldType, query) {
        await this.selectSearchField(fieldType);
        await this.enterSearchQuery(query);
    }

    async deleteReservation() {
        await this.deleteReservationButton.first().waitFor({ state: 'visible' });

        await this.deleteReservationButton.first().click();
    }

}

module.exports = ArrivalPage;
