import { test, expect } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";

test.describe("Projects grid", () => {
  test.beforeEach(async ({ page }) => {
    await setupTestAuth(page, testUsers.admin);
    await page.goto("/projects");
    await expect(page.locator(".ag-row").first()).toBeVisible({ timeout: 30000 });
  });

  test("grid displays rows of data", async ({ page }) => {
    const rowCount = await page.locator(".ag-row").count();
    expect(rowCount).toBeGreaterThan(0);
  });

  test("clicking a column header sorts the grid", async ({ page }) => {
    // Click the first sortable column header text
    const firstHeader = page.locator(".ag-header-cell").first();
    await firstHeader.click();

    // After clicking, a sort indicator should appear
    await expect(
      page.locator(".ag-sort-ascending-icon:visible, .ag-sort-descending-icon:visible").first()
    ).toBeVisible({ timeout: 5000 });
  });

  test("grid has toolbar with CSV download and fullscreen", async ({ page }) => {
    await expect(page.getByTitle("Download grid data as CSV")).toBeVisible();
    await expect(page.getByTitle("Make grid full screen")).toBeVisible();
  });
});
