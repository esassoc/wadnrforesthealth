---
name: audit-a11y
description: Run WCAG 2.2 AA accessibility audit via Playwright axe-core runtime scans and/or ESLint template static analysis, then report findings with scores, grades, and prioritized fix recommendations.
allowed-tools: [Bash(npx playwright:*), Bash(npm run:*), Bash(cd:*), Bash(node:*), Bash(rm:*), Bash(ESLINT_USE_FLAT_CONFIG:*), Read, Grep, Glob]
---

Run a WCAG 2.2 AA accessibility audit against the Angular frontend. Combines two layers:

- **Runtime scan**: Playwright + axe-core scans every page in the running app
- **Static scan**: ESLint template accessibility rules catch common pitfalls in HTML templates

## Arguments

- `$ARGUMENTS` — Optional flags:
  - `--lint-only` — Run only the static ESLint scan (no Playwright / no running app needed)
  - `--runtime-only` — Run only the Playwright axe-core scan (requires dev server running)
  - No flags — Run both scans

### Examples

```
/audit-a11y
/audit-a11y --lint-only
/audit-a11y --runtime-only
```

## Prerequisites

- **Runtime scan**: The dev server must be running (`npm start` in WADNR.Web)
- **Static scan**: No prerequisites beyond `npm install`

## Steps

### 1. Parse arguments

Split `$ARGUMENTS` to determine mode:
- If `--lint-only` → skip to Step 3
- If `--runtime-only` → skip to Step 2, then skip Step 3
- Otherwise → run both

### 2. Runtime scan (Playwright + axe-core)

**Important**: Do NOT use `--reporter=json` to stdout — the output is megabytes of base64-encoded data that will overflow. Instead, write JSON to a file and use the helper script.

Run the Playwright tests, writing JSON to a file:

```bash
cd WADNR.Web && PLAYWRIGHT_JSON_OUTPUT_FILE=a11y-results.json npx playwright test --project=accessibility --reporter=json,list 2>&1
```

- `PLAYWRIGHT_JSON_OUTPUT_FILE` directs the JSON reporter to write to a file instead of stdout
- `--reporter=json,list` uses both reporters: JSON to file (for the helper script), list to stdout (one line per test for progress)
- The `list` reporter output is small and fits in the tool output buffer

If the command fails (dev server not running, etc.), report the error and suggest `npm start`.

Then run the report helper to parse results and compute scores:

```bash
cd WADNR.Web && node e2e-tests/accessibility/a11y-report.mjs --json-file a11y-results.json
```

This prints a compact summary (~80 lines) with:
- Overall score and letter grade (A/B/C/D/F)
- Grade distribution across all pages
- Violations grouped by impact severity
- Top 10 rules by weighted impact
- Per-page scores (worst 20)
- Clean pages list

For per-page violation details, add `--verbose`:

```bash
cd WADNR.Web && node e2e-tests/accessibility/a11y-report.mjs --json-file a11y-results.json --verbose
```

### 3. Static scan (ESLint template rules)

Run ESLint on HTML templates using the dedicated a11y config (avoids TypeScript parser conflicts with the main `.eslintrc.js`):

```bash
cd WADNR.Web && ESLINT_USE_FLAT_CONFIG=false npx eslint --no-eslintrc --config .eslintrc.a11y.js --format json "src/**/*.html" 2>&1
```

**Parse the JSON output** to extract template accessibility warnings. Filter to only `@angular-eslint/template/*` rules. For each file with violations:
- File path (relative to WADNR.Web)
- Rule ID
- Message
- Line number

### 4. Present combined results

#### Scoring summary (from runtime scan)

Present the overall score and letter grade prominently at the top of the report. Include:
- **Overall score**: X / 100 (Grade)
- **Grade distribution**: How many pages got A, B, C, D, F
- **Trend**: If you know the previous score, show the delta

#### Runtime violations summary (from helper script output)

The helper script already formats the violations — present its output directly. Key sections:
- Violations by impact
- Top rules by weighted impact
- Per-page scores (worst first)
- Clean pages

#### Static lint violations table (if run)

```
| Rule | Count | Files Affected | Example |
|------|-------|----------------|---------|
| click-events-have-key-events | 16 | 6 | photos-step.component.html:42 |
| ... |
```

### 5. Prioritized action plan

Combine all findings and create a prioritized fix list:

**a. Categorize by priority:**

| Priority | Applies to |
|----------|-----------|
| **Critical** | axe critical/serious violations, missing keyboard support on interactive elements |
| **High** | Missing form labels, missing alt text, invalid ARIA |
| **Medium** | Color contrast, missing focus indicators |
| **Low** | Minor axe violations, informational lint warnings |

**b. Output an action plan table** sorted by priority, then by number of affected pages/elements:

```
| # | Priority | Issue | Impact | Affected | Suggested Fix |
|---|----------|-------|--------|----------|---------------|
| 1 | Critical | click-events-have-key-events | 16 elements | Convert <span (click)> to <button> in photos, documents, image-gallery |
| ... |
```

**c. Summary:**
- Total violations by layer (runtime vs static)
- Total violations by priority
- **Recommended next batch**: Top 5 fixes that address the most violations with minimum effort
- Pages with zero violations (celebrate the wins)

### 6. Clean up

Remove the temporary JSON file:

```bash
rm -f WADNR.Web/a11y-results.json
```

## Scoring Formula

The helper script (`e2e-tests/accessibility/a11y-report.mjs`) uses the **Deque axe Monitor scoring method**.

**Per-page score**: Each page is classified by its **worst** (highest-severity) violation:

| Worst violation | Page score |
|----------------|-----------|
| Critical | 0% |
| Serious | 40% |
| Moderate | 80% |
| Minor or none | 100% |

**Overall score**: Arithmetic mean of all per-page scores.

**Letter grades** (standard academic scale): A (90-100), B (80-89), C (70-79), D (60-69), F (<60)

Source: [Deque axe DevTools Mobile - Score](https://docs.deque.com/devtools-mobile/2024.9.18/en/score/)

## Modal Scanning

The runtime scan also tests **modal dialogs** opened from page-level trigger buttons. Modal scans use `checkModalA11y()` which scopes the axe-core scan to the `.ngneat-dialog-content` container, avoiding double-counting page-level violations.

### How modal routes work

Modal routes are defined in `a11y-routes.ts` using the `A11yModalRoute` interface:

```typescript
export interface A11yModalRoute {
    name: string;           // Display name (prefix with "Modal: ")
    pagePath: string;       // URL to navigate to first
    pageWaitFor: string;    // Selector to wait for before clicking trigger
    triggerSelector: string; // Selector to click to open the modal
    auth: AuthLevel;        // Auth level required
    timeout?: number;       // Optional timeout override
}
```

The test pattern for each modal:
1. Navigate to `pagePath` and wait for `pageWaitFor`
2. Click `triggerSelector` to open the modal
3. Wait for `.ngneat-dialog-content` to appear
4. Run axe-core scan scoped to the modal container
5. Results are attached with `axe-results-Modal: {name}` naming

### Adding new modal routes

Add entries to the appropriate array in `a11y-routes.ts`:
- `adminModalRoutes` — modals on admin pages
- `elevatedModalRoutes` — modals on elevated-access pages
- `authedModalRoutes` — modals on authenticated pages
- `publicPageModalRoutes` — modals on public pages (still need admin auth for the trigger buttons)

Common trigger selectors:
- `button[title="Edit Basics"]` — card header edit links
- `button.btn-primary` — primary action buttons (e.g., "Create New")
- `button.btn-primary >> nth=0` — when multiple primary buttons exist, use nth to target a specific one

### Spec file

Modal tests live in `WADNR.Web/e2e-tests/accessibility/modal-pages.a11y.spec.ts`, separate from page-level tests.

## Notes

- The runtime scan uses `@axe-core/playwright` which includes all WCAG 2.2 AA rules for free (no API key needed)
- Tests are in **report-only mode** — violations are logged but do not fail the tests
- Route registry: `WADNR.Web/e2e-tests/accessibility/a11y-routes.ts` — add new routes here as pages are migrated
- Modal tests: `WADNR.Web/e2e-tests/accessibility/modal-pages.a11y.spec.ts` — scans modals opened by trigger buttons
- Report helper: `WADNR.Web/e2e-tests/accessibility/a11y-report.mjs` — parses JSON, computes scores, prints summary
- Playwright config: `WADNR.Web/playwright.config.ts` — accessibility project
- ESLint a11y config: `WADNR.Web/.eslintrc.a11y.js` — standalone HTML-only config for template accessibility rules (separate from main `.eslintrc.js` to avoid TypeScript parser conflicts)
- The `PLAYWRIGHT_JSON_OUTPUT_FILE` env var tells Playwright's JSON reporter to write to a file instead of stdout
