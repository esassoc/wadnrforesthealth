---
name: upgrade-frontend
description: >
  Refresh WADNR.Web's npm dependencies end to end — in-range bumps via npm-check-updates, then a
  triaged pass over available major versions — with a license check on every lockfile change
  (catching packages that went source-available / commercial / GPL), per-package breaking-change
  review, targeted verification, one commit per package group, and a full gate at the end. Use when
  the user says "update the frontend packages", "upgrade web dependencies", "bump npm packages",
  "what majors can we take", or "/upgrade-frontend". Frontend only — NuGet (backend) updates are
  /upgrade-backend.
---

# Upgrade Frontend Packages

Work on the current branch unless the user asks for a new one. All commands run from `WADNR.Web/` in
Git Bash unless noted, on the Node version in `.nvmrc` (`nvm use $(cat .nvmrc)`). Commit each package
group after `/precommit-review`, so each upgrade is independently revertible; push only when the user
asks.

The scripts next to this file do the mechanical parts:

- `license-diff.mjs [git-ref]` — license of every lockfile entry whose version changed vs `git-ref`
  (default `HEAD`), reading the shipped LICENSE file when the `license` field is missing or odd.
  `--all` audits the whole installed tree instead.
- `reset-family.mjs <@scope|package>...` — drop one package family's lockfile entries so `npm install`
  re-resolves that family only.
- `lint-diff.mjs <before.json> <after.json>` — ESLint message counts by rule and severity between two
  `ng lint --format json` reports; exits 1 if any rule went up.

```bash
node ../.claude/skills/upgrade-frontend/license-diff.mjs
node ../.claude/skills/upgrade-frontend/reset-family.mjs @angular @angular-eslint
```

WADNR.Web has **no unit tests** (no `*.spec.ts`, no `test` target in `angular.json` — `npm test` fails),
**no `typecheck` script**, and a large standing lint backlog (~49K errors, mostly formatting rules).
Verification is therefore: the Angular build (TypeScript + template type-checking), lint by rule against
the baseline, the production build, and the Playwright e2e suite when an app is running.

## 1 · Baseline

```bash
npm ci
npx ng lint --format json --output-file lint-report.json; mv lint-report.json <scratch>/lint-before.json
npm run build-dev                          # must pass before you start
```

`--output-file` resolves relative to `WADNR.Web/` (an absolute path fails), hence the `mv` into a scratch
directory outside the repo. `ng lint` exits 1 on the backlog; that's expected. The JSON lets you diff
**by rule** after each upgrade — a total that stays flat can hide one rule rising while another falls.

## 2 · In-range bumps

```bash
npx npm-check-updates --target semver --peer -u && npm install && npm run build-dev
npm outdated
```

If `npm install` fails, `package.json` is bumped and the lockfile is not — resolve before moving on.

**ERESOLVE on a lockstep family** (`@angular/*`, `@angular-eslint/*`, anything whose packages peer on
each other at *exact* versions): npm resolves incrementally against the locked siblings, so the family
can never move together. Reset just that family and reinstall:

```bash
node ../.claude/skills/upgrade-frontend/reset-family.mjs @angular @angular-eslint && npm install
```

Only if that still fails, delete `package-lock.json` + `node_modules` and reinstall — it re-resolves
every transitive, so the diff (and the license review) is far larger. Resolve peer conflicts rather
than passing `--legacy-peer-deps` / `--force`, which install a tree npm knows is inconsistent.

Then run the license check (§4), `npm run build-dev`, the lint diff, and commit.

## 3 · Major-version triage

```bash
npx npm-check-updates --target latest
```

For every package whose major moves, record current → latest, prod/dev, and the license **at the
latest version** (`npm view <pkg>@<ver> license`, and the LICENSE file for `SEE LICENSE IN` packages).
Sort each into one bucket:

| Bucket | Test | Action |
|---|---|---|
| **License blocker** | Latest is source-available / commercial, or its terms changed | Stay on the last acceptable major; pin the ceiling (`~` / exact range in `package.json`). |
| **Unused** | No reference in `src/`, `angular.json`, `e2e-tests/`, configs, `tools/`, scripts | Remove it instead (`npm uninstall`), plus any script entry that only it served. `git log -S'"<pkg>"' -- package.json` usually shows it arrived with a template or migration. Candidates to verify, not assume: `@fortawesome/fontawesome-free` (FA 4.7 via `font-awesome` and the FA 5 CDN are the ones in use), `webpack` (the app builds with `@angular/build`/esbuild), `ts-node`, `npm`. |
| **Blocked** | A peer range excludes it (e.g. TypeScript vs Angular's `typescript` peer) | Defer until the blocker moves. |
| **Angular-tracking** | Its major follows Angular's (`@angular-eslint/*`, `@ng-select/ng-select`, `ngx-mask`) | `npx ng update <pkg>@<angular-major>` — it runs migrations. |
| **Low** | Small API surface, or dev/test-only (`@playwright/test`, `@axe-core/playwright`, eslint plugins, `@types/*`) | Take it now. |
| **Medium** | Wide usage or visual/behavioral surface (`ag-grid-*`, `@turf/turf`, `leaflet` and its plugins, `esri-leaflet*`, `@geoman-io/leaflet-geoman-free`, `vega`/`vega-lite`/`vega-embed`, `tinymce`/`@tinymce/tinymce-angular`, `@ngneat/dialog`) | Read the upgrade guide first; hand the user a smoke list. |
| **Separate project** | Auth (`@auth0/auth0-angular`), telemetry (`@datadog/browser-logs`), the client generator (`@openapitools/openapi-generator-cli`) | Its own story with its own test pass. |

Present the table and a recommended order before upgrading anything.

## 4 · License check — after every lockfile change

```bash
node ../.claude/skills/upgrade-frontend/license-diff.mjs
```

- **`RESTRICTIVE?`** — the declared license or LICENSE text names a source-available term (Commons
  Clause, BUSL, SSPL, Elastic, PolyForm, non-commercial), the declared license binds GPL, AGPL or LGPL
  (alone, or joined by `AND`), or a `SEE LICENSE IN` file is GPL. Stop and raise it. **Known and
  accepted:** `tinymce` 7+ is "GPL-2.0-or-later or Tiny's commercial terms", used under the GPL option
  (`license_key: "gpl"` in `src/app/shared/helpers/tiny-mce-helpers.ts`). It's flagged whenever it
  changes. Re-read `node_modules/tinymce/license.md` on every bump, and on a major confirm the `"gpl"`
  key is still accepted.
- **`CHECK`** — no declared license, a non-standard spelling, or weak copyleft. Read the shipped
  LICENSE file before deciding. Known fine in this tree: legacy `BSD` strings (`arc`, `wellknown`), the
  `splaytree-ts` "BDS-3-Clause" typo, EPL/EDL `jsts` / `@turf/jsts`, MPL `axe-core` (dev), OFL+MIT font
  packages, CC-BY data (`caniuse-lite`), Artistic-2.0 `npm` (dev).
- **`CHANGED`** on an existing package — always look, even when both ends look permissive.

The npm `license` field is metadata the publisher can change without changing the terms, so a
suspicious package is confirmed from its tarball: `npm pack <pkg>@<ver>` and read `LICENSE`.

## 5 · Per-package procedure

1. **Breaking changes.** Package `CHANGELOG.md` (in `node_modules` after install) → GitHub releases
   (`gh api repos/<owner>/<repo>/releases/tags/<tag>`) → the repo's raw `CHANGELOG.md` → the vendor's
   upgrade guide (AG Grid: `ag-grid.com/angular-data-grid/upgrading-to-ag-grid-<N>/`).
2. **Our usage.** Grep imports, template selectors, `angular.json` styles/assets/scripts, SCSS
   selectors on the library's classes (`::ng-deep .ag-*`, `.leaflet-*`, `.ng-*`), and any DOM we reach
   into. Map each breaking change to a hit, or record "no hit".
3. **Install** (`npm install <pkg>@^<ver>`, or `npx ng update` for Angular-tracking packages).
4. **Verify, cheapest first:** `npm run build-dev` (TypeScript + template type-checking) → lint diff
   (`lint-diff.mjs`) → `npm run build-prod` for packages that affect bundling/optimization → the
   Playwright suites that cover the area (below).
5. **Cover what the build can't see.** With no unit tests, behavior is covered by Playwright e2e
   (`e2e-tests/`). It needs the app running: `npm start` (https://wadnr.localhost.esassoc.com:3215) plus
   the API with `EnableE2ETestAuth` enabled. Ask the user before relying on it. Useful projects:
   `npx playwright test --project=chromium <dir>` for the area's specs, `npm run e2e:visual` for
   rendering changes (grids, maps, dialogs), `npm run e2e:a11y` for component-library majors. When none
   of that covers the change, hand the user a manual smoke list.
6. **Numeric/geometry libraries** (`@turf/turf`, `leaflet-geometryutil`, `wellknown`): run the exact code
   path against old and new versions side by side (install each into a scratch dir) over fixed edge
   cases plus random inputs, and diff. If they disagree, compare both against an independent reference
   before deciding which is right.
7. **Commit** one package group per commit, after `/precommit-review`. The message records the breaking
   changes checked against our code, what changed in `src/`, the license result, and what was verified.

## 6 · Traps

| Trap | What to do |
|---|---|
| `ng update` rewrites `package.json` / `package-lock.json` with 2-space indent (a huge diff) | Both files use 4 spaces. Re-serialize: `JSON.stringify(json, null, 4) + "\n"`. |
| Bumping `@openapitools/openapi-generator-cli` doesn't change the generator | The generator version is pinned separately in `WADNR.Web/openapitools.json` (`generator-cli.version`). Changing *that* changes every generated client: run `npm run gen-model` and review the `src/app/shared/generated/` diff (and the custom templates in `Build/typescript-angular`). |
| A wrapper bundles another library's major (`@tinymce/tinymce-angular` vs `tinymce`) | Keep wrapper and core on compatible majors; list the internals our code touches (`tiny-mce-helpers.ts` init options, plugin names) and confirm each exists in the new version. |
| A new recommended lint rule flags a handful of real sites | Fix them in their own commit before the version bump; they can be real bugs. |
| Built files move into `dist/` (Leaflet plugins, font packages) | Update the `angular.json` `styles` / `assets` / `scripts` path; `ng build` fails on a missing entry. |
| Types move out of the library (`@turf/turf` 7 → `@types/geojson`) | Import the types from `geojson`. |
| A plugin option compiles but the plugin never loads (`gestureHandling: true`, `fullscreenControl: true` behind `as MapOptions`) | Grep for the plugin's import before trusting the option; report it as dead rather than "upgraded and working". |
| FontAwesome has three sources (FA 4.7 SCSS import, FA 5 CDN, `@fortawesome/fontawesome-free` 7) | FA 4.7 (`fa fa-*`, the grid filter glyph) is load-bearing; don't let a "font package" bump or removal touch it without a visual check. |

## 7 · Finish

Run the gate once, after the last upgrade:

```bash
npm ci && npm ls --all | grep -ci invalid      # clean install, 0 invalid entries
npm run build-dev
npm run build-prod                             # production build
npx ng lint --format json --output-file lint-report.json; mv lint-report.json <scratch>/lint-after.json
node ../.claude/skills/upgrade-frontend/lint-diff.mjs <scratch>/lint-before.json <scratch>/lint-after.json
node ../.claude/skills/upgrade-frontend/license-diff.mjs --all
```

…plus the Playwright suites from §5 if an app is running. When the user asks for a PR
(`gh pr create`), include: the upgrades table (package, from → to, license, what changed in our code),
anything removed, anything deferred and why, and a manual smoke list for the medium-risk upgrades the
build can't see (grids, maps, rich-text editor, charts, dialogs).
