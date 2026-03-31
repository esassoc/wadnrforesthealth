/**
 * Visual regression tests for admin and authenticated pages not covered
 * by the existing auth-pages.vr.spec.ts and public-pages.vr.spec.ts.
 *
 * Covers: project management pages, admin configuration pages, and
 * other authenticated pages that were missing from VR baselines.
 */

import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import {
    DEFAULT_SCREENSHOT_OPTIONS,
    waitForPageStable,
    waitForGrid,
    getGridBodyMasks,
} from "./visual-config";

test.describe("Admin pages - visual regression", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    // ── Project Management ─────────────────────────────────────────────────────

    test("Featured projects", async ({ page }) => {
        await page.goto("/featured-projects");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("featured-projects.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Project updates", async ({ page }) => {
        await page.goto("/project-updates");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("project-updates.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    // ── Admin Configuration ────────────────────────────────────────────────────

    test("Organization and relationship types", async ({ page }) => {
        await page.goto("/organization-and-relationship-types");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("org-relationship-types.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Jobs", async ({ page }) => {
        await page.goto("/jobs");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("jobs.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Upload excel files", async ({ page }) => {
        await page.goto("/upload-excel-files");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("upload-excel-files.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Labels and definitions", async ({ page }) => {
        await page.goto("/labels-and-definitions");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("labels-and-definitions.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Manage page content", async ({ page }) => {
        await page.goto("/manage-page-content");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("manage-page-content.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Manage custom pages", async ({ page }) => {
        await page.goto("/manage-custom-pages");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("manage-custom-pages.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Homepage configuration", async ({ page }) => {
        await page.goto("/homepage-configuration");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("homepage-configuration.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Internal setup notes", async ({ page }) => {
        await page.goto("/internal-setup-notes");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("internal-setup-notes.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Manage find your forester", async ({ page }) => {
        await page.goto("/manage-find-your-forester");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("manage-find-your-forester.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    // ── Report pages ───────────────────────────────────────────────────────────

    test("Project reports", async ({ page }) => {
        await page.goto("/reports/projects");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("project-reports.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("IPR reports", async ({ page }) => {
        await page.goto("/reports/ipr");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("ipr-reports.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    // ── Additional list pages ──────────────────────────────────────────────────

    test("Project steward organizations", async ({ page }) => {
        await page.goto("/project-steward-organizations");
        await waitForPageStable(page);
        await waitForGrid(page);
        await expect(page).toHaveScreenshot("project-steward-organizations.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getGridBodyMasks(page),
        });
    });

    test("Projects by type", async ({ page }) => {
        await page.goto("/projects-by-type");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("projects-by-type.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Projects by theme", async ({ page }) => {
        await page.goto("/projects-by-theme");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("projects-by-theme.png", DEFAULT_SCREENSHOT_OPTIONS);
    });
});
