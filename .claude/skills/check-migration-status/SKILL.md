# Check Migration Status Skill

> **Scope**: fullstack
> **Prereqs**: None

When the user invokes `/check-migration-status`:

## Overview

This skill audits the full migration progress from legacy MVC to Angular + ASP.NET Core API. It covers page/controller migration, background jobs, and infrastructure components.

## Steps to Execute

### 1. Inventory Legacy Controllers

List all controllers in the legacy MVC project:
```
{LegacyPath}/Controllers/
```

For each controller, gather:
- Controller name (e.g., `ProjectController.cs`)
- Number of action methods (complexity indicator)
- Number of views in `{LegacyPath}/Views/{Entity}/`

### 2. Inventory New API Controllers

List all controllers in the new API project:
```
{ApiProject}/Controllers/
```

Note which entity each controller handles.

### 3. Inventory Angular Pages

List all page components in:
```
{FrontendProject}/src/app/pages/
```

Check `app.routes.ts` for registered routes.

### 4. Compare and Classify

Create a migration status for each entity using these categories:

| Status | Meaning |
|--------|---------|
| **Migrated** | Has API controller + Angular page/route (or is consumed via modal on parent page) |
| **API Only** | Supporting lookup/reference endpoint — no standalone page needed |
| **Consolidated** | Legacy controller merged into another new controller |
| **Partial** | Has API controller but Angular UI work genuinely incomplete |
| **Not Started** | Only exists in legacy MVC |
| **New Only** | Only exists in new stack (no legacy equivalent) |

#### Known Consolidations

These legacy controllers have been merged into a parent controller in the new API. Mark them as **Consolidated**, not "Not Started":

| Legacy Controller | Consolidated Into |
|-------------------|-------------------|
| `ProjectLocationController` | `ProjectController` |
| `ProjectRegionController` | `ProjectController` |
| `ProjectCountyController` | `ProjectController` |
| `ProjectClassificationController` | `ProjectController` |
| `ProjectExternalLinkController` | `ProjectController` |
| `ProjectOrganizationController` | `ProjectController` |
| `ProjectPersonController` | `ProjectController` |
| `ProjectPriorityLandscapeController` | `ProjectController` |
| `ProjectStewardOrganizationController` | `ProjectController` |

#### Known Sub-Component APIs

These controllers serve modals, dropdowns, or inline components on parent pages — they do not need standalone Angular pages. Mark them as **API Only**, not "Partial":

- `FederalFundCodeController`
- `FundSourceTypeController`
- `FundSourceAllocationNoteController`
- `FundSourceAllocationNoteInternalController`
- `FundSourceAllocationPriorityController`
- `InvoicePaymentRequestController`

### 5. Background Jobs Audit

Check both legacy and new scheduled job locations:
- Legacy: `{LegacyPath}/ScheduledJobs/` and search for Hangfire job registrations
- New: `{ApiProject}/Hangfire/` or search for `RecurringJob` registrations in the new API

Report which jobs are migrated, pending, or new:

| Job | Schedule | Legacy | New | Status |
|-----|----------|--------|-----|--------|
| Expenditure Import | Every 15 min | `{LegacyPath}/ScheduledJobs/` | — | **Not migrated** |
| Vendor Import | Every 15 min | `{LegacyPath}/ScheduledJobs/` | — | **Not migrated** |
| Project Code Import | Every 15 min | `{LegacyPath}/ScheduledJobs/` | — | **Not migrated** |
| Program Index Import | Every 15 min | `{LegacyPath}/ScheduledJobs/` | — | **Not migrated** |
| LOA Data Import | Daily | `{LegacyPath}/ScheduledJobs/` | — | **Not migrated** |
| USFS Data Import | Daily (2 jobs) | `{LegacyPath}/ScheduledJobs/` | — | **Not migrated** |
| Program Notifications | Daily | `{LegacyPath}/ScheduledJobs/` | — | **Not migrated** |
| Blob File Transfer | N/A | — | `{ApiProject}/Hangfire/` | **New** |

Verify the above by scanning the actual source directories. Report any additional jobs found that are not in this list.

### 6. Infrastructure Audit

Report the migration status of cross-cutting infrastructure concerns:

| Component | Legacy Implementation | New Implementation | Status |
|-----------|----------------------|-------------------|--------|
| Email Service | SitkaSmtpClient | SendGrid (`WADNR.Common`) | Migrated |
| Project Notifications | Custom notification system | ProjectNotificationService | Migrated |
| Program Notifications | Scheduled job | Not yet implemented | **Not migrated** |
| Report Generation | SharpDocx templates | SharpDocx (`{ApiProject}`) | Migrated |
| PDF/Screenshot | N/A | SitkaCaptureService | New |
| Authentication | Keystone OIDC | Auth0 JWT | Migrated |
| File Storage | Local filesystem | Azure Blob Storage | Migrated |
| GeoServer/Maps | DB Views + JavaScript | DB Views + Leaflet/Angular | Migrated |

Verify the above by checking for the existence of key files:
- SendGrid: search for `SendGrid` in `WADNR.Common/`
- Auth0: search for `Auth0` in `{ApiProject}/`
- Blob Storage: search for `BlobStorage` or `Azure.Storage` in the solution
- SharpDocx: search for `SharpDocx` in `{ApiProject}/`
- Notifications: search for `ProjectNotification` in `{ApiProject}/`

### 7. Generate Report

Output a markdown report covering all three audit areas.

### 8. Priority Assessment

For unmigrated entities, suggest priority based on:
- **High**: Core business entities (Project, Agreement, Organization)
- **Medium**: Supporting entities with moderate complexity
- **Low**: Simple lookup/reference entities

Also consider:
- Dependencies on other entities (migrate dependencies first)
- User impact (frequently used pages = higher priority)
- Complexity (simpler entities = quick wins)

### 9. Recommendations

After all status tables, provide:
1. **Quick wins**: Simple entities that can be migrated quickly
2. **Dependencies**: Entities that should be migrated in a specific order
3. **Complex migrations**: Entities requiring significant effort
4. **Background jobs**: Priority order for job migration
5. **Infrastructure gaps**: Any remaining infrastructure items to address

## Output Format

```markdown
# Migration Status Report

Generated: {current date}

## Summary
- Total Legacy Controllers: X
- Migrated: Y (includes entities consumed via modals on parent pages)
- API Only: A (supporting endpoints, no page needed)
- Consolidated: B (merged into parent controllers)
- Partial: Z (API exists, Angular UI incomplete)
- Not Started: W
- New Only: N

## Page/Controller Migration Status

| Entity | Legacy | Views | API | Angular | Status | Notes |
|--------|--------|-------|-----|---------|--------|-------|
| Project | Yes | 12 | Yes | Yes | Migrated | — |
| ProjectLocation | Yes | 3 | — | — | Consolidated | Merged into ProjectController |
| FederalFundCode | No | — | Yes | — | API Only | Used by fund source modals |
| ... | ... | ... | ... | ... | ... | ... |

## Background Jobs Status

| Job | Schedule | Legacy | New | Status |
|-----|----------|--------|-----|--------|
| Expenditure Import | Every 15 min | Yes | No | Not migrated |
| Blob File Transfer | N/A | No | Yes | New |
| ... | ... | ... | ... | ... |

**Jobs Migrated: X / Y total**

## Infrastructure Status

| Component | Legacy | New | Status |
|-----------|--------|-----|--------|
| Email Service | SitkaSmtpClient | SendGrid | Migrated |
| Program Notifications | Scheduled job | — | Not migrated |
| ... | ... | ... | ... |

**Infrastructure Migrated: X / Y total**

## Recommendations

### Quick Wins (Low Complexity, High Value)
1. {EntityName} - {reason}

### Dependencies to Address First
1. {EntityName} should be migrated before {OtherEntity}

### Complex Migrations (Plan Carefully)
1. {EntityName} - {complexity factors}

### Background Job Migration Priority
1. {JobName} - {reason}

### Remaining Infrastructure Gaps
1. {Component} - {what's needed}
```

## Notes

- Ignore controllers that are infrastructure-related (Home, Account, Error, etc.)
- Focus on domain entity controllers
- Check for deprecated markers on legacy controllers (already migrated)
- Consider shared components that may need to be created first
- Use the Known Consolidations and Known Sub-Component APIs lists above to avoid misclassifying entities
- When in doubt about whether a controller is consolidated, check if the parent new controller has endpoints that cover the legacy controller's functionality

---

## Cross-References

| After the audit... | Load |
|---------------------|------|
| Migrating a page | `/migrate-page` |
| Migrating a grid | `/migrate-grid` |
| Migrating a workflow | `/migrate-workflow` |
