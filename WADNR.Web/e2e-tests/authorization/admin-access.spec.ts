import { test, expect } from "../fixtures/base-test";

/**
 * Positive complement to guard-redirect tests: verifies that an admin user
 * CAN access routes protected by adminGuard, authGuard, and projectEditGuard.
 */

test.describe("Admin user can access guarded routes", () => {
    test("/roles renders heading (adminGuard)", async ({ authedPage: page }) => {
        await page.goto("/roles");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
        await expect(page.locator("h2.page-title")).toContainText("Roles");
    });

    test("/project-types renders heading (adminGuard)", async ({ authedPage: page }) => {
        await page.goto("/project-types");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
        await expect(page.locator("h2.page-title")).toContainText("Project Types");
    });

    test("/project-themes renders heading (adminGuard)", async ({ authedPage: page }) => {
        await page.goto("/project-themes");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
        await expect(page.locator("h2.page-title")).toContainText("Project Themes");
    });

    test("/upload-excel-files renders heading (adminGuard)", async ({ authedPage: page }) => {
        await page.goto("/upload-excel-files");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
    });

    test("/map-layers renders heading (adminGuard)", async ({ authedPage: page }) => {
        await page.goto("/map-layers");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
        await expect(page.locator("h2.page-title")).toContainText("Manage External Map Layers");
    });

    test("/jobs renders heading (adminGuard)", async ({ authedPage: page }) => {
        await page.goto("/jobs");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
        await expect(page.locator("h2.page-title")).toContainText("Finance API Import Jobs");
    });

    test("/projects/new/basics renders form (projectEditGuard)", async ({ authedPage: page }) => {
        await page.goto("/projects/new/basics");
        await expect(page).toHaveURL(/\/projects\/new\/basics/);
        await expect(page.locator("form, .workflow-sidebar, .step-content").first()).toBeVisible({ timeout: 15000 });
    });

    test("/manage-find-your-forester renders heading (authGuard)", async ({ authedPage: page }) => {
        await page.goto("/manage-find-your-forester");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
    });
});
