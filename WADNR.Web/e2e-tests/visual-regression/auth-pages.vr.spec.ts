import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import {
    DEFAULT_SCREENSHOT_OPTIONS,
    waitForPageStable,
    waitForGrid,
    getGridBodyMasks,
} from "./visual-config";

test.describe("Authenticated pages - visual regression", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    test("Focus areas", async ({ page }) => {
        await page.goto("/focus-areas");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("focus-areas.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Agreements", async ({ page }) => {
        await page.goto("/agreements");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("agreements.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Interactions/events", async ({ page }) => {
        await page.goto("/interactions-events");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("interactions-events.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Invoices", async ({ page }) => {
        await page.goto("/invoices");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("invoices.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Programs", async ({ page }) => {
        await page.goto("/programs");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("programs.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("My projects", async ({ page }) => {
        await page.goto("/my-projects");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("my-projects.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Pending projects", async ({ page }) => {
        await page.goto("/pending-projects");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("pending-projects.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("People", async ({ page }) => {
        await page.goto("/people");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("people.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Vendors", async ({ page }) => {
        await page.goto("/vendors");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("vendors.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Project types", async ({ page }) => {
        await page.goto("/project-types");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("project-types.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Project themes", async ({ page }) => {
        await page.goto("/project-themes");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("project-themes.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Map layers", async ({ page }) => {
        await page.goto("/map-layers");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("map-layers.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Reports", async ({ page }) => {
        await page.goto("/reports");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("reports.png", DEFAULT_SCREENSHOT_OPTIONS);
    });
});
