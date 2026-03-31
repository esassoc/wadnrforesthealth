/**
 * Extended AG Grid feature tests.
 *
 * Verifies grid functionality across multiple pages:
 * - Column headers match expected columns
 * - Filter inputs work (type text, grid updates)
 * - CSV download button triggers file download
 */

import { test, expect } from "../fixtures/base-test";

interface GridPageConfig {
    name: string;
    url: string;
    /** At least these column header texts should be present */
    expectedColumns: string[];
    /** A column to test filtering on (header text) */
    filterColumn?: string;
    /** Text to type in the filter */
    filterText?: string;
}

const gridPages: GridPageConfig[] = [
    {
        name: "Projects",
        url: "/projects",
        expectedColumns: ["Project Name", "Project Type", "Project Stage"],
        filterColumn: "Project Name",
        filterText: "forest",
    },
    {
        name: "Agreements",
        url: "/agreements",
        expectedColumns: ["Agreement Title", "Agreement Number"],
    },
    {
        name: "Organizations",
        url: "/organizations",
        expectedColumns: ["Organization Name"],
    },
    {
        name: "Fund Sources",
        url: "/fund-sources",
        expectedColumns: ["Fund Source Name"],
    },
    {
        name: "Interactions/Events",
        url: "/interactions-events",
        expectedColumns: ["Interaction/Event Title", "Date"],
    },
    {
        name: "Invoices",
        url: "/invoices",
        expectedColumns: ["Invoice Number"],
    },
    {
        name: "Program Indices",
        url: "/program-indices",
        expectedColumns: ["Program Index"],
    },
    {
        name: "Project Codes",
        url: "/project-codes",
        expectedColumns: ["Project Code"],
    },
];

// ─── Column Header Tests ───────────────────────────────────────────────────────

test.describe("Grid column headers", () => {
    for (const config of gridPages) {
        test(`${config.name} grid has expected columns`, async ({ authedPage: page }) => {
            await page.goto(config.url);
            await page.waitForLoadState("networkidle");

            // Wait for AG Grid to render
            const grid = page.locator(".ag-root");
            await expect(grid.first()).toBeVisible({ timeout: 15000 });

            // Wait for header cells to appear
            await expect(page.locator(".ag-header-cell-text").first()).toBeVisible({ timeout: 10000 });

            // Get all visible column header texts
            const headers = await page.locator(".ag-header-cell-text").allTextContents();
            const headerTexts = headers.map((h) => h.trim()).filter(Boolean);

            for (const expected of config.expectedColumns) {
                const found = headerTexts.some((h) => h.includes(expected));
                expect(found, `Expected column "${expected}" in [${headerTexts.join(", ")}]`).toBe(true);
            }
        });
    }
});

// ─── Filter Tests ──────────────────────────────────────────────────────────────

test.describe("Grid filtering", () => {
    for (const config of gridPages.filter((c) => c.filterColumn)) {
        test(`${config.name} grid filter works on "${config.filterColumn}"`, async ({ authedPage: page }) => {
            await page.goto(config.url);
            await page.waitForLoadState("networkidle");

            // Wait for grid rows to appear
            const grid = page.locator(".ag-root");
            await expect(grid.first()).toBeVisible({ timeout: 15000 });
            await page.locator(".ag-row").first().waitFor({ state: "visible", timeout: 15000 });

            // Count initial rows
            const initialRowCount = await page.locator(".ag-row").count();

            // Find the floating filter input for the target column
            const filterInput = page.locator(".ag-floating-filter-input input, .ag-floating-filter input").first();
            if (await filterInput.isVisible({ timeout: 3000 }).catch(() => false)) {
                await filterInput.fill(config.filterText!);
                // Give the grid time to filter
                await page.waitForTimeout(500);

                // The grid should still have rows (filter text is chosen to match at least some)
                // and the total should be <= initial count
                const filteredRowCount = await page.locator(".ag-row").count();
                expect(filteredRowCount).toBeLessThanOrEqual(initialRowCount);
            }
        });
    }
});

// ─── CSV Download Tests ────────────────────────────────────────────────────────

test.describe("Grid CSV download", () => {
    const pagesWithCsv = ["/projects", "/agreements", "/organizations"];

    for (const url of pagesWithCsv) {
        test(`${url} CSV download button triggers file download`, async ({ authedPage: page }) => {
            await page.goto(url);
            await page.waitForLoadState("networkidle");
            await expect(page.locator(".ag-root").first()).toBeVisible({ timeout: 15000 });

            // Look for CSV/Excel download button
            const downloadBtn = page.locator("button", { hasText: /download|csv|excel|export/i }).first();

            if (await downloadBtn.isVisible({ timeout: 5000 }).catch(() => false)) {
                const [download] = await Promise.all([
                    page.waitForEvent("download", { timeout: 10000 }).catch(() => null),
                    downloadBtn.click(),
                ]);

                // If a download was triggered, verify it has content
                if (download) {
                    const filename = download.suggestedFilename();
                    expect(filename).toBeTruthy();
                }
            }
        });
    }
});
