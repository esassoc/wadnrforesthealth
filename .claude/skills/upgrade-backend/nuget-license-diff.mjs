#!/usr/bin/env node
// Usage (repo root):
//   node .claude/skills/upgrade-backend/list-packages.mjs --include-transitive > before.json   # before changing anything
//   ... upgrade ...
//   node .claude/skills/upgrade-backend/list-packages.mjs --include-transitive > after.json
//   node .claude/skills/upgrade-backend/nuget-license-diff.mjs before.json after.json
//   node .claude/skills/upgrade-backend/nuget-license-diff.mjs --check <Id>@<version> [...]   # one-off lookups
import { execFileSync } from "node:child_process";
import { mkdtempSync, readFileSync, rmSync, writeFileSync } from "node:fs";
import { tmpdir } from "node:os";
import { join } from "node:path";

const PERMISSIVE = /^\(?(MIT|Apache-2\.0|BSD-2-Clause|BSD-3-Clause|MS-PL|ISC|0BSD|Unlicense|PostgreSQL|Zlib|CC0-1\.0)(\s+(OR|AND)\s+(MIT|Apache-2\.0|BSD-2-Clause|BSD-3-Clause|MS-PL|ISC|0BSD|Unlicense|Zlib))*\)?$/;
const RESTRICTIVE = /commons clause|business source|\bBUSL\b|\bSSPL\b|server side public|elastic licen[cs]e|polyform|non-?commercial|source[- ]available|commercial licen[cs]e|massient/i;
const STRONG_COPYLEFT = /\b(A|L)?GPL\b|gnu\.org\/licenses\/(a|l)?gpl|GNU (Affero |Lesser )?General Public License/i;
// Release packages can still carry these (DacFx 170.3+ -> SqlServer.Types 170.1000.7).
const PRE_RELEASE = /pre-?release software licen[cs]e|solely to internally evaluate|\bevaluation (only|purposes|licen[cs]e)/i;
// Microsoft's pre-SPDX license URLs, all MIT/Apache/.NET-library terms.
const KNOWN_URLS = [/^https?:\/\/(go\.microsoft\.com\/fwlink\/\?LinkId=329770|aka\.ms\/deprecateLicenseUrl|licenses\.nuget\.org\/(MIT|Apache-2\.0))/i, /github\.com\/dotnet\/[^/]+\/blob\/[^/]+\/LICENSE/i];

// Any problem means a project's packages may be missing from the snapshot.
function assertNoProblems(report, path) {
    const problems = report.problems ?? [];
    if (!problems.length) return;
    for (const p of problems) console.error(`${path}: ${p.level} ${p.project ?? ""}: ${p.text}`);
    console.error("Restore/build the solution and re-take the snapshot.");
    process.exit(1);
}

function load(path) {
    const map = new Map();
    const report = JSON.parse(readFileSync(path, "utf8"));
    assertNoProblems(report, path);
    for (const p of report.projects ?? [])
        for (const f of p.frameworks ?? [])
            for (const pkg of [...(f.topLevelPackages ?? []), ...(f.transitivePackages ?? [])]) {
                const v = pkg.resolvedVersion;
                if (!map.has(pkg.id)) map.set(pkg.id, new Set());
                map.get(pkg.id).add(v);
            }
    return map;
}

const MS_TERMS = /MICROSOFT SOFTWARE LICENSE TERMS|Microsoft Corporation/i;
async function embedded(lid, lv, file) {
    const dir = mkdtempSync(join(tmpdir(), "nupkg-"));
    try {
        const res = await fetch(`https://api.nuget.org/v3-flatcontainer/${lid}/${lv}/${lid}.${lv}.nupkg`);
        if (!res.ok) return { text: `file:${file} (nupkg HTTP ${res.status})`, flag: "CHECK" };
        const pkg = join(dir, "p.nupkg");
        writeFileSync(pkg, Buffer.from(await res.arrayBuffer()));
        const text = execFileSync("unzip", ["-p", pkg, file], { encoding: "utf8", maxBuffer: 1 << 24 });
        const head = text.split("\n").map((l) => l.trim()).find((l) => l && !/^#+\s*$/.test(l))?.replace(/^#+\s*/, "").slice(0, 60) ?? "";
        if (RESTRICTIVE.test(text) || PRE_RELEASE.test(text)) return { text: `file:${file} "${head}"`, flag: "RESTRICTIVE?" };
        if (/\bMIT License\b|Permission is hereby granted, free of charge/i.test(text)) return { text: `file:${file} (MIT text)`, flag: "ok" };
        if (/Apache License,?\s+Version 2\.0/i.test(text)) return { text: `file:${file} (Apache-2.0 text)`, flag: "ok" };
        if (STRONG_COPYLEFT.test(text)) return { text: `file:${file} "${head}"`, flag: "RESTRICTIVE?" };
        // Proprietary-but-free Microsoft terms (SNI, SqlServer.Types, DacFx, VS tooling): record, confirm the terms didn't change.
        return { text: `file:${file} "${head}"${MS_TERMS.test(text) ? " [Microsoft terms]" : ""}`, flag: "CHECK" };
    } catch (e) {
        return { text: `file:${file} (read failed: ${e.message})`, flag: "CHECK" };
    } finally {
        rmSync(dir, { recursive: true, force: true });
    }
}

async function license(id, version) {
    const lid = id.toLowerCase(), lv = version.toLowerCase();
    try {
        const res = await fetch(`https://api.nuget.org/v3-flatcontainer/${lid}/${lv}/${lid}.nuspec`);
        if (!res.ok) return { text: `(nuspec HTTP ${res.status})`, flag: "CHECK" };
        const xml = await res.text();
        const expr = xml.match(/<license[^>]*type="expression"[^>]*>([^<]+)<\/license>/i)?.[1]?.trim();
        const file = xml.match(/<license[^>]*type="file"[^>]*>([^<]+)<\/license>/i)?.[1]?.trim();
        const url = xml.match(/<licenseUrl>([^<]+)<\/licenseUrl>/i)?.[1]?.trim();
        if (expr) {
            const permissive = PERMISSIVE.test(expr);
            return { text: expr, flag: RESTRICTIVE.test(expr) || (!permissive && STRONG_COPYLEFT.test(expr)) ? "RESTRICTIVE?" : permissive ? "ok" : "CHECK" };
        }
        if (file) return await embedded(lid, lv, file);
        if (url) return { text: url, flag: RESTRICTIVE.test(url) || STRONG_COPYLEFT.test(url) ? "RESTRICTIVE?" : KNOWN_URLS.some((r) => r.test(url)) ? "ok" : "CHECK" };
        return { text: "(none declared)", flag: "CHECK" };
    } catch (e) {
        return { text: `(fetch failed: ${e.message})`, flag: "CHECK" };
    }
}

const args = process.argv.slice(2);
const rows = [];
if (args[0] === "--check") {
    for (const spec of args.slice(1)) {
        const [id, v] = spec.split("@");
        const l = await license(id, v);
        rows.push([l.flag, "LOOKUP", `${id}@${v}`, l.text]);
    }
} else {
    const [before, after] = args.map(load);
    const tasks = [];
    for (const [id, versions] of after)
        for (const v of versions) {
            const prev = before.get(id);
            if (prev?.has(v)) continue;
            const was = prev ? [...prev].join(",") : null;
            tasks.push(
                (async () => {
                    const now = await license(id, v);
                    let status = "NEW", text = now.text;
                    if (was) {
                        const old = await license(id, [...prev][0]);
                        status = old.text === now.text ? "bumped" : "CHANGED";
                        if (status === "CHANGED") text = `${old.text} -> ${now.text}`;
                        if (status === "bumped" && now.flag === "ok") return;
                    }
                    rows.push([now.flag, status, `${id}@${was ? `${was}->` : ""}${v}`, text]);
                })()
            );
        }
    await Promise.all(tasks);
    for (const [id, versions] of before) if (!after.has(id)) rows.push(["ok", "REMOVED", `${id}@${[...versions].join(",")}`, ""]);
}
rows.sort((a, b) => a[0].localeCompare(b[0]) || a[2].localeCompare(b[2]));
for (const r of rows) console.log(`${r[0].padEnd(12)} ${r[1].padEnd(8)} ${r[2].padEnd(70)} ${r[3]}`);
console.log(`${rows.length} row(s); RESTRICTIVE?=${rows.filter((r) => r[0] === "RESTRICTIVE?").length} CHECK=${rows.filter((r) => r[0] === "CHECK").length}`);
