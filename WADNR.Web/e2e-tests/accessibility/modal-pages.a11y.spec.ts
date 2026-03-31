import { test, checkModalA11y } from "./a11y-fixtures";
import {
    adminModalRoutes,
    elevatedModalRoutes,
    authedModalRoutes,
    publicPageModalRoutes,
    projectDetailModalRoutes,
    A11yModalRoute,
} from "./a11y-routes";

const MODAL_SELECTOR = ".ngneat-dialog-content";

async function testModal(route: A11yModalRoute, page: import("@playwright/test").Page, testInfo: import("@playwright/test").TestInfo) {
    await page.goto(route.pagePath);
    await page.waitForSelector(route.pageWaitFor, { timeout: route.timeout ?? 30000 });

    // Run optional setup steps (e.g., open a dropdown, click a grid row action)
    if (route.setupSteps) {
        for (const step of route.setupSteps) {
            const el = page.locator(step.click).first();
            await el.waitFor({ state: "visible", timeout: 10000 });
            await el.click();
            if (step.waitAfter) {
                await page.waitForSelector(step.waitAfter, { state: "visible", timeout: 10000 });
            }
        }
    }

    // Click the trigger to open the modal
    const trigger = page.locator(route.triggerSelector).first();
    await trigger.scrollIntoViewIfNeeded({ timeout: route.timeout ?? 30000 });
    await trigger.waitFor({ state: "visible", timeout: 10000 });
    await trigger.click();

    // Wait for the modal to render
    await page.waitForSelector(MODAL_SELECTOR, { state: "visible", timeout: 10000 });

    // Give the modal content a moment to finish rendering (form fields, dropdowns, etc.)
    await page.waitForTimeout(500);

    await checkModalA11y(page, route.name, testInfo);
}

test.describe("Accessibility: Admin page modals", () => {
    for (const route of adminModalRoutes) {
        test(`${route.name} (${route.pagePath})`, async ({ authedPage: page }, testInfo) => {
            await testModal(route, page, testInfo);
        });
    }
});

test.describe("Accessibility: Elevated access modals", () => {
    for (const route of elevatedModalRoutes) {
        test(`${route.name} (${route.pagePath})`, async ({ authedPage: page }, testInfo) => {
            await testModal(route, page, testInfo);
        });
    }
});

test.describe("Accessibility: Authenticated page modals", () => {
    for (const route of authedModalRoutes) {
        test(`${route.name} (${route.pagePath})`, async ({ authedPage: page }, testInfo) => {
            await testModal(route, page, testInfo);
        });
    }
});

test.describe("Accessibility: Public page modals (admin user)", () => {
    for (const route of publicPageModalRoutes) {
        test(`${route.name} (${route.pagePath})`, async ({ authedPage: page }, testInfo) => {
            await testModal(route, page, testInfo);
        });
    }
});

test.describe("Accessibility: Project Detail modals", () => {
    for (const route of projectDetailModalRoutes) {
        test(`${route.name} (${route.pagePath})`, async ({ authedPage: page }, testInfo) => {
            await testModal(route, page, testInfo);
        });
    }
});
