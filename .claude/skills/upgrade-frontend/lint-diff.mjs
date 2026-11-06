#!/usr/bin/env node
// Usage (from WADNR.Web/):
//   npx ng lint --format json --output-file lint-report.json; mv lint-report.json <scratch>/lint-before.json
//   ... upgrade, lint again into <scratch>/lint-after.json ...
//   node ../.claude/skills/upgrade-frontend/lint-diff.mjs <scratch>/lint-before.json <scratch>/lint-after.json
// Counts ESLint messages by rule (and severity) in two `--format json` reports and prints every rule whose
// count changed. WADNR.Web has a large standing lint backlog, so the gate is "no rule goes up", not "0 errors".
// Exits 1 if any rule increased.
import { readFileSync } from "node:fs";

const [beforePath, afterPath] = process.argv.slice(2);
if (!beforePath || !afterPath) {
    console.error("usage: lint-diff.mjs <before.json> <after.json>");
    process.exit(2);
}
const count = (path) => {
    const m = new Map();
    for (const file of JSON.parse(readFileSync(path, "utf8")))
        for (const msg of file.messages) {
            const key = `${msg.severity === 2 ? "error" : "warn "} ${msg.ruleId ?? "(parse error)"}`;
            m.set(key, (m.get(key) ?? 0) + 1);
        }
    return m;
};
const before = count(beforePath), after = count(afterPath);
const rows = [];
for (const key of new Set([...before.keys(), ...after.keys()])) {
    const b = before.get(key) ?? 0, a = after.get(key) ?? 0;
    if (a !== b) rows.push([key, b, a, a - b]);
}
rows.sort((x, y) => y[3] - x[3]);
for (const [key, b, a, d] of rows) console.log(`${(d > 0 ? `+${d}` : `${d}`).padStart(7)}  ${key.padEnd(60)} ${b} -> ${a}`);
const sum = (m) => [...m.values()].reduce((s, n) => s + n, 0);
const up = rows.filter((r) => r[3] > 0).length;
console.log(`total ${sum(before)} -> ${sum(after)}; ${rows.length} rule(s) changed, ${up} increased`);
process.exit(up ? 1 : 0);
