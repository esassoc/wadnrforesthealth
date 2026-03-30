import { test, expect } from "../fixtures/base-test";
import { testData } from "../fixtures/test-data";

test.describe("Financial page cross-entity links", () => {
    test("Fund source detail has organization link", async ({ authedPage: page }) => {
        await page.goto(`/fund-sources/${testData.fundSourceID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
        const orgLink = page.locator("a[href*='/organizations/']").first();
        if (await orgLink.isVisible()) {
            await orgLink.click();
            await expect(page).toHaveURL(/\/organizations\/\d+/);
        }
    });

    test("Fund source detail has fund source allocation links", async ({ authedPage: page }) => {
        await page.goto(`/fund-sources/${testData.fundSourceID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
        const allocationLink = page.locator("a[href*='/fund-source-allocations/']").first();
        if (await allocationLink.isVisible()) {
            await allocationLink.click();
            await expect(page).toHaveURL(/\/fund-source-allocations\/\d+/);
        }
    });
});
