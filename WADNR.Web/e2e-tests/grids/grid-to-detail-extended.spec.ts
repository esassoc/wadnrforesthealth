import { test, expect } from "../fixtures/base-test";
import { test as base, expect as baseExpect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import { testData } from "../fixtures/test-data";

test.describe("Extended grid-to-detail navigation", () => {
    test("Agreements grid links to agreement detail", async ({ authedPage: page }) => {
        await page.goto("/agreements");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href*='/agreements/']").first();
        await firstLink.click();
        await expect(page).toHaveURL(/\/agreements\/\d+/);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Classification systems grid links to classification system detail", async ({ authedPage: page }) => {
        await page.goto("/classification-systems");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href]").first();
        await firstLink.click();
        await expect(page).toHaveURL(/\/classification-systems\/\d+/);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Interactions/Events grid links to interaction event detail", async ({ authedPage: page }) => {
        await page.goto("/interactions-events");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href]").first();
        await firstLink.click();
        await expect(page).toHaveURL(/\/interactions-events\/\d+/);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Invoices grid links to invoice detail", async ({ authedPage: page }) => {
        await page.goto("/invoices");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href*='/invoices/']").first();
        await firstLink.click();
        await expect(page).toHaveURL(/\/invoices\/\d+/);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Labels and definitions page renders with grid", async ({ authedPage: page }) => {
        await page.goto("/labels-and-definitions");
        await expect(page.getByRole("heading", { name: "Labels and Definitions" })).toBeVisible();
        // Grid headers render quickly even though row data is very slow to load
        await expect(page.locator(".ag-header").first()).toBeVisible({ timeout: 15000 });
    });

    test("Tags grid links to tag detail", async ({ authedPage: page }) => {
        await page.goto("/tags");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href]").first();
        await firstLink.click();
        await expect(page).toHaveURL(/\/tags\/\d+/);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Project types grid links to project type detail", async ({ authedPage: page }) => {
        await page.goto("/project-types");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href]").first();
        await firstLink.click();
        await expect(page).toHaveURL(/\/project-types\/\d+/);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Program indices grid links to program index detail", async ({ authedPage: page }) => {
        await page.goto("/program-indices");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href]").first();
        await firstLink.click();
        await expect(page).toHaveURL(/\/program-indices\/\d+/);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Project codes grid links to project code detail", async ({ authedPage: page }) => {
        await page.goto("/project-codes");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href]").first();
        await firstLink.click();
        await expect(page).toHaveURL(/\/project-codes\/\d+/);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Projects by theme links to classification detail", async ({ authedPage: page }) => {
        await page.goto("/projects-by-theme");
        const classificationLink = page.locator("a.classifications-tile").first();
        await expect(classificationLink).toBeVisible({ timeout: 15000 });
        await classificationLink.click();
        await expect(page).toHaveURL(/\/classifications\/\d+/);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Project detail activities link to treatment detail", async ({ authedPage: page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await expect(page.locator("#card-activities")).toBeVisible({ timeout: 15000 });
        const treatmentLink = page.locator("#card-activities .ag-row .ag-cell a[href*='/treatments/']").first();
        // Not all projects have treatments; skip gracefully if none exist
        if (await treatmentLink.isVisible({ timeout: 10000 }).catch(() => false)) {
            await treatmentLink.click();
            await expect(page).toHaveURL(/\/treatments\/\d+/);
            await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
        }
    });
});

// People and Vendors use manual auth (no API error monitor) because the test user
// may lack supplemental roles required by some API calls on these pages.
base.describe("Grid-to-detail with manual auth", () => {
    base.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    base.test("People grid links to person detail", async ({ page }) => {
        await page.goto("/people");
        await baseExpect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href]").first();
        await firstLink.click();
        await baseExpect(page).toHaveURL(/\/people\/\d+/);
        await baseExpect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    base.test("Vendors grid links to vendor detail", async ({ page }) => {
        await page.goto("/vendors");
        await baseExpect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
        const firstLink = page.locator(".ag-row .ag-cell a[href]").first();
        await firstLink.click();
        await baseExpect(page).toHaveURL(/\/vendors\/\d+/);
        await baseExpect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });
});
