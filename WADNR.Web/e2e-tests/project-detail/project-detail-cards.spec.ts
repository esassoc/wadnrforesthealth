import { test, expect } from "../fixtures/base-test";
import { testData } from "../fixtures/test-data";

test.describe("Project detail card sections", () => {
    test.beforeEach(async ({ authedPage: page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Basics card renders", async ({ authedPage: page }) => {
        await expect(page.locator("#card-basics")).toBeVisible();
    });

    test("Location card renders with map", async ({ authedPage: page }) => {
        await expect(page.locator("#card-location")).toBeVisible();
        await expect(page.locator("#card-location .leaflet-container")).toBeVisible({ timeout: 15000 });
    });

    test("Tags card renders", async ({ authedPage: page }) => {
        await expect(page.locator("#card-tags")).toBeVisible();
    });

    test("Organizations card renders", async ({ authedPage: page }) => {
        await expect(page.locator("#card-organizations")).toBeVisible();
    });

    test("Contacts card renders", async ({ authedPage: page }) => {
        await expect(page.locator("#card-contacts")).toBeVisible();
    });

    test("Funding card renders", async ({ authedPage: page }) => {
        await expect(page.locator("#card-funding")).toBeVisible();
    });

    test("Activities card renders", async ({ authedPage: page }) => {
        await expect(page.locator("#card-activities")).toBeVisible();
    });

    test("Notes card renders", async ({ authedPage: page }) => {
        await expect(page.locator("#card-notes")).toBeVisible();
    });
});
