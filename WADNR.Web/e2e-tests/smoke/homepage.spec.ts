import { test, expect } from "@playwright/test";

test.describe("Homepage", () => {
  test("should load the homepage", async ({ page }) => {
    await page.goto("/");
    await expect(page.getByRole("heading", { name: "Welcome to the Home Page" })).toBeVisible();
  });

  test("should display the main navigation", async ({ page }) => {
    await page.goto("/");
    // Two <nav> elements exist: top bar (logo/search) and secondary nav (menu links)
    await expect(page.getByRole("navigation").first()).toBeVisible();
    await expect(page.getByRole("link", { name: "About" })).toBeVisible();
    await expect(page.getByRole("link", { name: "Projects" })).toBeVisible();
  });
});
