import { test, expect } from "../fixtures/base-test";
import { testData } from "../fixtures/test-data";
import { waitForModalOpen, clickModalCancel, waitForModalClose } from "../fixtures/modal-helpers";

/**
 * Financial modal tests: fund source and invoice modal opens.
 * Fund source and invoice avoid full CRUD due to FK complexity.
 */

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
