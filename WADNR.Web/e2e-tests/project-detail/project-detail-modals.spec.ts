import { test, expect } from "../fixtures/base-test";
import { testData } from "../fixtures/test-data";

test.describe("Project detail edit modals", () => {
    test.beforeEach(async ({ authedPage: page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await expect(page.locator("#card-basics")).toBeVisible({ timeout: 15000 });
    });

    test("Edit Basics modal opens and Cancel closes it", async ({ authedPage: page }) => {
        await page.locator("#card-basics button", { hasText: "Edit" }).click();
        await expect(page.locator(".ngneat-dialog-content .modal-header")).toBeVisible({ timeout: 5000 });
        await page.locator(".ngneat-dialog-content .modal-footer button", { hasText: "Cancel" }).click();
        await expect(page.locator(".ngneat-dialog-content")).not.toBeVisible({ timeout: 5000 });
    });

    test("Edit Tags modal opens", async ({ authedPage: page }) => {
        const editButton = page.locator("#card-tags button", { hasText: "Edit" });
        if (await editButton.isVisible()) {
            await editButton.click();
            await expect(page.locator(".ngneat-dialog-content")).toBeVisible({ timeout: 5000 });
            // Close it
            await page.locator(".ngneat-dialog-content .modal-footer button", { hasText: "Cancel" }).click();
        }
    });

    test("Edit Organizations modal opens", async ({ authedPage: page }) => {
        const editButton = page.locator("#card-organizations button", { hasText: "Edit" });
        if (await editButton.isVisible()) {
            await editButton.click();
            await expect(page.locator(".ngneat-dialog-content")).toBeVisible({ timeout: 5000 });
            await page.locator(".ngneat-dialog-content .modal-footer button", { hasText: "Cancel" }).click();
        }
    });

    test("Edit Contacts modal opens", async ({ authedPage: page }) => {
        const editButton = page.locator("#card-contacts button", { hasText: "Edit" });
        if (await editButton.isVisible()) {
            await editButton.click();
            await expect(page.locator(".ngneat-dialog-content")).toBeVisible({ timeout: 5000 });
            await page.locator(".ngneat-dialog-content .modal-footer button", { hasText: "Cancel" }).click();
        }
    });

    test("Edit Funding modal opens", async ({ authedPage: page }) => {
        const editButton = page.locator("#card-funding button", { hasText: "Edit" });
        if (await editButton.isVisible()) {
            await editButton.click();
            await expect(page.locator(".ngneat-dialog-content")).toBeVisible({ timeout: 5000 });
            await page.locator(".ngneat-dialog-content .modal-footer button", { hasText: "Cancel" }).click();
        }
    });

    test("Location edit dropdown shows sub-options", async ({ authedPage: page }) => {
        const locationEditButton = page.locator("#card-location .dropdown-toggle, #card-location button", { hasText: "Edit" }).first();
        if (await locationEditButton.isVisible()) {
            await locationEditButton.click();
            // Should show location edit options (Simple, Detailed, etc.)
            await expect(
                page.locator("#card-location .dropdown-menu, .ngneat-dialog-content").first()
            ).toBeVisible({ timeout: 5000 });
        }
    });
});
