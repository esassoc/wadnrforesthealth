/**
 * Registry audit tests — validate the page registry itself and surface migration gaps.
 *
 * These tests ensure the registry stays accurate as the codebase evolves:
 * - Every "migrated" entry actually has a modern route
 * - Every "excluded" entry has a documented reason
 * - Every "restructured" entry has a note explaining the change
 * - Coverage meets the expected threshold
 * - No Angular routes exist without a corresponding registry entry
 */

import { test, expect } from "@playwright/test";
import * as fs from "fs";
import * as path from "path";
import { pageRegistry } from "./page-registry";
import { getCoverageStats, getGaps, getExcluded, getRestructured } from "./registry-helpers";

test.describe("Registry integrity", () => {
    test("all migrated pages have a modern route", () => {
        const migrated = pageRegistry.filter((e) => e.status === "migrated");
        const missing = migrated.filter((e) => e.modernPath == null);

        expect(
            missing,
            `Migrated pages without modernPath:\n${missing.map((e) => `  - ${e.id}: ${e.name}`).join("\n")}`,
        ).toHaveLength(0);
    });

    test("all excluded pages have a reason", () => {
        const excluded = getExcluded();
        const missing = excluded.filter((e) => !e.exclusionReason);

        expect(
            missing,
            `Excluded pages without exclusionReason:\n${missing.map((e) => `  - ${e.id}: ${e.name}`).join("\n")}`,
        ).toHaveLength(0);
    });

    test("all restructured pages have a note", () => {
        const restructured = getRestructured();
        const missing = restructured.filter((e) => !e.restructureNote);

        expect(
            missing,
            `Restructured pages without restructureNote:\n${missing.map((e) => `  - ${e.id}: ${e.name}`).join("\n")}`,
        ).toHaveLength(0);
    });

    test("no duplicate registry IDs", () => {
        const ids = pageRegistry.map((e) => e.id);
        const duplicates = ids.filter((id, i) => ids.indexOf(id) !== i);

        expect(
            duplicates,
            `Duplicate registry IDs: ${[...new Set(duplicates)].join(", ")}`,
        ).toHaveLength(0);
    });

    test("no duplicate modern paths", () => {
        const paths = pageRegistry
            .filter((e) => e.modernPath != null && e.pageType === "page")
            .map((e) => e.modernPath!);
        const duplicates = paths.filter((p, i) => paths.indexOf(p) !== i);

        expect(
            duplicates,
            `Duplicate modern paths: ${[...new Set(duplicates)].join(", ")}`,
        ).toHaveLength(0);
    });
});

test.describe("Coverage threshold", () => {
    test("coverage percentage meets threshold (>= 95%)", () => {
        const stats = getCoverageStats();

        console.log(`\n  Registry Coverage: ${stats.coveragePercent.toFixed(1)}%`);
        console.log(`    Total: ${stats.total}  Migrated: ${stats.migrated}  Restructured: ${stats.restructured}  Excluded: ${stats.excluded}  Not Yet: ${stats.notYet}\n`);

        expect(stats.coveragePercent).toBeGreaterThanOrEqual(95);
    });
});

test.describe("Gap report", () => {
    test("list not-yet-migrated pages", () => {
        const gaps = getGaps();

        if (gaps.length > 0) {
            console.log("\n  NOT YET MIGRATED:");
            for (const gap of gaps) {
                console.log(`    - ${gap.name} (${gap.legacyPath})`);
            }
            console.log("");
        }

        // This test doesn't fail — it just surfaces the gaps.
        // The threshold test above enforces the overall coverage level.
        expect(gaps.length).toBeGreaterThanOrEqual(0);
    });
});

test.describe("Route sync", () => {
    test("no orphan modern routes — every app.routes.ts path appears in registry", () => {
        // Read app.routes.ts and extract route paths
        const routesFile = path.resolve(__dirname, "../../src/app/app.routes.ts");
        const routesContent = fs.readFileSync(routesFile, "utf-8");
        const lines = routesContent.split("\n");

        // Workflow child step names that are ONLY child routes (not also top-level routes).
        // Names like "counties", "organizations", "priority-landscapes", "dnr-upland-regions"
        // are both child steps AND top-level routes — they are NOT in this list so they pass through.
        const childOnlySteps = new Set([
            "basics", "location-simple", "location-detailed",
            "treatments", "contacts", "expected-funding",
            "classifications", "photos", "documents-notes",
            "external-links", "instructions", "upload", "validate-metadata",
        ]);

        // Extract paths from route config — matches: path: "some-path" and path: `some-path`
        const pathRegex = /path:\s*["'`]([^"'`]+)["'`]/g;
        const appPaths: string[] = [];
        const seen = new Set<string>();

        for (const line of lines) {
            // Skip commented-out lines
            if (line.trimStart().startsWith("//")) continue;

            let match: RegExpExecArray | null;
            pathRegex.lastIndex = 0;
            while ((match = pathRegex.exec(line)) !== null) {
                const routePath = match[1];

                // Skip wildcard, empty, and not-found
                if (routePath === "**" || routePath === "" || routePath === "not-found") continue;

                // Skip child-only workflow steps
                if (childOnlySteps.has(routePath)) continue;

                // Normalize parameterized paths: :projectID → {projectID}
                const normalized = "/" + routePath.replace(/:(\w+)/g, "{$1}");

                // Deduplicate (e.g., "counties" appears as both top-level and child route)
                if (!seen.has(normalized)) {
                    seen.add(normalized);
                    appPaths.push(normalized);
                }
            }
        }

        // Check each app route against the registry
        const registryPaths = new Set(
            pageRegistry
                .filter((e) => e.modernPath != null)
                .map((e) => e.modernPath!),
        );

        const orphans = appPaths.filter((p) => !registryPaths.has(p));

        if (orphans.length > 0) {
            console.log("\n  ORPHAN ROUTES (in app.routes.ts but not in registry):");
            for (const orphan of orphans) {
                console.log(`    - ${orphan}`);
            }
            console.log("  Add these to e2e-tests/registry/page-registry.ts\n");
        }

        expect(
            orphans,
            `Routes in app.routes.ts not in registry:\n${orphans.join("\n")}`,
        ).toHaveLength(0);
    });
});
