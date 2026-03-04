import type {
    FullConfig,
    FullResult,
    Reporter,
    Suite,
    TestCase,
    TestResult,
} from "@playwright/test/reporter";
import * as fs from "fs";
import * as path from "path";

interface MigrationStats {
    total: number;
    passed: number;
    failed: number;
    skipped: number;
    errors: number;
    duration: number;
    pages: PageResult[];
}

interface PageResult {
    name: string;
    status: "pass" | "fail" | "error" | "skipped";
    duration: number;
    error?: string;
}

/**
 * Custom Playwright reporter that generates a migration status summary.
 *
 * Tracks comparison test results and outputs:
 * - Console summary with pass/fail counts
 * - JSON file with per-page results for CI integration
 *
 * Generic — works with any project that has comparison specs.
 */
class MigrationReporter implements Reporter {
    private stats: MigrationStats = {
        total: 0,
        passed: 0,
        failed: 0,
        skipped: 0,
        errors: 0,
        duration: 0,
        pages: [],
    };
    private outputDir: string;

    constructor(options?: { outputDir?: string }) {
        this.outputDir = options?.outputDir ?? "comparison-reports";
    }

    onBegin(config: FullConfig, suite: Suite) {
        // Count comparison tests
        const comparisonTests = this.findComparisonTests(suite);
        this.stats.total = comparisonTests.length;

        if (this.stats.total > 0) {
            console.log(`\n📊 Migration Reporter: Tracking ${this.stats.total} comparison tests\n`);
        }
    }

    onTestEnd(test: TestCase, result: TestResult) {
        // Only track comparison tests
        if (!test.parent?.title?.includes("comparison") && !test.title.startsWith("Compare:")) {
            return;
        }

        const pageResult: PageResult = {
            name: test.title.replace("Compare: ", ""),
            status: "pass",
            duration: result.duration,
        };

        switch (result.status) {
            case "passed":
                this.stats.passed++;
                pageResult.status = "pass";
                break;
            case "failed":
                this.stats.failed++;
                pageResult.status = "fail";
                pageResult.error = result.errors?.[0]?.message;
                break;
            case "skipped":
                this.stats.skipped++;
                pageResult.status = "skipped";
                break;
            case "timedOut":
                this.stats.errors++;
                pageResult.status = "error";
                pageResult.error = "Test timed out";
                break;
            case "interrupted":
                this.stats.errors++;
                pageResult.status = "error";
                pageResult.error = "Test interrupted";
                break;
        }

        this.stats.pages.push(pageResult);
        this.stats.duration += result.duration;
    }

    onEnd(result: FullResult) {
        if (this.stats.total === 0) return;

        // Console summary
        console.log("\n" + "=".repeat(60));
        console.log("  MIGRATION COMPARISON SUMMARY");
        console.log("=".repeat(60));
        console.log(`  Total pages:  ${this.stats.total}`);
        console.log(`  Passed:       ${this.stats.passed}`);
        console.log(`  Failed:       ${this.stats.failed}`);
        console.log(`  Errors:       ${this.stats.errors}`);
        console.log(`  Skipped:      ${this.stats.skipped}`);
        console.log(`  Duration:     ${(this.stats.duration / 1000).toFixed(1)}s`);

        const coverage = this.stats.total > 0
            ? ((this.stats.passed / this.stats.total) * 100).toFixed(1)
            : "0";
        console.log(`  Coverage:     ${coverage}%`);
        console.log("=".repeat(60));

        // Failed pages
        const failedPages = this.stats.pages.filter(p => p.status === "fail" || p.status === "error");
        if (failedPages.length > 0) {
            console.log("\n  Pages needing attention:");
            for (const p of failedPages) {
                console.log(`    - ${p.name}: ${p.status}${p.error ? ` (${p.error.substring(0, 80)})` : ""}`);
            }
        }

        console.log("");

        // Write JSON summary
        const outputPath = path.resolve(this.outputDir, "migration-status.json");
        try {
            if (!fs.existsSync(this.outputDir)) {
                fs.mkdirSync(this.outputDir, { recursive: true });
            }
            fs.writeFileSync(outputPath, JSON.stringify(this.stats, null, 2));
        } catch {
            // Non-critical — just skip if we can't write
        }
    }

    private findComparisonTests(suite: Suite): TestCase[] {
        const tests: TestCase[] = [];
        for (const child of suite.suites) {
            tests.push(...this.findComparisonTests(child));
        }
        for (const test of suite.tests) {
            if (test.title.startsWith("Compare:")) {
                tests.push(test);
            }
        }
        return tests;
    }
}

export default MigrationReporter;
