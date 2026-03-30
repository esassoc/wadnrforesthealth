import { test, expect } from "../fixtures/base-test";

test.describe("Grid interactivity", () => {
    test.beforeEach(async ({ authedPage: page }) => {
        await page.goto("/projects");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
    });

    test("Clicking a column header shows sort indicator", async ({ authedPage: page }) => {
        const firstHeader = page.locator(".ag-header-cell").first();
        await firstHeader.click();
        await expect(
            page.locator(".ag-sort-ascending-icon:visible, .ag-sort-descending-icon:visible").first()
        ).toBeVisible({ timeout: 5000 });
    });

    test("CSV download button is clickable", async ({ authedPage: page }) => {
        const csvButton = page.getByTitle("Download grid data as CSV");
        await expect(csvButton).toBeVisible();
        await expect(csvButton).toBeEnabled();
    });

    test("Fullscreen toggle works", async ({ authedPage: page }) => {
        const fullscreenButton = page.getByTitle("Make grid full screen");
        await expect(fullscreenButton).toBeVisible();
        await fullscreenButton.click();
        await expect(page.getByTitle("Exit full screen")).toBeVisible({ timeout: 5000 });
    });
});
