import { test, expect } from "@playwright/test";

test.describe("Navigation", () => {
  test("navigating to a non-existent route shows not-found page", async ({ page }) => {
    await page.goto("/this-route-does-not-exist-12345");
    await expect(page.getByText(/not found|page.*not.*exist|404|under construction/i)).toBeVisible({ timeout: 10000 });
  });

  test("can navigate from projects dropdown to full project list", async ({ page }) => {
    await page.goto("/");

    // The "Projects" nav item uses a click-based [dropdownToggle] directive
    await page.getByRole("link", { name: "Projects" }).click();
    await page.getByRole("link", { name: "Full Project List" }).click();

    await expect(page).toHaveURL(/\/projects/);
    await expect(page.getByRole("heading", { name: "Full Project List" })).toBeVisible();
  });
});
