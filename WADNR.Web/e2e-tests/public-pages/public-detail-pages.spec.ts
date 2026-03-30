import { test, expect } from "../fixtures/base-test";
import { testData } from "../fixtures/test-data";

test.describe("Public detail pages", () => {
    test("Project detail page loads with cards", async ({ publicPage: page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("County detail page loads with map and Projects card", async ({ publicPage: page }) => {
        await page.goto(`/counties/${testData.countyID}`);
        await expect(page.locator(".leaflet-container")).toBeVisible({ timeout: 15000 });
        await expect(page.locator(".card").first()).toBeVisible();
    });

    test("Project fact sheet page loads", async ({ publicPage: page }) => {
        await page.goto(`/projects/fact-sheet/${testData.projectID}`);
        await expect(page.locator("main, .fact-sheet, .page-header").first()).toBeVisible({ timeout: 15000 });
    });

    test("About page renders", async ({ publicPage: page }) => {
        await page.goto("/about");
        await expect(page.locator("main, .page-header").first()).toBeVisible();
    });

    test("Projects map page renders with Leaflet map", async ({ publicPage: page }) => {
        await page.goto("/projects/map");
        await expect(page.locator(".leaflet-container")).toBeVisible({ timeout: 15000 });
    });

    test("Projects by type page renders", async ({ authedPage: page }) => {
        // This page's API requires auth
        await page.goto("/projects-by-type");
        await expect(page.locator("main, .page-header, h2").first()).toBeVisible();
    });

    test("Projects by theme page renders", async ({ publicPage: page }) => {
        await page.goto("/projects-by-theme");
        await expect(page.locator("main, .page-header, h2").first()).toBeVisible();
    });

    test("Labels and definitions page renders", async ({ publicPage: page }) => {
        await page.goto("/labels-and-definitions");
        await expect(page.getByRole("heading", { name: "Labels and Definitions" })).toBeVisible();
    });

    test("Homepage renders with Project Map heading and Leaflet map", async ({ publicPage: page }) => {
        await page.goto("/");
        await expect(page.getByRole("heading", { name: "Project Map" })).toBeVisible();
        await expect(page.locator(".leaflet-container")).toBeVisible({ timeout: 15000 });
    });
});
