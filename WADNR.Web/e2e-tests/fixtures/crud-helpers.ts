/**
 * Reusable CRUD modal test helpers.
 *
 * Extracts the repeated pattern from admin-crud-modals.spec.ts into composable
 * functions that can be used across entity-specific CRUD test files.
 */

import { Page, expect } from "@playwright/test";
import { waitForModalOpen, clickModalSave, clickModalCancel, waitForModalClose, expectValidationErrors, getModalTitle } from "./modal-helpers";

const MODAL = ".ngneat-dialog-content";

/**
 * Click a button to open a modal and verify it opened with the expected title.
 */
export async function testCrudModalOpen(
    page: Page,
    buttonText: string,
    expectedTitle: string,
): Promise<void> {
    await page.locator("button", { hasText: buttonText }).click();
    await waitForModalOpen(page);
    const title = await getModalTitle(page);
    expect(title.trim()).toBe(expectedTitle);
}

/**
 * Open a modal via button click, submit with empty form, and verify
 * that at least the expected number of validation errors appear.
 */
export async function testModalValidation(
    page: Page,
    buttonText: string,
    minExpectedErrors = 1,
): Promise<void> {
    await page.locator("button", { hasText: buttonText }).click();
    await waitForModalOpen(page);
    await clickModalSave(page);
    await expectValidationErrors(page);

    const errorCount = await page.locator(`${MODAL} note`).count();
    expect(errorCount).toBeGreaterThanOrEqual(minExpectedErrors);

    await clickModalCancel(page);
    await waitForModalClose(page);
}

/**
 * Open a modal, fill fields, save, and verify the result appears in the grid.
 *
 * @param page - Playwright page
 * @param buttonText - Text of the button that opens the modal
 * @param fieldFills - Array of { label, value } pairs to fill in the modal
 * @param gridVerifyText - Text to verify in the grid after saving
 * @returns The filled values (useful for cleanup)
 */
export async function testModalSaveAndVerify(
    page: Page,
    buttonText: string,
    fieldFills: { label: string; value: string }[],
    gridVerifyText: string,
): Promise<void> {
    await page.locator("button", { hasText: buttonText }).click();
    await waitForModalOpen(page);

    const modal = page.locator(MODAL);

    for (const { label, value } of fieldFills) {
        // Try form-field component first, then fall back to generic inputs
        const formField = modal.locator("form-field").filter({ hasText: label });
        const input = formField.locator("input, textarea, select").first();

        if (await input.isVisible({ timeout: 2000 }).catch(() => false)) {
            await input.fill(value);
        } else {
            // Fallback: find input by placeholder or nearby label
            const fallbackInput = modal.locator(`input[placeholder*="${label}" i], textarea[placeholder*="${label}" i]`).first();
            if (await fallbackInput.isVisible({ timeout: 1000 }).catch(() => false)) {
                await fallbackInput.fill(value);
            }
        }
    }

    await clickModalSave(page);
    await waitForModalClose(page, 10000);

    // Verify the new entry appears in the grid
    await expect(
        page.locator(".ag-row", { hasText: gridVerifyText }).first(),
    ).toBeVisible({ timeout: 15000 });
}

/**
 * Open a modal and close it via Cancel, verifying it closes cleanly.
 */
export async function testModalCancel(
    page: Page,
    buttonText: string,
): Promise<void> {
    await page.locator("button", { hasText: buttonText }).click();
    await waitForModalOpen(page);
    await clickModalCancel(page);
    await waitForModalClose(page);
}

/**
 * Wait for a page's heading to be visible (common page-load gate).
 */
export async function waitForPageHeading(page: Page, timeout = 15000): Promise<void> {
    await expect(page.locator("h2.page-title")).toBeVisible({ timeout });
}

/**
 * Intercept a POST response to capture the created entity's ID.
 * Returns a function that retrieves the captured ID.
 */
export function interceptCreatedId(
    page: Page,
    apiPathFragment: string,
    idFieldName: string,
): () => number | null {
    let createdID: number | null = null;

    page.on("response", async (resp) => {
        if (
            resp.url().includes(apiPathFragment) &&
            resp.request().method() === "POST" &&
            resp.status() < 300
        ) {
            try {
                const body = await resp.json();
                // Try both PascalCase and camelCase
                createdID = body[idFieldName] ?? body[idFieldName.charAt(0).toLowerCase() + idFieldName.slice(1)];
            } catch {
                // ignore parse errors
            }
        }
    });

    return () => createdID;
}
