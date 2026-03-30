import { test, expect } from "../fixtures/base-test";
import { testData } from "../fixtures/test-data";

test.describe("Authenticated detail pages", () => {
    test("Focus area detail loads", async ({ authedPage: page }) => {
        await page.goto(`/focus-areas/${testData.focusAreaID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Fund source detail loads", async ({ authedPage: page }) => {
        await page.goto(`/fund-sources/${testData.fundSourceID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Program detail loads", async ({ authedPage: page }) => {
        await page.goto(`/programs/${testData.programID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

});
