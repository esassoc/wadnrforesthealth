import { test, expect } from "@playwright/test";
import { testData } from "../fixtures/test-data";
import {
    DEFAULT_SCREENSHOT_OPTIONS,
    waitForPageStable,
    waitForGrid,
    getMapMasks,
    getGridBodyMasks,
} from "./visual-config";

test.describe("Public pages - visual regression", () => {
    test("Homepage", async ({ page }) => {
        await page.goto("/");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("homepage.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getMapMasks(page),
        });
    });

    test("About", async ({ page }) => {
        await page.goto("/about");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("about.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Projects list", async ({ page }) => {
        await page.goto("/projects");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("projects-list.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Projects map", async ({ page }) => {
        await page.goto("/projects/map");
        await waitForPageStable(page, 60000);
        await expect(page).toHaveScreenshot("projects-map.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getMapMasks(page),
        });
    });

    test("Project detail", async ({ page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("project-detail.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: [...getMapMasks(page), ...getGridBodyMasks(page)],
        });
    });

    test("Project fact sheet", async ({ page }) => {
        await page.goto(`/projects/fact-sheet/${testData.projectID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("project-fact-sheet.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getMapMasks(page),
        });
    });

    test("Counties list", async ({ page }) => {
        await page.goto("/counties");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("counties-list.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("County detail", async ({ page }) => {
        await page.goto(`/counties/${testData.countyID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("county-detail.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: [...getMapMasks(page), ...getGridBodyMasks(page)],
        });
    });

    test("Organizations list", async ({ page }) => {
        await page.goto("/organizations");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("organizations-list.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Organization detail", async ({ page }) => {
        await page.goto(`/organizations/${testData.organizationID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("organization-detail.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Tags list", async ({ page }) => {
        await page.goto("/tags");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("tags-list.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Taxonomy branches", async ({ page }) => {
        await page.goto("/taxonomy-branches");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("taxonomy-branches.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Classification systems", async ({ page }) => {
        await page.goto("/classification-systems");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("classification-systems.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Priority landscapes", async ({ page }) => {
        await page.goto("/priority-landscapes");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("priority-landscapes.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("DNR upland regions", async ({ page }) => {
        await page.goto("/dnr-upland-regions");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("dnr-upland-regions.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Find your forester", async ({ page }) => {
        await page.goto("/find-your-forester");
        await waitForPageStable(page, 60000);
        await expect(page).toHaveScreenshot("find-your-forester.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getMapMasks(page),
        });
    });
});
