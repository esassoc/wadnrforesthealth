import { Page } from "@playwright/test";

const TEST_AUTH_HEADER = "X-E2E-User-GlobalID";

/**
 * Intercepts all API calls made by the Angular app and injects the
 * X-E2E-User-GlobalID header. The request still goes to the real API —
 * we're just adding a header so the TestAuthHandler can identify the user.
 */
async function injectTestAuthHeader(page: Page, globalID: string) {
  await page.route("**/api/**", async (route) => {
    const headers = {
      ...route.request().headers(),
      [TEST_AUTH_HEADER]: globalID,
    };
    await route.continue({ headers });
  });
}

/**
 * Sets up test authentication for E2E tests.
 *
 * This injects the X-E2E-User-GlobalID header on all /api/ requests.
 * For endpoints marked [AllowAnonymous] on the backend, this is sufficient —
 * the request goes through and the header identifies the user.
 *
 * For endpoints that require auth (e.g. [NormalUserFeature]), the Auth0
 * authHttpInterceptorFn must also let the request through. This requires
 * the Auth0 SDK to have a valid token. Without a real Auth0 session,
 * secured-only endpoints will not receive data.
 *
 * Call this in test.beforeEach() for tests that need an authenticated user.
 */
export async function setupTestAuth(page: Page, globalID: string) {
  await injectTestAuthHeader(page, globalID);
}
