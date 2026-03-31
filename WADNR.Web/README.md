# WADNR.Web

Angular frontend for the WA DNR Forest Health Tracker.

## Development server

Run `npm start` for a dev server at `https://wadnr.localhost.esassoc.com:3215`. The app will automatically reload on source file changes.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum`.

## Build

Run `npm run build` to build the project. The build artifacts will be stored in the `dist/` directory. Use `npm run build-prod` for a production build.

## Running unit tests

Run `npm test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

E2E tests use [Playwright](https://playwright.dev/) and are organized into three projects:

### Functional tests

```bash
npm run e2e                    # run all functional tests (headless)
npm run e2e:headed             # run with browser visible
npm run e2e:ui                 # run with Playwright UI mode
```

~203 tests covering page rendering, navigation, modals, grids, workflows, authorization, and error handling.

### Legacy comparison tests

```bash
npm run e2e:comparison
```

Compares 62 Angular pages against their legacy MVC equivalents. Checks visual layout, card titles, grid columns, row counts, link text, and content parity. Requires the `LEGACY_BASE_URL` environment variable pointing to the legacy site.

### Visual regression tests

A two-step workflow for catching unintended UI changes:

```bash
npm run e2e:visual:update      # 1. Generate/update baseline screenshots
npm run e2e:visual             # 2. Compare current UI against baselines
```

95 screenshot tests across 7 spec files (public pages, authenticated pages, detail pages, modals, validation states, responsive viewports, workflow steps).

Baseline screenshots are **gitignored** — they are environment-specific (font rendering differs across OS/machine). Each developer generates their own baselines with `e2e:visual:update`.

### Useful flags

```bash
npx playwright test -g "test name"          # run tests matching a pattern
npx playwright test --project=chromium      # run a specific project
npm run e2e:report                          # open the HTML report
```

### Test structure

```
e2e-tests/
├── admin-pages/          # Admin list page tests
├── auth-pages/           # Authenticated page rendering
├── authorization/        # Role-based access & guard redirects
├── comparison/           # Legacy MVC comparison engine
├── detail-pages/         # Entity detail page tests
├── error-handling/       # 404 and error pages
├── financial-pages/      # Agreement/invoice cross-links
├── fixtures/             # Shared auth, test data, helpers
├── grids/                # Grid features and navigation
├── homepage/             # Homepage features
├── maps/                 # Map rendering tests
├── modals/               # CRUD modal tests
├── navigation/           # Nav menus, breadcrumbs, dropdowns
├── program-info/         # Program info cross-links
├── project-detail/       # Project detail cards, TOC, modals
├── public-pages/         # Public (unauthenticated) pages
├── reporters/            # Custom Playwright reporters
├── search/               # Project search tests
├── visual-regression/    # Screenshot comparison tests
└── workflows/            # Multi-step workflow tests
```

### Environment variables

| Variable | Required for | Description |
|----------|-------------|-------------|
| `LEGACY_BASE_URL` | comparison | URL of the legacy MVC site |
| `LEGACY_USERNAME` | comparison | Legacy site login username |
| `LEGACY_PASSWORD` | comparison | Legacy site login password |

## Further help

To get more help on the Angular CLI use `ng help` or check the [Angular CLI documentation](https://angular.dev/tools/cli).
