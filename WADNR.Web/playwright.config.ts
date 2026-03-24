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
      testIgnore: ["**/comparison/**", "**/visual-regression/**", "**/registry/**", "**/accessibility/**"],
    },
    {
      name: "comparison",
      testMatch: "comparison/*.spec.ts",
      fullyParallel: false,
      use: {
        ...devices["Desktop Chrome"],
        channel: "chrome",
      },
    },
    {
      name: "visual",
      testMatch: "visual-regression/*.vr.spec.ts",
      use: {
        ...devices["Desktop Chrome"],
        channel: "chrome",
      },
      timeout: 60000,
      expect: {
        toHaveScreenshot: {
          maxDiffPixelRatio: 0.01,
          threshold: 0.2,
          animations: "disabled",
        },
      },
    },
    {
      name: "registry-audit",
      testMatch: "registry/*.spec.ts",
      use: {
        ...devices["Desktop Chrome"],
        channel: "chrome",
      },
    },
    {
      name: "accessibility",
      testMatch: "accessibility/*.a11y.spec.ts",
      use: {
        ...devices["Desktop Chrome"],
        channel: "chrome",
      },
    },
  ],
});
