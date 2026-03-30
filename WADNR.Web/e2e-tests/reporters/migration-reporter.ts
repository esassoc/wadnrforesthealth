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
    legacyOnly: number;
    modernOnly: number;
    duration: number;
    pages: PageResult[];
}

interface PageResult {
    name: string;
    status: "pass" | "fail" | "error" | "skipped" | "legacy-only" | "modern-only";
    duration: number;
    error?: string;
}

interface RegistryCoverage {
    total: number;
    migrated: number;
    restructured: number;
    excluded: number;
    notYet: number;
    coveragePercent: number;
    gaps: { name: string; legacyPath: string }[];
}

/**
 * Custom Playwright reporter that generates a migration status summary.
 *
 * Tracks comparison test results and outputs:
 * - Console summary with pass/fail counts
 * - Registry coverage statistics (from page-registry.ts)
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
        legacyOnly: 0,
        modernOnly: 0,
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
            console.log(`\n  Migration Reporter: Tracking ${this.stats.total} comparison tests\n`);
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
        // Always print registry coverage (even if no comparison tests ran)
        const registryCoverage = this.getRegistryCoverage();
        if (registryCoverage) {
            this.printRegistryCoverage(registryCoverage);
        }

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
        console.log(`  Legacy Only:  ${this.stats.legacyOnly}`);
        console.log(`  Modern Only:  ${this.stats.modernOnly}`);
        console.log(`  Duration:     ${(this.stats.duration / 1000).toFixed(1)}s`);

        const coverage = this.stats.total > 0
            ? ((this.stats.passed / this.stats.total) * 100).toFixed(1)
            : "0";
        console.log(`  Coverage:     ${coverage}%`);
        console.log("=".repeat(60));

        // Failed pages
        const failedPages = this.stats.pages.filter(p => p.status === "fail" || p.status === "error" || p.status === "legacy-only");
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
            fs.writeFileSync(outputPath, JSON.stringify({
                ...this.stats,
                registryCoverage,
            }, null, 2));
        } catch {
            // Non-critical — just skip if we can't write
        }
    }

    private printRegistryCoverage(coverage: RegistryCoverage) {
        console.log("\n" + "=".repeat(60));
        console.log("  MIGRATION COVERAGE");
        console.log("=".repeat(60));
        console.log(`  Total legacy pages:     ${coverage.total}`);
        console.log(`  Migrated:               ${coverage.migrated}  (${((coverage.migrated / coverage.total) * 100).toFixed(1)}%)`);
        console.log(`  Restructured:           ${coverage.restructured}  (${((coverage.restructured / coverage.total) * 100).toFixed(1)}%)`);
        console.log(`  Excluded (dead code):   ${coverage.excluded}  (${((coverage.excluded / coverage.total) * 100).toFixed(1)}%)`);
        console.log(`  Not yet migrated:       ${coverage.notYet}  (${((coverage.notYet / coverage.total) * 100).toFixed(1)}%)`);
        console.log(`  Coverage:               ${coverage.coveragePercent.toFixed(1)}%  (migrated + restructured + excluded)`);
        console.log("=".repeat(60));

        if (coverage.gaps.length > 0) {
            console.log("\n  GAPS (not yet migrated):");
            for (const gap of coverage.gaps) {
                console.log(`    - ${gap.name} (${gap.legacyPath})`);
            }
        }

        console.log("");
    }

    private getRegistryCoverage(): RegistryCoverage | null {
        try {
            // Dynamic import of registry — may not be available in all environments
            const registryPath = path.resolve(__dirname, "../registry/page-registry");
            // eslint-disable-next-line @typescript-eslint/no-require-imports
            const { pageRegistry } = require(registryPath);

            const counts = { migrated: 0, restructured: 0, excluded: 0, "not-yet-migrated": 0 };
            const gaps: { name: string; legacyPath: string }[] = [];

            for (const entry of pageRegistry) {
                const status = entry.status as keyof typeof counts;
                if (status in counts) {
                    counts[status]++;
                }
                if (entry.status === "not-yet-migrated") {
                    gaps.push({ name: entry.name, legacyPath: entry.legacyPath });
                }
            }

            const total = pageRegistry.length;
            const done = counts.migrated + counts.restructured + counts.excluded;

            return {
                total,
                migrated: counts.migrated,
                restructured: counts.restructured,
                excluded: counts.excluded,
                notYet: counts["not-yet-migrated"],
                coveragePercent: total > 0 ? (done / total) * 100 : 0,
                gaps,
            };
        } catch {
            // Registry not available — skip coverage section
            return null;
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
