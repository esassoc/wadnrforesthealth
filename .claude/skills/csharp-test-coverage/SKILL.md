---
name: csharp-test-coverage
description: Run C# tests with code coverage and report per-class/method line and branch coverage with uncovered line numbers. Accepts an optional test filter and optional class filter.
allowed-tools: [Bash(dotnet test:*), Bash(reportgenerator:*), Bash(node:*), Bash(rm:*), Read]
---

Run C# tests with coverlet code coverage collection and produce a per-class coverage summary **including uncovered line numbers** for the classes you care about.

## Arguments

- `$ARGUMENTS` — An optional string with two parts (both optional):
  - **Test filter**: a `dotnet test --filter` expression (e.g. `FullyQualifiedName~ProjectControllerTests`)
  - **Class filter**: prefixed with `--classes` followed by comma-separated partial class names to include in the report (e.g. `--classes Projects,ProjectController`)

  If no test filter is provided, all tests are run.
  If no class filter is provided, all classes with >0% coverage are shown.

### Examples

```
/csharp-test-coverage FullyQualifiedName~ProjectControllerHttpTests --classes Projects,ProjectController
/csharp-test-coverage --classes Agreements,AgreementController
/csharp-test-coverage FullyQualifiedName~AuthorizationTests
/csharp-test-coverage
```

## Steps

1. **Parse arguments** from `$ARGUMENTS`:
   - Split on `--classes` to extract the test filter (before) and class name filters (after, comma-separated).
   - If no `--classes` flag, treat the entire argument as the test filter.

2. **Run tests with coverlet** to collect coverage in cobertura format:
   ```
   dotnet test WADNR.API.Tests/WADNR.API.Tests.csproj [--filter "<test-filter>"] -p:CollectCoverage=true -p:CoverletOutputFormat=cobertura -p:CoverletOutput=./coverage-results/ --no-restore
   ```
   - If a test filter was provided, include `--filter "<test-filter>"`.
   - If tests fail, report the failure and stop.

3. **Extract per-class coverage with uncovered lines** using the helper script:
   ```
   node .claude/skills/csharp-test-coverage/parse-coverage.js WADNR.API.Tests/coverage-results/coverage.cobertura.xml [classFilter1,classFilter2,...]
   ```
   - If class name filters were provided, pass them as a comma-separated second argument.
   - The script outputs one JSON object per line with: `class`, `file`, `lineCoverage`, `branchCoverage`, `uncoveredLines`.
   - The `uncoveredLines` field contains line ranges (e.g. `"18-20, 34-36"`) referring to the source file.

4. **Present results** as a markdown table with columns: Class, File, Line%, Branch%, Uncovered Lines.

5. **Build a prioritized action plan** toward 100% coverage. For every class with < 100% line or branch coverage:

   **a. Read uncovered lines** — Use the Read tool to read the specific uncovered line ranges from each source file.

   **b. Categorize each gap by difficulty:**
   | Difficulty | Criteria |
   |------------|----------|
   | **Quick win** | ≤ 5 uncovered lines, simple null checks / early returns / trivial branches, no new test infrastructure needed |
   | **Standard** | 6–20 uncovered lines, CRUD paths / validation logic / moderate branches, needs a new test method but no new mocks or fixtures |
   | **Complex** | > 20 uncovered lines, multi-step flows / external-service interactions / spatial-geometry code / authorization edge cases, needs new mocks or test infrastructure |

   **c. Assess priority based on what the code does:**
   | Priority | Applies to |
   |----------|-----------|
   | **High** | Core business logic, security/authorization paths, data mutation (create/update/delete) |
   | **Medium** | Read paths, lookup/reference data, grid/list projections |
   | **Low** | Logging, defensive null guards on already-validated internal data, infrastructure plumbing — still list these but note *why* they are low priority |

   **d. Output an action-plan table** sorted by priority (High first), then difficulty (Quick win first within same priority):

   ```
   | # | Class | Gap Description | Lines | Priority | Difficulty | Suggested Test |
   |---|-------|----------------|-------|----------|------------|----------------|
   | 1 | ProjectController | Delete — missing 403 and 404 branches | 82-95 | High | Quick win | Add Delete_Returns403_WhenUnauthorized and _Returns404 |
   | 2 | Agreements | CreateAsync — null FundSource branch | 44-51 | High | Standard | Seed Agreement without FundSource, assert validation error |
   ```

   Every uncovered line range MUST appear in the table — never silently skip gaps.

   **e. Summarize at the bottom:**
   - Total gaps by priority × difficulty (e.g. "3 High/Quick-win, 2 High/Standard, 1 Medium/Complex")
   - **Recommended next batch**: pick the top items (up to ~5) that maximize coverage gain with minimum effort — label these as "do next"
   - Remaining coverage gap as a percentage: what line% and branch% would reaching if the "do next" batch is completed

6. **Clean up** temporary coverage files:
   ```
   rm -rf WADNR.API.Tests/coverage-results/
   ```

## Notes

- The test project uses `coverlet.msbuild` (a dependency in `WADNR.API.Tests.csproj`).
- `reportgenerator` is installed as a global dotnet tool.
- Coverage files are written to `WADNR.API.Tests/coverage-results/` and cleaned up after reporting.
- The `--no-restore` flag speeds up the test run by skipping NuGet restore.
- The helper script `parse-coverage.js` lives alongside this SKILL.md and parses cobertura XML using Node.js.
