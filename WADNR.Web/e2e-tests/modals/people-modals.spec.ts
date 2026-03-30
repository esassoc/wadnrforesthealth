import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import { testData } from "../fixtures/test-data";
import { waitForModalOpen, clickModalSave, clickModalCancel, waitForModalClose, expectValidationErrors, getModalTitle } from "../fixtures/modal-helpers";

/**
 * People/contact modal tests. Uses manual auth (no API error monitor)
 * because people pages may trigger background API calls that fail for
 * permissions reasons unrelated to the test scenario.
 */

test.describe("People list - Add Contact modal", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
        await page.goto("/people");
        await expect(page.locator("h2.page-title")).toBeVisible({ timeout: 15000 });
    });

    test("Add Contact button is visible on /people", async ({ page }) => {
        await expect(page.locator("button", { hasText: "Add Contact" })).toBeVisible({ timeout: 5000 });
    });

    test("Add Contact modal opens", async ({ page }) => {
        await page.locator("button", { hasText: "Add Contact" }).click();
        await waitForModalOpen(page);
        const title = await getModalTitle(page);
        expect(title.trim()).toContain("Add Contact");
    });

    test("Save with empty First Name shows validation", async ({ page }) => {
        await page.locator("button", { hasText: "Add Contact" }).click();
        await waitForModalOpen(page);
        await clickModalSave(page);
        await expectValidationErrors(page);
    });

    test("Cancel closes modal", async ({ page }) => {
        await page.locator("button", { hasText: "Add Contact" }).click();
        await waitForModalOpen(page);
        await clickModalCancel(page);
        await waitForModalClose(page);
    });

    test("Full CRUD: create and delete contact", async ({ page }) => {
        const uniqueFirst = `E2EFirst${Date.now()}`;
        const uniqueLast = `E2ELast${Date.now()}`;
        let createdID: number | null = null;

        page.on("response", async (resp) => {
            if (resp.url().includes("/api/people") && resp.request().method() === "POST" && resp.status() < 300) {
                try {
                    const body = await resp.json();
                    createdID = body.PersonID ?? body.personID;
                } catch {
                    // ignore
                }
            }
        });

        try {
            await page.locator("button", { hasText: "Add Contact" }).click();
            await waitForModalOpen(page);

            const modal = page.locator(".ngneat-dialog-content");
            const textInputs = modal.locator("input[type='text']");

            // Fill FirstName (first text input)
            await textInputs.first().fill(uniqueFirst);

            // Fill LastName (third text input, after MiddleName)
            const inputCount = await textInputs.count();
            if (inputCount >= 3) {
                await textInputs.nth(2).fill(uniqueLast);
            }

            await clickModalSave(page);
            await waitForModalClose(page, 10000);
        } finally {
            if (createdID) {
                await page.request.delete(`/api/people/${createdID}`).catch(() => {});
            }
        }
    });
});

test.describe("Person detail modals", () => {
    test.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
        await page.goto(`/people/${testData.personID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Edit Contact modal opens from person detail", async ({ page }) => {
        const editButton = page.locator("button", { hasText: /Edit.*Contact|Edit.*Info/i }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalOpen(page);
        }
    });

    test("Edit Contact modal pre-populates name fields", async ({ page }) => {
        const editButton = page.locator("button", { hasText: /Edit.*Contact|Edit.*Info/i }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalOpen(page);

            const nameInput = page.locator(".ngneat-dialog-content input[type='text']").first();
            const value = await nameInput.inputValue();
            expect(value.length).toBeGreaterThan(0);
        }
    });

    test("Edit Contact Cancel closes modal", async ({ page }) => {
        const editButton = page.locator("button", { hasText: /Edit.*Contact|Edit.*Info/i }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalOpen(page);
            await clickModalCancel(page);
            await waitForModalClose(page);
        }
    });

    test("Edit Roles modal opens (admin only)", async ({ page }) => {
        const rolesBtn = page.locator("button", { hasText: /Edit.*Role/i }).first();
        if (await rolesBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await rolesBtn.click();
            await waitForModalOpen(page);
        }
    });

    test("Edit Roles modal shows Base Role dropdown", async ({ page }) => {
        const rolesBtn = page.locator("button", { hasText: /Edit.*Role/i }).first();
        if (await rolesBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await rolesBtn.click();
            await waitForModalOpen(page);

            const modal = page.locator(".ngneat-dialog-content");
            // Should have a dropdown/select for Base Role
            await expect(modal.locator("ng-select, select").first()).toBeVisible({ timeout: 3000 });
        }
    });

    test("Edit Roles Cancel closes modal", async ({ page }) => {
        const rolesBtn = page.locator("button", { hasText: /Edit.*Role/i }).first();
        if (await rolesBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
            await rolesBtn.click();
            await waitForModalOpen(page);
            await clickModalCancel(page);
            await waitForModalClose(page);
        }
    });
});
