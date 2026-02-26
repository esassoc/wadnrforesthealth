import { test, expect } from "../fixtures/base-test";
import { testData } from "../fixtures/test-data";

test.describe("Project detail cross-entity links", () => {
    test.beforeEach(async ({ authedPage: page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await expect(page.locator("#card-basics")).toBeVisible({ timeout: 15000 });
    });

    test("Organization link in Basics card navigates to org detail", async ({ authedPage: page }) => {
        const orgLink = page.locator("#card-basics a[href*='/organizations/']").first();
        if (await orgLink.isVisible()) {
            await orgLink.click();
            await expect(page).toHaveURL(/\/organizations\/\d+/);
            await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
        }
    });

    test("County link in Location card navigates to county detail", async ({ authedPage: page }) => {
        const countyLink = page.locator("#card-location a[href*='/counties/']").first();
        if (await countyLink.isVisible()) {
            await countyLink.click();
            await expect(page).toHaveURL(/\/counties\/\d+/);
        }
    });

    test("Program link in Basics card navigates to program detail", async ({ authedPage: page }) => {
        const programLink = page.locator("#card-basics a[href*='/programs/']").first();
        if (await programLink.isVisible()) {
            await programLink.click();
            await expect(page).toHaveURL(/\/programs\/\d+/);
        }
    });

    test("View Fact Sheet button navigates to fact sheet page", async ({ authedPage: page }) => {
        const factSheetLink = page.locator("a[href*='/fact-sheet/']").first();
        if (await factSheetLink.isVisible()) {
            await factSheetLink.click();
            await expect(page).toHaveURL(/\/projects\/fact-sheet\/\d+/);
        }
    });

    test("Fund source allocation link in Funding card navigates to allocation detail", async ({ authedPage: page }) => {
        const allocationLink = page.locator("#card-funding a[href*='/fund-source-allocations/']").first();
        if (await allocationLink.isVisible()) {
            await allocationLink.click();
            await expect(page).toHaveURL(/\/fund-source-allocations\/\d+/);
        }
    });
});
