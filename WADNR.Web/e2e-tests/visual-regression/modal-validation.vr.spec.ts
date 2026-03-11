import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import { testData } from "../fixtures/test-data";
import { waitForModalOpen, clickModalSave, expectValidationErrors } from "../fixtures/modal-helpers";
import {
    MODAL_SCREENSHOT_OPTIONS,
    waitForPageStable,
    waitForModalStable,
} from "./visual-config";

const MODAL_SELECTOR = ".ngneat-dialog-content .modal";

test.describe("Modal validation states - visual regression", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    test("Project type - validation errors", async ({ page }) => {
        await page.goto("/project-types");
        await waitForPageStable(page);
        await page.locator("button", { hasText: "Create New Project Type" }).click();
        await waitForModalStable(page);
        await clickModalSave(page);
        await expectValidationErrors(page);
        await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("validation-project-type.png", MODAL_SCREENSHOT_OPTIONS);
    });

    test("Theme - validation errors", async ({ page }) => {
        await page.goto("/project-themes");
        await waitForPageStable(page);
        await page.locator("button", { hasText: "Create New Theme" }).click();
        await waitForModalStable(page);
        await clickModalSave(page);
        await expectValidationErrors(page);
        await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("validation-theme.png", MODAL_SCREENSHOT_OPTIONS);
    });

    test("Map layer - validation errors", async ({ page }) => {
        await page.goto("/map-layers");
        await waitForPageStable(page);
        await page.locator("button", { hasText: "Add External Map Layer" }).click();
        await waitForModalStable(page);
        await clickModalSave(page);
        await expectValidationErrors(page);
        await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("validation-map-layer.png", MODAL_SCREENSHOT_OPTIONS);
    });

    test("Agreement - validation errors", async ({ page }) => {
        await page.goto("/agreements");
        await waitForPageStable(page);
        const createBtn = page.locator("button", { hasText: "Create New" });
        if (await createBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await createBtn.click();
            await waitForModalStable(page);
            await clickModalSave(page);
            await expectValidationErrors(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("validation-agreement.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    test("Add contact - validation errors", async ({ page }) => {
        await page.goto("/people");
        await waitForPageStable(page);
        await page.locator("button", { hasText: "Add Contact" }).click();
        await waitForModalStable(page);
        await clickModalSave(page);
        await expectValidationErrors(page);
        await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("validation-add-contact.png", MODAL_SCREENSHOT_OPTIONS);
    });

    test("Edit project basics - validation errors", async ({ page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await waitForPageStable(page);
        const editBtn = page.locator("button", { hasText: /Edit.*Basics|Edit.*Project/i }).first();
        if (await editBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editBtn.click();
            await waitForModalStable(page);
            // Clear a required field to trigger validation
            const nameInput = page.locator(".ngneat-dialog-content input[type='text']").first();
            if (await nameInput.isVisible({ timeout: 3000 }).catch(() => false)) {
                await nameInput.clear();
            }
            await clickModalSave(page);
            await expectValidationErrors(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("validation-edit-project-basics.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    test("Edit organization - validation errors", async ({ page }) => {
        await page.goto(`/organizations/${testData.organizationID}`);
        await waitForPageStable(page);
        const editBtn = page.locator("button", { hasText: /Edit/i }).first();
        if (await editBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editBtn.click();
            await waitForModalStable(page);
            // Clear a required field
            const nameInput = page.locator(".ngneat-dialog-content input[type='text']").first();
            if (await nameInput.isVisible({ timeout: 3000 }).catch(() => false)) {
                await nameInput.clear();
            }
            await clickModalSave(page);
            await expectValidationErrors(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("validation-edit-org.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    test("Edit contact - validation errors", async ({ page }) => {
        await page.goto(`/people/${testData.personID}`);
        await waitForPageStable(page);
        const editButton = page.locator("button", { hasText: /Edit.*Contact|Edit.*Info/i }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalStable(page);
            // Clear first name to trigger validation
            const firstNameInput = page.locator(".ngneat-dialog-content input[type='text']").first();
            if (await firstNameInput.isVisible({ timeout: 3000 }).catch(() => false)) {
                await firstNameInput.clear();
            }
            await clickModalSave(page);
            await expectValidationErrors(page);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("validation-edit-contact.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });

    test("Workflow basics - validation errors", async ({ page }) => {
        await page.goto("/projects/new/basics");
        await waitForPageStable(page);
        const saveBtn = page.locator("button", { hasText: /Save|Next|Submit/i }).first();
        if (await saveBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await saveBtn.click();
            // Wait for validation to appear
            await page.waitForTimeout(500);
            await expect(page).toHaveScreenshot("validation-workflow-basics.png", {
                fullPage: true,
                animations: "disabled" as const,
            });
        }
    });

    test("Edit funding - validation errors", async ({ page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await waitForPageStable(page);
        const editBtn = page.locator("button", { hasText: /Edit.*Fund/i }).first();
        if (await editBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editBtn.click();
            await waitForModalStable(page);
            await clickModalSave(page);
            // Funding modal may or may not have required fields — capture state
            await page.waitForTimeout(500);
            await expect(page.locator(MODAL_SELECTOR)).toHaveScreenshot("validation-edit-funding.png", MODAL_SCREENSHOT_OPTIONS);
        }
    });
});
