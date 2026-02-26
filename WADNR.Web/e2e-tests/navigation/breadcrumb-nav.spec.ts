import { test, expect } from "../fixtures/base-test";
import { testData } from "../fixtures/test-data";

test.describe("Breadcrumb navigation", () => {
    test("Project detail breadcrumb navigates back to Projects list", async ({ authedPage: page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        // Wait for page content before checking breadcrumbs
        await expect(page.locator("#card-basics")).toBeVisible({ timeout: 15000 });
        const breadcrumbLink = page.locator("a.breadcrumb-item").first();
        await expect(breadcrumbLink).toBeVisible({ timeout: 5000 });
        await breadcrumbLink.click();
        await expect(page).toHaveURL(/\/projects$/);
    });

    test("County detail breadcrumb navigates back to Counties list", async ({ authedPage: page }) => {
        await page.goto(`/counties/${testData.countyID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
        const breadcrumbLink = page.locator("a.breadcrumb-item").first();
        await expect(breadcrumbLink).toBeVisible({ timeout: 5000 });
        await breadcrumbLink.click();
        await expect(page).toHaveURL(/\/counties$/);
    });

    test("Focus area detail breadcrumb navigates back to Focus Areas list", async ({ authedPage: page }) => {
        await page.goto(`/focus-areas/${testData.focusAreaID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
        const breadcrumbLink = page.locator("a.breadcrumb-item").first();
        await expect(breadcrumbLink).toBeVisible({ timeout: 5000 });
        await breadcrumbLink.click();
        await expect(page).toHaveURL(/\/focus-areas$/);
    });

    test("Fund source detail breadcrumb navigates back to list", async ({ authedPage: page }) => {
        await page.goto(`/fund-sources/${testData.fundSourceID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
        const breadcrumbLink = page.locator("a.breadcrumb-item").first();
        await expect(breadcrumbLink).toBeVisible({ timeout: 5000 });
        await breadcrumbLink.click();
        await expect(page).toHaveURL(/\/fund-sources$/);
    });
});
