import { test, expect } from "../fixtures/base-test";

test.describe("Project search typeahead", () => {
    test("Typing shows dropdown results", async ({ authedPage: page }) => {
        await page.goto("/");
        const searchInput = page.locator(".main-nav__search-input");
        await expect(searchInput).toBeVisible();
        await searchInput.fill("forest");
        // Wait for debounce (200ms) + API response
        await expect(page.locator(".typeahead-dropdown .typeahead-results li, .typeahead-dropdown .result-name").first()).toBeVisible({ timeout: 10000 });
    });

    test("Clicking a search result navigates to project detail", async ({ authedPage: page }) => {
        await page.goto("/");
        const searchInput = page.locator(".main-nav__search-input");
        await searchInput.fill("forest");
        await expect(page.locator(".typeahead-dropdown .typeahead-results li, .typeahead-dropdown .result-name").first()).toBeVisible({ timeout: 10000 });
        // Click the first result
        await page.locator(".typeahead-dropdown .typeahead-results li a, .typeahead-dropdown .result-name").first().click();
        await expect(page).toHaveURL(/\/projects\/\d+/);
    });
});
