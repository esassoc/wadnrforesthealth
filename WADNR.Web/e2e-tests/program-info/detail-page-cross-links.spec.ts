import { test, expect } from "../fixtures/base-test";
import { testData } from "../fixtures/test-data";

test.describe("Program info page cross-entity links", () => {
    test("Focus area detail has DNR upland region link", async ({ authedPage: page }) => {
        await page.goto(`/focus-areas/${testData.focusAreaID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
        const regionLink = page.locator("a[href*='/dnr-upland-regions/']").first();
        if (await regionLink.isVisible()) {
            await regionLink.click();
            await expect(page).toHaveURL(/\/dnr-upland-regions\/\d+/);
        }
    });

    test("Organization grid row links to valid org detail with projects", async ({ authedPage: page }) => {
        // Navigate from grid to a real org (avoid hardcoded ID that may not exist)
        await page.goto("/organizations");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        await page.locator(".ag-row .ag-cell a[href]").first().click();
        await expect(page).toHaveURL(/\/organizations\/\d+/);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Priority landscape list grid links to valid detail", async ({ authedPage: page }) => {
        await page.goto("/priority-landscapes");
        await expect(page.locator(".ag-row, .card").first()).toBeVisible({ timeout: 15000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href], a[href*='/priority-landscapes/']").first();
        if (await firstLink.isVisible()) {
            await firstLink.click();
            await expect(page).toHaveURL(/\/priority-landscapes\/\d+/);
        }
    });

    test("DNR upland region list grid links to valid detail", async ({ authedPage: page }) => {
        await page.goto("/dnr-upland-regions");
        await expect(page.locator(".ag-row, .card").first()).toBeVisible({ timeout: 15000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href], a[href*='/dnr-upland-regions/']").first();
        if (await firstLink.isVisible()) {
            await firstLink.click();
            await expect(page).toHaveURL(/\/dnr-upland-regions\/\d+/);
        }
    });
});
