import { test, expect } from "../fixtures/base-test";

test.describe("Public list pages", () => {
    test("/projects renders with grid data", async ({ publicPage: page }) => {
        await page.goto("/projects");
        await expect(page.getByRole("heading", { name: "Full Project List" })).toBeVisible();
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
    });

    test("/counties renders with grid data", async ({ publicPage: page }) => {
        await page.goto("/counties");
        await expect(page.getByRole("heading", { name: "Counties" })).toBeVisible();
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
    });

    test("/organizations renders with grid data", async ({ publicPage: page }) => {
        await page.goto("/organizations");
        await expect(page.getByRole("heading", { name: "Contributing Organizations" })).toBeVisible();
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
    });

    test("/tags renders with heading", async ({ publicPage: page }) => {
        await page.goto("/tags");
        await expect(page.getByRole("heading", { name: "Tags" })).toBeVisible();
    });

    test("/taxonomy-branches renders with heading", async ({ publicPage: page }) => {
        await page.goto("/taxonomy-branches");
        await expect(page.getByRole("heading", { name: "Taxonomy Branches" })).toBeVisible();
    });

    test("/taxonomy-trunks renders with heading", async ({ publicPage: page }) => {
        await page.goto("/taxonomy-trunks");
        await expect(page.getByRole("heading", { name: "Taxonomy Trunks" })).toBeVisible();
    });

    test("/classification-systems renders with heading", async ({ publicPage: page }) => {
        await page.goto("/classification-systems");
        await expect(page.getByRole("heading", { name: "Classification Systems" })).toBeVisible();
    });

    test("/priority-landscapes renders with heading", async ({ publicPage: page }) => {
        await page.goto("/priority-landscapes");
        await expect(page.getByRole("heading", { name: "Priority Landscapes" })).toBeVisible();
    });

    test("/dnr-upland-regions renders with heading", async ({ publicPage: page }) => {
        await page.goto("/dnr-upland-regions");
        await expect(page.getByRole("heading", { name: "DNR Upland Regions" })).toBeVisible();
    });

    test("/project-types renders with heading", async ({ publicPage: page }) => {
        await page.goto("/project-types");
        await expect(page.getByRole("heading", { name: "Project Types" })).toBeVisible();
    });

    test("/program-indices renders with grid data", async ({ publicPage: page }) => {
        await page.goto("/program-indices");
        await expect(page.getByRole("heading", { name: "Program Indices" })).toBeVisible();
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
    });

    test("/project-codes renders with grid data", async ({ publicPage: page }) => {
        await page.goto("/project-codes");
        await expect(page.getByRole("heading", { name: "Project Codes" })).toBeVisible();
        await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
    });

    test("/project-steward-organizations renders with content", async ({ publicPage: page }) => {
        await page.goto("/project-steward-organizations");
        await expect(page.getByRole("heading", { name: "Project Steward Organizations" })).toBeVisible();
    });
});
