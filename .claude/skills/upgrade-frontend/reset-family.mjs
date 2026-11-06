#!/usr/bin/env node
// Usage (from WADNR.Web/): node ../.claude/skills/upgrade-frontend/reset-family.mjs @angular @angular-eslint
// Drops the named scopes'/packages' entries from package-lock.json (and node_modules) so the next
// `npm install` re-resolves just those families. Keeps the lockfile's 4-space indent (git stores it with
// LF line endings, so the LF this writes is not a whole-file diff).
import { readFileSync, rmSync, writeFileSync } from "node:fs";

const families = process.argv.slice(2);
if (!families.length) {
    console.error("usage: reset-family.mjs <@scope | package> [...]");
    process.exit(2);
}
const matches = (key) => families.some((f) => (f.startsWith("@") && !f.includes("/") ? key.startsWith(`node_modules/${f}/`) : key === `node_modules/${f}`));
const lock = JSON.parse(readFileSync("package-lock.json", "utf8"));
let removed = 0;
for (const key of Object.keys(lock.packages)) {
    if (matches(key)) {
        delete lock.packages[key];
        removed++;
    }
}
writeFileSync("package-lock.json", JSON.stringify(lock, null, 4) + "\n");
for (const f of families) rmSync(`node_modules/${f}`, { recursive: true, force: true });
console.log(`removed ${removed} lockfile entries for ${families.join(", ")}; now run: npm install`);
