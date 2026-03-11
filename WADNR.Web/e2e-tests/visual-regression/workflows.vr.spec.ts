import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import { testData } from "../fixtures/test-data";
import {
    DEFAULT_SCREENSHOT_OPTIONS,
    waitForPageStable,
    getMapMasks,
} from "./visual-config";

test.describe("Workflow pages - visual regression", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    // ─── Create workflow ────────────────────────────────────────────────

    test("Create workflow - basics step", async ({ page }) => {
        await page.goto("/projects/new/basics");
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-create-basics.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    // ─── Edit workflow steps ────────────────────────────────────────────

    test("Edit workflow - basics", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/basics`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-edit-basics.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Edit workflow - location-simple", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/location-simple`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-edit-location-simple.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getMapMasks(page),
        });
    });

    test("Edit workflow - location-detailed", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/location-detailed`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-edit-location-detailed.png", {
            ...DEFAULT_SCREENSHOT_OPTIONS,
            mask: getMapMasks(page),
        });
    });

    test("Edit workflow - contacts", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/contacts`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-edit-contacts.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Edit workflow - organizations", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/organizations`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-edit-organizations.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Edit workflow - expected-funding", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/expected-funding`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-edit-expected-funding.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Edit workflow - treatments", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/treatments`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-edit-treatments.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Edit workflow - classifications", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/classifications`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-edit-classifications.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Edit workflow - photos", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/photos`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-edit-photos.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Edit workflow - attachments", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/attachments`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-edit-attachments.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Edit workflow - notes", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/notes`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-edit-notes.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Edit workflow - tags", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/tags`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-edit-tags.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Edit workflow - review-and-submit", async ({ page }) => {
        await page.goto(`/projects/edit/${testData.projectID}/review-and-submit`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-edit-review-and-submit.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    // ─── Update workflow ────────────────────────────────────────────────

    test("Update workflow - basics", async ({ page }) => {
        await page.goto(`/projects/${testData.projectID}/update/basics`);
        await waitForPageStable(page);
        // May redirect if no active update batch — just capture whatever renders
        await expect(page).toHaveScreenshot("workflow-update-basics.png", DEFAULT_SCREENSHOT_OPTIONS);
    });

    test("Update workflow - expenditures", async ({ page }) => {
        await page.goto(`/projects/${testData.projectID}/update/expenditures`);
        await waitForPageStable(page);
        await expect(page).toHaveScreenshot("workflow-update-expenditures.png", DEFAULT_SCREENSHOT_OPTIONS);
    });
});
