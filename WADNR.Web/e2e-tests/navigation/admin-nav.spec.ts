import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";

// Admin nav tests use manual auth setup (no API error monitor) because
// destination pages may call APIs the test user lacks supplemental roles for.

/**
 * Scoped dropdown click helper. See dropdown-nav.spec.ts for details on why evaluate() is used.
 */
async function clickDropdownItem(page: import("@playwright/test").Page, toggleText: string, itemText: string) {
    const dropdown = page.locator(".secondary-nav .nav-item.dropdown").filter({
        has: page.locator(".dropdown-toggle", { hasText: toggleText }),
    });
    await dropdown.locator(".dropdown-toggle").click();
    await expect(dropdown.locator(".dropdown-menu")).toHaveClass(/active/, { timeout: 5000 });
    await dropdown.locator(".dropdown-menu .dropdown-item", { hasText: itemText }).evaluate((el) => (el as HTMLElement).click());
}

test.describe("Admin navigation", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    test("Manage dropdown is visible for admin", async ({ page }) => {
        await page.goto("/");
        await expect(page.locator(".secondary-nav .dropdown-toggle", { hasText: "Manage" })).toBeVisible();
    });

    test("Manage > Users and Contacts navigates to /people", async ({ page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Manage", "Users and Contacts");
        await expect(page).toHaveURL(/\/people$/);
    });

    test("Manage > Organization Types navigates to /organization-and-relationship-types", async ({ page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Manage", "Organization Types");
        await expect(page).toHaveURL(/\/organization-and-relationship-types/);
    });

    test("Manage > Custom Labels navigates to /labels-and-definitions", async ({ page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Manage", "Custom Labels");
        await expect(page).toHaveURL(/\/labels-and-definitions/);
    });

    test("Reports > Projects navigates to /reports/projects", async ({ page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Reports", "Projects");
        await expect(page).toHaveURL(/\/reports\/projects/);
    });

    test("Reports > Manage Report Templates navigates to /reports", async ({ page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Reports", "Manage Report Templates");
        await expect(page).toHaveURL(/\/reports$/);
    });

    test("/reports/ipr loads IPR Reports page", async ({ page }) => {
        await page.goto("/reports/ipr");
        await expect(page.getByRole("heading", { name: "Invoice Payment Request Reports" })).toBeVisible();
    });
});
