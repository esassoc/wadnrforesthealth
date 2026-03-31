import { test, expect } from "../fixtures/base-test";

test.describe("Grid row links navigate to detail pages", () => {
    test("Projects grid row links to project detail", async ({ authedPage: page }) => {
        await page.goto("/projects");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href]").first();
        await firstLink.click();
        await expect(page).toHaveURL(/\/projects\/\d+/);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Organizations grid row links to organization detail", async ({ authedPage: page }) => {
        await page.goto("/organizations");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href]").first();
        await firstLink.click();
        await expect(page).toHaveURL(/\/organizations\/\d+/);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Counties grid row links to county detail", async ({ authedPage: page }) => {
        await page.goto("/counties");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href]").first();
        await firstLink.click();
        await expect(page).toHaveURL(/\/counties\/\d+/);
    });

    test("Focus areas grid row links to focus area detail", async ({ authedPage: page }) => {
        await page.goto("/focus-areas");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href]").first();
        await firstLink.click();
        await expect(page).toHaveURL(/\/focus-areas\/\d+/);
    });
});
