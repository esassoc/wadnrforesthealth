import { test, checkA11y } from "./a11y-fixtures";
import { authedRoutes } from "./a11y-routes";

test.describe("Accessibility: Authenticated pages", () => {
    for (const route of authedRoutes) {
        test(`${route.name} (${route.path})`, async ({ authedPage: page }, testInfo) => {
            await page.goto(route.path);
            await page.waitForSelector(route.waitFor, { timeout: route.timeout ?? 30000 });
            await checkA11y(page, route.name, testInfo);
        });
    }
});
