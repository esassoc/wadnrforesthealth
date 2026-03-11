import { test, expect } from "../fixtures/base-test";

/**
 * Fills coverage gaps for admin and authenticated pages that previously had
 * no dedicated render tests.
 */

test.describe("Admin page rendering", () => {
    test("/project-types renders grid with data", async ({ authedPage: page }) => {
        await page.goto("/project-types");
        await expect(page.locator("h2.page-title")).toContainText("Project Types", { timeout: 15000 });
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
    });

    test("/project-types shows Create New Project Type button", async ({ authedPage: page }) => {
        await page.goto("/project-types");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
        await expect(page.locator("button", { hasText: "Create New Project Type" })).toBeVisible();
    });

    test("/project-themes renders grid with data", async ({ authedPage: page }) => {
        await page.goto("/project-themes");
        await expect(page.locator("h2.page-title")).toContainText("Project Themes", { timeout: 15000 });
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
    });

    test("/project-themes shows Create New Theme and Edit Sort Order buttons", async ({ authedPage: page }) => {
        await page.goto("/project-themes");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
        await expect(page.locator("button", { hasText: "Create New Theme" })).toBeVisible();
        await expect(page.locator("button", { hasText: "Edit Sort Order" })).toBeVisible();
    });

    test("/upload-excel-files renders with heading", async ({ authedPage: page }) => {
        await page.goto("/upload-excel-files");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
        await expect(page.locator(".card").first()).toBeVisible();
    });

    test("/map-layers renders with heading and grid", async ({ authedPage: page }) => {
        await page.goto("/map-layers");
        await expect(page.locator("h2.page-title")).toContainText("Manage External Map Layers", { timeout: 15000 });
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
    });

    test("/jobs renders with heading", async ({ authedPage: page }) => {
        await page.goto("/jobs");
        await expect(page.locator("h2.page-title")).toContainText("Finance API Import Jobs", { timeout: 15000 });
        await expect(page.locator("button", { hasText: "Run Vendor Import Job" })).toBeVisible();
    });

    test("/manage-page-content renders with heading", async ({ authedPage: page }) => {
        await page.goto("/manage-page-content");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
    });

    test("/manage-custom-pages renders with heading", async ({ authedPage: page }) => {
        await page.goto("/manage-custom-pages");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
    });

    test("/manage-find-your-forester renders with heading", async ({ authedPage: page }) => {
        await page.goto("/manage-find-your-forester");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
    });

    test("/organization-and-relationship-types renders with content", async ({ authedPage: page }) => {
        await page.goto("/organization-and-relationship-types");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
        await expect(page.locator(".card").first()).toBeVisible();
    });

    test("/reports/projects renders with heading", async ({ authedPage: page }) => {
        await page.goto("/reports/projects");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
    });

    test("/reports renders with heading", async ({ authedPage: page }) => {
        await page.goto("/reports");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
    });
});
