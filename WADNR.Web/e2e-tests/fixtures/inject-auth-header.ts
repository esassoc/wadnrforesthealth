import { APIRequestContext, Page } from "@playwright/test";

const TEST_AUTH_HEADER = "X-E2E-User-GlobalID";

/**
 * Sets up test authentication for E2E tests.
 *
 * 1. Sets localStorage.__e2e_globalID via addInitScript() so the Angular app
 *    detects e2e mode before it bootstraps. This causes:
 *    - app.config.ts to give Auth0 an empty allowedList (no-op interceptor)
 *    - AuthenticationService to call postUser() even without an Auth0 session
 *    - e2eAuthInterceptor to add the X-E2E-User-GlobalID header to all requests
 *
 * 2. Also intercepts /api/ routes at the Playwright level as a belt-and-suspenders
 *    fallback for any requests that might bypass the Angular HTTP client.
 *
 * Call this in test.beforeEach() BEFORE any page.goto().
 */
export async function setupTestAuth(page: Page, globalID: string) {
    // Set localStorage before Angular bootstraps
    await page.addInitScript((id) => {
        localStorage.setItem("__e2e_globalID", id);
    }, globalID);

    // Fallback: intercept API routes at the Playwright level
    await page.route("**/api/**", async (route) => {
        const headers = {
            ...route.request().headers(),
            [TEST_AUTH_HEADER]: globalID,
        };
        await route.continue({ headers });
    });
}

/**
 * Clears any DB-persisted impersonation on the test user.
 *
 * Impersonation is stored on the Person record (ImpersonatedPersonID), so it
 * leaks across browser sessions — including into Playwright tests. This call
 * ensures the test user is always themselves, not whoever a developer was
 * impersonating in their browser.
 *
 * Uses the standalone Playwright APIRequestContext (not the page) so the call
 * doesn't trigger the page-level API error monitor. A 403 is expected when
 * the user isn't currently impersonating — it's silently ignored.
 */
export async function clearImpersonation(request: APIRequestContext, globalID: string) {
    await request.post("/impersonation/stop", {
        headers: { [TEST_AUTH_HEADER]: globalID },
    });
}
