import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";

test.describe("Public list pages", () => {
  test.beforeEach(async ({ page }) => {
    await setupTestAuth(page, testUsers.admin);
  });

  test("projects list renders with data", async ({ page }) => {
    await page.goto("/projects");
    await expect(page.getByRole("heading", { name: "Full Project List" })).toBeVisible();
    await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
  });

  test("focus areas list renders with data", async ({ page }) => {
    await page.goto("/focus-areas");
    await expect(page.getByRole("heading", { name: "Focus Areas" })).toBeVisible();
    await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
  });

  test("counties list renders with data", async ({ page }) => {
    await page.goto("/counties");
    await expect(page.getByRole("heading", { name: "Counties" })).toBeVisible();
    await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
  });

  test("organizations list renders with data", async ({ page }) => {
    await page.goto("/organizations");
    await expect(page.getByRole("heading", { name: "Contributing Organizations" })).toBeVisible();
    await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
  });

  test("about page renders", async ({ page }) => {
    await page.goto("/about");
    await expect(page.locator("main")).toBeVisible();
  });
});
