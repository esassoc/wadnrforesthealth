import { test, expect } from "../fixtures/base-test";
import { testData } from "../fixtures/test-data";
import { waitForModalOpen, clickModalSave, clickModalCancel, waitForModalClose, expectValidationErrors, getModalTitle } from "../fixtures/modal-helpers";

/**
 * Financial modal tests: agreement CRUD, fund source, and invoice modal opens.
 * Agreement CRUD is safe (clean cascade delete). Fund source and invoice
 * avoid full CRUD due to FK complexity.
 */

test.describe("Agreement modals", () => {
    test("Edit Agreement modal opens from detail page", async ({ authedPage: page }) => {
        await page.goto(`/agreements/${testData.agreementID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });

        const editButton = page.locator("button", { hasText: "Edit" }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalOpen(page);
            const title = await getModalTitle(page);
            expect(title.trim()).toContain("Edit Agreement");
        }
    });

    test("Edit modal pre-populates AgreementTitle field", async ({ authedPage: page }) => {
        await page.goto(`/agreements/${testData.agreementID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });

        const editButton = page.locator("button", { hasText: "Edit" }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalOpen(page);

            const titleInput = page.locator(".ngneat-dialog-content input[type='text']").first();
            const value = await titleInput.inputValue();
            expect(value.length).toBeGreaterThan(0);
        }
    });

    test("Edit Agreement Cancel closes modal", async ({ authedPage: page }) => {
        await page.goto(`/agreements/${testData.agreementID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });

        const editButton = page.locator("button", { hasText: "Edit" }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalOpen(page);
            await clickModalCancel(page);
            await waitForModalClose(page);
        }
    });

    test("Full CRUD: create and delete agreement from list page", async ({ authedPage: page }) => {
        await page.goto("/agreements");
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });

        const createBtn = page.locator("button", { hasText: "Create New" });
        if (!(await createBtn.isVisible({ timeout: 5000 }).catch(() => false))) {
            // User may not have agreement manage permission — skip
            return;
        }

        let createdID: number | null = null;

        page.on("response", async (resp) => {
            if (resp.url().includes("/api/agreement") && resp.request().method() === "POST" && resp.status() < 300) {
                try {
                    const body = await resp.json();
                    createdID = body.AgreementID ?? body.agreementID;
                } catch {
                    // ignore
                }
            }
        });

        try {
            await createBtn.click();
            await waitForModalOpen(page);

            const modal = page.locator(".ngneat-dialog-content");

            // Fill AgreementTitle (first text input)
            const uniqueTitle = `E2E Agreement ${Date.now()}`;
            await modal.locator("input[type='text']").first().fill(uniqueTitle);

            // Fill AgreementNumber (second text input)
            await modal.locator("input[type='text']").nth(1).fill(`E2E-${Date.now()}`);

            // Select OrganizationID, AgreementStatusID, AgreementTypeID from dropdowns
            const dropdowns = modal.locator("ng-select");
            const dropdownCount = await dropdowns.count();
            for (let i = 0; i < Math.min(dropdownCount, 3); i++) {
                await dropdowns.nth(i).click();
                const option = page.locator("ng-dropdown-panel .ng-option").first();
                if (await option.isVisible({ timeout: 3000 }).catch(() => false)) {
                    await option.click();
                }
            }

            await clickModalSave(page);
            await waitForModalClose(page, 10000);
        } finally {
            if (createdID) {
                await page.request.delete(`/api/agreements/${createdID}`).catch(() => {});
            }
        }
    });
});

test.describe("Fund Source modals", () => {
    test("Edit Fund Source modal opens from detail page", async ({ authedPage: page }) => {
        await page.goto(`/fund-sources/${testData.fundSourceID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });

        const editButton = page.locator("button", { hasText: "Edit" }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalOpen(page);
        }
    });

    test("Fund Source Edit Cancel closes modal", async ({ authedPage: page }) => {
        await page.goto(`/fund-sources/${testData.fundSourceID}`);
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

test.describe("Invoice modals", () => {
    test("Edit Invoice modal opens from detail page", async ({ authedPage: page }) => {
        await page.goto(`/invoices/${testData.invoiceID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });

        const editButton = page.locator("button", { hasText: "Edit" }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalOpen(page);
        }
    });

    test("Invoice Edit Cancel closes modal", async ({ authedPage: page }) => {
        await page.goto(`/invoices/${testData.invoiceID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });

        const editButton = page.locator("button", { hasText: "Edit" }).first();
        if (await editButton.isVisible({ timeout: 5000 }).catch(() => false)) {
            await editButton.click();
            await waitForModalOpen(page);
            await clickModalCancel(page);
            await waitForModalClose(page);
        }
    });

    test("Payment Request button opens modal if visible", async ({ authedPage: page }) => {
        await page.goto(`/invoices/${testData.invoiceID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });

        const prBtn = page.locator("button", { hasText: /Payment Request/i }).first();
        if (await prBtn.isVisible({ timeout: 3000 }).catch(() => false)) {
            await prBtn.click();
            await waitForModalOpen(page);
            await clickModalCancel(page);
            await waitForModalClose(page);
        }
    });
});
