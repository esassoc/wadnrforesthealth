#!/usr/bin/env node

/**
 * a11y-report.mjs — WCAG 2.2 AA Accessibility Report Generator
 *
 * Reads Playwright JSON reporter output (with axe-core attachments),
 * computes per-page and overall accessibility scores, and prints
 * a compact summary to stdout.
 *
 * Usage:
 *   node a11y-report.mjs [--json-file path] [--verbose]
 *
 * The JSON file is produced by setting PLAYWRIGHT_JSON_OUTPUT_FILE
 * when running Playwright with the json reporter.
 */

import { readFileSync } from "fs";
import { resolve } from "path";

// --- CLI args ---
const args = process.argv.slice(2);
const verbose = args.includes("--verbose");
const jsonFileIdx = args.indexOf("--json-file");
const jsonFile = jsonFileIdx !== -1 && args[jsonFileIdx + 1]
    ? resolve(args[jsonFileIdx + 1])
    : resolve("a11y-results.json");

// --- Deque axe Monitor scoring ---
// Each page is classified by its worst (highest-severity) violation.
// Page weight: critical=0%, serious=40%, moderate=80%, minor/none=100%
// Overall score = average of page weights.
// Source: https://docs.deque.com/devtools-mobile/2024.9.18/en/score/
const PAGE_WEIGHT = { critical: 0, serious: 0.4, moderate: 0.8, minor: 1, none: 1 };

// Impact weights for the "top rules" ranking (not used in scoring)
const IMPACT_WEIGHT = { critical: 10, serious: 5, moderate: 3, minor: 1 };

// --- Read and parse Playwright JSON ---
let data;
try {
    data = JSON.parse(readFileSync(jsonFile, "utf-8"));
} catch (err) {
    console.error(`Error reading ${jsonFile}: ${err.message}`);
    process.exit(1);
}

// --- Extract axe results from attachments ---
function extractAxeResults(suites) {
    const pages = [];

    function walk(suite) {
        for (const spec of suite.specs || []) {
            for (const test of spec.tests || []) {
                for (const result of test.results || []) {
                    for (const att of result.attachments || []) {
                        if (!att.name?.startsWith("axe-results-")) continue;
                        if (att.contentType !== "application/json") continue;

                        const pageName = att.name.replace("axe-results-", "");
                        let axeData;
                        try {
                            if (att.body) {
                                axeData = JSON.parse(Buffer.from(att.body, "base64").toString());
                            } else if (att.path) {
                                axeData = JSON.parse(readFileSync(att.path, "utf-8"));
                            }
                        } catch { /* skip unparseable */ }

                        if (axeData) {
                            pages.push({ name: pageName, axe: axeData });
                        }
                    }
                }
            }
        }
        for (const child of suite.suites || []) {
            walk(child);
        }
    }

    for (const s of suites) {
        walk(s);
    }
    return pages;
}

// --- Score a single page (Deque method) ---
// Page is classified by its worst violation's impact level.
// critical → 0%, serious → 40%, moderate → 80%, none/minor → 100%
function scorePage(axe) {
    const hasData = (axe.passes || []).length > 0 || (axe.violations || []).length > 0;
    if (!hasData) return null;

    let worstImpact = "none";
    const severity = ["minor", "moderate", "serious", "critical"];
    for (const v of axe.violations || []) {
        const idx = severity.indexOf(v.impact);
        if (idx > severity.indexOf(worstImpact)) {
            worstImpact = v.impact;
        }
    }

    return Math.round(PAGE_WEIGHT[worstImpact] * 100 * 10) / 10;
}

function letterGrade(score) {
    if (score == null) return "N/A";
    if (score >= 90) return "A";
    if (score >= 80) return "B";
    if (score >= 70) return "C";
    if (score >= 60) return "D";
    return "F";
}

// --- Aggregate violations ---
function aggregateViolations(pages) {
    const byImpact = { critical: { nodes: 0, rules: new Set(), pages: new Set() }, serious: { nodes: 0, rules: new Set(), pages: new Set() }, moderate: { nodes: 0, rules: new Set(), pages: new Set() }, minor: { nodes: 0, rules: new Set(), pages: new Set() } };
    const byRule = {};

    for (const page of pages) {
        for (const v of page.axe.violations || []) {
            const impact = v.impact || "minor";
            const nodeCount = (v.nodes || []).length;

            byImpact[impact].nodes += nodeCount;
            byImpact[impact].rules.add(v.id);
            byImpact[impact].pages.add(page.name);

            if (!byRule[v.id]) {
                byRule[v.id] = { impact, help: v.help || "", nodes: 0, pages: new Set() };
            }
            byRule[v.id].nodes += nodeCount;
            byRule[v.id].pages.add(page.name);
        }
    }

    return { byImpact, byRule };
}

// --- Format helpers ---
function pad(str, len) { return String(str).padEnd(len); }
function padL(str, len) { return String(str).padStart(len); }

// --- Main ---
const pages = extractAxeResults(data.suites || []);

if (pages.length === 0) {
    console.log("No axe-core results found in the Playwright JSON output.");
    process.exit(0);
}

// Compute scores and classify pages
const scored = pages.map(p => {
    const score = scorePage(p.axe);
    const violationCount = (p.axe.violations || []).reduce((sum, v) => sum + (v.nodes || []).length, 0);

    // Determine worst impact for display
    const severity = ["minor", "moderate", "serious", "critical"];
    let worstImpact = "none";
    for (const v of p.axe.violations || []) {
        const idx = severity.indexOf(v.impact);
        if (idx > severity.indexOf(worstImpact)) {
            worstImpact = v.impact;
        }
    }

    return { name: p.name, score, grade: letterGrade(score), worstImpact, violations: p.axe.violations || [], violationCount };
});

const validScores = scored.filter(s => s.score != null);
const overallScore = validScores.length > 0
    ? Math.round(validScores.reduce((sum, s) => sum + s.score, 0) / validScores.length * 10) / 10
    : 0;
const overallGrade = letterGrade(overallScore);

const cleanPages = scored.filter(s => s.violationCount === 0);
const pagesWithViolations = scored.filter(s => s.violationCount > 0);
const totalViolations = scored.reduce((sum, s) => sum + s.violationCount, 0);

const { byImpact, byRule } = aggregateViolations(pages);

// --- Print report ---
const SEP = "=".repeat(60);

console.log("");
console.log(SEP);
console.log("  WCAG 2.2 AA ACCESSIBILITY REPORT");
console.log(SEP);
console.log(`  Pages scanned:    ${pages.length}`);
console.log(`  Overall score:    ${overallScore} / 100  (${overallGrade})`);
console.log(`  Total violations: ${totalViolations} across ${pagesWithViolations.length} pages`);
console.log(`  Clean pages:      ${cleanPages.length}`);
console.log(SEP);

// Page classification (Deque method)
const pageDist = { none: 0, minor: 0, moderate: 0, serious: 0, critical: 0 };
for (const s of scored) { pageDist[s.worstImpact]++; }
console.log("");
console.log("PAGE CLASSIFICATION (by worst violation)");
console.log(`  Critical (0%):   ${padL(pageDist.critical, 3)} pages`);
console.log(`  Serious  (40%):  ${padL(pageDist.serious, 3)} pages`);
console.log(`  Moderate (80%):  ${padL(pageDist.moderate, 3)} pages`);
console.log(`  Clean    (100%): ${padL(pageDist.none + pageDist.minor, 3)} pages`);

// Grade distribution
const gradeDist = { A: 0, B: 0, C: 0, D: 0, F: 0, "N/A": 0 };
for (const s of scored) { gradeDist[s.grade]++; }
console.log("");
console.log("GRADE DISTRIBUTION");
console.log(`  A: ${gradeDist.A}  B: ${gradeDist.B}  C: ${gradeDist.C}  D: ${gradeDist.D}  F: ${gradeDist.F}`);

// Violations by impact
console.log("");
console.log("VIOLATIONS BY IMPACT");
for (const impact of ["critical", "serious", "moderate", "minor"]) {
    const d = byImpact[impact];
    if (d.nodes > 0) {
        console.log(`  ${pad(impact.charAt(0).toUpperCase() + impact.slice(1) + ":", 12)} ${padL(d.nodes, 4)} elements  (${d.rules.size} rules, ${d.pages.size} pages)`);
    }
}

// Top rules by weighted impact
const sortedRules = Object.entries(byRule)
    .map(([id, d]) => ({ id, ...d, weighted: d.nodes * (IMPACT_WEIGHT[d.impact] || 1) }))
    .sort((a, b) => b.weighted - a.weighted);

console.log("");
console.log("TOP RULES BY WEIGHTED IMPACT");
console.log(`  ${pad("#", 3)} ${pad("Rule ID", 28)} ${pad("Impact", 10)} ${padL("Nodes", 6)} ${padL("Pages", 6)}`);
const topN = Math.min(sortedRules.length, 10);
for (let i = 0; i < topN; i++) {
    const r = sortedRules[i];
    console.log(`  ${pad(i + 1, 3)} ${pad(r.id, 28)} ${pad(r.impact, 10)} ${padL(r.nodes, 6)} ${padL(r.pages.size, 6)}`);
}

// Per-page scores (worst first, top 20)
const worstFirst = [...scored].sort((a, b) => (a.score ?? 999) - (b.score ?? 999));
const showCount = Math.min(worstFirst.length, 20);

console.log("");
console.log(`PER-PAGE SCORES (worst ${showCount} of ${scored.length})`);
console.log(`  ${pad("#", 3)} ${pad("Page", 32)} ${padL("Score", 6)} ${padL("Grade", 6)} ${pad(" Worst", 10)} ${padL("Violations", 11)}`);
for (let i = 0; i < showCount; i++) {
    const s = worstFirst[i];
    console.log(`  ${pad(i + 1, 3)} ${pad(s.name, 32)} ${padL(s.score ?? "N/A", 6)} ${padL(s.grade, 6)} ${pad(" " + s.worstImpact, 10)} ${padL(s.violationCount, 11)}`);
}

// Verbose: per-page violation details
if (verbose) {
    console.log("");
    console.log("PER-PAGE VIOLATION DETAILS");
    for (const s of worstFirst.filter(s => s.violationCount > 0)) {
        console.log(`\n  ${s.name} (${s.score} — ${s.grade})`);
        for (const v of s.violations) {
            console.log(`    [${v.impact}] ${v.id}: ${v.help} (${(v.nodes || []).length} elements)`);
        }
    }
}

// Clean pages
if (cleanPages.length > 0) {
    console.log("");
    console.log(`CLEAN PAGES (${cleanPages.length})`);
    const names = cleanPages.map(p => p.name).sort();
    // Wrap at ~80 chars
    let line = "  ";
    for (const name of names) {
        if (line.length + name.length + 2 > 80 && line.length > 2) {
            console.log(line);
            line = "  ";
        }
        line += (line.length > 2 ? ", " : "") + name;
    }
    if (line.length > 2) console.log(line);
}

console.log("");
console.log(SEP);
