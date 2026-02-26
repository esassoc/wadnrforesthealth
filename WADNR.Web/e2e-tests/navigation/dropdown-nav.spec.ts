import { test, expect } from "../fixtures/base-test";

/**
 * Helper: open a scoped dropdown and click a menu item.
 *
 * The [dropdownToggle] directive adds `.active` to the menu <ul>.
 * Uses evaluate() to dispatch the click via the DOM instead of Playwright's
 * actionability-checked click(), because long dropdown menus can be clipped
 * by CSS overflow — Playwright sees the item as "not visible" even though
 * Angular's routerLink can still handle the click event.
 */
async function clickDropdownItem(page: import("@playwright/test").Page, toggleText: string, itemText: string) {
    const dropdown = page.locator(".secondary-nav .nav-item.dropdown").filter({
        has: page.locator(".dropdown-toggle", { hasText: toggleText }),
    });
    await dropdown.locator(".dropdown-toggle").click();
    await expect(dropdown.locator(".dropdown-menu")).toHaveClass(/active/, { timeout: 5000 });
    await dropdown.locator(".dropdown-menu .dropdown-item", { hasText: itemText }).evaluate((el) => (el as HTMLElement).click());
}

test.describe("Dropdown navigation", () => {
    test("About dropdown opens and lists items", async ({ authedPage: page }) => {
        await page.goto("/");
        const dropdown = page.locator(".secondary-nav .nav-item.dropdown").filter({
            has: page.locator(".dropdown-toggle", { hasText: "About" }),
        });
        await dropdown.locator(".dropdown-toggle").click();
        await expect(dropdown.locator(".dropdown-menu")).toHaveClass(/active/, { timeout: 5000 });
        await expect(dropdown.locator(".dropdown-menu .dropdown-item").first()).toBeVisible();
    });

    test("Projects > Full Project List navigates to /projects", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Projects", "Full Project List");
        await expect(page).toHaveURL(/\/projects$/);
        await expect(page.getByRole("heading", { name: "Full Project List" })).toBeVisible();
    });

    test("Projects > Projects By Type navigates to /projects-by-type", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Projects", "Projects By Type");
        await expect(page).toHaveURL(/\/projects-by-type/);
    });

    test("Projects > Project Tags navigates to /tags", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Projects", "Project Tags");
        await expect(page).toHaveURL(/\/tags/);
    });

    test("Financials > Full Fund Source List navigates to /fund-sources", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Financials", "Full Fund Source List");
        await expect(page).toHaveURL(/\/fund-sources$/);
    });

    test("Financials > Full Agreement List navigates to /agreements", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Financials", "Full Agreement List");
        await expect(page).toHaveURL(/\/agreements$/);
    });

    test("Program Info > Programs navigates to /programs", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Program Info", "Programs");
        await expect(page).toHaveURL(/\/programs$/);
    });
});
