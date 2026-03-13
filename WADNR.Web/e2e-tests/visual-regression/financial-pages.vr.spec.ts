/**
 * Visual regression tests for financial entity detail pages.
 *
 * Covers detail pages for fund sources, fund source allocations,
 * agreements, invoices, program indices, and project codes.
 */

import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import { testData } from "../fixtures/test-data";
import {
    DEFAULT_SCREENSHOT_OPTIONS,
    waitForPageStable,
    waitForGrid,
    getGridBodyMasks,
} from "./visual-config";

test.describe("Financial pages - visual regression", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    // ── List pages ─────────────────────────────────────────────────────────────

    test("Fund sources list", async ({ page }) => {
        await page.goto("/fund-sources");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("fund-sources-list.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Program indices list", async ({ page }) => {
        await page.goto("/program-indices");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("program-indices-list.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Project codes list", async ({ page }) => {
        await page.goto("/project-codes");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("project-codes-list.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    // ── Detail pages ───────────────────────────────────────────────────────────

    test("Fund source allocation detail", async ({ page }) => {
        await page.goto(`/fund-source-allocations/${testData.fundSourceAllocationID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("financial-fund-source-allocation.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Program index detail", async ({ page }) => {
        await page.goto(`/program-indices/${testData.programIndexID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("financial-program-index.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Project code detail", async ({ page }) => {
        await page.goto(`/project-codes/${testData.projectCodeID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("financial-project-code.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Interaction event detail", async ({ page }) => {
        await page.goto(`/interactions-events/${testData.interactionEventID}`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("financial-interaction-event.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });
});
