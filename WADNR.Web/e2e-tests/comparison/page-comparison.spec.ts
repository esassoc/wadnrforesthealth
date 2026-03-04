import { test, expect, Page, BrowserContext } from "@playwright/test";
import { setupTestAuth } from "../fixtures/inject-auth-header";
import { testUsers } from "../fixtures/test-users";
import { testData } from "../fixtures/test-data";
import { getAllComparisonPages, ComparisonPage } from "./comparison-config";
import {
    takePageSnapshot,
    compareSnapshots,
    isPageAccepted,
    generateComparisonReport,
    waitForPageReady,
    waitForLegacyPageReady,
    resolvePath,
    PageComparisonResult,
} from "./comparison-helpers";
import { authenticateToLegacy, getLegacyContext } from "./legacy-auth";

const LEGACY_BASE_URL = process.env.LEGACY_BASE_URL;
const REPORT_DIR = "comparison-reports";

// All comparison tests run serially — they share browser context and accumulate results
test.describe.configure({ mode: "serial" });

// Skip the entire suite if LEGACY_BASE_URL is not configured
test.skip(!LEGACY_BASE_URL, "LEGACY_BASE_URL not configured — skipping comparison tests");

const allResults: PageComparisonResult[] = [];
let legacyContext: BrowserContext;

/**
 * Build the token map for resolving URL placeholders.
 * Both Angular and Legacy paths use the same token values, except:
 * - Tag detail: Angular uses {tagID}, Legacy uses {tagName}
 */
function getTokenMap(): Record<string, string | number> {
    return {
        projectID: testData.projectID,
        focusAreaID: testData.focusAreaID,
        countyID: testData.countyID,
        fundSourceID: testData.fundSourceID,
        programID: testData.programID,
        roleID: testData.roleID,
        agreementID: testData.agreementID,
        organizationID: testData.organizationID,
        dnrUplandRegionID: testData.dnrUplandRegionID,
        priorityLandscapeID: testData.priorityLandscapeID,
        tagID: testData.tagID,
        tagName: testData.tagName,
        interactionEventID: testData.interactionEventID,
        invoiceID: testData.invoiceID,
        vendorID: testData.vendorID,
        personID: testData.personID,
        taxonomyBranchID: testData.taxonomyBranchID,
        taxonomyTrunkID: testData.taxonomyTrunkID,
        classificationID: testData.classificationID,
        projectTypeID: testData.projectTypeID,
        fundSourceAllocationID: testData.fundSourceAllocationID,
    };
}

/**
 * Check if all required tokens for a page are available in testData.
 */
function hasRequiredTokens(page: ComparisonPage): boolean {
    if (!page.tokens) return true;
    const tokenMap = getTokenMap();
    return page.tokens.every(t => tokenMap[t] != null);
}

test.beforeAll(async ({ browser }) => {
    if (!LEGACY_BASE_URL) return;

    // Authenticate to the legacy site
    try {
        await authenticateToLegacy(browser);
        legacyContext = await getLegacyContext(browser);
    } catch (error) {
        console.error("Failed to authenticate to legacy site:", error);
        // We'll handle this gracefully in individual tests
    }
});

const pages = getAllComparisonPages();
const tokenMap = getTokenMap();

for (const compPage of pages) {
    test(`Compare: ${compPage.name}`, async ({ page, browser }) => {
        test.setTimeout(compPage.slowPage ? 120_000 : 90_000);

        const start = Date.now();
        const result: PageComparisonResult = {
            pageName: compPage.name,
            angularUrl: resolvePath(compPage.angularPath, tokenMap),
            legacyUrl: resolvePath(compPage.legacyPath, tokenMap),
            status: "error",
            diffs: [],
            duration: 0,
        };

        try {
            // Skip if required tokens are missing
            if (!hasRequiredTokens(compPage)) {
                const missing = compPage.tokens!.filter(t => tokenMap[t] == null);
                result.status = "skipped";
                result.error = `Missing test data tokens: ${missing.join(", ")}. Add them to test-data.ts.`;
                allResults.push(result);
                return;
            }

            // ── Angular page ──
            await setupTestAuth(page, testUsers.admin);

            const angularUrl = resolvePath(compPage.angularPath, tokenMap);
            await page.goto(angularUrl, { waitUntil: "domcontentloaded", timeout: 30_000 });
            await waitForPageReady(page, compPage.slowPage ? 60_000 : 30_000);

            const angularSnapshot = await takePageSnapshot(page);
            result.angularSnapshot = angularSnapshot;
            result.angularScreenshot = angularSnapshot.screenshot;

            // ── Legacy page ──
            if (!legacyContext) {
                result.status = "skipped";
                result.error = "Legacy site authentication failed — cannot compare";
                allResults.push(result);
                return;
            }

            const legacyPage = await legacyContext.newPage();
            try {
                const legacyUrl = `${LEGACY_BASE_URL}${resolvePath(compPage.legacyPath, tokenMap)}`;
                await legacyPage.goto(legacyUrl, { waitUntil: "domcontentloaded", timeout: 60_000 });
                await waitForLegacyPageReady(legacyPage, compPage.slowPage ? 60_000 : 30_000);

                const legacySnapshot = await takePageSnapshot(legacyPage);
                result.legacySnapshot = legacySnapshot;
                result.legacyScreenshot = legacySnapshot.screenshot;
            } finally {
                await legacyPage.close();
            }

            // ── Compare ──
            if (result.angularSnapshot && result.legacySnapshot) {
                result.diffs = compareSnapshots(result.angularSnapshot, result.legacySnapshot);

                if (isPageAccepted(compPage.name, result.diffs)) {
                    result.status = "accepted";
                } else if (result.diffs.length === 0) {
                    result.status = "pass";
                } else {
                    const hasErrors = result.diffs.some(d => d.severity === "error");
                    const hasWarnings = result.diffs.some(d => d.severity === "warning");
                    result.status = hasErrors ? "fail" : hasWarnings ? "fail" : "pass";
                }
            }
        } catch (error) {
            result.status = "error";
            result.error = error instanceof Error ? error.message : String(error);
        }

        result.duration = Date.now() - start;

        // Strip large snapshot data from results (screenshots are kept separately)
        if (result.angularSnapshot) {
            delete result.angularSnapshot.screenshot;
        }
        if (result.legacySnapshot) {
            delete result.legacySnapshot.screenshot;
        }

        allResults.push(result);

        // Don't fail the test for comparison diffs — just collect them
        // The report shows what needs attention
        if (result.status === "error") {
            console.error(`[${compPage.name}] Error: ${result.error}`);
        } else if (result.status === "fail") {
            console.warn(`[${compPage.name}] ${result.diffs.length} differences found`);
        }
    });
}

test.afterAll(async () => {
    if (allResults.length === 0) return;

    const reportPath = generateComparisonReport(allResults, REPORT_DIR);
    console.log(`\nComparison report generated: ${reportPath}`);
    console.log(`Open with: npx playwright show-report or open ${reportPath} directly\n`);

    const passed = allResults.filter(r => r.status === "pass" || r.status === "accepted").length;
    const failed = allResults.filter(r => r.status === "fail").length;
    const errors = allResults.filter(r => r.status === "error").length;
    const skipped = allResults.filter(r => r.status === "skipped").length;

    console.log(`Results: ${passed} passed, ${failed} failed, ${errors} errors, ${skipped} skipped out of ${allResults.length} total`);

    if (legacyContext) {
        await legacyContext.close();
    }
});
