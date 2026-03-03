# Pre-Commit Review Skill

When the user invokes `/precommit-review`:

Review all uncommitted changes like a thorough PR reviewer before committing. Optionally pass `--card JIRA-123` to cross-reference a Jira ticket.

## Arguments

- `--card JIRA-123` (optional) — Fetch the Jira ticket and verify changes align with acceptance criteria

---

## Step 1: Gather Changes

Run `git diff` and `git diff --cached` to collect all staged and unstaged changes. Also run `git status` to identify untracked files.

If there are untracked files, read them to include in the review.

---

## Step 2: (Optional) Fetch Jira Context

If `--card` was provided:
1. Call `mcp__atlassian__getAccessibleAtlassianResources` to get the cloud ID
2. Call `mcp__atlassian__getJiraIssue` with the card key to fetch the ticket details
3. Note the acceptance criteria, description, and scope for cross-referencing

---

## Step 3: Review Checklist

Evaluate every changed file against the following checklist. Report findings grouped by severity: **Blockers**, **Warnings**, **Suggestions**.

### Code Quality
- [ ] No TODO/FIXME/HACK comments introduced without a tracking ticket
- [ ] No commented-out code left behind
- [ ] No console.log / Debug.WriteLine / print statements left in
- [ ] No hardcoded secrets, connection strings, or API keys
- [ ] Variable and method names are clear and follow existing conventions

### Security
- [ ] No SQL injection vulnerabilities (raw string concatenation in queries)
- [ ] No XSS vulnerabilities (unsanitized user input rendered in templates)
- [ ] No command injection (unsanitized input passed to shell commands)
- [ ] Endpoints check authorization attributes where appropriate
- [ ] Sensitive data is not logged or exposed in error messages

### Backend ({ApiProject})
- [ ] Controllers extend `{BaseController}` (or appropriate base class)
- [ ] New endpoints have appropriate authorization attributes
- [ ] Queries use `.AsNoTracking().Select()` projections (not `.Include().AsDto()`)
- [ ] DTOs are in `{ModelsProject}/DataTransferObjects/` (not in API or EFModels)
- [ ] Static helpers follow the established pattern in `{EFModelsProject}/Entities/`
- [ ] No N+1 query patterns (loading related data in loops)
- [ ] Async methods are properly awaited

### Frontend ({FrontendProject})
- [ ] Components are standalone with explicit imports
- [ ] Route params use `@Input()` + `BehaviorSubject` pattern
- [ ] No Bootstrap classes used (project uses its own styling framework)
- [ ] Services are properly injected; no manual `new` of services
- [ ] Subscriptions are properly cleaned up (takeUntilDestroyed, async pipe, etc.)
- [ ] Generated files in `shared/generated/` are not manually edited

### Database
- [ ] Migration scripts follow the `DatabaseMigration` idempotent pattern
- [ ] No destructive changes without rollback consideration
- [ ] Foreign keys and indexes are appropriate

### Project Patterns
- [ ] Review CLAUDE.md for project-specific patterns to check against
- [ ] Changes are consistent with patterns in `.claude/docs/api-patterns.md` and `.claude/docs/angular-patterns.md`
- [ ] File placement follows the project's directory conventions

### Tests
- [ ] New functionality has corresponding tests (or justification for skipping)
- [ ] Existing tests are not broken by the changes
- [ ] Test assertions are meaningful (not just "doesn't throw")

---

## Step 4: Build Verification

Run the following to verify the changes compile:

```powershell
# Backend build
dotnet build LtInfo.sln

# Frontend build
cd {FrontendProject}
npx ng build --configuration development
```

Report any build errors as **Blockers**.

---

## Step 5: If Jira Card Provided

Compare the changes against the Jira ticket:
- [ ] All acceptance criteria are addressed
- [ ] No scope creep (changes unrelated to the ticket)
- [ ] Edge cases from the ticket description are handled

---

## Step 6: Output Summary

Format the review as:

```
## Pre-Commit Review Summary

### Files Changed
- list of files with brief description of change

### Blockers (must fix before commit)
- ...

### Warnings (should fix, but not blocking)
- ...

### Suggestions (nice to have)
- ...

### Jira Alignment (if --card provided)
- ...

### Verdict: READY TO COMMIT | NEEDS CHANGES
```

If verdict is **READY TO COMMIT**, suggest a commit message following the repository's commit message conventions (check recent `git log` for style).

If verdict is **NEEDS CHANGES**, list the specific items that need to be addressed before committing.
