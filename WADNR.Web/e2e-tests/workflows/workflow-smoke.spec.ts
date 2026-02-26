import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import { testData } from "../fixtures/test-data";

// Workflow smoke tests use manual auth (no API error monitor) because workflow pages
// may trigger API calls the test user doesn't have full access to. We only verify
// that the page components render.

test.describe("Project workflow smoke tests", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    test("Create workflow loads first step", async ({ page }) => {
        // Navigate directly to /projects/new/basics (child route)
        await page.goto("/projects/new/basics");
        await expect(page).toHaveURL(/\/projects\/new\/basics/);
        await expect(page.locator("form, .workflow-sidebar, .step-content").first()).toBeVisible({ timeout: 15000 });
    });

    test("Edit workflow loads first step", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/basics`);
        await expect(page).toHaveURL(new RegExp(`/projects/edit/${testData.projectID}/basics`));
        await expect(page.locator("form, .workflow-sidebar, .step-content").first()).toBeVisible({ timeout: 15000 });
    });

    test("Update workflow route is accessible", async ({ page }) => {
        await page.goto(`/projects/${testData.projectID}/update/basics`);
        // Project may not have an active update batch, so the page may show
        // a loading state, redirect, or render a form. Just verify navigation
        // didn't error out (no "Page Not Found") and the route was reached.
        await expect(page).toHaveURL(new RegExp(`/projects/${testData.projectID}/update`));
        // Verify Angular rendered something (nav bar is always present if app bootstrapped)
        await expect(page.locator(".secondary-nav")).toBeVisible({ timeout: 15000 });
    });
});
