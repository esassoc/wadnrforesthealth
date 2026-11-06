#!/usr/bin/env node
// Usage (repo root):
//   node .claude/skills/upgrade-backend/list-packages.mjs [dotnet list package flags...] > out.json
//   e.g. --include-transitive | --outdated --highest-minor | --outdated | --vulnerable --include-transitive
// Runs `dotnet list <csproj> package <flags> --format json` for every tracked .csproj and merges the results
// into one report with the same shape as `dotnet list WADNR.sln ...`. Listing the solution doesn't work here:
// WADNR.Database.sqlproj is an SSDT project the dotnet CLI can't load (the whole command fails), and
// WADNR.Web.csproj (the Angular host, which has NuGet packages) isn't in WADNR.sln.
import { execFileSync } from "node:child_process";

const flags = process.argv.slice(2);
const csprojs = execFileSync("git", ["ls-files", "*.csproj"], { encoding: "utf8" })
    .split("\n")
    .map((l) => l.trim())
    .filter((l) => l && !l.startsWith("Source/"));

const merged = { version: 1, parameters: flags.join(" "), problems: [], projects: [] };
for (const proj of csprojs) {
    let out;
    try {
        out = execFileSync("dotnet", ["list", proj, "package", ...flags, "--format", "json"], { encoding: "utf8", maxBuffer: 1 << 28, stdio: ["ignore", "pipe", "pipe"] });
    } catch (e) {
        out = e.stdout ?? "";
    }
    let report;
    try {
        report = JSON.parse(out);
    } catch {
        merged.problems.push({ project: proj, level: "error", text: `dotnet list produced no JSON: ${out.trim().slice(0, 300)}` });
        continue;
    }
    merged.problems.push(...(report.problems ?? []));
    merged.projects.push(...(report.projects ?? []));
}
process.stdout.write(JSON.stringify(merged, null, 2) + "\n");
if (merged.problems.length) {
    for (const p of merged.problems) console.error(`${p.level} ${p.project ?? ""}: ${p.text}`);
    process.exit(1);
}
