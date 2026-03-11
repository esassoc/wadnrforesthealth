import { test, expect } from "@playwright/test";

/**
 * Tests that Angular route guards redirect anonymous (unauthenticated) users.
 * Uses raw @playwright/test (no API error monitor) since guards redirect
 * before any API calls happen.
 */

test.describe("authGuard routes redirect anonymous users to /", () => {
    const authGuardRoutes = [
        "/focus-areas",
        "/focus-areas/1",
        "/vendors",
        "/people",
        "/people/5230",
        "/my-projects",
        "/pending-projects",
        "/manage-page-content",
        "/manage-custom-pages",
        "/manage-find-your-forester",
        "/organization-and-relationship-types",
        "/reports/projects",
        "/labels-and-definitions/1",
    ];

    for (const route of authGuardRoutes) {
        test(`${route} redirects anonymous to /`, async ({ page }) => {
            await page.goto(route);
            await page.waitForURL("/", { timeout: 15000 });
        });
    }
});

test.describe("adminGuard routes redirect anonymous users to /", () => {
    const adminGuardRoutes = ["/roles", "/project-types", "/project-themes", "/upload-excel-files", "/map-layers", "/jobs"];

    for (const route of adminGuardRoutes) {
        test(`${route} redirects anonymous to /`, async ({ page }) => {
            await page.goto(route);
            await page.waitForURL("/", { timeout: 15000 });
        });
    }
});

test.describe("projectEditGuard routes redirect anonymous to /projects", () => {
    test("/projects/new/basics redirects anonymous to /projects", async ({ page }) => {
        await page.goto("/projects/new/basics");
        await page.waitForURL("/projects", { timeout: 15000 });
    });
});
