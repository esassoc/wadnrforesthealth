import { Browser, BrowserContext } from "@playwright/test";
import * as fs from "fs";
import * as path from "path";

const AUTH_STATE_PATH = path.resolve(__dirname, ".legacy-auth.json");

const LEGACY_BASE_URL = process.env.LEGACY_BASE_URL!;
const LEGACY_USERNAME = process.env.LEGACY_USERNAME;
const LEGACY_PASSWORD = process.env.LEGACY_PASSWORD;
const LEGACY_AUTH_METHOD = process.env.LEGACY_AUTH_METHOD ?? "saw";

/**
 * Authenticate to the legacy WADNR MVC site via SAML (ADFS or SAW).
 *
 * Flow:
 *   1. Navigate to /Account/Login on the legacy site
 *   2. Click "Log In with WA Account" (ADFS) or "Log In with SecureAccess WA" (SAW)
 *      — these are <a class="btn btn-firma btn-lg"> links pointing to the IdP
 *   3. Fill the IdP login form and submit
 *   4. IdP POSTs SAMLResponse back to /Account/ADFSPost or /Account/SAWPost
 *   5. Legacy app sets cookie "WashingtonDNRForestHealthTrackerIdentity" and redirects home
 *   6. Save browser cookies to disk for reuse
 *
 * SAW form (test-secureaccess.wa.gov):
 *   - POST to /pkmslogin.form
 *   - #username (text), #password (password), submit button.button
 *
 * ADFS form (Microsoft on-prem):
 *   - #userNameInput, #passwordInput, #submitButton
 */
export async function authenticateToLegacy(browser: Browser): Promise<void> {
    if (!LEGACY_BASE_URL) {
        throw new Error("LEGACY_BASE_URL environment variable is required");
    }
    if (!LEGACY_USERNAME || !LEGACY_PASSWORD) {
        throw new Error("LEGACY_USERNAME and LEGACY_PASSWORD environment variables are required");
    }

    // Reuse saved auth state if still valid
    if (fs.existsSync(AUTH_STATE_PATH)) {
        try {
            const saved = JSON.parse(fs.readFileSync(AUTH_STATE_PATH, "utf-8"));
            if (saved.expires && new Date(saved.expires) > new Date()) {
                console.log("Using saved legacy auth state");
                return;
            }
        } catch {
            // Saved state is invalid, re-authenticate
        }
    }

    console.log(`Authenticating to legacy site via ${LEGACY_AUTH_METHOD.toUpperCase()}...`);

    const context = await browser.newContext({ ignoreHTTPSErrors: true });
    const page = await context.newPage();

    try {
        // ── Step 1: Navigate to legacy login page ──
        await page.goto(`${LEGACY_BASE_URL}/Account/Login`, {
            waitUntil: "networkidle",
            timeout: 30_000,
        });

        // ── Step 2: Click the login button for the chosen auth method ──
        // The login page has two <a class="btn btn-firma btn-lg"> links
        if (LEGACY_AUTH_METHOD === "adfs") {
            await page.locator('a.btn:has-text("Log In with WA Account")').click({ timeout: 10_000 });
        } else {
            await page.locator('a.btn:has-text("Log In with SecureAccess WA")').click({ timeout: 10_000 });
        }

        // ── Step 3: Wait for IdP login page, then fill credentials ──
        await page.waitForLoadState("networkidle", { timeout: 30_000 });

        if (LEGACY_AUTH_METHOD === "saw") {
            // SAW form: test-secureaccess.wa.gov
            // Form POSTs to /pkmslogin.form with fields #username, #password
            await page.locator("#username").fill(LEGACY_USERNAME, { timeout: 10_000 });
            await page.locator("#password").fill(LEGACY_PASSWORD, { timeout: 10_000 });
            await page.locator('#submit-button-row button[type="submit"]').click({ timeout: 10_000 });
        } else {
            // ADFS on-prem form: standard Microsoft ADFS
            // Fields: #userNameInput, #passwordInput, #submitButton
            await page.locator("#userNameInput").fill(LEGACY_USERNAME, { timeout: 10_000 });
            await page.locator("#passwordInput").fill(LEGACY_PASSWORD, { timeout: 10_000 });
            await page.locator("#submitButton").click({ timeout: 10_000 });
        }

        // ── Step 4: Wait for SAML redirect back to legacy site ──
        // The IdP POSTs SAMLResponse to /Account/SAWPost or /Account/ADFSPost,
        // which sets the WashingtonDNRForestHealthTrackerIdentity cookie and
        // redirects to the home page.
        await page.waitForURL(url => {
            const u = url.toString();
            return u.startsWith(LEGACY_BASE_URL) && !u.includes("/Account/Login");
        }, { timeout: 60_000 });

        await page.waitForLoadState("networkidle", { timeout: 15_000 });

        // Verify we're actually authenticated
        const currentUrl = page.url();
        if (currentUrl.includes("/Account/Login")) {
            throw new Error("Authentication failed — redirected back to login page after SAML flow");
        }

        // ── Step 5: Save cookies to disk ──
        const storageState = await context.storageState();
        const authData = {
            ...storageState,
            expires: new Date(Date.now() + 4 * 60 * 60 * 1000).toISOString(), // 4 hour expiry
        };
        fs.writeFileSync(AUTH_STATE_PATH, JSON.stringify(authData, null, 2));

        console.log("Legacy auth successful, state saved.");
    } finally {
        await page.close();
        await context.close();
    }
}

/**
 * Get a browser context authenticated to the legacy site.
 * Call authenticateToLegacy() first.
 */
export async function getLegacyContext(browser: Browser): Promise<BrowserContext> {
    if (!fs.existsSync(AUTH_STATE_PATH)) {
        throw new Error("No legacy auth state found. Call authenticateToLegacy() first.");
    }

    const authData = JSON.parse(fs.readFileSync(AUTH_STATE_PATH, "utf-8"));

    return await browser.newContext({
        storageState: {
            cookies: authData.cookies ?? [],
            origins: authData.origins ?? [],
        },
        ignoreHTTPSErrors: true,
    });
}
