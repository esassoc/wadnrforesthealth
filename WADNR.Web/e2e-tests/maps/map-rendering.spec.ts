import { test, expect } from "../fixtures/base-test";
import { testData } from "../fixtures/test-data";

test.describe("Leaflet map rendering", () => {
    test("Homepage map renders", async ({ publicPage: page }) => {
        await page.goto("/");
        await expect(page.locator(".leaflet-container")).toBeVisible({ timeout: 30000 });
    });

    test("Projects map page renders", async ({ publicPage: page }) => {
        await page.goto("/projects/map");
        await expect(page.locator(".leaflet-container")).toBeVisible({ timeout: 30000 });
    });

    test("County detail page map renders", async ({ publicPage: page }) => {
        await page.goto(`/counties/${testData.countyID}`);
        await expect(page.locator(".leaflet-container")).toBeVisible({ timeout: 30000 });
    });

    test("Project detail location card map renders", async ({ authedPage: page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await expect(page.locator("#card-location .leaflet-container")).toBeVisible({ timeout: 30000 });
    });

});
