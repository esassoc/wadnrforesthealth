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

    // ── Projects dropdown ──────────────────────────────────────────────────────

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

    test("Projects > Projects Map navigates to /projects/map", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Projects", "Projects Map");
        await expect(page).toHaveURL(/\/projects\/map/);
    });

    test("Projects > My Projects navigates to /my-projects", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Projects", "My Projects");
        await expect(page).toHaveURL(/\/my-projects/);
    });

    test("Projects > Pending Projects navigates to /pending-projects", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Projects", "Pending Projects");
        await expect(page).toHaveURL(/\/pending-projects/);
    });

    // ── Financials dropdown ────────────────────────────────────────────────────

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

    test("Financials > Full Invoice List navigates to /invoices", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Financials", "Full Invoice List");
        await expect(page).toHaveURL(/\/invoices$/);
    });

    test("Financials > Program Indices navigates to /program-indices", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Financials", "Program Indices");
        await expect(page).toHaveURL(/\/program-indices/);
    });

    test("Financials > Project Codes navigates to /project-codes", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Financials", "Project Codes");
        await expect(page).toHaveURL(/\/project-codes/);
    });

    // ── Program Info dropdown ──────────────────────────────────────────────────

    test("Program Info > Programs navigates to /programs", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Program Info", "Programs");
        await expect(page).toHaveURL(/\/programs$/);
    });

    test("Program Info > Focus Areas navigates to /focus-areas", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Program Info", "Focus Areas");
        await expect(page).toHaveURL(/\/focus-areas/);
    });

    test("Program Info > Interactions/Events navigates to /interactions-events", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Program Info", "Interactions/Events");
        await expect(page).toHaveURL(/\/interactions-events/);
    });

    // ── Reports dropdown ───────────────────────────────────────────────────────

    test("Reports > Project Reports navigates to /reports/projects", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Reports", "Project Reports");
        await expect(page).toHaveURL(/\/reports\/projects/);
    });

    test("Reports > IPR Reports navigates to /reports/ipr", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Reports", "IPR Reports");
        await expect(page).toHaveURL(/\/reports\/ipr/);
    });

    // ── Manage dropdown (admin) ────────────────────────────────────────────────

    test("Manage > Report Templates navigates to /reports", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Manage", "Report Templates");
        await expect(page).toHaveURL(/\/reports$/);
    });

    test("Manage > Labels and Definitions navigates to /labels-and-definitions", async ({ authedPage: page }) => {
        await page.goto("/");
        await clickDropdownItem(page, "Manage", "Labels and Definitions");
        await expect(page).toHaveURL(/\/labels-and-definitions/);
    });
});
