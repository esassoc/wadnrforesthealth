---
name: audit-a11y
description: Run WCAG 2.2 AA accessibility audit via Playwright axe-core runtime scans and/or ESLint template static analysis, then report findings with prioritized fix recommendations.
allowed-tools: [Bash(npx playwright:*), Bash(npm run:*), Bash(cd:*), Read, Grep, Glob]
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

Run the accessibility Playwright test suite:

```bash
cd WADNR.Web && npx playwright test --project=accessibility --reporter=json 2>&1
```

If the command fails (dev server not running, etc.), report the error and suggest `npm start`.

**Parse the JSON output** to extract per-page violations. For each page:
- Page name
- Number of violations
- Violations grouped by impact: critical, serious, moderate, minor
- For each violation: rule ID, help text, WCAG tags, number of affected elements

### 3. Static scan (ESLint template rules)

Run ESLint on HTML templates:

```bash
cd WADNR.Web && npx ng lint --format json 2>&1
```

**Parse the JSON output** to extract template accessibility warnings. Filter to only `@angular-eslint/template/*` rules. For each file with violations:
- File path (relative to WADNR.Web)
- Rule ID
- Message
- Line number

### 4. Present combined results

#### Runtime violations table (if run)

```
| Page | Critical | Serious | Moderate | Minor | Total |
|------|----------|---------|----------|-------|-------|
| Home | 0 | 2 | 1 | 0 | 3 |
| ... |
```

#### Top offending axe rules (if run)

```
| Rule ID | Impact | Description | Pages Affected |
|---------|--------|-------------|----------------|
| color-contrast | serious | Elements must meet color contrast | 12 |
| ... |
```

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

No cleanup needed — Playwright reports are stored in the standard `playwright-report/` directory.

## Notes

- The runtime scan uses `@axe-core/playwright` which includes all WCAG 2.2 AA rules for free (no API key needed)
- Tests are in **report-only mode** — violations are logged but do not fail the tests
- Route registry: `WADNR.Web/e2e-tests/accessibility/a11y-routes.ts` — add new routes here as pages are migrated
- Playwright config: `WADNR.Web/playwright.config.ts` — accessibility project
- ESLint config: `WADNR.Web/.eslintrc.js` — template accessibility rules in the HTML override
