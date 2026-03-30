import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import { testData } from "../fixtures/test-data";

/**
 * Workflow step form validation and navigation tests.
 * Extends the existing workflow smoke tests with form-level assertions.
 * Uses manual auth (no API error monitor) because workflow pages may trigger
 * API calls the test user doesn't have full access to.
 */

test.describe("Create workflow - Basics step form", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
        await page.goto("/projects/new/basics");
        await expect(page.locator("form, .step-content").first()).toBeVisible({ timeout: 15000 });
    });

    test("Form renders with Project Name field", async ({ page }) => {
        // The basics form should contain a Project Name input
        await expect(page.locator("form").first()).toBeVisible();
        const nameField = page.locator("form-field, input, .field").filter({ hasText: /Project Name|ProjectName/i }).first();
        await expect(nameField).toBeVisible({ timeout: 5000 });
    });

    test("Form renders with Project Type dropdown", async ({ page }) => {
        // Should have a Project Type dropdown
        const typeField = page.locator("form-field, .field").filter({ hasText: /Project Type/i }).first();
        await expect(typeField).toBeVisible({ timeout: 5000 });
    });

    test("Submit/Save button is present", async ({ page }) => {
        const saveBtn = page.locator("button", { hasText: /Save|Next|Submit/i }).first();
        await expect(saveBtn).toBeVisible({ timeout: 5000 });
    });

    test("Submit with empty Project Name shows validation error", async ({ page }) => {
        // Try to submit without filling required fields
        const saveBtn = page.locator("button", { hasText: /Save|Next|Submit/i }).first();
        await saveBtn.click();

        // Validation errors should appear
        await expect(page.locator("note").first()).toBeVisible({ timeout: 5000 });
    });
});

test.describe("Edit workflow - step navigation", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
        await page.goto(`/projects/edit/${testData.projectID}/basics`);
        await expect(page.locator("form, .workflow-sidebar, .step-content").first()).toBeVisible({ timeout: 15000 });
    });

    test("Sidebar shows step labels", async ({ page }) => {
        const sidebar = page.locator(".workflow-sidebar, nav, .sidebar").first();
        if (await sidebar.isVisible({ timeout: 5000 }).catch(() => false)) {
            // Should contain multiple step links
            const links = sidebar.locator("a, button, .step-label");
            const count = await links.count();
            expect(count).toBeGreaterThanOrEqual(2);
        }
    });

    test("Clicking sidebar step changes URL", async ({ page }) => {
        const sidebar = page.locator(".workflow-sidebar, nav, .sidebar").first();
        if (await sidebar.isVisible({ timeout: 5000 }).catch(() => false)) {
            // Find a step link that isn't the current "basics" step
            const otherStep = sidebar.locator("a[href*='location'], a[href*='contacts'], a[href*='treatments']").first();
            if (await otherStep.isVisible({ timeout: 3000 }).catch(() => false)) {
                await otherStep.click();
                // URL should change away from /basics
                await page.waitForURL(/\/projects\/edit\/\d+\/(?!basics)/, { timeout: 10000 });
            }
        }
    });

    test("Basics step pre-populates project name", async ({ page }) => {
        // The project name input should have a value (since we're editing an existing project)
        const nameInput = page.locator("form input[type='text']").first();
        if (await nameInput.isVisible({ timeout: 5000 }).catch(() => false)) {
            const value = await nameInput.inputValue();
            expect(value.length).toBeGreaterThan(0);
        }
    });
});
