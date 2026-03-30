import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import { testData } from "../fixtures/test-data";
import {
    DEFAULT_SCREENSHOT_OPTIONS,
    waitForPageStable,
    getMapMasks,
    getGridBodyMasks,
} from "./visual-config";

test.describe("Detail pages - visual regression", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    test("Project detail", async ({ page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-project.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: [...getMapMasks(page), ...getGridBodyMasks(page)],
        });
    });

    test("Focus area detail", async ({ page }) => {
        await page.goto(`/focus-areas/${testData.focusAreaID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-focus-area.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: [...getMapMasks(page), ...getGridBodyMasks(page)],
        });
    });

    test("Fund source detail", async ({ page }) => {
        await page.goto(`/fund-sources/${testData.fundSourceID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-fund-source.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Fund source allocation detail", async ({ page }) => {
        await page.goto(`/fund-source-allocations/${testData.fundSourceAllocationID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-fund-source-allocation.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Program detail", async ({ page }) => {
        await page.goto(`/programs/${testData.programID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-program.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Agreement detail", async ({ page }) => {
        await page.goto(`/agreements/${testData.agreementID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-agreement.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Invoice detail", async ({ page }) => {
        await page.goto(`/invoices/${testData.invoiceID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-invoice.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Person detail", async ({ page }) => {
        await page.goto(`/people/${testData.personID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-person.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Vendor detail", async ({ page }) => {
        await page.goto(`/vendors/${testData.vendorID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-vendor.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Classification detail", async ({ page }) => {
        await page.goto(`/classifications/${testData.classificationID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-classification.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Tag detail", async ({ page }) => {
        await page.goto(`/tags/${testData.tagID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-tag.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("County detail", async ({ page }) => {
        await page.goto(`/counties/${testData.countyID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-county.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: [...getMapMasks(page), ...getGridBodyMasks(page)],
        });
    });

    test("DNR upland region detail", async ({ page }) => {
        await page.goto(`/dnr-upland-regions/${testData.dnrUplandRegionID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-dnr-upland-region.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: [...getMapMasks(page), ...getGridBodyMasks(page)],
        });
    });

    test("Priority landscape detail", async ({ page }) => {
        await page.goto(`/priority-landscapes/${testData.priorityLandscapeID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-priority-landscape.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: [...getMapMasks(page), ...getGridBodyMasks(page)],
        });
    });

    test("Project type detail", async ({ page }) => {
        await page.goto(`/project-types/${testData.projectTypeID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("detail-project-type.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });
});
