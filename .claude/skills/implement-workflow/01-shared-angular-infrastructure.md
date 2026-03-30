# 01 — Shared Angular Workflow Infrastructure

> **Purpose**: Shared components needed by ANY workflow. Skip entirely if they already exist.

## Pre-flight Check

Look for these files. If ALL exist, skip this document entirely:

- `WADNR.Web/src/app/shared/services/workflow-progress.service.ts`
- `WADNR.Web/src/app/shared/components/workflow/create-workflow-step-base.ts`
- `WADNR.Web/src/app/shared/components/workflow/update-workflow-step-base.ts`
- `WADNR.Web/src/app/shared/components/workflow/workflow-step-actions/workflow-step-actions.component.ts`
- `WADNR.Web/src/app/shared/components/workflow/step-diff-modal/step-diff-modal.component.ts`
- `WADNR.Web/src/app/shared/components/workflow-nav/workflow-nav.component.ts`
- `WADNR.Web/src/app/shared/components/workflow-nav/workflow-nav-group/workflow-nav-group.component.ts`
- `WADNR.Web/src/app/shared/components/workflow-nav/workflow-nav-item/workflow-nav-item.component.ts`

---

## 1. WorkflowProgressService

**File**: `shared/services/workflow-progress.service.ts`

Shared bridge between step components and outlet components. Steps call `triggerRefresh()` after save/revert; outlets subscribe to `refreshProgress$` to re-fetch sidebar nav state.

**Key API**:
- `refreshProgress$: Observable<void>` — emits after step save/revert
- `formDirty$: Observable<boolean>` — tracks unsaved form state
- `triggerRefresh()` — called by step base classes after save
- `setFormDirty(dirty: boolean)` — called by form tracking
- `isFormDirty: boolean` — synchronous getter for `canExit()`

**Reference**: Read `WADNR.Web/src/app/shared/services/workflow-progress.service.ts` for exact implementation (30 lines).

---

## 2. CreateWorkflowStepBase

**File**: `shared/components/workflow/create-workflow-step-base.ts`

Abstract base class for Create workflow steps. Provides:

### Key Properties
- `abstract readonly nextStep: string` — route segment for next step
- `_projectID$: BehaviorSubject<number | null>` — input from route
- `projectID$: Observable<number>` — filtered, non-null project ID
- `isSaving$ / isSaving` — save-in-progress state

### Key Methods
- `initProjectID()` — call in `ngOnInit()`. Creates `projectID$` from `_projectID$`
- `trackFormDirty(form: FormGroup)` — subscribes to `form.valueChanges`, updates shared service
- `setFormDirty()` — manual dirty marking for non-form steps
- `canExit(): boolean` — implements `IDeactivateComponent` for `UnsavedChangesGuard`
- `saveStep<T>(saveOperation, successMessage, errorMessage, navigate, onSuccess?)` — generic save with:
  - Sets `isSaving = true`
  - Calls `saveOperation(projectID)`
  - On success: marks form pristine, triggers progress refresh, optionally navigates
  - On error: shows alert with error message
- `navigateToNextStep(projectID)` — `router.navigate(["/projects", "edit", projectID, this.nextStep])`
- `navigateToStep(projectID, step)` — navigate to arbitrary step

### Route Input Pattern
```typescript
@Input() set projectID(value: string | number) {
    if (value !== undefined && value !== null && value !== "") {
        const numValue = Number(value);
        if (!Number.isNaN(numValue)) {
            this._projectID$.next(numValue);
        }
    }
}
```

**Reference**: Read `WADNR.Web/src/app/shared/components/workflow/create-workflow-step-base.ts` for exact implementation (182 lines).

---

## 3. UpdateWorkflowStepBase

**File**: `shared/components/workflow/update-workflow-step-base.ts`

Abstract base class for Update workflow steps. Extends the Create pattern with:

### Additional Properties
- `abstract readonly stepKey: string` — identifier for API calls (e.g., `"basics"`, `"organizations"`)
- `stepRefresh$: Observable<number>` — **critical**: re-emits projectID when `refreshStepData$` fires
- `isReadOnly$ / isReadOnly` — true when batch is submitted (prevents saves)
- `hasChanges$ / hasChanges` — true when step has changes vs approved project
- `reviewerComment$` — populated when batch is Returned
- `refreshStepData$: Subject<void>` — trigger for reloading step data

### Key Methods
- `initProjectID()` — creates both `projectID$` AND `stepRefresh$`. The `stepRefresh$` pattern:
  ```typescript
  this.stepRefresh$ = this._projectID$.pipe(
      filter((id): id is number => id != null && !Number.isNaN(id)),
      switchMap((id) =>
          this.refreshStepData$.pipe(
              startWith(undefined),
              map(() => id)
          )
      ),
      shareReplay({ bufferSize: 1, refCount: true })
  );
  ```
- `initHasChanges()` — fetches progress, extracts `HasChanges` and `ReviewerComment` for this step using PascalCase key conversion
- `onStepReverted()` — calls `refreshStepData$.next()` + `triggerRefresh()`
- `saveStep<T>(...)` — same as Create but adds read-only guard and calls `refreshStepData$.next()` after save

### Critical Pattern: stepRefresh$

All step data loading MUST use `stepRefresh$` (not `projectID$`):

```typescript
// CORRECT — data reloads after save/revert
this.data$ = this.stepRefresh$.pipe(
    switchMap((projectID) => this.service.getStepData(projectID))
);

// WRONG — data does NOT reload after save/revert
this.data$ = this.projectID$.pipe(
    switchMap((projectID) => this.service.getStepData(projectID))
);
```

**Reference**: Read `WADNR.Web/src/app/shared/components/workflow/update-workflow-step-base.ts` for exact implementation (261 lines).

---

## 4. WorkflowStepActionsComponent

**Files**: `shared/components/workflow/workflow-step-actions/` (TS + HTML + SCSS)

Reusable action buttons for both Create and Update workflow steps.

### Inputs
| Input | Type | Default | Description |
|-------|------|---------|-------------|
| `showRevertActions` | `boolean` | `false` | Show revert/show-changes (update mode) |
| `isSaving` | `boolean` | `false` | Save in progress |
| `hasChanges` | `boolean` | `false` | Step has changes vs approved |
| `canRevert` | `boolean` | `true` | Revert allowed |
| `stepKey` | `string` | `""` | Step identifier for API |
| `projectID` | `number \| null` | `null` | Project ID for API |
| `saveButtonText` | `string` | `"Save"` | Custom save text |
| `continueButtonText` | `string` | `"Save & Continue"` | Custom continue text |

### Outputs
| Output | Description |
|--------|-------------|
| `save` | Save button clicked |
| `saveAndContinue` | Save & Continue clicked |
| `reverted` | Revert completed — parent should refresh |

### Behaviors
- **Create mode** (`showRevertActions=false`): Shows Save + Save & Continue on the right, required note on the left
- **Update mode** (`showRevertActions=true`): Adds Revert + Show Changes on the left
- **Revert**: Shows confirm dialog, calls `projectService.revertUpdateStepProject()`, emits `reverted`
- **Show Changes**: Calls `projectService.getUpdateStepDiffProject()`, opens `StepDiffModalComponent` with response

### Loading States
- `isReverting$: BehaviorSubject<boolean>` — shown via async pipe
- `isLoadingDiff$: BehaviorSubject<boolean>` — shown via async pipe

**Reference**: Read `WADNR.Web/src/app/shared/components/workflow/workflow-step-actions/` for exact implementation.

---

## 5. StepDiffModalComponent

**Files**: `shared/components/workflow/step-diff-modal/` (TS + HTML + SCSS)

Modal that displays differences between update and approved data. Opened by `WorkflowStepActionsComponent.onShowChangesClick()`.

### Input Data (via DialogRef)
```typescript
{
    stepKey: string;         // PascalCase step key
    sections: DiffSection[]; // From StepDiffResponse.Sections
}
```

### DiffSection Types

The backend returns sections with one of three types. The modal renders each differently:

1. **`fields`** — Field-by-field comparison table:
   - Headers: Field | Original | Updated
   - Changed rows highlighted (green for added, red/strikethrough for deleted)

2. **`list`** — Simple list comparison:
   - Unchanged items, added items (green), removed items (red/strikethrough)
   - Toggle to show/hide deletions

3. **`table`** — Tabular data comparison:
   - Column headers from `section.Headers`
   - Unchanged rows, added rows (green), removed rows (red/strikethrough)

### Backend DTO: StepDiffResponse

```csharp
public class StepDiffResponse
{
    public bool HasChanges { get; set; }
    public List<DiffSection> Sections { get; set; } = new();
}

public class DiffSection
{
    public string? Title { get; set; }
    public string Type { get; set; } = "fields"; // "fields", "list", "table"
    public List<DiffField>? Fields { get; set; }           // For "fields" type
    public List<string>? OriginalItems { get; set; }       // For "list" type
    public List<string>? UpdatedItems { get; set; }        // For "list" type
    public List<string>? Headers { get; set; }             // For "table" type
    public List<string[]>? OriginalRows { get; set; }      // For "table" type
    public List<string[]>? UpdatedRows { get; set; }       // For "table" type
}

public class DiffField
{
    public string Label { get; set; } = string.Empty;
    public string? OriginalValue { get; set; }
    public string? UpdatedValue { get; set; }
}
```

### Step Key Normalization

The step key arrives as PascalCase from the frontend but must be normalized for the step title lookup:
```typescript
const key = this.stepKey.replace(/([a-z])([A-Z])/g, "$1-$2").toLowerCase();
```

**Reference**: Read `WADNR.Web/src/app/shared/components/workflow/step-diff-modal/` for exact implementation.

---

## 6. WorkflowNavComponent

**Files**: `shared/components/workflow-nav/` (TS + HTML + SCSS)

Container component. Renders a `<ul class="sidebar-nav workflow-nav">` with `<ng-content>` for child items/groups.

---

## 7. WorkflowNavGroupComponent

**Files**: `shared/components/workflow-nav/workflow-nav-group/` (TS + HTML + SCSS)

Collapsible group container.

### Inputs
| Input | Type | Default | Description |
|-------|------|---------|-------------|
| `title` | `string` | `""` | Group header text |
| `expanded` | `boolean` | `true` | Whether group is open |
| `complete` | `boolean` | `false` | All items complete (shows checkmark) |
| `childRoutes` | `string[]` | `[]` | Route fragments for active detection |

### Behavior
- Auto-expands when a child route is active (checks `router.url.includes()`)
- Prevents collapse when a child is active
- Shows checkmark icon when `complete=true`
- Shows expand/collapse chevron when no active child

**Reference**: Read `WADNR.Web/src/app/shared/components/workflow-nav/workflow-nav-group/` for exact implementation.

---

## 8. WorkflowNavItemComponent

**Files**: `shared/components/workflow-nav/workflow-nav-item/` (TS + HTML + SCSS)

Individual nav item with status icons.

### Inputs
| Input | Type | Default | Description |
|-------|------|---------|-------------|
| `navRouterLink` | `string \| string[]` | — | Router link for the step |
| `complete` | `boolean` | `false` | Step is complete (green check) |
| `disabled` | `boolean` | `false` | Step is disabled (no click) |
| `required` | `boolean` | `true` | Step is required (incomplete icon) |
| `hasChanges` | `boolean` | `false` | Has changes flag (amber icon, update only) |

### Icon Logic
- Complete: `StepComplete` icon (green)
- Incomplete + Required: `StepIncomplete` icon (faded)
- Incomplete + Optional: `Info` icon (teal)
- Has Changes: `Flag` icon (amber, right side)
- Active: highlighted background via `routerLinkActive="active"`

**Reference**: Read `WADNR.Web/src/app/shared/components/workflow-nav/workflow-nav-item/` for exact implementation.

---

## 9. Backend DTO: WorkflowStepStatus

**File**: `{ModelsProject}/DataTransferObjects/Project/Workflow/WorkflowStepStatus.cs`

```csharp
public class WorkflowStepStatus
{
    public bool IsComplete { get; set; }
    public bool IsDisabled { get; set; }
    public bool IsRequired { get; set; }
    public bool HasChanges { get; set; } // Update workflow only
}
```

This DTO is shared between Create and Update workflow progress responses.

---

## 10. Barrel Export

**File**: `shared/components/workflow/index.ts`

```typescript
export * from "./create-workflow-step-base";
export * from "./workflow-step-actions/workflow-step-actions.component";
```

Add Update base class export if creating Update workflow:
```typescript
export * from "./update-workflow-step-base";
```
