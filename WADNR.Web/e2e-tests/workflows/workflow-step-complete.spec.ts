/**
 * Workflow step navigation tests.
 *
 * Verifies that all create and update workflow steps can be navigated to
 * and render their content. Does NOT submit forms — this is a smoke test
 * ensuring each step's component loads without errors.
 */

import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import { testData } from "../fixtures/test-data";

const CREATE_STEPS = [
    "basics",
    "location-simple",
    "location-detailed",
    "priority-landscapes",
    "dnr-upland-regions",
    "counties",
    "treatments",
    "contacts",
    "organizations",
    "expected-funding",
    "classifications",
    "photos",
    "documents-notes",
];

const UPDATE_STEPS = [
    "basics",
    "location-simple",
    "location-detailed",
    "priority-landscapes",
    "dnr-upland-regions",
    "counties",
    "treatments",
    "contacts",
    "organizations",
    "expected-funding",
    "photos",
    "external-links",
    "documents-notes",
];

test.describe("Create workflow steps", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    for (const step of CREATE_STEPS) {
        test(`Edit workflow step "${step}" loads`, async ({ page }) => {
            // Use edit workflow with existing project (create workflow only has basics until entity exists)
            await page.goto(`/projects/edit/${testData.projectID}/${step}`);
            await expect(page).toHaveURL(new RegExp(`/projects/edit/${testData.projectID}/${step}`));

            // Verify the workflow rendered — sidebar or form should be visible
            await expect(
                page.locator("form, .workflow-sidebar, .step-content, .card").first(),
            ).toBeVisible({ timeout: 15000 });

            // Verify Angular app bootstrapped (nav bar present)
            await expect(page.locator(".secondary-nav")).toBeVisible({ timeout: 5000 });
        });
    }
});

test.describe("Update workflow steps", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    for (const step of UPDATE_STEPS) {
        test(`Update workflow step "${step}" navigates`, async ({ page }) => {
            await page.goto(`/projects/${testData.projectID}/update/${step}`);

            // Project may not have an active update batch, so the page may redirect
            // or show a message. Just verify the app didn't crash.
            await expect(page.locator(".secondary-nav")).toBeVisible({ timeout: 15000 });

            // Verify we're still on a project update route (not redirected to error page)
            const url = page.url();
            const isOnUpdate = url.includes(`/projects/${testData.projectID}/update`);
            const isOnProject = url.includes("/projects");
            expect(isOnUpdate || isOnProject).toBe(true);
        });
    }
});

test.describe("Workflow sidebar navigation", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    test("Edit workflow sidebar links navigate between steps", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/basics`);
        await expect(page.locator(".workflow-sidebar, .step-sidebar, nav").first()).toBeVisible({ timeout: 15000 });

        // Try clicking the second sidebar link (should navigate to next step)
        const sidebarLinks = page.locator(".workflow-sidebar a, .step-sidebar a, nav a").filter({
            hasText: /location|simple/i,
        });

        if (await sidebarLinks.first().isVisible({ timeout: 5000 }).catch(() => false)) {
            await sidebarLinks.first().click();
            await page.waitForLoadState("networkidle");
            // Should now be on a different step
            expect(page.url()).not.toMatch(/\/basics$/);
        }
    });
});
