import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import { testData } from "../fixtures/test-data";
import {
    VIEWPORTS,
    DEFAULT_SCREENSHOT_OPTIONS,
    waitForPageStable,
    waitForGrid,
    getMapMasks,
    getGridBodyMasks,
} from "./visual-config";

const responsiveViewports = [
    { name: "mobile", ...VIEWPORTS.mobile },
    { name: "tablet", ...VIEWPORTS.tablet },
    { name: "large", ...VIEWPORTS.large },
] as const;

for (const viewport of responsiveViewports) {
    test.describe(`Responsive ${viewport.name} (${viewport.width}x${viewport.height})`, () => {
        test.beforeEach(async ({ page }) => {
            await page.setViewportSize({ width: viewport.width, height: viewport.height });
        });

        test(`Homepage - ${viewport.name}`, async ({ page }) => {
            await page.goto("/");
            await waitForPageStable(page);
            await expect(page).toHaveScreenshot(`responsive-homepage-${viewport.name}.png`, {
                ...DEFAULT_SCREENSHOT_OPTIONS,
                mask: getMapMasks(page),
            });
        });

        test(`Projects list - ${viewport.name}`, async ({ page }) => {
            await page.goto("/projects");
            await waitForPageStable(page);
            await waitForGrid(page);
            await expect(page).toHaveScreenshot(`responsive-projects-list-${viewport.name}.png`, {
                ...DEFAULT_SCREENSHOT_OPTIONS,
                mask: getGridBodyMasks(page),
            });
        });

        test(`Project detail - ${viewport.name}`, async ({ page }) => {
            await page.goto(`/projects/${testData.projectID}`);
            await waitForPageStable(page);
            await expect(page).toHaveScreenshot(`responsive-project-detail-${viewport.name}.png`, {
                ...DEFAULT_SCREENSHOT_OPTIONS,
                mask: [...getMapMasks(page), ...getGridBodyMasks(page)],
            });
        });

        test(`Agreements - ${viewport.name}`, async ({ page }) => {
            await setupTestAuth(page, testUsers.admin);
            await page.goto("/agreements");
            await waitForPageStable(page);
            await waitForGrid(page);
            await expect(page).toHaveScreenshot(`responsive-agreements-${viewport.name}.png`, {
                ...DEFAULT_SCREENSHOT_OPTIONS,
                mask: getGridBodyMasks(page),
            });
        });

        test(`Workflow basics - ${viewport.name}`, async ({ page }) => {
            await setupTestAuth(page, testUsers.admin);
            await page.goto(`/projects/edit/${testData.projectID}/basics`);
            await waitForPageStable(page);
            await expect(page).toHaveScreenshot(`responsive-workflow-basics-${viewport.name}.png`, DEFAULT_SCREENSHOT_OPTIONS);
        });
    });
}
