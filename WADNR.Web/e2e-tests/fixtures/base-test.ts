import { test as base, expect, Page } from "@playwright/test";
import { setupTestAuth, clearImpersonation } from "./inject-auth-header";
import { testUsers } from "./test-users";

/**
 * Collects API errors (4xx/5xx) during a test.
 * Attached to both authedPage and publicPage fixtures.
 *
 * Excludes known false positives:
 * - /api/custom-pages/vanity-url/ 404s: background CMS lookups that 404 normally
 */
function attachApiErrorMonitor(page: Page): string[] {
    const errors: string[] = [];
    page.on("response", (response) => {
        const url = response.url();
        const status = response.status();
        if (url.includes("/api/") && status >= 400) {
            // Custom page lookups (vanity URL, about, detail) 404 normally for non-CMS routes
            if (url.includes("/api/custom-pages/") && status === 404) {
                return;
            }
            errors.push(`${status} ${response.request().method()} ${url}`);
        }
    });
    return errors;
}

export const test = base.extend<{ authedPage: Page; publicPage: Page }>({
    /**
     * A page with test auth configured and API error monitoring.
     * Automatically fails the test if any API request returned 4xx/5xx.
     */
    authedPage: async ({ page, request }, use) => {
        await setupTestAuth(page, testUsers.admin);
        await clearImpersonation(request, testUsers.admin);
        const errors = attachApiErrorMonitor(page);
        await use(page);
        expect(errors, `API errors during test:\n${errors.join("\n")}`).toHaveLength(0);
    },

    /**
     * A page with API error monitoring but no authentication.
     * For public/anonymous page tests.
     */
    publicPage: async ({ page }, use) => {
        const errors = attachApiErrorMonitor(page);
        await use(page);
        expect(errors, `API errors during test:\n${errors.join("\n")}`).toHaveLength(0);
    },
});

export { expect };
