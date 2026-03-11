import { Page, expect } from "@playwright/test";

const MODAL = ".ngneat-dialog-content";
const MODAL_INNER = `${MODAL} .modal`;
const MODAL_HEADER = `${MODAL_INNER} .modal-header`;
const MODAL_FOOTER = `${MODAL_INNER} .modal-footer`;

/**
 * Waits for a modal dialog to become visible.
 */
export async function waitForModalOpen(page: Page, timeout = 5000) {
    await expect(page.locator(MODAL_INNER)).toBeVisible({ timeout });
}

/**
 * Clicks the Save button in the modal footer.
 */
export async function clickModalSave(page: Page) {
    await page.locator(`${MODAL_FOOTER} button`, { hasText: "Save" }).click();
}

/**
 * Clicks the Cancel button in the modal footer.
 */
export async function clickModalCancel(page: Page) {
    await page.locator(`${MODAL_FOOTER} button`, { hasText: "Cancel" }).click();
}

/**
 * Waits for the modal dialog to close (become invisible).
 */
export async function waitForModalClose(page: Page, timeout = 5000) {
    await expect(page.locator(MODAL)).not.toBeVisible({ timeout });
}

/**
 * Asserts that at least one validation error is visible inside the modal.
 * input-errors renders: <note noteType="danger">This field is required.</note>
 */
export async function expectValidationErrors(page: Page) {
    await expect(page.locator(`${MODAL} note`).first()).toBeVisible({ timeout: 3000 });
}

/**
 * Returns the text content of the modal header (h3 or h4).
 */
export async function getModalTitle(page: Page): Promise<string> {
    return (await page.locator(`${MODAL_HEADER} h3, ${MODAL_HEADER} h4`).textContent()) ?? "";
}

/**
 * Fills a text input inside the modal by its form-field label or placeholder.
 */
export async function fillModalField(page: Page, label: string, value: string) {
    const field = page.locator(`${MODAL} form-field`).filter({ hasText: label }).locator("input, textarea").first();
    await field.fill(value);
}
