class SidebarComponent {
    constructor(page) {
        this.page = page;

        this.sidebar = page.getByTestId('sidebar');
        this.arrivalsButton = page.getByTestId('sidebar-button-arrivals');
        this.reservationsButton = page.getByTestId('sidebar-button-reservations');
        this.guestsButton = page.getByTestId('sidebar-button-guests');
        this.roomsButton = page.getByTestId('sidebar-button-rooms');
        this.servicesButton = page.getByTestId('sidebar-button-services');
        this.historyButton = page.getByTestId('sidebar-button-history');
    }

    async isSidebarVisible() {
        await expect(this.sidebar).toBeVisible();
    }

    async isButtonVisible(buttonName) {
        const button = this.getButton(buttonName);
        await expect(button).toBeVisible();
    }

    async clickButton(buttonName) {
        const button = this.getButton(buttonName);
        await button.click();
    }

    getButton(buttonName) {
        switch (buttonName.toLowerCase()) {
            case 'arrivals':
                return this.arrivalsButton;
            case 'reservations':
                return this.reservationsButton;
            case 'guests':
                return this.guestsButton;
            case 'rooms':
                return this.roomsButton;
            case 'services':
                return this.servicesButton;
            case 'history':
                return this.historyButton;
            default:
                throw new Error(`Unknown button: ${buttonName}`);
        }
    }
}

module.exports = SidebarComponent;
