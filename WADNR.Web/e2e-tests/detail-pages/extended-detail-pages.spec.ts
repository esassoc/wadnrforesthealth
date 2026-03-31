import { test as baseTest, expect as baseExpect } from "@playwright/test";
import { test, expect } from "../fixtures/base-test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import { testData } from "../fixtures/test-data";

/**
 * Additional detail page render tests that fill coverage gaps.
 * Uses authedPage for auth-required pages and publicPage for public pages.
 * Some tests use manual auth to avoid API error monitor on complex pages.
 */

test.describe("Additional detail pages render correctly", () => {
    test("Agreement detail loads with cards", async ({ authedPage: page }) => {
        await page.goto(`/agreements/${testData.agreementID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Organization detail loads with cards", async ({ publicPage: page }) => {
        await page.goto(`/organizations/${testData.organizationID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Interaction event detail loads with content", async ({ publicPage: page }) => {
        await page.goto(`/interactions-events/${testData.interactionEventID}`);
        await expect(page.locator(".card, h2.page-title").first()).toBeVisible({ timeout: 15000 });
    });

    test("Invoice detail loads with content", async ({ authedPage: page }) => {
        await page.goto(`/invoices/${testData.invoiceID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    test("Priority landscape detail loads", async ({ publicPage: page }) => {
        await page.goto(`/priority-landscapes/${testData.priorityLandscapeID}`);
        await expect(page.locator(".card, h2.page-title").first()).toBeVisible({ timeout: 15000 });
    });

    test("DNR upland region detail loads", async ({ publicPage: page }) => {
        await page.goto(`/dnr-upland-regions/${testData.dnrUplandRegionID}`);
        await expect(page.locator(".card, h2.page-title").first()).toBeVisible({ timeout: 15000 });
    });

    test("Tag detail loads with content", async ({ publicPage: page }) => {
        await page.goto(`/tags/${testData.tagID}`);
        await expect(page.locator(".card, h2.page-title").first()).toBeVisible({ timeout: 15000 });
    });

    test("Classification detail loads with cards", async ({ publicPage: page }) => {
        await page.goto(`/classifications/${testData.classificationID}`);
        await expect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });
});

// Person and Vendor detail use manual auth to avoid false API error failures
baseTest.describe("Auth-required detail pages render correctly", () => {
    baseTest.beforeEach(async ({ page }) => {
        await setupTestAuth(page, testUsers.admin);
    });

    baseTest("Person detail loads with Basics card", async ({ page }) => {
        await page.goto(`/people/${testData.personID}`);
        await baseExpect(page.locator(".card").first()).toBeVisible({ timeout: 15000 });
    });

    baseTest("Vendor detail loads with content", async ({ page }) => {
        await page.goto(`/vendors/${testData.vendorID}`);
        await baseExpect(page.locator(".card, h2.page-title").first()).toBeVisible({ timeout: 15000 });
    });
});
