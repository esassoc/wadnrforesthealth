import { Page, TestInfo } from "@playwright/test";
import AxeBuilder from "@axe-core/playwright";
import { test, expect } from "../fixtures/base-test";

/**
 * Run an axe-core WCAG 2.2 AA scan on the current page.
 * Results are attached to the test report as JSON; violations are logged but do NOT fail the test.
 */
export async function checkA11y(page: Page, pageName: string, testInfo: TestInfo) {
    const results = await new AxeBuilder({ page })
        .withTags(["wcag2a", "wcag2aa", "wcag21a", "wcag21aa", "wcag22aa"])
        .analyze();

    // Attach full results as JSON for the HTML reporter
    await testInfo.attach(`axe-results-${pageName}`, {
        body: JSON.stringify(results, null, 2),
        contentType: "application/json",
    });

    // Log summary to console
    const violations = results.violations;
    if (violations.length > 0) {
        const summary = violations.map((v) => {
            const nodes = v.nodes.length;
            return `  [${v.impact}] ${v.id}: ${v.help} (${nodes} element${nodes > 1 ? "s" : ""})`;
        });
        console.log(`\n${pageName}: ${violations.length} violation(s)\n${summary.join("\n")}`);
    } else {
        console.log(`${pageName}: No violations`);
    }

    return results;
}

export { test, expect };
