import "dotenv/config";
import { defineConfig, devices } from "@playwright/test";

export default defineConfig({
  testDir: "./e2e-tests",
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: [["html"], ["list"], ["./e2e-tests/reporters/migration-reporter.ts"]],

  use: {
    baseURL: "https://wadnr.localhost.esassoc.com:3215",
    ignoreHTTPSErrors: true,
    trace: "on-first-retry",
    screenshot: "only-on-failure",
  },

  projects: [
    {
      name: "chromium",
      use: {
        ...devices["Desktop Chrome"],
        channel: "chrome", // Use system-installed Chrome (avoids Playwright CDN download behind corporate proxy)
      },
      testIgnore: ["**/comparison/**"],
    },
    {
      name: "comparison",
      testMatch: "comparison/*.spec.ts",
      use: {
        ...devices["Desktop Chrome"],
        channel: "chrome",
      },
    },
  ],
});
