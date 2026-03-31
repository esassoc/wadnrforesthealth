# 06 — Update Workflow: Angular Frontend

> **Purpose**: Update workflow frontend — outlet component, step components, modals, route configuration.
> **Prereq**: Update API must be built and `npm run gen-model` must have been run first.

---

## 1. Outlet Component

**Directory**: `pages/{entities-kebab}/{entity-kebab}-update-workflow/`
**Files**: TS + HTML + SCSS

**Pattern from**: `WADNR.Web/src/app/pages/projects/project-update-workflow/`

### TypeScript

```typescript
import { Component, Input, OnInit } from "@angular/core";
import { AsyncPipe, CommonModule, DatePipe } from "@angular/common";
import { Router, RouterLink, RouterOutlet } from "@angular/router";
import { BehaviorSubject, filter, Observable, shareReplay, switchMap, startWith } from "rxjs";
import { DialogService } from "@ngneat/dialog";

import { BreadcrumbComponent } from "src/app/shared/components/breadcrumb/breadcrumb.component";
import { PageHeaderComponent } from "src/app/shared/components/page-header/page-header.component";
import { WorkflowNavComponent } from "src/app/shared/components/workflow-nav/workflow-nav.component";
import { WorkflowNavItemComponent } from "src/app/shared/components/workflow-nav/workflow-nav-item/workflow-nav-item.component";
import { WorkflowNavGroupComponent } from "src/app/shared/components/workflow-nav/workflow-nav-group/workflow-nav-group.component";
import { IconComponent } from "src/app/shared/components/icon/icon.component";
import { DropdownToggleDirective } from "src/app/shared/directives/dropdown-toggle.directive";
import { ButtonLoadingDirective } from "src/app/shared/directives/button-loading.directive";
import { AsyncConfirmModalComponent, AsyncConfirmModalData } from "src/app/shared/components/async-confirm-modal/async-confirm-modal.component";
import { AlertService } from "src/app/shared/services/alert.service";
import { Alert } from "src/app/shared/models/alert";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";
import { WorkflowProgressService } from "src/app/shared/services/workflow-progress.service";
import { {Entity}Service } from "src/app/shared/generated/api/{entity-kebab}.service";
import { UpdateWorkflowProgressResponse } from "src/app/shared/generated/model/update-workflow-progress-response";
import { {Entity}UpdateStateEnum } from "src/app/shared/generated/enum/{entity-kebab}-update-state-enum";
import { ReturnWithCommentsModalComponent, ReturnWithCommentsModalData } from "./return-with-comments-modal/return-with-comments-modal.component";
import { UpdateHistoryModalComponent, UpdateHistoryModalData } from "../update-history-modal/update-history-modal.component";

export interface WorkflowStep {
    key: string;    // PascalCase key matching backend step
    name: string;   // Display name
    route: string;  // kebab-case route segment
    required: boolean;
}

export interface WorkflowStepGroup {
    title: string;
    steps: WorkflowStep[];
}

@Component({
    selector: "{entity-kebab}-update-workflow-outlet",
    standalone: true,
    imports: [
        CommonModule, AsyncPipe, DatePipe, RouterOutlet, RouterLink,
        BreadcrumbComponent, PageHeaderComponent,
        WorkflowNavComponent, WorkflowNavItemComponent, WorkflowNavGroupComponent,
        IconComponent, DropdownToggleDirective, ButtonLoadingDirective,
    ],
    templateUrl: "./{entity-kebab}-update-workflow-outlet.component.html",
    styleUrls: ["./{entity-kebab}-update-workflow-outlet.component.scss"],
})
export class {Entity}UpdateWorkflowOutletComponent implements OnInit {
    @Input() set {entity}ID(value: string | number | undefined) { ... }
    private _{entity}ID$ = new BehaviorSubject<number | null>(null);

    public {entity}ID$: Observable<number>;
    public progress$: Observable<UpdateWorkflowProgressResponse | null>;
    public formDirty$: Observable<boolean>;
    public {Entity}UpdateStateEnum = {Entity}UpdateStateEnum;
    public isReturning$ = new BehaviorSubject<boolean>(false);

    public stepGroups: WorkflowStepGroup[] = [ /* step definitions */ ];
}
```

### Key Properties

| Property | Type | Description |
|----------|------|-------------|
| `{entity}ID$` | `Observable<number>` | Filtered non-null entity ID |
| `progress$` | `Observable<UpdateWorkflowProgressResponse>` | Re-fetches on `refreshProgress$` |
| `formDirty$` | `Observable<boolean>` | From `WorkflowProgressService` |
| `isReturning$` | `BehaviorSubject<boolean>` | Loading state for return action |
| `stepGroups` | `WorkflowStepGroup[]` | Static step group definitions |

### Progress$ Pattern

Same as Create but calls the update progress endpoint:

```typescript
this.progress$ = this._{entity}ID$.pipe(
    filter((id): id is number => id != null && !Number.isNaN(id)),
    switchMap(({entity}ID) => {
        return this.workflowProgressService.refreshProgress$.pipe(
            startWith(undefined),
            switchMap(() => this.{entity}Service.getUpdateWorkflowProgress{Entity}({entity}ID))
        );
    }),
    shareReplay({ bufferSize: 1, refCount: true })
);
```

### Action Methods

#### submitForApproval — Uses AsyncConfirmModal

```typescript
submitForApproval({entity}ID: number, name: string): void {
    const data: AsyncConfirmModalData = {
        title: "Submit Update for review",
        message: `Are you sure you want to submit the updates for "${name}" to the reviewer?`,
        buttonTextYes: "Continue",
        buttonClassYes: "btn-primary",
        actionFn: () => this.{entity}Service.submitUpdateForApproval{Entity}({entity}ID),
    };
    this.dialogService
        .open(AsyncConfirmModalComponent, { data, size: "md" })
        .afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.pushAlert(new Alert("Update submitted for approval successfully.", AlertContext.Success, true));
                this.router.navigate(["/{entities}", {entity}ID]);
            }
        });
}
```

#### approveUpdate — Uses AsyncConfirmModal

```typescript
approveUpdate({entity}ID: number, name: string): void {
    const data: AsyncConfirmModalData = {
        title: "Approve Update",
        message: `Are you sure you want to approve the updates for "${name}"? This will apply all changes.`,
        buttonTextYes: "Approve",
        buttonClassYes: "btn-primary",
        actionFn: () => this.{entity}Service.approveUpdate{Entity}({entity}ID),
    };
    this.dialogService
        .open(AsyncConfirmModalComponent, { data, size: "md" })
        .afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.pushAlert(new Alert("Update approved and changes applied.", AlertContext.Success, true));
                this.router.navigate(["/{entities}", {entity}ID]);
            }
        });
}
```

#### deleteUpdate — Uses AsyncConfirmModal

```typescript
deleteUpdate({entity}ID: number, name: string): void {
    const data: AsyncConfirmModalData = {
        title: "Delete Update",
        message: `Are you sure you want to delete all pending updates for "${name}"? This cannot be undone.`,
        buttonTextYes: "Delete",
        buttonClassYes: "btn-danger",
        actionFn: () => this.{entity}Service.deleteUpdateBatch{Entity}({entity}ID),
    };
    this.dialogService
        .open(AsyncConfirmModalComponent, { data, size: "md" })
        .afterClosed$.subscribe((result) => {
            if (result) {
                this.alertService.pushAlert(new Alert("Update deleted.", AlertContext.Success, true));
                this.router.navigate(["/{entities}", {entity}ID]);
            }
        });
}
```

#### returnUpdate — Opens Custom Comments Modal

```typescript
returnUpdate({entity}ID: number, name: string): void {
    const data: ReturnWithCommentsModalData = { {entity}Name: name };
    this.dialogService
        .open(ReturnWithCommentsModalComponent, { data, size: "lg" })
        .afterClosed$.subscribe((result) => {
            if (result) {
                this.isReturning$.next(true);
                this.{entity}Service.returnUpdate{Entity}({entity}ID, result).subscribe({
                    next: () => {
                        this.alertService.pushAlert(new Alert("Update returned for revisions.", AlertContext.Success, true));
                        this.router.navigate(["/{entities}", {entity}ID]);
                    },
                    error: (err) => {
                        this.isReturning$.next(false);
                        const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to return update.";
                        this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                    },
                });
            }
        });
}
```

### Template Helper Methods

```typescript
getStepLink(step: WorkflowStep): string[] {
    const {entity}ID = this._{entity}ID$.getValue();
    if ({entity}ID) return ["/{entities}", {entity}ID.toString(), "update", step.route];
    return [];
}

isStepHasChanges(progress, step): boolean { return progress?.Steps?.[step.key]?.HasChanges ?? false; }
isStepComplete(progress, step): boolean { return progress?.Steps?.[step.key]?.IsComplete ?? false; }
isStepDisabled(progress, step): boolean { return progress?.Steps?.[step.key]?.IsDisabled ?? true; }
isStepRequired(step): boolean { return step.required; }
isGroupComplete(progress, group): boolean { return group.steps.every(s => this.isStepComplete(progress, s)); }
getGroupChildRoutes(group): string[] { return group.steps.map(s => s.route); }

canEdit(progress): boolean {
    if (!progress) return false;
    return progress.{Entity}UpdateStateID === {Entity}UpdateStateEnum.Created
        || progress.{Entity}UpdateStateID === {Entity}UpdateStateEnum.Returned;
}
canSubmit(progress): boolean { return progress?.CanSubmit ?? false; }
canApprove(progress): boolean { return progress?.CanApprove ?? false; }
canReturn(progress): boolean { return progress?.CanReturn ?? false; }
isReadyToApprove(progress): boolean { return progress?.IsReadyToApprove ?? false; }
```

### Feedback Modal

```typescript
openFeedbackModal(): void {
    this.dialogService.open(FeedbackModalComponent, {
        data: { currentPageUrl: window.location.href },
        size: "md",
    });
}
```

Import `FeedbackModalComponent` from `src/app/shared/components/feedback-modal/feedback-modal.component`.

### History Modal

```typescript
openHistoryModal({entity}ID: number): void {
    this.{entity}Service.listUpdateBatchHistory{Entity}({entity}ID).subscribe((entries) => {
        this.dialogService.open(UpdateHistoryModalComponent, {
            data: { entries } as UpdateHistoryModalData,
            size: "md",
        });
    });
}

getStateBadgeClass(stateID: number): string {
    switch (stateID) {
        case {Entity}UpdateStateEnum.Created: return "badge-secondary";
        case {Entity}UpdateStateEnum.Submitted: return "badge-info";
        case {Entity}UpdateStateEnum.Approved: return "badge-success";
        case {Entity}UpdateStateEnum.Returned: return "badge-warning";
        default: return "badge-secondary";
    }
}
```

### HTML Template Structure

```html
<breadcrumb></breadcrumb>

@if (progress$ | async; as progress) {
    <page-header [pageTitle]="'Update: ' + (progress?.{Entity}Name ?? '{Entity}')" [templateRight]="actionsTemplate">
    </page-header>

    <ng-template #actionsTemplate>
        <div class="actions-dropdown" [dropdownToggle]="actionsMenu">
            <a href="javascript:void(0);" class="actions-toggle">
                See more Actions <icon icon="AngleDown"></icon>
            </a>
            <ul #actionsMenu class="dropdown-menu dropdown-menu-right">
                <li>
                    <a [routerLink]="['/{entities}', progress.{Entity}ID]" class="dropdown-item">
                        Go back to {Entity}
                    </a>
                </li>
                <li>
                    <a href="javascript:void(0);" (click)="openHistoryModal(progress.{Entity}ID)" class="dropdown-item">
                        Show Update History
                    </a>
                </li>
                <li>
                    <a routerLink="/my-{entities}" [queryParams]="{ filter: 'requiring-update' }" class="dropdown-item">
                        Back to Update My {Entities} list
                    </a>
                </li>
                <li>
                    <a href="javascript:void(0);" (click)="openFeedbackModal()" class="dropdown-item">
                        Provide Feedback
                    </a>
                </li>
                @if (canEdit(progress)) {
                    <li class="divider"></li>
                    <li>
                        <a href="javascript:void(0);"
                            (click)="deleteUpdate(progress.{Entity}ID, progress.{Entity}Name)"
                            class="dropdown-item text-danger">
                            Delete Entire Update
                        </a>
                    </li>
                }
            </ul>
        </div>
    </ng-template>

    <!-- Batch State Alerts -->
    @if (progress.{Entity}UpdateStateID === {Entity}UpdateStateEnum.Submitted) {
        <div class="alert alert-info">
            <strong>Pending Review:</strong> This update has been submitted and is awaiting approval. Changes cannot be made while under review.
        </div>
    }
    @if (progress.{Entity}UpdateStateID === {Entity}UpdateStateEnum.Returned) {
        <div class="alert alert-warning">
            <strong>Returned for Revisions:</strong> This update has been returned by the reviewer. Please review the section-specific comments and make the requested changes, then resubmit.
        </div>
    }

    <div class="dashboard workflow">
        <div class="sidebar">
            <div class="sidebar-body sticky-nav">
                <workflow-nav>
                    @for (group of stepGroups; track group.title) {
                        <workflow-nav-group
                            [title]="group.title"
                            [expanded]="false"
                            [complete]="isGroupComplete(progress, group)"
                            [childRoutes]="getGroupChildRoutes(group)">
                            @for (step of group.steps; track step.key) {
                                <workflow-nav-item
                                    [navRouterLink]="getStepLink(step)"
                                    [complete]="isStepComplete(progress, step)"
                                    [disabled]="isStepDisabled(progress, step)"
                                    [required]="isStepRequired(step)"
                                    [hasChanges]="isStepHasChanges(progress, step)">
                                    {{ step.name }}
                                </workflow-nav-item>
                            }
                        </workflow-nav-group>
                    }
                </workflow-nav>
            </div>
        </div>
        <div class="main">
            <div class="outlet-container">
                <router-outlet></router-outlet>
            </div>

            <!-- Update Workflow Footer -->
            <div class="workflow-footer">
                <div class="workflow-footer__info">
                    @if (progress.{Entity}UpdateStateID === {Entity}UpdateStateEnum.Submitted && progress.SubmittedByPersonName) {
                        <span class="workflow-footer__created">
                            Submitted by {{ progress.SubmittedByPersonName }}
                            @if (progress.SubmittedDate) {
                                on {{ progress.SubmittedDate | date:'M/d/yyyy h:mm:ss a' }}
                            }
                        </span>
                    } @else if (progress.{Entity}UpdateStateID === {Entity}UpdateStateEnum.Returned && progress.ReturnedByPersonName) {
                        <span class="workflow-footer__created">
                            Returned by {{ progress.ReturnedByPersonName }}
                            @if (progress.ReturnedDate) {
                                on {{ progress.ReturnedDate | date:'M/d/yyyy h:mm:ss a' }}
                            }
                        </span>
                    }
                </div>
                <div class="workflow-footer__actions">
                    @if (canSubmit(progress)) {
                        <button class="btn btn-primary"
                            [disabled]="formDirty$ | async"
                            [title]="(formDirty$ | async) ? 'Save your changes before submitting' : ''"
                            (click)="submitForApproval(progress.{Entity}ID, progress.{Entity}Name)">
                            Submit
                        </button>
                    } @else if (canEdit(progress)) {
                        <button class="btn btn-primary" disabled>Submit</button>
                    }
                    @if (canApprove(progress)) {
                        <button class="btn btn-warning me-2"
                            [disabled]="isReturning$ | async"
                            [buttonLoading]="isReturning$ | async"
                            (click)="returnUpdate(progress.{Entity}ID, progress.{Entity}Name)">
                            Return
                        </button>
                        @if (isReadyToApprove(progress)) {
                            <button class="btn btn-success"
                                (click)="approveUpdate(progress.{Entity}ID, progress.{Entity}Name)">
                                Approve
                            </button>
                        } @else {
                            <button class="btn btn-success" disabled>Approve</button>
                        }
                    }
                </div>
            </div>
        </div>
    </div>
} @else {
    <div class="loading-spinner">Loading...</div>
}
```

### SCSS

```scss
@use "/src/scss/abstracts" as *;
@use "/src/scss/components/workflow-outlet";

// Update-specific styles
.alert {
    margin-bottom: 1rem;
    padding: 1rem;
    border-radius: 0.25rem;

    &.alert-info {
        background-color: var(--info-bg, #d1ecf1);
        border: 1px solid var(--info-border, #bee5eb);
        color: var(--info-text, #0c5460);
    }

    &.alert-warning {
        background-color: var(--warning-bg, #fff3cd);
        border: 1px solid var(--warning-border, #ffeeba);
        color: var(--warning-text, #856404);
    }
}

.badge {
    display: inline-block;
    padding: 0.25em 0.5em;
    font-size: 0.875em;
    font-weight: 600;
    border-radius: 0.25rem;

    &.badge-secondary { background-color: var(--gray-500); color: #fff; }
    &.badge-info { background-color: var(--info, #17a2b8); color: #fff; }
    &.badge-success { background-color: var(--success, #28a745); color: #fff; }
    &.badge-warning { background-color: var(--warning, #ffc107); color: #212529; }
}
```

---

## 2. Step Components

**Directory**: `pages/{entities-kebab}/{entity-kebab}-update-workflow/steps/{step-route}/`
**Files**: TS + HTML per step

### Common Pattern: All Update Steps

Key differences from Create steps:
- Extend `UpdateWorkflowStepBase` (not `CreateWorkflowStepBase`)
- Declare `stepKey` in addition to `nextStep`
- Call `initHasChanges()` in `ngOnInit()`
- Use `stepRefresh$` for data loading (not `projectID$`)
- Template binds `<workflow-step-actions>` with update-specific inputs
- Template shows `reviewerComment$` when present

```typescript
@Component({
    selector: "update-{step-route}-step",
    standalone: true,
    imports: [
        CommonModule, AsyncPipe, ReactiveFormsModule,
        FormFieldComponent, IconComponent, WorkflowStepActionsComponent,
    ],
    templateUrl: "./update-{step-route}-step.component.html",
})
export class Update{StepName}StepComponent extends UpdateWorkflowStepBase implements OnInit {
    readonly nextStep = "{next-step-route}";
    readonly stepKey = "{StepKey}";  // PascalCase key matching backend

    // Step-specific observables
    vm$: Observable<{ isLoading: boolean; data: {StepDto} | null; }>;

    form: FormGroup;

    constructor(private {entity}Service: {Entity}Service) {
        super();
        this.form = new FormGroup({ /* form controls */ });
    }

    ngOnInit(): void {
        this.initProjectID();
        this.initHasChanges();     // <-- Update-specific!
        this.trackFormDirty(this.form);

        // CRITICAL: Use stepRefresh$ (not projectID$) for auto-reload after save/revert
        const stepData$ = this.stepRefresh$.pipe(
            switchMap((id) => this.{entity}Service.getUpdate{StepName}Step{Entity}(id).pipe(
                catchError(() => {
                    this.alertService.pushAlert(new Alert("Failed to load data.", AlertContext.Danger, true));
                    return of(null);
                })
            )),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        this.vm$ = stepData$.pipe(
            map((data) => {
                if (data) this.populateForm(data);
                return { isLoading: false, data };
            }),
            startWith({ isLoading: true, data: null }),
            shareReplay({ bufferSize: 1, refCount: true })
        );
    }

    onSave(navigate: boolean): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }
        const request = { /* map from form */ };
        this.saveStep(
            (id) => this.{entity}Service.saveUpdate{StepName}Step{Entity}(id, request),
            "Update saved successfully.",
            "Failed to save update.",
            navigate
        );
    }
}
```

### Template Pattern: Form Step (Update)

```html
@if (vm$ | async; as vm) {
    @if ({ saving: isSaving$ | async, hasChanges: hasChanges$ | async }; as state) {
    <div class="card">
        <div class="card-header">
            <span class="card-title">Update {Step Display Name}</span>
        </div>
        <div class="card-body">
            <!-- Reviewer Comment Banner (shown when batch was Returned) -->
            @if (reviewerComment$ | async; as reviewerComment) {
                <div class="reviewer-comment-banner">
                    <strong>Reviewer Comments:</strong>
                    <p>{{ reviewerComment }}</p>
                </div>
            }

            @if (vm.data) {
                <form [formGroup]="form">
                    <div class="grid-12">
                        <div class="g-col-6">
                            <form-field
                                [formControl]="$any(form.controls.fieldName)"
                                fieldLabel="Field Label"
                                [type]="FormFieldType.Text">
                            </form-field>
                        </div>
                    </div>
                </form>
            }
        </div>
        <div class="card-footer">
            <workflow-step-actions
                [showRevertActions]="true"
                [isSaving]="state.saving"
                [hasChanges]="state.hasChanges"
                [stepKey]="stepKey"
                [projectID]="vm.data?.{Entity}UpdateBatchID ? (projectID$ | async) : null"
                continueButtonText="Save & Continue"
                (save)="onSave(false)"
                (saveAndContinue)="onSave(true)"
                (reverted)="onStepReverted()">
            </workflow-step-actions>
        </div>
    </div>
    }
} @else {
    <div class="card">
        <div class="card-body">
            <div class="text-center p-4">Loading...</div>
        </div>
    </div>
}
```

### Pattern: Read-Only Display Fields

Some fields are editable in the Create workflow but display-only in Update (e.g., entity name, entity type). Show these as static text above the form:

```html
@if (vm.data) {
    <div class="readonly-info mb-3">
        <div class="grid-12">
            <div class="g-col-6">
                <label class="field-label">{Entity} Name</label>
                <div class="readonly-value">{{ vm.data.{Entity}Name }}</div>
            </div>
            <div class="g-col-6">
                <label class="field-label">{Entity} Type</label>
                <div class="readonly-value">{{ vm.data.{Entity}TypeName }}</div>
            </div>
        </div>
    </div>
    <div class="mb-3">
        <p class="text-muted">Update the information below. Note: {Entity} Name and {Entity} Type cannot be changed through the update process.</p>
    </div>
}
```

**Backend**: The Update step GET DTO includes the read-only fields for display but the PUT request DTO omits them. The Update step form has fewer controls than the Create step form.

### Pattern: Conditional Field Disabling (Import Flags)

When data is imported from an external system (e.g., GIS), fields should be disabled with a helper message. The API returns `Is{FieldName}Imported` booleans:

**TypeScript** — in `populateForm()`:
```typescript
private populateForm(data: StepDto): void {
    this.form.patchValue({ /* ... */ });

    // Disable GIS-imported fields
    if (data.IsProjectStageImported) {
        this.form.controls.projectStageID.disable();
    } else {
        this.form.controls.projectStageID.enable();
    }
    if (data.IsPlannedDateImported) {
        this.form.controls.plannedDate.disable();
    } else {
        this.form.controls.plannedDate.enable();
    }
}
```

**HTML** — show import notice below the field:
```html
<div class="g-col-6">
    <form-field
        [formControl]="$any(form.controls.projectStageID)"
        fieldDefinitionName="ProjectStage"
        [type]="FormFieldType.Select"
        [formInputOptions]="projectStageOptions"
        placeholder="Select...">
    </form-field>
    @if (vm.data?.IsProjectStageImported) {
        <div class="text-muted small mt-1">
            This field is imported for the program. To edit data, visit the system of record.
        </div>
    }
</div>
```

**Important**: When form has disabled controls, use `form.getRawValue()` (not `form.value`) in `onSave()` to include disabled field values in the request.

### Pattern: Dynamic Validation

Validators that change based on data from the API (not user input):

```typescript
// In populateForm():
if (data.SomeCondition) {
    this.form.controls.conditionalField.setValidators([Validators.required]);
} else {
    this.form.controls.conditionalField.clearValidators();
}
this.form.controls.conditionalField.updateValueAndValidity();
```

### Pattern: Cross-Field Date Validation

Validate date relationships in `onSave()` before calling the API:

```typescript
onSave(navigate: boolean): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    const rawValue = this.form.getRawValue(); // getRawValue() includes disabled fields

    // Cross-field validation
    if (rawValue.startDate && rawValue.endDate && new Date(rawValue.endDate) < new Date(rawValue.startDate)) {
        this.alertService.pushAlert(new Alert("End Date must be on or after Start Date.", AlertContext.Danger, true));
        return;
    }

    // Non-blocking stage-specific warnings (don't prevent save)
    if (rawValue.stageID === StageEnum.Completed && rawValue.completionDate) {
        const completionYear = new Date(rawValue.completionDate).getFullYear();
        if (completionYear > new Date().getFullYear()) {
            this.alertService.pushAlert(new Alert(
                "Since the entity is in the Completed stage, the Completion Date should be in the past.",
                AlertContext.Warning, true));
        }
    }

    const request = { /* map from rawValue */ };
    this.saveStep((id) => this.service.saveUpdateStep(id, request), "Saved.", "Failed.", navigate);
}
```

---

### Key Differences: `<workflow-step-actions>` in Update vs Create

| Input | Create | Update |
|-------|--------|--------|
| `[showRevertActions]` | `false` (or omitted) | `true` |
| `[hasChanges]` | Not needed | `state.hasChanges` from `hasChanges$` |
| `[stepKey]` | Not needed | `stepKey` (PascalCase) |
| `[projectID]` | Not needed | `projectID$ \| async` |
| `(reverted)` | Not needed | `onStepReverted()` |

### Step Type: Collection/List (Update)

```typescript
export class Update{StepName}StepComponent extends UpdateWorkflowStepBase implements OnInit {
    readonly nextStep = "...";
    readonly stepKey = "...";
    items: {ItemType}[] = [];

    ngOnInit(): void {
        this.initProjectID();
        this.initHasChanges();

        // Use stepRefresh$ for data loading
        this.stepRefresh$.pipe(
            switchMap((id) => this.service.getUpdate{StepName}Step(id))
        ).subscribe((data) => {
            this.items = data.Items;
        });
    }

    addItem(): void { /* open modal or inline add */ this.setFormDirty(); }
    removeItem(item): void { /* remove from array */ this.setFormDirty(); }

    onSave(navigate: boolean): void {
        const request = { Items: this.items.map(...) };
        this.saveStep(
            (id) => this.service.saveUpdate{StepName}Step(id, request),
            "Saved.", "Failed.", navigate
        );
    }
}
```

### Step Type: Geographic Assignment (Update)

```typescript
export class Update{StepName}StepComponent extends UpdateWorkflowStepBase implements OnInit {
    readonly nextStep = "...";
    readonly stepKey = "...";
    selectedIDs: number[] = [];
    explanation: string | null = null;

    ngOnInit(): void {
        this.initProjectID();
        this.initHasChanges();
        // Use stepRefresh$ ...
    }

    toggleSelection(id: number): void { ... this.setFormDirty(); }

    onSave(navigate: boolean): void {
        const request = { SelectedIDs: this.selectedIDs, NoSelectionExplanation: this.explanation };
        this.saveStep((id) => this.service.saveUpdate{StepName}Step(id, request), ...);
    }
}
```

### Step Type: Map (Update)

```typescript
import { signal } from "@angular/core";

export class Update{StepName}StepComponent extends UpdateWorkflowStepBase implements OnInit {
    readonly nextStep = "...";
    readonly stepKey = "...";
    mapIsReady = signal(false);

    onMapReady(): void { this.mapIsReady.set(true); }
}
```

### `onStepReverted()` Pattern

All Update step components inherit `onStepReverted()` from `UpdateWorkflowStepBase`. When the revert button is clicked in `<workflow-step-actions>`, it calls the API to revert, then emits `(reverted)`. The parent step calls `onStepReverted()`, which:

1. Triggers `refreshStepData$.next()` — re-fetches step data via `stepRefresh$`
2. Triggers `workflowProgressService.triggerRefresh()` — updates sidebar nav

This is why `stepRefresh$` is critical — it makes the data reload automatically.

---

## 3. ReturnWithCommentsModal

**Directory**: `pages/{entities-kebab}/{entity-kebab}-update-workflow/return-with-comments-modal/`
**Files**: TS + HTML

### TypeScript

```typescript
import { Component, inject, OnInit } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormControl, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { DialogRef } from "@ngneat/dialog";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { {Entity}UpdateReturnRequest } from "src/app/shared/generated/model/{entity-kebab}-update-return-request";

export interface ReturnWithCommentsModalData {
    {entity}Name: string;
}

@Component({
    selector: "return-with-comments-modal",
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormFieldComponent],
    templateUrl: "./return-with-comments-modal.component.html",
})
export class ReturnWithCommentsModalComponent implements OnInit {
    private dialogRef = inject(DialogRef<ReturnWithCommentsModalData, {Entity}UpdateReturnRequest | null>);

    FormFieldType = FormFieldType;
    {entity}Name: string = "";

    // One FormControl per reviewable step
    form = new FormGroup({
        BasicsComment: new FormControl<string>(""),
        // ... one per step that supports reviewer comments
    });

    ngOnInit(): void {
        this.{entity}Name = this.dialogRef.data?.{entity}Name ?? "";
    }

    submit(): void {
        const v = this.form.value;
        const request: {Entity}UpdateReturnRequest = {
            BasicsComment: v.BasicsComment || null,
            // ... map all comment controls, converting empty to null
        };
        this.dialogRef.close(request);
    }

    cancel(): void {
        this.dialogRef.close(null);
    }
}
```

### HTML Template

```html
<div class="modal">
    <div class="modal-header">
        <h3>Return Update for Revisions</h3>
    </div>

    <div class="modal-body">
        <p class="mb-3">Return the update for <strong>{{ {entity}Name }}</strong> to the submitter for revisions. You may optionally provide comments for specific sections.</p>

        <form [formGroup]="form">
            <h4>Group Title</h4>

            <form-field
                [formControl]="form.controls.BasicsComment"
                fieldLabel="Basics"
                [type]="FormFieldType.Textarea"
                placeholder="Optional comments for Basics section...">
            </form-field>

            <!-- Repeat for each reviewable step -->
        </form>
    </div>

    <div class="modal-footer">
        <button type="button" class="btn btn-secondary" (click)="cancel()">Cancel</button>
        <button type="button" class="btn btn-warning" (click)="submit()">Return for Revisions</button>
    </div>
</div>
```

---

## 4. UpdateHistoryModal

**Directory**: `pages/{entities-kebab}/update-history-modal/`
**Files**: TS (inline template)

```typescript
import { Component } from "@angular/core";
import { CommonModule, DatePipe } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { {Entity}UpdateHistoryEntry } from "src/app/shared/generated/model/{entity-kebab}-update-history-entry";

export interface UpdateHistoryModalData {
    entries: {Entity}UpdateHistoryEntry[];
}

@Component({
    selector: "update-history-modal",
    standalone: true,
    imports: [CommonModule, DatePipe],
    template: `
        <div class="modal">
            <div class="modal-header">
                <h4>History</h4>
            </div>
            <div class="modal-body">
                <p>The following is the high-level summary of the history for this update.</p>
                @if (lastEntry) {
                    <p>
                        Last Updated: {{ lastEntry.TransitionDate | date : "M/d/yyyy h:mm a" }}
                        - {{ lastEntry.UpdatePersonName }}
                    </p>
                }
                <hr />
                <table class="table table-sm">
                    <thead>
                        <tr>
                            <th>Date</th>
                            <th>Action</th>
                            <th>User</th>
                        </tr>
                    </thead>
                    <tbody>
                        @for (entry of entries; track entry.TransitionDate) {
                            <tr>
                                <td>{{ entry.TransitionDate | date : "M/d/yyyy h:mm a" }}</td>
                                <td>{{ entry.{Entity}UpdateStateName }}</td>
                                <td>{{ entry.UpdatePersonName }}</td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
            <div class="modal-footer">
                <button class="btn btn-secondary" (click)="dialogRef.close()">Close</button>
            </div>
        </div>
    `,
})
export class UpdateHistoryModalComponent {
    entries: {Entity}UpdateHistoryEntry[];
    lastEntry: {Entity}UpdateHistoryEntry | null;

    constructor(public dialogRef: DialogRef<UpdateHistoryModalData>) {
        this.entries = dialogRef.data.entries;
        this.lastEntry = this.entries.length > 0 ? this.entries[this.entries.length - 1] : null;
    }
}
```

---

## 5. UpdateHistoryDiffModal

**Directory**: `pages/{entities-kebab}/update-history-diff-modal/`
**Files**: TS + HTML + SCSS

This modal shows the full diff summary for a past (historical) update batch. It supports **two rendering modes**: legacy HTML diffs (from MVC-era batches) and structured diffs (from new Angular batches). Use `ViewEncapsulation.None` because legacy HTML diffs contain inline styles that need global access.

### TypeScript

```typescript
import { Component, inject, OnInit, ViewEncapsulation } from "@angular/core";
import { CommonModule } from "@angular/common";
import { DialogRef } from "@ngneat/dialog";
import { {Entity}UpdateDiffSummary } from "src/app/shared/generated/model/{entity-kebab}-update-diff-summary";
import { DiffSection } from "src/app/shared/generated/model/diff-section";
import { StepDiffResponse } from "src/app/shared/generated/model/step-diff-response";

interface LegacySectionDef {
    title: string;
    htmlKey: keyof {Entity}UpdateDiffSummary;
    hasChangesKey: keyof {Entity}UpdateDiffSummary;
}

interface StructuredSectionDef {
    title: string;
    key: string;  // kebab-case step key
}

export interface UpdateHistoryDiffModalData {
    updateDate: string;
    diffSummary: {Entity}UpdateDiffSummary;
}

@Component({
    selector: "update-history-diff-modal",
    standalone: true,
    imports: [CommonModule],
    templateUrl: "./update-history-diff-modal.component.html",
    styleUrls: ["./update-history-diff-modal.component.scss"],
    encapsulation: ViewEncapsulation.None,
})
export class UpdateHistoryDiffModalComponent implements OnInit {
    private dialogRef = inject(DialogRef);

    updateDate: string = "";
    diffSummary: {Entity}UpdateDiffSummary | null = null;
    showDeletions: boolean = true;
    hasStructuredDiffs: boolean = false;

    // Legacy sections: for batches created before structured diffs existed
    legacySections: LegacySectionDef[] = [
        { title: "{Entity} Basics", htmlKey: "BasicsDiffHtml", hasChangesKey: "HasBasicsChanges" },
        // Add more legacy sections as needed for backward compatibility
    ];

    // Structured sections: one per workflow step (kebab-case keys)
    structuredSections: StructuredSectionDef[] = [
        { title: "{Entity} Basics", key: "basics" },
        { title: "Organizations", key: "organizations" },
        { title: "Contacts", key: "contacts" },
        { title: "Expected Funding", key: "expected-funding" },
        { title: "External Links", key: "external-links" },
        { title: "Documents & Notes", key: "documents-notes" },
        { title: "Simple Location", key: "location-simple" },
        { title: "Detailed Location", key: "location-detailed" },
        { title: "Photos", key: "photos" },
        // Add all step keys matching the workflow
    ];

    ngOnInit(): void {
        const data = this.dialogRef.data as UpdateHistoryDiffModalData;
        this.updateDate = data?.updateDate ?? "";
        this.diffSummary = data?.diffSummary ?? null;
        this.hasStructuredDiffs = !!this.diffSummary?.StructuredStepDiffs;
    }

    // --- Legacy helpers ---

    getSectionHtml(section: LegacySectionDef): string | null | undefined {
        return this.diffSummary?.[section.htmlKey] as string | null | undefined;
    }

    hasLegacySectionChanges(section: LegacySectionDef): boolean {
        return !!this.diffSummary?.[section.hasChangesKey];
    }

    // --- Structured helpers ---

    getStepDiff(key: string): StepDiffResponse | null {
        return this.diffSummary?.StructuredStepDiffs?.[key] ?? null;
    }

    hasStepChanges(key: string): boolean {
        return this.getStepDiff(key)?.HasChanges === true;
    }

    getStepSections(key: string): DiffSection[] {
        return this.getStepDiff(key)?.Sections ?? [];
    }

    isFieldChanged(field: { OriginalValue?: string; UpdatedValue?: string }): boolean {
        return (field.OriginalValue ?? "") !== (field.UpdatedValue ?? "");
    }

    isRowInSet(row: string[], rows: string[][]): boolean {
        return rows.some(r => r.length === row.length && r.every((cell, i) => cell === row[i]));
    }

    getAddedItems(section: DiffSection): string[] {
        const original = new Set(section.OriginalItems ?? []);
        return (section.UpdatedItems ?? []).filter(item => !original.has(item));
    }

    getRemovedItems(section: DiffSection): string[] {
        const updated = new Set(section.UpdatedItems ?? []);
        return (section.OriginalItems ?? []).filter(item => !updated.has(item));
    }

    getUnchangedItems(section: DiffSection): string[] {
        const updated = new Set(section.UpdatedItems ?? []);
        return (section.OriginalItems ?? []).filter(item => updated.has(item));
    }

    getRemovedRows(section: DiffSection): string[][] {
        const updated = section.UpdatedRows ?? [];
        return (section.OriginalRows ?? []).filter(row => !this.isRowInSet(row, updated));
    }

    getAddedRows(section: DiffSection): string[][] {
        const original = section.OriginalRows ?? [];
        return (section.UpdatedRows ?? []).filter(row => !this.isRowInSet(row, original));
    }

    getUnchangedRows(section: DiffSection): string[][] {
        const updated = section.UpdatedRows ?? [];
        return (section.OriginalRows ?? []).filter(row => this.isRowInSet(row, updated));
    }

    toggleDeletions(): void {
        this.showDeletions = !this.showDeletions;
    }

    close(): void {
        this.dialogRef.close();
    }
}
```

### HTML Template

```html
<div class="modal update-history-diff-modal">
    <div class="modal-header">
        <h3>{Entity} Update Change Log</h3>
    </div>

    <div class="modal-body">
        <div class="diff-toolbar">
            <span class="diff-subtitle">{Entity} Update from {{ updateDate }}</span>
            <label class="form-check">
                <input
                    type="checkbox"
                    class="form-check-input"
                    [checked]="showDeletions"
                    (change)="toggleDeletions()">
                <span class="form-check-label">Show deletion details</span>
            </label>
        </div>

        <div class="diff-legend">
            <span class="legend-item legend-added">Added</span>
            <span class="legend-item legend-deleted">Deleted</span>
            <span class="legend-item legend-unchanged">Unchanged</span>
        </div>

        @if (hasStructuredDiffs) {
            <!-- Structured rendering: one panel per step -->
            <div [class.hide-deletions]="!showDeletions">
                @for (step of structuredSections; track step.key) {
                    <div class="diff-section-panel">
                        <div class="diff-section-header">
                            <span>{{ step.title }}</span>
                            @if (hasStepChanges(step.key)) {
                                <span class="diff-badge diff-badge-changed">Changed</span>
                            } @else {
                                <span class="diff-badge diff-badge-unchanged">No Changes</span>
                            }
                        </div>
                        <div class="diff-section-body">
                            @if (hasStepChanges(step.key)) {
                                @for (section of getStepSections(step.key); track section.Title ?? $index) {
                                    @if (section.Title) {
                                        <h5>{{ section.Title }}</h5>
                                    }

                                    <!-- Fields section -->
                                    @if (section.Type === 'fields') {
                                        <table class="diff-fields-table">
                                            <thead>
                                                <tr><th>Field</th><th>Original</th><th>Updated</th></tr>
                                            </thead>
                                            <tbody>
                                                @for (field of section.Fields; track field.Label) {
                                                    <tr [class.diff-changed]="isFieldChanged(field)">
                                                        <td class="diff-label">{{ field.Label }}</td>
                                                        <td [class.diff-deleted]="isFieldChanged(field)">{{ field.OriginalValue }}</td>
                                                        <td [class.diff-added]="isFieldChanged(field)">{{ field.UpdatedValue }}</td>
                                                    </tr>
                                                }
                                            </tbody>
                                        </table>
                                    }

                                    <!-- Table section -->
                                    @if (section.Type === 'table') {
                                        <table class="diff-table-section">
                                            <thead>
                                                <tr>
                                                    @for (header of section.Headers; track header) {
                                                        <th>{{ header }}</th>
                                                    }
                                                </tr>
                                            </thead>
                                            <tbody>
                                                @for (row of getUnchangedRows(section); track $index) {
                                                    <tr>
                                                        @for (cell of row; track $index) { <td>{{ cell }}</td> }
                                                    </tr>
                                                }
                                                @for (row of getAddedRows(section); track $index) {
                                                    <tr class="diff-row-added">
                                                        @for (cell of row; track $index) { <td>{{ cell }}</td> }
                                                    </tr>
                                                }
                                                @for (row of getRemovedRows(section); track $index) {
                                                    <tr class="diff-row-deleted">
                                                        @for (cell of row; track $index) { <td>{{ cell }}</td> }
                                                    </tr>
                                                }
                                                @if ((section.OriginalRows?.length ?? 0) === 0 && (section.UpdatedRows?.length ?? 0) === 0) {
                                                    <tr>
                                                        <td [attr.colspan]="section.Headers?.length ?? 1" class="diff-empty">
                                                            <em>None</em>
                                                        </td>
                                                    </tr>
                                                }
                                            </tbody>
                                        </table>
                                    }

                                    <!-- List section -->
                                    @if (section.Type === 'list') {
                                        <ul class="diff-list">
                                            @for (item of getUnchangedItems(section); track item) {
                                                <li>{{ item }}</li>
                                            }
                                            @for (item of getAddedItems(section); track item) {
                                                <li class="diff-item-added">{{ item }}</li>
                                            }
                                            @if (showDeletions) {
                                                @for (item of getRemovedItems(section); track item) {
                                                    <li class="diff-item-deleted">{{ item }}</li>
                                                }
                                            }
                                            @if ((section.OriginalItems?.length ?? 0) === 0 && (section.UpdatedItems?.length ?? 0) === 0) {
                                                <li class="diff-empty"><em>None</em></li>
                                            }
                                        </ul>
                                    }
                                }
                            } @else {
                                <p class="diff-no-changes"><em>No changes to {{ step.title }} section</em></p>
                            }
                        </div>
                    </div>
                }
            </div>
        } @else {
            <!-- Legacy HTML fallback: sections with [innerHTML] -->
            <div [class.hide-deletions]="!showDeletions">
                @for (section of legacySections; track section.title) {
                    <div class="diff-section-panel">
                        <div class="diff-section-header">
                            <span>{{ section.title }}</span>
                            @if (hasLegacySectionChanges(section)) {
                                <span class="diff-badge diff-badge-changed">Changed</span>
                            } @else {
                                <span class="diff-badge diff-badge-unchanged">No Changes</span>
                            }
                        </div>
                        <div class="diff-section-body">
                            @if (hasLegacySectionChanges(section)) {
                                <div [innerHTML]="getSectionHtml(section)"></div>
                            } @else {
                                <p class="diff-no-changes"><em>No changes to {{ section.title }} section</em></p>
                            }
                        </div>
                    </div>
                }
            </div>
        }
    </div>

    <div class="modal-footer">
        <button type="button" class="btn btn-primary" (click)="close()">Close</button>
    </div>
</div>
```

### SCSS

```scss
.update-history-diff-modal {
    .diff-toolbar {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 1rem;
    }

    .diff-subtitle {
        font-weight: 600;
    }

    .diff-legend {
        display: flex;
        gap: 1rem;
        margin-bottom: 1rem;
        font-size: 0.85rem;

        .legend-added { color: #28a745; }
        .legend-deleted { color: #dc3545; text-decoration: line-through; }
        .legend-unchanged { color: #6c757d; }
    }

    .diff-section-panel {
        border: 1px solid #dee2e6;
        border-radius: 0.25rem;
        margin-bottom: 0.75rem;
    }

    .diff-section-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 0.5rem 0.75rem;
        background-color: #f8f9fa;
        border-bottom: 1px solid #dee2e6;
        font-weight: 600;
    }

    .diff-badge {
        font-size: 0.75rem;
        padding: 0.15em 0.5em;
        border-radius: 0.25rem;

        &.diff-badge-changed { background-color: #ffc107; color: #212529; }
        &.diff-badge-unchanged { background-color: #e9ecef; color: #6c757d; }
    }

    .diff-section-body {
        padding: 0.75rem;
    }

    .diff-fields-table, .diff-table-section {
        width: 100%;
        border-collapse: collapse;
        margin-bottom: 0.5rem;

        th, td {
            padding: 0.35rem 0.5rem;
            border: 1px solid #dee2e6;
            font-size: 0.875rem;
        }

        th { background-color: #f8f9fa; font-weight: 600; }
    }

    .diff-added { background-color: #d4edda; }
    .diff-deleted { background-color: #f8d7da; text-decoration: line-through; }
    .diff-changed td.diff-label { font-weight: 600; }
    .diff-row-added { background-color: #d4edda; }
    .diff-row-deleted { background-color: #f8d7da; text-decoration: line-through; }
    .diff-item-added { color: #28a745; }
    .diff-item-deleted { color: #dc3545; text-decoration: line-through; }
    .diff-empty { color: #6c757d; font-style: italic; }
    .diff-no-changes { color: #6c757d; }

    .diff-list {
        list-style: none;
        padding-left: 0;
        li { padding: 0.15rem 0; }
    }

    .hide-deletions {
        .diff-row-deleted, .diff-item-deleted { display: none; }
        td.diff-deleted { text-decoration: none; background-color: transparent; }
    }
}
```

### Key Design Decisions

1. **Dual rendering modes**: `hasStructuredDiffs` switches between legacy `[innerHTML]` and structured `@for` rendering. Legacy mode is needed for historical batches created before the structured diff system existed.
2. **`ViewEncapsulation.None`**: Required because legacy HTML diffs contain inline styles and class names that need global CSS access.
3. **Toggle deletions**: The `showDeletions` checkbox and `.hide-deletions` class let users focus on additions only.
4. **Three section types**: Same `fields`/`table`/`list` types as `StepDiffModal`, but rendered for all steps at once (not per-step).

### Backend DTO: {Entity}UpdateDiffSummary

```csharp
public class {Entity}UpdateDiffSummary
{
    // Legacy HTML diff fields (for backward compatibility)
    public string? BasicsDiffHtml { get; set; }
    public bool HasBasicsChanges { get; set; }
    // ... more legacy fields as needed

    // Structured diffs (new system)
    public Dictionary<string, StepDiffResponse>? StructuredStepDiffs { get; set; }
}
```

---

## 6. Wiring the UpdateHistoryDiffModal

The `UpdateHistoryDiffModal` is opened from the `UpdateHistoryModal` when a user clicks on a specific batch entry to see its full diff. Add a "View Changes" link/button in the history entries table that fetches the batch diff and opens the modal:

```typescript
// In UpdateHistoryModalComponent — add a method to view a batch's diff:
viewBatchDiff(batchID: number, updateDate: string): void {
    this.{entity}Service.getUpdateBatchDiffSummary{Entity}(batchID).subscribe((diffSummary) => {
        this.dialogService.open(UpdateHistoryDiffModalComponent, {
            data: { updateDate, diffSummary } as UpdateHistoryDiffModalData,
            size: "xl",
        });
    });
}
```

Alternatively, this can be opened directly from the outlet's `openHistoryModal()` if the history modal embeds diff viewing inline. Match the pattern used in the source project.

---

## 7. Route Configuration

**File**: `app.routes.ts`

### Update Workflow Route

```typescript
{
    path: "{entities-kebab}/:{entity}ID/update",
    title: "Update {Entity}",
    canActivate: [projectEditGuard],
    loadComponent: () =>
        import("./pages/{entities-kebab}/{entity-kebab}-update-workflow/{entity-kebab}-update-workflow-outlet.component").then(
            (m) => m.{Entity}UpdateWorkflowOutletComponent
        ),
    children: [
        { path: "", redirectTo: "basics", pathMatch: "full" },
        {
            path: "basics",
            canDeactivate: [UnsavedChangesGuard],
            loadComponent: () =>
                import("./pages/{entities-kebab}/{entity-kebab}-update-workflow/steps/basics/update-basics-step.component").then(
                    (m) => m.UpdateBasicsStepComponent
                ),
        },
        {
            path: "step-two-route",
            canDeactivate: [UnsavedChangesGuard],
            loadComponent: () =>
                import("./pages/{entities-kebab}/{entity-kebab}-update-workflow/steps/step-two/update-step-two-step.component").then(
                    (m) => m.UpdateStepTwoStepComponent
                ),
        },
        // ... all remaining steps
    ],
},
```

### Route Pattern

- URL: `/{entities-kebab}/:id/update/{step-route}`
- Outlet component receives `:id` via `@Input() {entity}ID`
- Step components receive it via router inheritance (through the base class)
- `canActivate: [projectEditGuard]` on the outlet route
- `canDeactivate: [UnsavedChangesGuard]` on every step child route
- Step component names: `Update{StepName}StepComponent` (prefix with `Update`)
- Step filenames: `update-{step-route}-step.component.ts`

---

## 8. Critical Pattern Summary: `stepRefresh$`

The most important pattern difference between Create and Update workflow steps:

```typescript
// CREATE: Data loads once when projectID arrives
this.data$ = this.projectID$.pipe(
    switchMap((id) => this.service.getStep(id))
);

// UPDATE: Data reloads automatically after save/revert via stepRefresh$
this.data$ = this.stepRefresh$.pipe(
    switchMap((id) => this.service.getUpdateStep(id))
);
```

This works because `stepRefresh$` re-emits the projectID whenever:
1. `saveStep()` completes successfully (calls `refreshStepData$.next()`)
2. `onStepReverted()` is called (also calls `refreshStepData$.next()`)

Without this pattern, data would be stale after save/revert until the user navigates away and back.

---

## 9. Checklist

- [ ] Outlet component with sidebar nav, step groups, progress observable
- [ ] Batch state alerts (Submitted = info, Returned = warning)
- [ ] `isStepHasChanges()` bound to `[hasChanges]` on nav items
- [ ] `canEdit()` / `canSubmit()` / `canApprove()` / `canReturn()` / `isReadyToApprove()` helpers
- [ ] Submit via `AsyncConfirmModal`, Approve via `AsyncConfirmModal`, Delete via `AsyncConfirmModal`
- [ ] Return via custom `ReturnWithCommentsModal`
- [ ] History modal via `UpdateHistoryModalComponent`
- [ ] History diff modal via `UpdateHistoryDiffModalComponent` (both legacy + structured rendering)
- [ ] Actions dropdown: Go back, Show History, My {Entities} list, Provide Feedback, Delete
- [ ] `FeedbackModalComponent` imported and wired to actions dropdown
- [ ] Footer shows submitted/returned metadata + action buttons
- [ ] Footer: disabled Submit when `formDirty$`, disabled Approve when not `isReadyToApprove`
- [ ] Per-step components extending `UpdateWorkflowStepBase`
- [ ] Each step: `initProjectID()`, `initHasChanges()`, `trackFormDirty(form)` (or `setFormDirty()`)
- [ ] Each step: data loaded via `stepRefresh$` (NOT `projectID$`)
- [ ] Each step: `<workflow-step-actions [showRevertActions]="true" ...>`
- [ ] Each step: `reviewerComment$` displayed when present
- [ ] Each step: `onSave(navigate)` calling `this.saveStep()`
- [ ] Map steps: `mapIsReady = signal(false)`
- [ ] `ReturnWithCommentsModalComponent` with per-step comment fields
- [ ] `UpdateHistoryModalComponent` with entries table
- [ ] `UpdateHistoryDiffModalComponent` with dual-mode rendering (legacy HTML + structured diffs)
- [ ] Routes at `/{entities}/:id/update` with all step children
- [ ] All routes have `canActivate` and step routes have `canDeactivate`
- [ ] No Bootstrap classes in templates
- [ ] `<form-field>` used for all form inputs
