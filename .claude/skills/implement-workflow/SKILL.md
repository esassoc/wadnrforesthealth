# Implement Workflow Skill

> **Scope**: fullstack
> **Prereqs**: Load `/dotnet-patterns` and `/angular-patterns` first
> **Cross-references**: `/migrate-map` (map steps), `/migrate-grid` (grid steps), `/crud-modal` (CRUD modals)

When the user invokes `/implement-workflow`:

This skill builds complete **Create** and **Update** workflow systems from scratch. It covers database tables, stored procedures, API backend (progress, steps, diffs, revert, state transitions), and Angular frontend (outlet, nav, step components, diff modal).

---

## Step 1: Choose Mode

Ask the user:

> **Clone** the Project workflow for a new solution (same steps, same tables), or build a **Custom** workflow for a different entity?

### Mode A: Clone Project Workflow

For solutions with the **same architecture** that want the same 13-step workflow replicated. Claude reads each WADNR source file and replicates it, substituting solution-level identifiers.

**Gather from user:**
- Target namespace prefix (replaces `WADNR`)
- Target DbContext name (replaces `WADNRDbContext`)
- Target route prefix (replaces `/projects`)
- Target Angular selector prefix (replaces `project`)

**Substitution table:**

| Find | Replace With |
|------|-------------|
| `WADNR.API` | `{ApiProject}` |
| `WADNR.EFModels` | `{EFModelsProject}` |
| `WADNR.Models` | `{ModelsProject}` |
| `WADNR.Web` | `{FrontendProject}` |
| `WADNRDbContext` | `{DbContext}` |
| `SitkaController` | `{BaseController}` |
| `wadnr-grid` | `{GridComponent}` |
| `wadnr-map` | `{MapComponent}` |
| Route paths (`/projects`) | Target route paths |
| Angular selector prefixes | Target prefix |

Then proceed to the **Execution Sequence** below.

**Note**: In the WADNR Project workflow, the Create and Update workflows have slightly different step rosters (Create has Classifications; Update has External Links instead). When cloning, replicate both step lists exactly as they appear in the source outlets.

### Mode B: Custom Entity Workflow

For a different entity (e.g., Agreement, Grant) with different steps and fields.

**Gather from user:**

1. **Entity name** (singular PascalCase): e.g., `Agreement`
2. **Entity plural**: e.g., `Agreements`
3. **Entity kebab**: e.g., `agreement`
4. **Step definitions** — for each step:
   - Step name (PascalCase): e.g., `Basics`, `Parties`, `FundingSources`
   - Display name: e.g., "Agreement Basics", "Parties", "Funding Sources"
   - Route segment (kebab): e.g., `basics`, `parties`, `funding-sources`
   - Step type: `form` | `collection` | `geographic` | `map` | `review`
   - Required for submission: yes/no
   - Fields (for form steps) or collection entity (for collection steps)
5. **Step groups** — organize steps into sidebar groups with titles
6. **Auth attributes** — which controller attributes (from `/dotnet-patterns`)
7. **Whether to include Update workflow** or Create only
8. **Step roster differences** — Create and Update may have different steps. Some steps may exist only in Create (e.g., a one-time classification step), and Update may add steps not in Create (e.g., external links). Confirm with the user which steps belong to which workflow.

**Template variables:**

| Variable | Example | Description |
|----------|---------|-------------|
| `{Entity}` | `Agreement` | PascalCase singular |
| `{Entities}` | `Agreements` | PascalCase plural |
| `{entity}` | `agreement` | camelCase singular |
| `{entities}` | `agreements` | camelCase plural |
| `{entity-kebab}` | `agreement` | kebab-case singular |
| `{entities-kebab}` | `agreements` | kebab-case plural |
| `{StepName}` | `Basics` | PascalCase step name |
| `{step-route}` | `basics` | kebab-case step route |

Then proceed to the **Execution Sequence** below.

---

## Step 2: Execution Sequence

Execute phases in order. **API must be built before Angular phases** because `npm run gen-model` depends on `swagger.json`.

### Phase 1: Shared Angular Infrastructure

Read `01-shared-angular-infrastructure.md`. Check if shared components already exist. If ALL exist, skip this phase. Otherwise, create the missing ones.

### Phase 2: Database

Read `02-database.md`. Create all database artifacts (tables, stored procedure, lookup scripts, indexes). Run:

```powershell
cd Build
.\DatabaseBuild.ps1
.\Scaffold.ps1
```

### Phase 3: Create Workflow — API

Read `03-create-api.md`. Create:
- `{EFModelsProject}/Workflows/{Entity}CreateWorkflowProgress.cs`
- `{EFModelsProject}/Entities/{Entity}CreateWorkflowSteps.cs`
- Per-step DTOs in `{ModelsProject}/DataTransferObjects/{Entity}/Workflow/`
- Controller endpoints in `{ApiProject}/Controllers/{Entity}Controller.cs`

### Phase 4: Build API + Generate TypeScript

```powershell
dotnet build WADNR.sln
cd WADNR.Web
npm run gen-model
```

### Phase 5: Create Workflow — Angular

Read `04-create-angular.md`. Create:
- Outlet component in `pages/{entities-kebab}/{entity-kebab}-create-workflow/`
- Per-step components in `pages/{entities-kebab}/{entity-kebab}-create-workflow/steps/`
- Route entries in `app.routes.ts`

### Phase 6: Update Workflow — API (if requested)

Read `05-update-api.md`. Create:
- `{EFModelsProject}/Workflows/{Entity}UpdateWorkflowProgress.cs`
- `{EFModelsProject}/Entities/{Entity}UpdateWorkflowSteps.cs`
- `{EFModelsProject}/Entities/{Entity}UpdateDiffs.cs`
- Per-step Update DTOs in `{ModelsProject}/DataTransferObjects/{Entity}Update/`
- Controller endpoints (`#region Update Workflow`)

### Phase 7: Build API + Generate TypeScript (again)

```powershell
dotnet build WADNR.sln
cd WADNR.Web
npm run gen-model
```

### Phase 8: Update Workflow — Angular (if requested)

Read `06-update-angular.md`. Create:
- Update outlet component in `pages/{entities-kebab}/{entity-kebab}-update-workflow/`
- Per-step Update components
- ReturnWithCommentsModal
- Route entries in `app.routes.ts`

### Phase 9: Verify

```powershell
dotnet build WADNR.sln
cd WADNR.Web
npm run build
```

---

## Clone Mode: Source File Manifest

When in Clone mode, read these actual WADNR files as the source of truth and replicate them with substitutions.

### Database Files (Phase 2)

**Update tables (mirrors of live tables):**

| Source File | Purpose |
|-------------|---------|
| `WADNR.Database/dbo/Tables/dbo.ProjectUpdateBatch.sql` | Batch table with per-step comments + diff columns |
| `WADNR.Database/dbo/Tables/dbo.ProjectUpdate.sql` | Scalar mirror of Project table |
| `WADNR.Database/dbo/Tables/dbo.ProjectLocationUpdate.sql` | Location mirror |
| `WADNR.Database/dbo/Tables/dbo.ProjectLocationStagingUpdate.sql` | Location staging mirror |
| `WADNR.Database/dbo/Tables/dbo.TreatmentUpdate.sql` | Treatment mirror |
| `WADNR.Database/dbo/Tables/dbo.ProjectCountyUpdate.sql` | County join mirror |
| `WADNR.Database/dbo/Tables/dbo.ProjectRegionUpdate.sql` | Region join mirror |
| `WADNR.Database/dbo/Tables/dbo.ProjectPriorityLandscapeUpdate.sql` | Priority landscape mirror |
| `WADNR.Database/dbo/Tables/dbo.ProjectOrganizationUpdate.sql` | Organization join mirror |
| `WADNR.Database/dbo/Tables/dbo.ProjectPersonUpdate.sql` | Contact join mirror |
| `WADNR.Database/dbo/Tables/dbo.ProjectFundingSourceUpdate.sql` | Funding source mirror |
| `WADNR.Database/dbo/Tables/dbo.ProjectFundSourceAllocationRequestUpdate.sql` | Allocation mirror |
| `WADNR.Database/dbo/Tables/dbo.ProjectImageUpdate.sql` | Photo mirror |
| `WADNR.Database/dbo/Tables/dbo.ProjectExternalLinkUpdate.sql` | External link mirror |
| `WADNR.Database/dbo/Tables/dbo.ProjectDocumentUpdate.sql` | Document mirror |
| `WADNR.Database/dbo/Tables/dbo.ProjectNoteUpdate.sql` | Note mirror |
| `WADNR.Database/dbo/Tables/dbo.ProjectUpdateProgram.sql` | Program join mirror |

**Supporting tables:**

| Source File | Purpose |
|-------------|---------|
| `WADNR.Database/dbo/Tables/dbo.ProjectUpdateHistory.sql` | State transition audit log |
| `WADNR.Database/dbo/Tables/dbo.ProjectUpdateConfiguration.sql` | Update workflow configuration settings |
| `WADNR.Database/dbo/Tables/dbo.ProjectUpdateSection.sql` | Update workflow section definitions |

**Stored procedures:**

| Source File | Purpose |
|-------------|---------|
| `WADNR.Database/dbo/Procs/dbo.pStartProjectUpdateBatch.sql` | Create batch + copy live data to mirror tables |

**Lookup table scripts:**

| Source File | Purpose |
|-------------|---------|
| `WADNR.Database/Scripts/LookupTables/dbo.ProjectApprovalStatus.sql` | Approval status lookup (Draft, PendingApproval, Approved, etc.) |
| `WADNR.Database/Scripts/LookupTables/dbo.ProjectUpdateState.sql` | Update batch state lookup (Created, Submitted, Approved, Returned) |
| `WADNR.Database/Scripts/LookupTables/dbo.ProjectUpdateSection.sql` | Update workflow section definitions |
| `WADNR.Database/Scripts/LookupTables/dbo.ProjectCreateSection.sql` | Create workflow section definitions |
| `WADNR.Database/Scripts/LookupTables/dbo.ProjectWorkflowSectionGrouping.sql` | Section-to-group mappings for sidebar nav |

### EF Models / Workflow Files (Phases 3 & 6)

| Source File | Purpose |
|-------------|---------|
| `WADNR.EFModels/Workflows/ProjectCreateWorkflowProgress.cs` | Create progress computation |
| `WADNR.EFModels/Workflows/ProjectUpdateWorkflowProgress.cs` | Update progress + has-changes |
| `WADNR.EFModels/Entities/ProjectCreateWorkflowSteps.cs` | Create per-step Get/Save + state transitions |
| `WADNR.EFModels/Entities/ProjectUpdateWorkflowSteps.cs` | Update batch mgmt + per-step Get/Save/Revert |
| `WADNR.EFModels/Entities/ProjectUpdateDiffs.cs` | Diff generation (HTML + structured) |
| `WADNR.EFModels/Entities/ProjectUpdateBatch.DtoProjections.cs` | Batch DTO projections |
| `WADNR.EFModels/Entities/ProjectUpdateBatch.StaticHelpers.cs` | Batch CRUD helpers |

### DTO Files (Phases 3 & 6)

**Create workflow DTOs** — `WADNR.Models/DataTransferObjects/Project/Workflow/`:

| Source File | Purpose |
|-------------|---------|
| `ProjectBasicsStep.cs` | Basics step DTO + request |
| `LocationSimpleStep.cs` | Location simple step DTO + request |
| `LocationDetailedStep.cs` | Location detailed step DTO + request |
| `GeographicAssignmentStep.cs` | Shared DTO for counties/regions/priority landscapes steps |
| `ProjectOrganizationsStep.cs` | Organizations step DTO + request |
| `ProjectContactsStep.cs` | Contacts step DTO + request |
| `ExpectedFundingStep.cs` | Expected funding step DTO + request |
| `ProjectClassificationsStep.cs` | Classifications step DTO + request |
| `MapExtentStep.cs` | Map extent/bounds DTO |
| `GdbImport.cs` | GDB file import DTOs |
| `WorkflowStepStatus.cs` | Step completion status DTO |
| `WorkflowStateTransition.cs` | State transition request/response DTOs |

**Update workflow DTOs** — `WADNR.Models/DataTransferObjects/ProjectUpdate/`:

| Source File | Purpose |
|-------------|---------|
| `ProjectUpdateBasicsStep.cs` | Update basics step DTO + request |
| `ProjectUpdateLocationSimpleStep.cs` | Update location simple step DTO + request |
| `ProjectUpdateLocationDetailedStep.cs` | Update location detailed step DTO + request |
| `ProjectUpdateGeographicStep.cs` | Update geographic assignment DTO + request |
| `ProjectUpdateOrganizationsStep.cs` | Update organizations step DTO + request |
| `ProjectUpdateContactsStep.cs` | Update contacts step DTO + request |
| `ProjectUpdateExpectedFundingStep.cs` | Update expected funding step DTO + request |
| `ProjectUpdateExternalLinksStep.cs` | Update external links step DTO + request |
| `ProjectUpdateTreatmentsStep.cs` | Update treatments step DTO + request |
| `ProjectUpdatePhotosStep.cs` | Update photos step DTO + request |
| `ProjectUpdateDocumentsNotesStep.cs` | Update documents & notes step DTO + request |
| `ProjectUpdateProgress.cs` | Update workflow progress response DTO |
| `ProjectUpdateBatch.cs` | Update batch detail DTO |
| `ProjectUpdateDiffSummary.cs` | Diff summary for approval/history |
| `StepDiffResponse.cs` | Per-step structured diff response |
| `ProjectUpdateStatusGridRow.cs` | Grid row DTO for update status lists |
| `ProjectUpdateHistoryEntry.cs` | History entry DTO for audit modal |

### Shared Angular Infrastructure (Phase 1)

| Source File | Purpose |
|-------------|---------|
| `WADNR.Web/src/app/shared/services/workflow-progress.service.ts` | Shared refresh + formDirty service |
| `WADNR.Web/src/app/shared/components/workflow/create-workflow-step-base.ts` | Create step base class |
| `WADNR.Web/src/app/shared/components/workflow/update-workflow-step-base.ts` | Update step base class |
| `WADNR.Web/src/app/shared/components/workflow/index.ts` | Barrel export |
| `WADNR.Web/src/app/shared/components/workflow/workflow-step-actions/` | Step action buttons (TS+HTML+SCSS) |
| `WADNR.Web/src/app/shared/components/workflow/step-diff-modal/` | Diff display modal (TS+HTML+SCSS) |
| `WADNR.Web/src/app/shared/components/workflow-nav/` | Nav container (TS+HTML+SCSS) |
| `WADNR.Web/src/app/shared/components/workflow-nav/workflow-nav-group/` | Collapsible group (TS+HTML+SCSS) |
| `WADNR.Web/src/app/shared/components/workflow-nav/workflow-nav-item/` | Nav item with status icons (TS+HTML+SCSS) |

### Other Shared Components (used by both workflows)

| Source File | Purpose |
|-------------|---------|
| `WADNR.Web/src/app/shared/components/feedback-modal/` | Feedback modal (TS+HTML+SCSS) — actions dropdown |
| `WADNR.Web/src/app/shared/components/async-confirm-modal/` | Async confirm dialog (TS+HTML) — update state transitions |
| `WADNR.Web/src/app/shared/services/confirm/` | Promise-based confirm service (3 files: `confirm.service.ts`, `confirm-options.ts`, `confirm-state.ts`) — create state transitions |

### Create Workflow Angular (Phase 5)

**Outlet:**

| Source File | Purpose |
|-------------|---------|
| `WADNR.Web/src/app/pages/projects/project-create-workflow/` | Outlet component (TS+HTML+SCSS) |

**Steps** — each directory contains `*.component.ts` + `*.component.html` + `*.component.scss`:

| Step Directory | Step Key | Sub-components |
|----------------|----------|----------------|
| `steps/basics/` | Basics | — |
| `steps/location-simple/` | LocationSimple | — |
| `steps/location-detailed/` | LocationDetailed | `import-gdb-modal/` (TS+HTML+SCSS) |
| `steps/priority-landscapes/` | PriorityLandscapes | — |
| `steps/dnr-upland-regions/` | DnrUplandRegions | — |
| `steps/counties/` | Counties | — |
| `steps/treatments/` | Treatments | — |
| `steps/contacts/` | Contacts | — |
| `steps/organizations/` | Organizations | — |
| `steps/expected-funding/` | ExpectedFunding | — |
| `steps/classifications/` | Classifications | — |
| `steps/photos/` | Photos | — |
| `steps/documents-notes/` | DocumentsNotes | — |

### Update Workflow Angular (Phase 8)

**Outlet + modals:**

| Source File | Purpose |
|-------------|---------|
| `WADNR.Web/src/app/pages/projects/project-update-workflow/` | Outlet component (TS+HTML+SCSS) |
| `WADNR.Web/src/app/pages/projects/project-update-workflow/return-with-comments-modal/` | Return modal (TS+HTML) |
| `WADNR.Web/src/app/pages/projects/update-history-modal/` | History entries modal (TS, inline template) |
| `WADNR.Web/src/app/pages/projects/update-history-diff-modal/` | History diff modal (TS+HTML+SCSS) |

**Steps** — each directory contains `update-*.component.ts` + `update-*.component.html` + `update-*.component.scss`:

| Step Directory | Step Key | Sub-components |
|----------------|----------|----------------|
| `steps/basics/` | Basics | — |
| `steps/location-simple/` | LocationSimple | — |
| `steps/location-detailed/` | LocationDetailed | — |
| `steps/priority-landscapes/` | PriorityLandscapes | — |
| `steps/dnr-upland-regions/` | DnrUplandRegions | — |
| `steps/counties/` | Counties | — |
| `steps/treatments/` | Treatments | `update-treatment-modal` (TS+HTML+SCSS) |
| `steps/contacts/` | Contacts | — |
| `steps/organizations/` | Organizations | — |
| `steps/expected-funding/` | ExpectedFunding | — |
| `steps/external-links/` | ExternalLinks | — |
| `steps/photos/` | Photos | — |
| `steps/documents-notes/` | DocumentsNotes | — |

**Note**: Create has 13 steps (includes Classifications, no External Links). Update has 13 steps (includes External Links, no Classifications).

### Route Entries

Read `WADNR.Web/src/app/app.routes.ts` and replicate the project workflow route blocks (Create: `/new` + `/edit/:id`; Update: `/:id/update`).

---

## Common Form Patterns (Cross-Reference)

These patterns appear in real step implementations but aren't standard "step types." Read the relevant skill doc section when you encounter them:

| Pattern | Where Documented | When to Use |
|---------|-----------------|-------------|
| Multi-Select List Management | `04-create-angular.md` | Step has an add/remove list (e.g., Programs) |
| Optional Fields Accordion | `04-create-angular.md` | Step has non-required fields grouped under `<details>` |
| Date Formatting Helper | `04-create-angular.md` | Step displays date fields from API |
| First Step Create Variant | `04-create-angular.md` | First step must handle both POST create and PUT save |
| Read-Only Display Fields | `06-update-angular.md` | Update step shows fields that are editable in Create but read-only in Update |
| Conditional Field Disabling | `06-update-angular.md` | Fields disabled when data is imported from external system |
| Dynamic Validation | `06-update-angular.md` | Validators that change based on API data |
| Cross-Field Date Validation | `06-update-angular.md` | Date range validation in `onSave()` |
| Auto-Geographic-Assignment | `03-create-api.md` | Location save triggers spatial intersection queries |
| Unique Identifier Generation | `03-create-api.md` | Entity needs a human-readable number on creation |

---

## Completion Checklist

### Database
- [ ] Update tables created (one per related entity)
- [ ] `ProjectUpdateBatch` table with comment + diff columns
- [ ] `ProjectUpdateHistory` table for audit
- [ ] Stored procedure `pStart{Entity}UpdateBatch` created
- [ ] FK indexes on all Update tables
- [ ] Lookup tables populated (ApprovalStatus, UpdateState)
- [ ] `DatabaseBuild.ps1` succeeds
- [ ] `Scaffold.ps1` generates EF entities

### Create Workflow — API
- [ ] `{Entity}CreateWorkflowProgress.cs` with step enum, context, completion logic
- [ ] `{Entity}CreateWorkflowSteps.cs` with per-step Get/Save, state transitions
- [ ] Per-step DTOs in `Workflow/` subfolder
- [ ] Controller endpoints: GET progress, GET/PUT per step, POST create, POST state transitions
- [ ] API builds, `swagger.json` includes all endpoints

### Create Workflow — Angular
- [ ] Outlet component with sidebar nav, step groups, `vm$` combining projectID + progress
- [ ] Complete actions dropdown: {Entity} Details, Provide Feedback, Withdraw Proposal
- [ ] Complete footer: created by info + approval status + Submit/Approve/Return/Reject buttons
- [ ] All five state transition methods using `ConfirmService`: Submit, Approve, Return, Reject, Withdraw
- [ ] `FeedbackModalComponent` wired to actions dropdown
- [ ] Per-step components extending `CreateWorkflowStepBase`
- [ ] `initProjectID()` and `trackFormDirty()` in each step
- [ ] Routes: `/new` (basics only) and `/edit/:projectID` (all steps)
- [ ] `canActivate: [projectEditGuard]` and `canDeactivate: [UnsavedChangesGuard]`

### Update Workflow — API
- [ ] `{Entity}UpdateWorkflowProgress.cs` with has-changes, reviewer comments
- [ ] `{Entity}UpdateWorkflowSteps.cs` with batch CRUD, per-step Get/Save/Revert
- [ ] `{Entity}UpdateDiffs.cs` with per-step diff generation (fields/list/table)
- [ ] Controller endpoints: batch start/delete, GET progress, GET/PUT per step, POST revert, GET diff, POST submit/approve/return
- [ ] API builds, `swagger.json` includes all endpoints

### Update Workflow — Angular
- [ ] Outlet with batch state alerts, has-changes flags, delete/return/approve/submit
- [ ] Complete actions dropdown: Go back, Show History, My {Entities} list, Provide Feedback, Delete
- [ ] Complete footer: submitted/returned metadata + conditional action buttons
- [ ] `FeedbackModalComponent` wired to actions dropdown
- [ ] Per-step components extending `UpdateWorkflowStepBase`
- [ ] `initProjectID()`, `initHasChanges()`, `trackFormDirty()` in each step
- [ ] `stepRefresh$` used (NOT `projectID$`) for data loading
- [ ] `<workflow-step-actions [showRevertActions]="true" ...>` in templates
- [ ] ReturnWithCommentsModal with per-step comment fields
- [ ] UpdateHistoryModal with entries table
- [ ] UpdateHistoryDiffModal with dual-mode rendering (legacy HTML + structured diffs)
- [ ] Routes: `/:projectID/update` with all step children

### Code Quality
- [ ] No Bootstrap classes
- [ ] All queries use `.AsNoTracking().Select()` (read) or targeted `.Include()` (write)
- [ ] Route params via `@Input()` + `BehaviorSubject`
- [ ] Standalone components with explicit imports
- [ ] `signal(false)` for `mapIsReady` on map steps
- [ ] `npm run build` succeeds without errors
