import { test, expect } from "@playwright/test";

test.describe("Maps", () => {
  test("counties page renders with Leaflet map", async ({ page }) => {
    await page.goto("/counties");
    await expect(page.locator(".leaflet-container")).toBeVisible({ timeout: 15000 });
    await expect(page.getByRole("button", { name: "Zoom in" })).toBeVisible();
  });

  test("projects map page renders", async ({ page }) => {
    await page.goto("/projects/map");
    await expect(page.locator(".leaflet-container")).toBeVisible({ timeout: 15000 });
  });
});
