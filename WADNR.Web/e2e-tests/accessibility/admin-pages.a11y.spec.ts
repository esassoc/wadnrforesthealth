import { test, checkA11y } from "./a11y-fixtures";
import { adminRoutes } from "./a11y-routes";

test.describe("Accessibility: Admin pages", () => {
    for (const route of adminRoutes) {
        test(`${route.name} (${route.path})`, async ({ authedPage: page }, testInfo) => {
            await page.goto(route.path);
            await page.waitForSelector(route.waitFor, { timeout: route.timeout ?? 30000 });
            await checkA11y(page, route.name, testInfo);
        });
    }
});
