const { defineConfig } = require('@playwright/test');

module.exports = defineConfig({
    workers: 1,
    testDir: './tests',
    timeout: 30000,
    retries: 1,
    reporter: 'html',
    use: {
        headless: false,
        viewport: { width: 1280, height: 720 },
        video: 'on-first-retry',
    },
});
