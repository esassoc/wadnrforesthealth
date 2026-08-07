import { test, expect, Page } from "@playwright/test";

/**
 * Exercises HttpErrorInterceptor at the network boundary, which is where it lives.
 *
 * Raw @playwright/test rather than the shared base fixture: every test here deliberately forces an API
 * failure, and that fixture's API error monitor fails a test when it sees one. The same reasoning as
 * not-found-pages.spec.ts.
 *
 * Failures are produced with page.route() rather than by finding a real broken endpoint, so these need
 * no particular server state — only the app and API running at the configured baseURL.
 */

/** Console output produced after this is called. Attach before navigating. */
function collectConsoleErrors(page: Page): string[] {
    const lines: string[] = [];
    page.on("console", (message) => {
        if (message.type() === "error") {
            lines.push(message.text());
        }
    });
    return lines;
}

test.describe("HttpErrorInterceptor", () => {
    test("a transport failure names the request and does not log [object ProgressEvent]", async ({ page }) => {
        // The reported bug: "Backend returned code 0, body was: [object ProgressEvent]" — no URL, no
        // method, and nothing to act on. Status 0 means no HTTP response arrived at all.
        const consoleErrors = collectConsoleErrors(page);
        await page.route("**/api/**", (route) => route.abort("failed"));

        await page.goto("/projects");

        await expect
            .poll(() => consoleErrors.join("\n"), { timeout: 15000 })
            .toMatch(/never reached the server \(status 0\)/);

        const logged = consoleErrors.join("\n");
        expect(logged, "the failing request must be identifiable").toMatch(/HTTP GET \S*\/api\//);
        expect(logged, "the transport event type is the diagnostic bit").toMatch(/Transport event type="/);
        expect(logged).not.toContain("[object ProgressEvent]");
        expect(logged).not.toContain("Backend returned code 0");
    });

    test("an empty error body still logs something actionable", async ({ page }) => {
        // A 500 with no body is common, and JSON.stringify(null) renders the string "null" — no more use
        // in a log than the "[object Object]" this replaced. Falls back to Angular's own message, which
        // at least carries the URL and status.
        const consoleErrors = collectConsoleErrors(page);
        await page.route("**/api/**", (route) => route.fulfill({ status: 500, body: "" }));

        await page.goto("/projects");

        await expect
            .poll(() => consoleErrors.filter((line) => line.startsWith("HTTP ")).join("\n"), { timeout: 15000 })
            .toMatch(/failed with 500: (no|empty) response body\. Angular reported: /);

        const logged = consoleErrors.filter((line) => line.startsWith("HTTP ")).join("\n");
        expect(logged).not.toMatch(/failed with 500: null\b/);
        expect(logged).not.toMatch(/failed with 500: \{\}/);
    });

    test("400 validation errors surface once per field, not once per request", async ({ page }) => {
        // ValidationProblemDetails, as produced by [ApiController] model validation. Its `title` is only
        // ever "One or more validation errors occurred."; the per-field entries say what to fix.
        await page.route("**/api/**", (route) =>
            route.fulfill({
                status: 400,
                contentType: "application/json",
                body: JSON.stringify({
                    title: "One or more validation errors occurred.",
                    errors: {
                        ProjectName: ["Project Name is required."],
                        ProjectStage: ["Project Stage is required."],
                    },
                }),
            })
        );

        await page.goto("/projects");

        // Every field gets its own alert. These are strict-mode locators with no .first(), so a second
        // copy of either message fails the assertion on its own.
        await expect(page.getByText("Project Name is required.")).toBeVisible({ timeout: 15000 });
        await expect(page.getByText("Project Stage is required.")).toBeVisible();

        // One alert per field however many requests the page fired — this page fires three, and each
        // is rejected identically. alert-display renders only the first three alerts, so without a
        // per-message uniqueCode the repeats of the first field crowd the second one out of view.
        await page.waitForLoadState("networkidle");
        await expect(page.getByText("Project Name is required.")).toHaveCount(1);
        await expect(page.getByText("Project Stage is required.")).toHaveCount(1);
    });

    test("400 with a plain string body surfaces that string", async ({ page }) => {
        // What BadRequest("...") produces.
        await page.route("**/api/**", (route) =>
            route.fulfill({ status: 400, contentType: "text/plain", body: "That file type is not accepted." })
        );

        await page.goto("/projects");

        await expect(page.getByText("That file type is not accepted.").first()).toBeVisible({ timeout: 15000 });
    });

    test("403 shows the server message and does not navigate to a dead route", async ({ page }) => {
        // This used to redirect to /subscription-insufficient — a ProjectFirma fork artifact with no
        // route in this app, so it fell through to the "**" handler and landed on /not-found.
        await page.route("**/api/**", (route) =>
            route.fulfill({ status: 403, contentType: "text/plain", body: "You do not have permission to view this." })
        );

        await page.goto("/projects");

        await expect(page.getByText("You do not have permission to view this.").first()).toBeVisible({ timeout: 15000 });
        expect(page.url()).not.toContain("/subscription-insufficient");
        expect(page.url()).not.toContain("/not-found");
    });

    test("a 404 from the external geocoder does not pull the user to not-found", async ({ page }) => {
        // The isOurApi gate. The app also talks to WAMAS, Nominatim and GeoServer; a miss from one of
        // those belongs to whichever component asked for it, not to the router.
        //
        // The geocoder is driven deliberately rather than relying on page load. An earlier version of
        // this test intercepted GeoServer on /projects/map and asserted no navigation — but that page
        // issues zero GeoServer requests through HttpClient (Leaflet fetches WMS tiles as <img>, and the
        // WFS call only fires on a map click elsewhere), so it passed without exercising the gate at all
        // and would have passed with isOurApi deleted. Hence the routeHit assertion below.
        let routeHit = false;
        await page.route(/wamas|nominatim|geocod/i, (route) => {
            routeHit = true;
            // Access-Control-Allow-Origin is required, not decoration. WAMAS is cross-origin; without it
            // the browser rejects the fulfilled response and Angular reports status 0, which never
            // reaches the 404 branch — the test would then pass with isOurApi deleted.
            return route.fulfill({
                status: 404,
                contentType: "text/plain",
                headers: { "Access-Control-Allow-Origin": "*" },
                body: "Address not found",
            });
        });

        // /find-your-forester rather than /projects/map: it is public, and it renders <map-search>
        // unconditionally rather than behind *ngIf="mapIsReady". Locators are scoped to that component
        // because getByLabel(/search/) also matches the main nav's search button.
        await page.goto("/find-your-forester");
        const mapSearch = page.locator("map-search").first();
        await mapSearch.getByRole("textbox").first().fill("1111 Washington St SE, Olympia WA");
        await mapSearch.getByRole("button", { name: /zoom|search/i }).first().click();

        await expect
            .poll(() => routeHit, { timeout: 15000 })
            .toBe(true);

        // The gate: an external 404 must not navigate.
        await page.waitForTimeout(1000);
        expect(page.url()).not.toContain("/not-found");
    });

    test("a 404 from our own API still redirects to not-found", async ({ page }) => {
        // The other half of the gate: narrowing which failures navigate must not stop the ones that should.
        await page.route("**/api/projects**", (route) =>
            route.fulfill({ status: 404, contentType: "text/plain", body: "Project does not exist!" })
        );

        await page.goto("/projects/999999");

        await expect.poll(() => page.url(), { timeout: 15000 }).toContain("/not-found");
    });
});
