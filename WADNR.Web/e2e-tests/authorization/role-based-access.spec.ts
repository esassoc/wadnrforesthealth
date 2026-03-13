/**
 * Role-based access control tests.
 *
 * Verifies that:
 * 1. Normal users can access auth-guarded routes but NOT admin-guarded routes
 * 2. Admin-only nav items are not visible to normal users
 * 3. Direct navigation to admin routes by normal users results in redirect
 *
 * NOTE: These tests require a real Normal-role user GlobalID in test-users.ts.
 * Tests are skipped if the normal user is not configured.
 */

import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers, isUserConfigured } from "../fixtures/test-users";

// Skip all tests if normal user is not configured
test.beforeEach(({ }, testInfo) => {
    if (!isUserConfigured("normal")) {
        testInfo.skip(true, "Normal user GlobalID not configured in test-users.ts");
    }
});

test.describe("Normal user cannot access admin routes", () => {
    const adminRoutes = [
        "/roles",
        "/project-types",
        "/project-themes",
        "/upload-excel-files",
        "/map-layers",
        "/jobs",
        "/labels-and-definitions",
        "/featured-projects",
        "/project-updates",
        "/homepage-configuration",
        "/internal-setup-notes",
    ];

    for (const route of adminRoutes) {
        test(`${route} redirects normal user to /`, async ({ page }) => {
            await setupTestAuth(page, testUsers.normal);
            await page.goto(route);
            await page.waitForURL("/", { timeout: 15000 });
        });
    }
});

test.describe("Normal user CAN access auth-guarded routes", () => {
    const authRoutes = [
        { path: "/my-projects", heading: "My Projects" },
        { path: "/pending-projects", heading: "Pending Projects" },
        { path: "/people", heading: "Users and Contacts" },
        { path: "/reports/projects", heading: "Project Reports" },
    ];

    for (const { path, heading } of authRoutes) {
        test(`${path} loads for normal user`, async ({ page }) => {
            await setupTestAuth(page, testUsers.normal);
            await page.goto(path);
            await expect(page).toHaveURL(new RegExp(path.replace("/", "\\/")));
            // Page should render (not redirect) — verify a heading or nav is visible
            await expect(page.locator(".secondary-nav")).toBeVisible({ timeout: 15000 });
        });
    }
});

test.describe("Admin nav items hidden from normal user", () => {
    test("Manage dropdown not visible to normal user", async ({ page }) => {
        await setupTestAuth(page, testUsers.normal);
        await page.goto("/");
        await expect(page.locator(".secondary-nav")).toBeVisible({ timeout: 15000 });

        // The "Manage" dropdown should not be visible to normal users
        const manageDropdown = page.locator(".secondary-nav .nav-item.dropdown").filter({
            has: page.locator(".dropdown-toggle", { hasText: "Manage" }),
        });
        await expect(manageDropdown).not.toBeVisible();
    });
});
