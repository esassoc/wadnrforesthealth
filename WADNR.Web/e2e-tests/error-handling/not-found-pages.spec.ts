import { test, expect } from "@playwright/test";

// These tests use raw @playwright/test (no API error monitor)
// because they deliberately navigate to invalid routes/IDs that trigger API 404s.

test.describe("Error handling and not-found pages", () => {
    test("Unknown route shows not-found content", async ({ page }) => {
        await page.goto("/this-route-does-not-exist-12345");
        await expect(page.getByText(/not found|page.*not.*exist|404|under construction/i)).toBeVisible({ timeout: 10000 });
    });

    test("Non-existent project ID shows graceful error", async ({ page }) => {
        await page.goto("/projects/999999");
        // Should show an error message or redirect — not crash
        await expect(
            page.getByText(/not found|does not exist|error|no project/i).or(page.locator(".alert"))
        ).toBeVisible({ timeout: 10000 });
    });

    test("Non-existent county ID shows graceful error", async ({ page }) => {
        await page.goto("/counties/999999");
        await expect(
            page.getByText(/not found|does not exist|error|no county/i).or(page.locator(".alert"))
        ).toBeVisible({ timeout: 10000 });
    });

    test("Known stub route (featured-projects) shows NotFoundComponent", async ({ page }) => {
        await page.goto("/featured-projects");
        await expect(page.getByText(/not found|page.*not.*exist|under construction/i)).toBeVisible({ timeout: 10000 });
    });
});
