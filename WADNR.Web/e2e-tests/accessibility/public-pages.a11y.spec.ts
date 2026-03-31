import { test, checkA11y } from "./a11y-fixtures";
import { publicRoutes } from "./a11y-routes";

test.describe("Accessibility: Public pages", () => {
    for (const route of publicRoutes) {
        test(`${route.name} (${route.path})`, async ({ publicPage: page }, testInfo) => {
            await page.goto(route.path);
            await page.waitForSelector(route.waitFor, { timeout: route.timeout ?? 30000 });
            await checkA11y(page, route.name, testInfo);
        });
    }
});
