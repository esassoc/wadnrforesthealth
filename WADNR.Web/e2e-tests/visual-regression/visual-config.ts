import { Page, Locator } from "@playwright/test";

// ─── Viewports ──────────────────────────────────────────────────────────────

export const VIEWPORTS = {
    mobile: { width: 375, height: 812 },
    tablet: { width: 768, height: 1024 },
    desktop: { width: 1280, height: 900 },
    large: { width: 1920, height: 1080 },
} as const;

// ─── Screenshot Options ─────────────────────────────────────────────────────

export const DEFAULT_SCREENSHOT_OPTIONS = {
    fullPage: true,
    animations: "disabled" as const,
};

export const MAP_PAGE_OPTIONS = {
    ...DEFAULT_SCREENSHOT_OPTIONS,
};

export const MODAL_SCREENSHOT_OPTIONS = {
    animations: "disabled" as const,
};

// ─── Stability Helpers ──────────────────────────────────────────────────────

/**
 * Wait for a page to be fully stable (network idle, no spinners, no animations).
 */
export async function waitForPageStable(page: Page, timeout = 30000): Promise<void> {
    await page.waitForLoadState("networkidle", { timeout });

    // Wait for Angular loading spinners to disappear
    try {
        await page.waitForSelector("app-loading-spinner", { state: "detached", timeout: 5000 });
    } catch {
        // No spinner found
    }

    // Wait for CSS animations to settle
    await page.waitForTimeout(500);
}

/**
 * Wait for AG Grid to finish loading data.
 */
export async function waitForGrid(page: Page, timeout = 30000): Promise<void> {
    try {
        const hasGrid = await page.locator(".ag-root").count();
        if (hasGrid > 0) {
            // Wait for loading overlay to disappear
            await page.waitForSelector(".ag-overlay-loading-center", { state: "detached", timeout });
            // Wait for at least one row to appear
            await page.locator(".ag-row").first().waitFor({ state: "visible", timeout: 15000 });
        }
    } catch {
        // Grid may not have data or may not be present
    }
}

/**
 * Wait for a modal to be fully rendered and stable.
 */
export async function waitForModalStable(page: Page): Promise<void> {
    await page.locator(".ngneat-dialog-content .modal").waitFor({ state: "visible", timeout: 5000 });
    await page.waitForTimeout(300); // Allow animations to complete
}

// ─── Non-Determinism Masks ──────────────────────────────────────────────────

/**
 * Get locators to mask Leaflet map containers (non-deterministic tile rendering).
 */
export function getMapMasks(page: Page): Locator[] {
    return [page.locator(".leaflet-container")];
}

/**
 * Get locators to mask AG Grid body viewports (row data may vary).
 */
export function getGridBodyMasks(page: Page): Locator[] {
    return [page.locator(".ag-body-viewport")];
}

/**
 * Get locators for common non-deterministic elements.
 */
export function getCommonMasks(page: Page): Locator[] {
    return [
        ...getMapMasks(page),
        ...getGridBodyMasks(page),
    ];
}
