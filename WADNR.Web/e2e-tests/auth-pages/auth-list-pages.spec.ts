import { test, expect } from "../fixtures/base-test";

test.describe("Authenticated list pages", () => {
    test("/focus-areas renders with grid data", async ({ authedPage: page }) => {
        await page.goto("/focus-areas");
        await expect(page.getByRole("heading", { name: "Focus Areas" })).toBeVisible();
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
    });

    test("/roles renders with content", async ({ authedPage: page }) => {
        await page.goto("/roles");
        await expect(page.getByRole("heading", { name: "Roles" })).toBeVisible();
    });

    test("/agreements renders with grid data", async ({ authedPage: page }) => {
        await page.goto("/agreements");
        await expect(page.getByRole("heading", { name: "Agreements" })).toBeVisible();
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
    });

    test("/interactions-events renders with grid data", async ({ authedPage: page }) => {
        await page.goto("/interactions-events");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
    });

    test("/invoices renders with heading", async ({ authedPage: page }) => {
        await page.goto("/invoices");
        await expect(page.getByRole("heading", { name: "Invoices" })).toBeVisible();
    });

    test("/programs renders with heading", async ({ authedPage: page }) => {
        await page.goto("/programs");
        await expect(page.getByRole("heading", { name: "Programs" })).toBeVisible();
    });

    test("/my-projects renders with heading", async ({ authedPage: page }) => {
        await page.goto("/my-projects");
        await expect(page.getByRole("heading", { name: "My Projects" })).toBeVisible();
    });

    test("/pending-projects renders with heading", async ({ authedPage: page }) => {
        await page.goto("/pending-projects");
        await expect(page.getByRole("heading", { name: "Pending Projects" })).toBeVisible();
    });
});

// Vendors and People use manual auth (no API error monitor) because the test user
// may lack supplemental roles required by some API calls on these pages.
import { test as base, expect as baseExpect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";

base.describe("Authenticated list pages (manual auth)", () => {
    base.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    base.test("/vendors renders with heading", async ({ page }) => {
        await page.goto("/vendors");
        await baseExpect(page.locator("page-header").getByRole("heading", { name: "Vendors" })).toBeVisible();
    });

    base.test("/people renders with heading", async ({ page }) => {
        await page.goto("/people");
        await baseExpect(page.locator("page-header").getByRole("heading", { name: "Users and Contacts" })).toBeVisible();
    });
});
