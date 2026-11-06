#!/usr/bin/env node
// Usage (from WADNR.Web/):
//   node ../.claude/skills/upgrade-frontend/license-diff.mjs [git-ref]   # lockfile vs <git-ref> (default HEAD)
//   node ../.claude/skills/upgrade-frontend/license-diff.mjs --all       # audit the whole installed tree
import { execFileSync } from "node:child_process";
import { existsSync, readFileSync, readdirSync } from "node:fs";
import { join } from "node:path";

const BASE = "MIT|ISC|BSD-2-Clause|BSD-3-Clause|0BSD|Apache-2\\.0|Unlicense|CC0-1\\.0|MIT-0|BlueOak-1\\.0\\.0|Python-2\\.0|Zlib|WTFPL";
// GPL/MPL are acceptable only as an OR alternative; AND means both terms apply.
const PERMISSIVE = new RegExp(`^\\(?(${BASE})(\\s+OR\\s+(${BASE}|GPL-3\\.0-or-later|MPL-2\\.0)|\\s+AND\\s+(${BASE}))*\\)?$`);
const STRONG_COPYLEFT = /\b(A|L)?GPL\b/i;
// For "SEE LICENSE IN <file>" packages the terms live only in the file (tinymce 7+: GPL-2.0-or-later or Tiny's
// commercial terms), so look for GPL in the text itself.
const STRONG_COPYLEFT_TEXT = /GNU (Affero |Lesser )?General Public License|gnu\.org\/licenses\/(a|l)?gpl/i;
const RESTRICTIVE = /commons clause|business source licen[cs]e|\bBUSL\b|server side public licen[cs]e|\bSSPL\b|elastic licen[cs]e|polyform|confluent community licen[cs]e|non-?commercial (use|purposes)|source[- ]available|proprietary licen[cs]e|may not (sell|be sold)|without a (paid|commercial) licen[cs]e/i;

const lock = JSON.parse(readFileSync("package-lock.json", "utf8")).packages;
const all = process.argv[2] === "--all";
const ref = all ? null : process.argv[2] || "HEAD";
const prev = ref ? JSON.parse(execFileSync("git", ["show", `${ref}:./package-lock.json`], { encoding: "utf8", maxBuffer: 1 << 28 })).packages : {};

function declared(dir, entry) {
    const pkgPath = join(dir, "package.json");
    const pkg = existsSync(pkgPath) ? JSON.parse(readFileSync(pkgPath, "utf8")) : {};
    const legacy = Array.isArray(pkg.licenses) ? pkg.licenses.map((l) => l.type ?? l).join(" OR ") : undefined;
    return entry.license ?? pkg.license ?? legacy;
}
function licenseFileText(dir) {
    if (!existsSync(dir)) return "(not installed)";
    const f = readdirSync(dir).find((n) => /^(licen[cs]e|copying)/i.test(n));
    return f ? readFileSync(join(dir, f), "utf8") : "";
}

const rows = [];
for (const [key, entry] of Object.entries(lock)) {
    if (!key || entry.link) continue;
    const before = prev[key];
    const changedVersion = !before || before.version !== entry.version;
    if (!all && !changedVersion) continue;
    const license = declared(key, entry);
    const text = licenseFileText(key);
    const permissive = !!license && PERMISSIVE.test(String(license).trim());
    const seeFile = /^SEE LICEN[CS]E IN/i.test(String(license ?? ""));
    const restrictiveText =
        RESTRICTIVE.test(text) ||
        RESTRICTIVE.test(String(license)) ||
        (!permissive && STRONG_COPYLEFT.test(String(license))) ||
        (seeFile && STRONG_COPYLEFT_TEXT.test(text));
    const status = all ? "-" : !before ? "NEW" : before.license !== entry.license ? "CHANGED" : "same";
    const needsLook = restrictiveText || !permissive;
    if (all && !needsLook) continue;
    if (!all && status === "same" && !needsLook) continue;
    rows.push({
        flag: restrictiveText ? "RESTRICTIVE?" : needsLook ? "CHECK" : "ok",
        status,
        name: `${key.replace(/.*node_modules\//, "")}@${entry.version}`,
        license: status === "CHANGED" ? `${before.license} -> ${license}` : String(license ?? "(none declared)"),
        dev: entry.dev ? "dev" : "prod",
        file: text ? text.split("\n").find((l) => l.trim()).trim().slice(0, 70) : "(no LICENSE file)",
    });
}
rows.sort((a, b) => a.flag.localeCompare(b.flag) || a.name.localeCompare(b.name));
for (const r of rows) console.log([r.flag.padEnd(12), r.status.padEnd(7), r.dev.padEnd(4), r.name.padEnd(48), r.license.padEnd(32), r.file].join(" "));
console.log(`${rows.length} row(s)${all ? " needing a look in the full tree" : ` changed vs ${ref}`}; RESTRICTIVE?=${rows.filter((r) => r.flag === "RESTRICTIVE?").length}`);
