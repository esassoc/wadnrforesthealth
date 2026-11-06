#!/usr/bin/env node
// Usage (repo root):
//   node .claude/skills/upgrade-backend/list-packages.mjs --outdated --highest-minor > outdated.json
//   node .claude/skills/upgrade-backend/apply-updates.mjs outdated.json [--only <Id>...] [--except <Id>...] [--dry-run]
// Sets each outdated top-level PackageReference to its latest in-range version, and to the SAME version in
// every .csproj that references it, so shared packages (EF Core, Serilog, Swashbuckle, NTS, ...) stay aligned.
import { readFileSync, writeFileSync } from "node:fs";

const args = process.argv.slice(2);
const file = args[0];
const pick = (flag) => {
    const i = args.indexOf(flag);
    if (i < 0) return null;
    const out = [];
    for (let j = i + 1; j < args.length && !args[j].startsWith("--"); j++) out.push(args[j].toLowerCase());
    return out;
};
const only = pick("--only"), except = pick("--except") ?? [], dry = args.includes("--dry-run");
const cmp = (a, b) => {
    const pa = a.split(/[.-]/), pb = b.split(/[.-]/);
    for (let i = 0; i < Math.max(pa.length, pb.length); i++) {
        const x = parseInt(pa[i] ?? "0"), y = parseInt(pb[i] ?? "0");
        if (x !== y) return x - y;
    }
    return 0;
};

const report = JSON.parse(readFileSync(file, "utf8"));
// Any problem means a project's packages may be missing from the report.
const problems = report.problems ?? [];
if (problems.length) {
    for (const p of problems) console.error(`${file}: ${p.level} ${p.project ?? ""}: ${p.text}`);
    console.error("Restore/build the solution and re-run dotnet list.");
    process.exit(1);
}
const target = new Map();
const projects = [];
for (const p of report.projects ?? []) {
    projects.push(p.path);
    for (const f of p.frameworks ?? [])
        for (const pkg of f.topLevelPackages ?? []) {
            const id = pkg.id, key = id.toLowerCase();
            if (!pkg.latestVersion || pkg.latestVersion === "Not found at the sources") continue;
            if (only && !only.includes(key)) continue;
            if (except.includes(key)) continue;
            if (!target.has(key) || cmp(pkg.latestVersion, target.get(key).v) > 0) target.set(key, { id, v: pkg.latestVersion });
        }
}

const label = (path) => path.replace(/\\/g, "/").replace(/.*\/([^/]+)\/[^/]+\.csproj$/, "$1").padEnd(24);
let edits = 0;
for (const path of projects) {
    let xml = readFileSync(path, "utf8");
    const before = xml;
    xml = xml.replace(/<PackageReference\s+Include="([^"]+)"\s+Version="([^"]+)"/g, (m, id, v) => {
        const t = target.get(id.toLowerCase());
        if (!t || t.v === v) return m;
        if (/^[[(]/.test(v)) {
            console.log(`${label(path)} ${id.padEnd(60)} ${v} kept (version range = deliberate ceiling)`);
            return m;
        }
        console.log(`${label(path)} ${id.padEnd(60)} ${v} -> ${t.v}`);
        edits++;
        return m.replace(`Version="${v}"`, `Version="${t.v}"`);
    });
    if (!dry && xml !== before) writeFileSync(path, xml);
}
console.log(`${edits} PackageReference edit(s)${dry ? " (dry run)" : ""}`);
