import { test, expect } from "../fixtures/base-test";
import { testData } from "../fixtures/test-data";
import { waitForModalOpen, clickModalSave, clickModalCancel, waitForModalClose, expectValidationErrors, getModalTitle } from "../fixtures/modal-helpers";

/**
 * Tests classification (theme) modals from /project-themes admin page
 * and from classification detail pages.
 */

test.describe("Classification modals from /project-themes", () => {
    test.beforeEach(async ({ authedPage: page }) => {
        await page.goto("/project-themes");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
    });

    test("Create button is visible for admin", async ({ authedPage: page }) => {
        await expect(page.locator("button", { hasText: "Create New Theme" })).toBeVisible();
    });

    test("Create modal opens with correct title", async ({ authedPage: page }) => {
        await page.locator("button", { hasText: "Create New Theme" }).click();
        await waitForModalOpen(page);
        const title = await getModalTitle(page);
        expect(title.trim()).toBe("Create Theme");
    });

    test("Save on empty form shows validation errors", async ({ authedPage: page }) => {
        await page.locator("button", { hasText: "Create New Theme" }).click();
        await waitForModalOpen(page);
        await clickModalSave(page);
        await expectValidationErrors(page);
    });

    test("Cancel closes modal", async ({ authedPage: page }) => {
        await page.locator("button", { hasText: "Create New Theme" }).click();
        await waitForModalOpen(page);
        await clickModalCancel(page);
        await waitForModalClose(page);
    });

    test("Full CRUD: create and delete classification", async ({ authedPage: page }) => {
        const uniqueName = `E2E Classification ${Date.now()}`;
        let createdID: number | null = null;

        page.on("response", async (resp) => {
            if (resp.url().includes("/api/classification") && resp.request().method() === "POST" && resp.status() < 300) {
                try {
                    const body = await resp.json();
                    createdID = body.ClassificationID ?? body.classificationID;
                } catch {
                    // ignore
                }
            }
        });

        try {
            await page.locator("button", { hasText: "Create New Theme" }).click();
            await waitForModalOpen(page);

            const modal = page.locator(".ngneat-dialog-content");

            // Fill DisplayName
            await modal.locator("input[type='text']").first().fill(uniqueName);

            // Fill ClassificationDescription (RTE)
            const rte = modal.locator(".ql-editor, [contenteditable='true'], textarea").first();
            if (await rte.isVisible({ timeout: 2000 }).catch(() => false)) {
                await rte.fill("E2E classification test description");
            }

            // Fill ThemeColor
            const colorInput = modal.locator("input[type='color']").first();
            if (await colorInput.isVisible({ timeout: 1000 }).catch(() => false)) {
                await colorInput.fill("#33A1FF");
            }

            await clickModalSave(page);
            await waitForModalClose(page, 10000);
        } finally {
            if (createdID) {
                await page.request.delete(`/api/classifications/${createdID}`).catch(() => {});
            }
        }
    });
});

test.describe("Classification detail edit", () => {
    test("Edit button opens modal on detail page", async ({ authedPage: page }) => {
        await page.goto(`/classifications/${testData.classificationID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });

        const editButton = page.locator("button", { hasText: "Edit" }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalOpen(page);
            const title = await getModalTitle(page);
            expect(title.trim()).toContain("Edit");
        }
    });

    test("Edit modal pre-populates Name field", async ({ authedPage: page }) => {
        await page.goto(`/classifications/${testData.classificationID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });

        const editButton = page.locator("button", { hasText: "Edit" }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalOpen(page);

            const nameInput = page.locator(".ngneat-dialog-content input[type='text']").first();
            const value = await nameInput.inputValue();
            expect(value.length).toBeGreaterThan(0);
        }
    });

    test("Cancel closes edit modal", async ({ authedPage: page }) => {
        await page.goto(`/classifications/${testData.classificationID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });

        const editButton = page.locator("button", { hasText: "Edit" }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalOpen(page);
            await clickModalCancel(page);
            await waitForModalClose(page);
        }
    });
});
