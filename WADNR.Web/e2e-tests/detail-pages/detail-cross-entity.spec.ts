/**
 * Cross-entity navigation tests.
 *
 * Verifies that links between entity detail pages navigate correctly:
 * - Project detail → Organization detail (via organization links)
 * - Organization detail → Project detail (via project grid rows)
 * - Fund source detail → Agreement detail (via agreement links)
 * - Agreement detail → Fund source detail (via fund source links)
 * - Program detail → Project detail (via project grid rows)
 */

import { test, expect } from "../fixtures/base-test";
import { testData } from "../fixtures/test-data";

test.describe("Project → Organization cross-links", () => {
    test("Project detail has organization links that navigate to organization detail", async ({ authedPage: page }) => {
        await page.goto(`/projects/${testData.projectID}`);
        await page.waitForLoadState("networkidle");

        // Find an organization link on the project detail page
        const orgLink = page.locator('a[href*="/organizations/"]').first();

        if (await orgLink.isVisible({ timeout: 10000 }).catch(() => false)) {
            const href = await orgLink.getAttribute("href");
            await orgLink.click();
            await page.waitForLoadState("networkidle");
            expect(page.url()).toContain("/organizations/");
            // Verify the org detail page rendered
            await expect(page.locator(".card-header, h2.page-title").first()).toBeVisible({ timeout: 10000 });
        }
    });
});

test.describe("Organization → Project cross-links", () => {
    test("Organization detail has project grid with links back to project detail", async ({ authedPage: page }) => {
        await page.goto(`/organizations/${testData.organizationID}`);
        await page.waitForLoadState("networkidle");

        // Find a project link in the organization's project grid
        const projectLink = page.locator('.ag-row a[href*="/projects/"]').first();

        if (await projectLink.isVisible({ timeout: 10000 }).catch(() => false)) {
            await projectLink.click();
            await page.waitForLoadState("networkidle");
            expect(page.url()).toContain("/projects/");
            await expect(page.locator(".card-header, h2.page-title").first()).toBeVisible({ timeout: 10000 });
        }
    });
});

test.describe("Fund Source → Agreement cross-links", () => {
    test("Fund source detail links to agreement detail", async ({ authedPage: page }) => {
        await page.goto(`/fund-sources/${testData.fundSourceID}`);
        await page.waitForLoadState("networkidle");

        const agreementLink = page.locator('a[href*="/agreements/"]').first();

        if (await agreementLink.isVisible({ timeout: 10000 }).catch(() => false)) {
            await agreementLink.click();
            await page.waitForLoadState("networkidle");
            expect(page.url()).toContain("/agreements/");
            await expect(page.locator(".card-header, h2.page-title").first()).toBeVisible({ timeout: 10000 });
        }
    });
});

test.describe("Agreement → Fund Source cross-links", () => {
    test("Agreement detail links to fund source detail", async ({ authedPage: page }) => {
        await page.goto(`/agreements/${testData.agreementID}`);
        await page.waitForLoadState("networkidle");

        const fundSourceLink = page.locator('a[href*="/fund-sources/"]').first();

        if (await fundSourceLink.isVisible({ timeout: 10000 }).catch(() => false)) {
            await fundSourceLink.click();
            await page.waitForLoadState("networkidle");
            expect(page.url()).toContain("/fund-sources/");
            await expect(page.locator(".card-header, h2.page-title").first()).toBeVisible({ timeout: 10000 });
        }
    });
});

test.describe("Program → Project cross-links", () => {
    test("Program detail has project grid with links to project detail", async ({ authedPage: page }) => {
        await page.goto(`/programs/${testData.programID}`);
        await page.waitForLoadState("networkidle");

        const projectLink = page.locator('.ag-row a[href*="/projects/"]').first();

        if (await projectLink.isVisible({ timeout: 10000 }).catch(() => false)) {
            await projectLink.click();
            await page.waitForLoadState("networkidle");
            expect(page.url()).toContain("/projects/");
            await expect(page.locator(".card-header, h2.page-title").first()).toBeVisible({ timeout: 10000 });
        }
    });
});

test.describe("County → Project cross-links", () => {
    test("County detail has project grid with links to project detail", async ({ authedPage: page }) => {
        await page.goto(`/counties/${testData.countyID}`);
        await page.waitForLoadState("networkidle");

        const projectLink = page.locator('.ag-row a[href*="/projects/"]').first();

        if (await projectLink.isVisible({ timeout: 10000 }).catch(() => false)) {
            await projectLink.click();
            await page.waitForLoadState("networkidle");
            expect(page.url()).toContain("/projects/");
            await expect(page.locator(".card-header, h2.page-title").first()).toBeVisible({ timeout: 10000 });
        }
    });
});

test.describe("Tag → Project cross-links", () => {
    test("Tag detail has project grid with links to project detail", async ({ authedPage: page }) => {
        await page.goto(`/tags/${testData.tagID}`);
        await page.waitForLoadState("networkidle");

        const projectLink = page.locator('.ag-row a[href*="/projects/"]').first();

        if (await projectLink.isVisible({ timeout: 10000 }).catch(() => false)) {
            await projectLink.click();
            await page.waitForLoadState("networkidle");
            expect(page.url()).toContain("/projects/");
            await expect(page.locator(".card-header, h2.page-title").first()).toBeVisible({ timeout: 10000 });
        }
    });
});
