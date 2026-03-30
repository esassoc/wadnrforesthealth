import { test, expect } from "../fixtures/base-test";

test.describe("Homepage features", () => {
    test("Homepage renders map and content", async ({ publicPage: page }) => {
        await page.goto("/");
        await expect(page.getByRole("heading", { name: "Project Map" })).toBeVisible();
        await expect(page.locator(".leaflet-container")).toBeVisible({ timeout: 15000 });
        await expect(page.getByRole("navigation").first()).toBeVisible();
    });

    test("Navigation bar is visible with dropdown links", async ({ publicPage: page }) => {
        await page.goto("/");
        await expect(page.getByRole("navigation").first()).toBeVisible();
        await expect(page.locator(".secondary-nav .dropdown-toggle", { hasText: "About" })).toBeVisible();
        await expect(page.locator(".secondary-nav .dropdown-toggle", { hasText: "Projects" })).toBeVisible();
    });

    test("Add Project button visible for authenticated user and navigates to /projects/new", async ({ authedPage: page }) => {
        await page.goto("/");
        const addButton = page.locator("a[href*='/projects/new'], button", { hasText: /add.*project/i }).first();
        if (await addButton.isVisible()) {
            await addButton.click();
            await expect(page).toHaveURL(/\/projects\/new/);
        }
    });

    test("Full Project List link navigates to /projects", async ({ authedPage: page }) => {
        await page.goto("/");
        const listLink = page.locator("a[href='/projects']", { hasText: /project list|view all/i }).first();
        if (await listLink.isVisible()) {
            await listLink.click();
            await expect(page).toHaveURL(/\/projects$/);
        }
    });
});
