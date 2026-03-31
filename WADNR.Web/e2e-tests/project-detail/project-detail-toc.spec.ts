import { test, expect } from "../fixtures/base-test";
import { testData } from "../fixtures/test-data";

test.describe("Project detail table of contents", () => {
    test.beforeEach(async ({ authedPage: page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await expect(page.locator(".toc-sidebar")).toBeVisible({ timeout: 15000 });
    });

    test("TOC sidebar renders with links", async ({ authedPage: page }) => {
        const tocLinks = page.locator(".toc-sidebar .toc-link");
        const count = await tocLinks.count();
        expect(count).toBeGreaterThan(0);
    });

    test("Clicking a TOC link scrolls to corresponding card", async ({ authedPage: page }) => {
        // Find a TOC link that targets a known card
        const tocLink = page.locator(".toc-sidebar .toc-link", { hasText: "Location" }).first();
        if (await tocLink.isVisible()) {
            await tocLink.click();
            // The location card should be near the top of the viewport
            await expect(page.locator("#card-location")).toBeInViewport({ timeout: 5000 });
        }
    });

    test("Active TOC link updates on scroll", async ({ authedPage: page }) => {
        // Scroll to the bottom of the page to trigger scrollspy
        await page.evaluate(() => window.scrollTo(0, document.body.scrollHeight));
        await page.waitForTimeout(500);
        // At least one TOC link should have the active class
        await expect(page.locator(".toc-sidebar .toc-link.active").first()).toBeVisible();
    });
});
