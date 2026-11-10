---
name: upgrade-backend
description: >
  Refresh WADNR's NuGet dependencies across every .NET project (API, EFModels, Models, Common, Tests,
  GDALAPI, Scalar, SitkaCaptureService, and the WADNR.Web host) — in-range bumps kept aligned across
  projects, removal of unused packages, then a triaged pass over major versions — with a vulnerability
  scan, a license check of the whole resolved (transitive) graph after every change (catching packages
  that went commercial or pull in pre-release-licensed dependencies), per-package breaking-change
  review, targeted verification, one commit per package group on a new feature branch, and a
  full-suite gate at the end. Use
  when the user says "update the backend packages", "upgrade NuGet packages", "bump the .NET
  dependencies", "what NuGet majors can we take", or "/upgrade-backend". The frontend counterpart is
  /upgrade-frontend.
---

# Upgrade Backend Packages

All work happens on a new upgrade branch (§0); never commit to `develop` or `main`. Commands run from
the repo root in Git Bash. There is no central package management — each `.csproj` pins its own
versions, so a package shared across projects (EF Core, Swashbuckle, Azure.Storage.Blobs, NTS, Serilog)
must move to one version everywhere. Commit each package group to the upgrade branch after
`/precommit-review`; push only when the user asks.

Scripts next to this file:

- `list-packages.mjs [dotnet list flags]` — `dotnet list package --format json` for every tracked
  `.csproj`, merged into one report. **Use it instead of `dotnet list WADNR.sln`**, which fails outright
  (the SSDT `WADNR.Database.sqlproj` can't be loaded by the dotnet CLI) and would miss
  `WADNR.Web.csproj` (the Angular host has NuGet packages but isn't in the solution). Exits 1 if any
  project failed to load — restore/build it and re-run.
- `apply-updates.mjs <outdated.json> [--only Id...] [--except Id...] [--dry-run]` — set each outdated
  top-level `PackageReference` to its target version, the same version in every `.csproj`. A reference
  written as a version **range** (`[1.8.22,2.0)`) is a deliberate ceiling and is left alone.
- `nuget-license-diff.mjs <before.json> <after.json>` — license of every package (direct *and*
  transitive) that is new or changed between two `list-packages.mjs --include-transitive` snapshots.
  Reads the SPDX expression, else the license URL, else downloads the `.nupkg` and reads the embedded
  license file. `--check Id@ver ...` for one-off lookups (use it on every major's latest version during
  triage).

Put snapshots in a scratch directory outside the repo (`<scratch>` below):

```bash
S=.claude/skills/upgrade-backend
node $S/list-packages.mjs --include-transitive      > <scratch>/pkgs-before.json
node $S/list-packages.mjs --outdated --highest-minor > <scratch>/outdated-minor.json
node $S/list-packages.mjs --outdated                 > <scratch>/outdated-latest.json
```

**Build all** — not `dotnet build WADNR.sln`: it exits with error MSB4278 because the dotnet CLI can't load
the SSDT `WADNR.Database.sqlproj`, so it breaks any `&&` chain. These four leaf projects reference every
other `.csproj` (Tests → API → EFModels/Models/Common/SitkaCaptureService; Scalar; GDALAPI and the Web
host stand alone):

```bash
build_all() { for p in WADNR.API.Tests WADNR.GDALAPI WADNR.Scalar WADNR.Web; do dotnet build "$p/$p.csproj" "$@" || return 1; done; }
```

**Vulnerability scan** (human-readable, per project):

```bash
for p in $(git ls-files '*.csproj'); do dotnet list "$p" package --vulnerable --include-transitive; done
```

## 0 · Branch

Create the upgrade branch before touching anything. Invoking the skill is the request to branch, so
don't ask first.

```bash
git status --porcelain        # must be empty; if not, stop and ask the user what to do with the changes
git fetch origin
git switch -c feature/upgrade-backend-$(date +%Y-%m-%d) origin/develop
```

- **Resuming:** if the current branch is already a `feature/upgrade-backend-*` branch from an
  interrupted run, keep working on it instead of creating another.
- **Before every commit:** run `git branch --show-current`. If it prints `develop` or `main`, stop
  and don't commit.
- **Database baseline:** the §1 baseline tests must run on the branch's starting point. If that means
  stashing work in progress (as when resuming), clean `WADNR.API.Tests/bin` and `obj` before the run.
  Otherwise newer test-adapter DLLs left over from a later build load alongside an older framework,
  and test discovery finds 0 tests.

## 1 · Baseline

- `build_all --no-incremental` and record the warning counts by code. Watch for `NU190x` (a
  known-vulnerable package, often transitive) and run the vulnerability scan.
- Transitive snapshot (above) — the license and vulnerability diffs compare against it.
- Tests: `WADNR.API.Tests` are integration tests against a real SQL Server database (`environment.json`,
  gitignored). Restore a fresh copy first (`Build/DatabaseRestore.ps1`, PowerShell), then
  `dotnet test WADNR.API.Tests/WADNR.API.Tests.csproj` and record the **names** of failing tests.
  Compare failures by name later and re-run any new one in isolation
  (`dotnet test WADNR.API.Tests/WADNR.API.Tests.csproj --no-build --filter "Name=<test>"`) before
  calling it a regression.

## 2 · In-range bumps

```bash
node $S/apply-updates.mjs <scratch>/outdated-minor.json
build_all
```

Same-major bumps can still break the build — Azure SDKs and packages with experimental APIs version
their breaking changes as minors:

- **An API redesigned in a minor**: migrate the call sites in this run — the changelog's
  breaking-change sections list every rename, and the compiler finds every site. When the migration is
  too large for this pass, hold the package (`--except <Id>`) and file the migration as its own story.
- **A type defined in two assemblies** after an Azure SDK bump (e.g. Azure.Core vs a transitive
  Azure.Identity): reference the newer package directly in the project that sees the collision.

Then the WADNR-specific follow-ups:

- **Swashbuckle**: `WADNR.API.csproj`'s Debug `PostBuild` target installs
  `Swashbuckle.AspNetCore.Cli` at a **hard-coded version** (`dotnet tool install --version …`). Bump it
  to the new `Swashbuckle.AspNetCore` version in the same commit. The build regenerates
  `WADNR.API/swagger.json`; if it changed, run `npm run gen-model` in `WADNR.Web` and review the diff
  in `src/app/shared/generated/`.
- **EF Core**: the scaffold pipeline uses the **global** `dotnet-ef` tool (`dotnet tool list -g`). When
  EF Core moves, update it to match: `dotnet tool update -g dotnet-ef --version <ef-version>`.
- Then run the vulnerability scan, the license diff (§4), and the full test suite.

## 3 · Remove what nothing uses, then triage majors

**Unused packages.** Grep each direct reference's real API surface — namespaces often differ from the
package id (`ProjNET` → `ProjNet`), and some packages only surface through `Add*()` extension methods
or `appsettings.json` (Serilog sinks/formatters, e.g. `Serilog.Formatting.Compact`). Before removing a
candidate:

- `git log -S'Include="<Id>"' -- '*.csproj'` — a reference added by an "update vulnerable packages"
  commit is a **security pin** forcing a patched transitive; keep it and just bump it.
- Remove, then diff the resolved graph against the previous snapshot. A package that is still pulled in
  transitively can drop to a *lower* version once its direct pin goes; re-run the vulnerability scan.

**Majors.** For each package whose major moves: current → latest, which projects, and
`nuget-license-diff.mjs --check <Id>@<latest>`. Bucket them:

| Bucket | Test | Action |
|---|---|---|
| **License hold** | Latest is commercial / source-available, or pulls in a package that is (or ships under pre-release terms) | Stay on the last acceptable version and write the reference as a range ceiling, e.g. `Version="[1.8.25,2.0)"`. A range is the only hold `apply-updates.mjs` respects, so every license hold is one. |
| **Blocked** | A dependency is built against the old major (e.g. EF Core's SqlServer provider pins a SqlClient major) | Wait for the dependent to move. |
| **Low** | Test-only or tiny surface (coverlet, `Microsoft.NET.Test.Sdk`) | Take it now. |
| **Medium** | Real API surface we can cover (MSTest, NTS / `NetTopologySuite.IO.GeoJSON4STJ`, Hangfire, ClosedXML, PDFsharp, Magick.NET, SkiaSharp) | Read the migration guide; take it with tests. |
| **Separate project** | Auth or hosting rewrites (JwtBearer/OpenIdConnect majors outside the .NET major, Serilog → OpenTelemetry) | Its own story. |

Present the table and a recommended order before upgrading.

## 4 · After every package change

```bash
build_all   # restores every edited project, so the snapshot sees its new graph
node $S/list-packages.mjs --include-transitive > <scratch>/pkgs-after.json
node $S/nuget-license-diff.mjs <scratch>/pkgs-before.json <scratch>/pkgs-after.json
```

…then the vulnerability scan.

- **`RESTRICTIVE?`** — stop and raise it. The script flags source-available and commercial terms,
  strong copyleft (GPL, AGPL, LGPL, unless only an `OR` alternative), and pre-release or evaluation
  terms. **Known and accepted in WADNR** (flagged on every bump; check the terms are unchanged, then
  continue):
  - `Hangfire.*`: LGPL-3.0 open-source edition (dual-licensed with Hangfire's commercial terms). A new
    *major* needs its license re-read.
  - `ProjNet`: LGPL-2.1-or-later (referenced only by `WADNR.Common`).
- **License holds in place** (version-range ceilings; `apply-updates.mjs` leaves them alone):
  - `PDFsharp` `[6.2.4,6.3)` in `WADNR.API`: 6.5.0 changed from MIT to **AGPL-3.0-only**. Re-check on each
    run whether a later release returns to MIT.
- **Security pins in place**: `System.Security.Cryptography.Pkcs` in `WADNR.EFModels` (SqlClient 6.1.x pulls a
  vulnerable 9.0.x). Bump it with the .NET patch; remove it once SqlClient depends on a patched version.
- `nuget-license-diff.mjs` compares embedded license files by their **first line only** (Hangfire's is just
  "License"), so a `bumped` row can hide changed terms. For flagged packages, diff the full file between
  versions (`unzip -p <nupkg> LICENSE.md`).
- **`CHANGED`** — read it even when both ends look fine. A *transitive* package moving to a new major
  can change terms (Microsoft has shipped release packages that pull a dependency under "MICROSOFT
  PRE-RELEASE SOFTWARE LICENSE TERMS"). A change can also be an improvement — say so in the commit.
- **`CHECK`** with `[Microsoft terms]` — a Microsoft release license
  (`Microsoft.VisualStudio.Azure.Containers.Tools.Targets`, SNI runtime); confirm the first line didn't
  change. `Microsoft.AspNetCore.Http` 2.x reports a legacy `aspnet/AspNetCore` LICENSE URL, which is
  Apache-2.0.
- **`CHECK`** otherwise — read the license file before deciding. Weak copyleft (MPL, EPL) used
  unmodified is fine; strong copyleft or pre-release terms the script missed are a stop, same as
  `RESTRICTIVE?`.

## 5 · Per-package procedure

1. **Breaking changes**: the vendor migration guide (MSTest `unit-testing-mstest-migration-v3-v4`),
   GitHub releases (`gh api repos/<o>/<r>/releases`), or the repo's raw `CHANGELOG.md`; for a behavior
   change, read the PR that introduced it.
2. **Our usage**: grep each breaking change's API; record a hit or "no hit".
3. **Install** with `apply-updates.mjs <scratch>/outdated-latest.json --only <Id>...`;
   `dotnet remove <proj> package <Id>` for anything folded into the new major.
4. **Verify**: build → the area's tests (`--filter "FullyQualifiedName~<Area>"`) → a new test where the
   upgrade changes behavior no test exercises. Prove it bites: swap the old version back in the
   `.csproj`, run it, restore.
5. **Commit** one group per commit, after `/precommit-review`: breaking changes checked against our
   code, what changed, license result, what was verified.

## 6 · Traps

| Trap | What to do |
|---|---|
| **Default interface members hide a signature change** (a listener/handler interface moves to `ValueTask` with default bodies, so the old `Task` methods compile and are *never called*) | Build success proves nothing for listener/handler/filter interfaces. Match the new signatures exactly and check the implementation is actually invoked. |
| A serializer changes output by default (NTS GeoJSON enforces RFC 7946 ring order in newer majors) | Find what reads the output back (the Leaflet maps, GeoServer, `GeoJsonSerializerGeometryTypeTests`). If every reader normalizes, take the default and pin it with an orientation + round-trip test; otherwise set the option to the old behavior. |
| Removed test-framework APIs (MSTest 4: `[ExpectedException]`, `Assert.ThrowsException`) | `Assert.ThrowsExactly<T>` (both matched the exact type). Wrap only the acting call so an arrange-step throw can't satisfy the test. |
| Swashbuckle package and CLI drift apart | The `PostBuild` `dotnet tool install --version` in `WADNR.API.csproj` must match `Swashbuckle.AspNetCore`, or `swagger.json` generation fails or silently uses the old generator. |
| Image libraries ship native assets per OS (SkiaSharp + `SkiaSharp.NativeAssets.Linux`, Magick.NET) | Keep the managed and native packages on the same version; the Linux container is what runs in AKS, so verify in the Docker image, not just on Windows. |
| Tests that trigger Hangfire jobs and don't wait | Rows left mid-run after a test are the process exiting mid-job, not a regression — confirm with a test that waits for completion before concluding either way. |

## 7 · Finish

```bash
# PowerShell: Build/DatabaseRestore.ps1 (fresh database), then:
dotnet test WADNR.API.Tests/WADNR.API.Tests.csproj         # compare failures by name to the baseline
build_all --no-incremental                                  # 0 errors; warning codes vs baseline
node $S/list-packages.mjs --include-transitive > <scratch>/pkgs-final.json
node $S/nuget-license-diff.mjs <scratch>/pkgs-before.json <scratch>/pkgs-final.json
```

…plus the vulnerability scan. Push the branch (`git push -u origin <branch>`) only when the user
asks. When the user asks for a PR (`gh pr create --base develop`), include the upgrades table,
removals, holds and ceilings (with the reason for each), deferred majors, and any behavior change a
reviewer should know (serializer output, job behavior, generated client changes).
