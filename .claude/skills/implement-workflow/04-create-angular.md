# 04 — Create Workflow: Angular Frontend

> **Purpose**: Create workflow frontend — outlet component, step components, route configuration.
> **Prereq**: API must be built and `npm run gen-model` must have been run first.

---

## 1. Outlet Component

**Directory**: `pages/{entities-kebab}/{entity-kebab}-create-workflow/`
**Files**: TS + HTML + SCSS

**Pattern from**: `WADNR.Web/src/app/pages/projects/project-create-workflow/`

### TypeScript

```typescript
@Component({
    selector: "{entity-kebab}-create-workflow-outlet",
    standalone: true,
    imports: [
        CommonModule, AsyncPipe, DatePipe, RouterOutlet, RouterLink,
        BreadcrumbComponent, PageHeaderComponent,
        WorkflowNavComponent, WorkflowNavItemComponent, WorkflowNavGroupComponent,
        IconComponent, DropdownToggleDirective,
    ],
    templateUrl: "./{entity-kebab}-create-workflow-outlet.component.html",
    styleUrls: ["./{entity-kebab}-create-workflow-outlet.component.scss"],
})
export class {Entity}CreateWorkflowOutletComponent implements OnInit {
    @Input() set {entity}ID(value: string | number | undefined) { ... }
    private _{entity}ID$ = new BehaviorSubject<number | null>(null);
```

### Key Properties

| Property | Type | Description |
|----------|------|-------------|
| `{entity}ID$` | `Observable<number>` | Filtered non-null entity ID |
| `progress$` | `Observable<CreateWorkflowProgressResponse \| null>` | Re-fetches on `refreshProgress$` |
| `vm$` | `Observable<{ isNew: boolean; progress: ... }>` | Combined template view model |
| `formDirty$` | `Observable<boolean>` | From `WorkflowProgressService` |
| `stepGroups` | `WorkflowStepGroup[]` | Static step group definitions |

### Interfaces

Define at top of file:

```typescript
export interface WorkflowStep {
    key: string;    // PascalCase key matching backend enum
    name: string;   // Display name
    route: string;  // kebab-case route segment
    required: boolean;
}

export interface WorkflowStepGroup {
    title: string;
    steps: WorkflowStep[];
}
```

### stepGroups Array

```typescript
public stepGroups: WorkflowStepGroup[] = [
    {
        title: "Group Title",
        steps: [
            { key: "StepKey", name: "Display Name", route: "step-route", required: true },
            // ...
        ],
    },
    // ... more groups
];
```

### Progress$ Pattern

```typescript
this.progress$ = this._{entity}ID$.pipe(
    switchMap(({entity}ID) => {
        if ({entity}ID == null || Number.isNaN({entity}ID)) return of(null);
        return this.workflowProgressService.refreshProgress$.pipe(
            startWith(undefined),
            switchMap(() => this.{entity}Service.getCreateWorkflowProgress{Entity}({entity}ID))
        );
    }),
    shareReplay({ bufferSize: 1, refCount: true })
);
```

### Template Helper Methods

```typescript
getStepLink(step: WorkflowStep): string[] {
    const {entity}ID = this._{entity}ID$.getValue();
    if ({entity}ID) return ["/{entities}", "edit", {entity}ID.toString(), step.route];
    return ["/{entities}", "new", step.route];
}

isStepComplete(progress, step): boolean { return progress?.Steps?.[step.key]?.IsComplete ?? false; }
isStepDisabled(progress, step, isNew): boolean { ... }
isStepRequired(step): boolean { return step.required; }
isGroupComplete(progress, group): boolean { return group.steps.every(...); }
getGroupChildRoutes(group): string[] { return group.steps.map(s => s.route); }
```

### Constructor Dependencies

```typescript
constructor(
    private {entity}Service: {Entity}Service,
    private router: Router,
    private alertService: AlertService,
    private confirmService: ConfirmService,
    private dialogService: DialogService,
    private workflowProgressService: WorkflowProgressService
) {}
```

### vm$ Pattern

Combines projectID + progress for the template, emitting immediately with a default "new project" state:

```typescript
this.vm$ = combineLatest([this._{entity}ID$, this.progress$.pipe(startWith(null))]).pipe(
    map(([{entity}ID, progress]) => ({
        isNewProject: {entity}ID == null || Number.isNaN({entity}ID),
        progress,
    })),
    startWith({ isNewProject: true, progress: null as CreateWorkflowProgressResponse | null }),
    shareReplay({ bufferSize: 1, refCount: true })
);
```

### State Transition Methods

The Create workflow uses `ConfirmService` (promise-based) for all state transitions. Each method follows the same pattern: confirm dialog → API call → alert → navigate.

```typescript
submitForApproval({entity}ID: number, name: string): void {
    this.confirmService
        .confirm({
            title: "Submit Proposal for review",
            message: `Are you sure you want to submit {Entity} "${name}" to the reviewer?`,
            buttonTextYes: "Continue",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-primary",
        })
        .then((confirmed) => {
            if (confirmed) {
                this.{entity}Service.submitCreateForApproval{Entity}({entity}ID, {}).subscribe({
                    next: () => {
                        this.alertService.pushAlert(new Alert("{Entity} submitted for approval successfully.", AlertContext.Success, true));
                        this.workflowProgressService.triggerRefresh();
                        this.router.navigate(["/{entities}", {entity}ID]);
                    },
                    error: (err) => {
                        const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to submit.";
                        this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                    },
                });
            }
        });
}

approveProposal({entity}ID: number, name: string): void {
    this.confirmService
        .confirm({
            title: "Approve Proposal",
            message: `Are you sure you want to approve {Entity} "${name}"?`,
            buttonTextYes: "Approve",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-success",
        })
        .then((confirmed) => {
            if (confirmed) {
                this.{entity}Service.approveCreate{Entity}({entity}ID, {}).subscribe({
                    next: () => {
                        this.alertService.pushAlert(new Alert("{Entity} approved successfully.", AlertContext.Success, true));
                        this.workflowProgressService.triggerRefresh();
                        this.router.navigate(["/{entities}", {entity}ID]);
                    },
                    error: (err) => {
                        const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to approve.";
                        this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                    },
                });
            }
        });
}

returnProposal({entity}ID: number, name: string): void {
    this.confirmService
        .confirm({
            title: "Return Proposal",
            message: `Are you sure you want to return {Entity} "${name}" to the submitter for revisions?`,
            buttonTextYes: "Return",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-warning",
        })
        .then((confirmed) => {
            if (confirmed) {
                this.{entity}Service.returnCreate{Entity}({entity}ID, {}).subscribe({
                    next: () => {
                        this.alertService.pushAlert(new Alert("{Entity} returned for revisions.", AlertContext.Success, true));
                        this.workflowProgressService.triggerRefresh();
                    },
                    error: (err) => {
                        const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to return.";
                        this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                    },
                });
            }
        });
}

rejectProposal({entity}ID: number, name: string): void {
    this.confirmService
        .confirm({
            title: "Reject Proposal",
            message: `Are you sure you want to reject {Entity} "${name}"? This action cannot be undone.`,
            buttonTextYes: "Reject",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-danger",
        })
        .then((confirmed) => {
            if (confirmed) {
                this.{entity}Service.rejectCreate{Entity}({entity}ID, {}).subscribe({
                    next: () => {
                        this.alertService.pushAlert(new Alert("{Entity} rejected.", AlertContext.Success, true));
                        this.workflowProgressService.triggerRefresh();
                        this.router.navigate(["/{entities}", {entity}ID]);
                    },
                    error: (err) => {
                        const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to reject.";
                        this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                    },
                });
            }
        });
}

withdrawProposal(): void {
    const {entity}ID = this._{entity}ID$.getValue();
    if (!{entity}ID) return;

    this.confirmService
        .confirm({
            title: "Withdraw Proposal",
            message: "Are you sure you want to withdraw this proposal? This will return the {entity} to Draft status.",
            buttonTextYes: "Withdraw",
            buttonTextNo: "Cancel",
            buttonClassYes: "btn-danger",
        })
        .then((confirmed) => {
            if (confirmed) {
                this.{entity}Service.withdrawCreate{Entity}({entity}ID, {}).subscribe({
                    next: () => {
                        this.alertService.pushAlert(new Alert("Proposal withdrawn successfully.", AlertContext.Success, true));
                        this.workflowProgressService.triggerRefresh();
                        this.router.navigate(["/{entities}", {entity}ID]);
                    },
                    error: (err) => {
                        const message = err?.error?.ErrorMessage ?? err?.error ?? "Failed to withdraw.";
                        this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                    },
                });
            }
        });
}
```

### Status Helper Methods

```typescript
canWithdraw(progress: CreateWorkflowProgressResponse | null): boolean {
    return progress?.{Entity}ApprovalStatusID === {Entity}ApprovalStatusEnum.PendingApproval;
}

canApprove(progress: CreateWorkflowProgressResponse | null): boolean {
    return progress?.CanApprove ?? false;
}

canReturn(progress: CreateWorkflowProgressResponse | null): boolean {
    return progress?.CanReturn ?? false;
}

canReject(progress: CreateWorkflowProgressResponse | null): boolean {
    return progress?.CanReject ?? false;
}

onProjectCreated({entity}ID: number): void {
    this.router.navigate(["/{entities}", "edit", {entity}ID, "location-simple"]);
}

openFeedbackModal(): void {
    this.dialogService.open(FeedbackModalComponent, {
        data: { currentPageUrl: window.location.href },
        size: "md",
    });
}
```

### HTML Template Structure

```html
<breadcrumb></breadcrumb>

@if (vm$ | async; as vm) {
    <page-header [pageTitle]="vm.isNewProject ? 'New {Entity}' : (vm.progress?.{Entity}Name ?? 'Edit {Entity}')" [templateRight]="actionsTemplate">
    </page-header>

    <ng-template #actionsTemplate>
        <div class="actions-dropdown" [dropdownToggle]="actionsMenu">
            <a href="javascript:void(0);" class="actions-toggle">
                See more Actions <icon icon="AngleDown"></icon>
            </a>
            <ul #actionsMenu class="dropdown-menu dropdown-menu-right">
                @if (!vm.isNewProject && vm.progress) {
                    <li>
                        <a [routerLink]="['/{entities}', vm.progress.{Entity}ID]" class="dropdown-item">
                            {Entity} Details
                        </a>
                    </li>
                }
                <li>
                    <a href="javascript:void(0);" (click)="openFeedbackModal()" class="dropdown-item">
                        Provide Feedback
                    </a>
                </li>
                @if (vm.progress && canWithdraw(vm.progress)) {
                    <li class="divider"></li>
                    <li>
                        <a href="javascript:void(0);" (click)="withdrawProposal()" class="dropdown-item text-danger">
                            Withdraw Proposal
                        </a>
                    </li>
                }
            </ul>
        </div>
    </ng-template>

    <div class="dashboard workflow">
        <div class="sidebar">
            <div class="sidebar-body sticky-nav">
                <workflow-nav>
                    @for (group of stepGroups; track group.title) {
                        <workflow-nav-group
                            [title]="group.title"
                            [expanded]="false"
                            [complete]="!vm.isNewProject && isGroupComplete(vm.progress, group)"
                            [childRoutes]="getGroupChildRoutes(group)">
                            @for (step of group.steps; track step.key) {
                                <workflow-nav-item
                                    [navRouterLink]="getStepLink(step)"
                                    [complete]="!vm.isNewProject && isStepComplete(vm.progress, step)"
                                    [disabled]="isStepDisabled(vm.progress, step, vm.isNewProject)"
                                    [required]="isStepRequired(step)">
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

            <!-- Workflow Footer: Status + Action Buttons -->
            @if (!vm.isNewProject && vm.progress) {
                <div class="workflow-footer">
                    <div class="workflow-footer__info">
                        @if (vm.progress.CreatedByPersonName) {
                            <span class="workflow-footer__created">
                                Created by {{ vm.progress.CreatedByPersonName }}
                                @if (vm.progress.CreatedByOrganizationName) {
                                    ({{ vm.progress.CreatedByOrganizationName }})
                                }
                                @if (vm.progress.CreateDate) {
                                    on {{ vm.progress.CreateDate | date:'M/d/yyyy h:mm:ss a' }}
                                }
                            </span>
                        }
                    </div>
                    <div class="workflow-footer__status">
                        <span class="workflow-footer__status-label">
                            New {Entity} Status: <strong>{{ vm.progress.{Entity}ApprovalStatusName }}</strong>
                        </span>
                        @if (vm.progress.CanSubmit) {
                            <button class="btn btn-primary"
                                [disabled]="formDirty$ | async"
                                [title]="(formDirty$ | async) ? 'Save your changes before submitting' : ''"
                                (click)="submitForApproval(vm.progress.{Entity}ID, vm.progress.{Entity}Name)">
                                Submit
                            </button>
                        }
                        @if (canReturn(vm.progress)) {
                            <button class="btn btn-warning me-2" (click)="returnProposal(vm.progress.{Entity}ID, vm.progress.{Entity}Name)">
                                Return
                            </button>
                        }
                        @if (canReject(vm.progress)) {
                            <button class="btn btn-danger me-2" (click)="rejectProposal(vm.progress.{Entity}ID, vm.progress.{Entity}Name)">
                                Reject
                            </button>
                        }
                        @if (canApprove(vm.progress)) {
                            <button class="btn btn-success" (click)="approveProposal(vm.progress.{Entity}ID, vm.progress.{Entity}Name)">
                                Approve
                            </button>
                        }
                    </div>
                </div>
            }
        </div>
    </div>
}
```

### Key Differences from Update Outlet Template

| Feature | Create Outlet | Update Outlet |
|---------|--------------|---------------|
| Top-level observable | `vm$` (combines `isNewProject` + `progress`) | `progress$` directly |
| Actions dropdown | {Entity} Details, Feedback, Withdraw | Go back, History, My {Entities} list, Feedback, Delete |
| Batch state alerts | None (approval is simpler) | Submitted (info) + Returned (warning) alerts |
| Footer info section | Created by person + org + date | Submitted/Returned by person + date |
| Footer status display | Approval status name label | None (state shown in alerts) |
| Submit disabled | `formDirty$` | `formDirty$` |
| Reviewer actions | Approve, Return, Reject (simple confirms) | Approve (conditional), Return (comments modal) |
| `nav-item [hasChanges]` | Not used | `isStepHasChanges(progress, step)` |

### SCSS

Use the standard workflow SCSS from the project. Key classes: `.dashboard.workflow`, `.sidebar`, `.sidebar-body.sticky-nav`, `.main`, `.outlet-container`, `.workflow-footer`.

**Reference**: Read `WADNR.Web/src/app/pages/projects/project-create-workflow/project-create-workflow-outlet.component.scss` for exact styles.

---

## 2. Step Components

**Directory**: `pages/{entities-kebab}/{entity-kebab}-create-workflow/steps/{step-route}/`
**Files**: TS + HTML per step

### Common Pattern: All Steps

```typescript
@Component({
    selector: "{entity-kebab}-{step-route}",
    standalone: true,
    imports: [
        CommonModule, AsyncPipe, ReactiveFormsModule,
        PageHeaderComponent, WorkflowStepActionsComponent, AlertDisplayComponent,
        FormFieldComponent, // etc.
    ],
    templateUrl: "./{step-route}.component.html",
})
export class {StepName}Component extends CreateWorkflowStepBase implements OnInit {
    readonly nextStep = "{next-step-route}";  // Route of the NEXT step

    // Step-specific observables
    stepData$: Observable<{StepName}Step>;

    ngOnInit(): void {
        this.initProjectID();

        this.stepData$ = this.projectID$.pipe(
            switchMap((id) => this.{entity}Service.getCreate{StepName}Step{Entity}(id)),
            shareReplay({ bufferSize: 1, refCount: true })
        );

        // For form steps: build form and track dirty
        this.stepData$.pipe(take(1)).subscribe((data) => {
            this.form.patchValue(data);
            this.trackFormDirty(this.form);
        });
    }

    onSave(navigate: boolean): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }
        const request = { ... };
        this.saveStep(
            (id) => this.{entity}Service.saveCreate{StepName}Step{Entity}(id, request),
            "Saved successfully.",
            "Failed to save.",
            navigate
        );
    }
}
```

### Step Type: Form

```html
@if (vm$ | async; as vm) {
    @if ({ saving: isSaving$ | async }; as state) {
    <div class="card">
        <div class="card-header">
            <span class="card-title">Section Title</span>
        </div>
        <div class="card-body">
            <form [formGroup]="form">
                <div class="grid-12">
                    <div class="g-col-6">
                        <form-field
                            [formControl]="$any(form.controls.fieldName)"
                            fieldDefinitionName="FieldName"
                            [type]="FormFieldType.Text"
                            placeholder="Enter value">
                        </form-field>
                    </div>
                    <div class="g-col-6">
                        <form-field
                            [formControl]="$any(form.controls.lookupID)"
                            fieldDefinitionName="LookupName"
                            [type]="FormFieldType.Select"
                            [formInputOptions]="vm.lookupOptions"
                            placeholder="Select...">
                        </form-field>
                    </div>
                    <div class="g-col-6">
                        <form-field
                            [formControl]="$any(form.controls.dateField)"
                            fieldDefinitionName="DateField"
                            [type]="FormFieldType.Date">
                        </form-field>
                    </div>
                </div>
            </form>
        </div>
        <div class="card-footer">
            <workflow-step-actions
                [isSaving]="state.saving"
                (save)="onSave(false)"
                (saveAndContinue)="onSave(true)">
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

**Template conventions**:
- Use `$any(form.controls.fieldName)` cast to avoid strict template type errors
- Use `fieldDefinitionName` (PascalCase) to get label + help icon from the FieldDefinition system
- Use `fieldLabel` (string) when a custom label is needed without a FieldDefinition
- Actions go inside `card-footer`, not outside the card
- Wrap with `@if (vm$ | async)` and secondary `@if ({ saving: isSaving$ | async }; as state)` for reactive state

### Step Type: Collection/List

```typescript
export class {StepName}Component extends CreateWorkflowStepBase implements OnInit {
    readonly nextStep = "...";
    items: {ItemType}[] = [];

    ngOnInit(): void {
        this.initProjectID();
        this.projectID$.pipe(
            switchMap((id) => this.service.get{StepName}Step(id))
        ).subscribe((data) => {
            this.items = data.Items;
        });
    }

    addItem(): void { /* open modal or inline add */ this.setFormDirty(); }
    removeItem(item): void { /* remove from array */ this.setFormDirty(); }

    onSave(navigate: boolean): void {
        const request = { Items: this.items.map(...) };
        this.saveStep(
            (id) => this.service.save{StepName}Step(id, request),
            "Saved.", "Failed.", navigate
        );
    }
}
```

### Step Type: Geographic Assignment

```typescript
export class {StepName}Component extends CreateWorkflowStepBase implements OnInit {
    readonly nextStep = "...";
    selectedIDs: number[] = [];
    explanation: string | null = null;
    availableOptions: GeographicLookupItem[] = [];

    // Toggle checkbox handler
    toggleSelection(id: number): void { ... this.setFormDirty(); }

    onSave(navigate: boolean): void {
        const request: GeographicOverrideRequest = {
            SelectedIDs: this.selectedIDs,
            NoSelectionExplanation: this.explanation
        };
        this.saveStep((id) => this.service.save{StepName}Step(id, request), ...);
    }
}
```

### Step Type: Map

For map steps, use `signal(false)` for `mapIsReady`:

```typescript
import { signal } from "@angular/core";

export class {StepName}Component extends CreateWorkflowStepBase implements OnInit {
    readonly nextStep = "...";
    mapIsReady = signal(false);

    onMapReady(): void {
        this.mapIsReady.set(true);
    }
}
```

```html
<wadnr-map (mapReady)="onMapReady()" ...></wadnr-map>

@if (mapIsReady()) {
    <!-- Map controls, overlays -->
}
```

### Pattern: Multi-Select List Management

For steps where users add/remove items from a list (e.g., Programs on a Basics step), use a `BehaviorSubject`-backed list with a dropdown for adding:

**TypeScript**:
```typescript
// Selected items state
private _selectedItems$ = new BehaviorSubject<ItemRow[]>([]);
public selectedItems$ = this._selectedItems$.asObservable();

// Available options = all options minus already selected
public availableOptions$: Observable<FormInputOption[]>;
public canAddItem$: Observable<boolean>;

// In ngOnInit():
this.availableOptions$ = combineLatest([this.allOptions$, this._selectedItems$]).pipe(
    map(([all, selected]) => {
        const selectedIDs = selected.map(s => s.ItemID);
        return all.filter(opt => !selectedIDs.includes(opt.Value as number));
    }),
    shareReplay({ bufferSize: 1, refCount: true })
);

this.canAddItem$ = this.form.controls.itemToAdd.valueChanges.pipe(
    startWith(this.form.controls.itemToAdd.value),
    map(value => value != null),
    shareReplay({ bufferSize: 1, refCount: true })
);

// In populateForm():
this._selectedItems$.next((data.ItemIDs ?? []).map(id => {
    const option = allOptions.find(o => o.Value === id);
    return { ItemID: id, ItemName: option?.Label ?? `Item ${id}` } as ItemRow;
}));

// Add/remove methods:
onItemSelect(event: any, allOptions: FormInputOption[]): void {
    const itemToAdd = event?.Value ?? event;
    if (itemToAdd == null) return;
    const currentIDs: number[] = this.form.value.itemIDs ?? [];
    if (!currentIDs.includes(itemToAdd)) {
        this.form.patchValue({ itemIDs: [...currentIDs, itemToAdd] });
        const option = allOptions.find(o => o.Value === itemToAdd);
        this._selectedItems$.next([
            ...this._selectedItems$.value,
            { ItemID: itemToAdd, ItemName: option?.Label ?? `Item ${itemToAdd}` } as ItemRow
        ]);
    }
    this.form.controls.itemToAdd.reset();
}

removeItem(itemID: number): void {
    const currentIDs: number[] = this.form.value.itemIDs ?? [];
    this.form.patchValue({ itemIDs: currentIDs.filter(id => id !== itemID) });
    this._selectedItems$.next(this._selectedItems$.value.filter(i => i.ItemID !== itemID));
    this.setFormDirty();
}
```

**HTML**:
```html
@if ({ items: selectedItems$ | async, available: availableOptions$ | async }; as state) {
    <div class="g-col-6">
        <form-field
            [formControl]="$any(form.controls.itemToAdd)"
            fieldLabel="Item to Add"
            [type]="FormFieldType.Select"
            [formInputOptions]="state.available"
            placeholder="Select an Item to Add"
            (change)="onItemSelect($event, vm.allOptions)">
        </form-field>
    </div>

    <div class="g-col-12">
        <label class="field-label">Items</label>
        @if (state.items && state.items.length > 0) {
            <ul class="program-list">
                @for (item of state.items; track item.ItemID) {
                    <li class="program-item">
                        <button type="button" class="btn btn-sm btn-link text-danger"
                            (click)="removeItem(item.ItemID)" title="Remove">
                            <icon icon="Delete"></icon>
                        </button>
                        <span>{{ item.ItemName }}</span>
                    </li>
                }
            </ul>
        } @else {
            <div class="text-muted">No items added.</div>
        }
    </div>
}
```

### Pattern: Optional Fields Accordion

Group non-required form fields in a collapsible `<details>` section:

```html
<div class="g-col-12">
    <details class="optional-fields-section">
        <summary class="optional-fields-toggle">Additional Optional Fields</summary>
        <div class="optional-fields-content grid-12">
            <!-- optional form fields go here -->
            <div class="g-col-6">
                <form-field
                    [formControl]="$any(form.controls.optionalField)"
                    fieldLabel="Optional Field"
                    [type]="FormFieldType.Text">
                </form-field>
            </div>
        </div>
    </details>
</div>
```

### Pattern: Date Formatting Helper

API dates come as ISO strings. Convert for `<input type="date">`:

```typescript
private formatDateForInput(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toISOString().split("T")[0];
}

// Usage in populateForm():
this.form.patchValue({
    plannedDate: data.PlannedDate ? this.formatDateForInput(data.PlannedDate) : null,
});
```

---

### First Step: Create Variant

The first step must handle both CREATE (new entity) and SAVE (existing entity).

**Important**: The create path does NOT use the base class `saveStep()` because it needs to:
1. Call a different endpoint (POST create, not PUT save)
2. Navigate to the `/edit/:id` route with the newly created entity ID
3. Handle the `workflowProgressService` manually

```typescript
get isNewEntity(): boolean {
    const id = this._projectID$.getValue();
    return id == null || Number.isNaN(id);
}

onSave(navigate: boolean): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    this.isSaving = true;
    const request = { /* map from form */ };

    if (this.isNewEntity) {
        // CREATE: Manual subscribe — cannot use saveStep() because no projectID exists yet
        this.{entity}Service.create{Entity}FromBasicsStep{Entity}(request).subscribe({
            next: (result) => {
                this.isSaving = false;
                this.workflowProgressService.setFormDirty(false);
                const nextStep = navigate ? this.nextStep : "basics";
                this.router.navigate(["/{entities}", "edit", result.{Entity}ID, nextStep]).then(() => {
                    this.alertService.pushAlert(new Alert("{Entity} created successfully.", AlertContext.Success, true));
                });
            },
            error: (err) => {
                this.isSaving = false;
                const message = err?.error ?? err?.message ?? "Failed to create.";
                this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
            },
        });
    } else {
        // SAVE: Manual subscribe for consistent navigation pattern
        this.projectID$.pipe(take(1)).subscribe(({entity}ID) => {
            this.{entity}Service.saveCreate{StepName}Step{Entity}({entity}ID, request).subscribe({
                next: () => {
                    this.isSaving = false;
                    this.workflowProgressService.setFormDirty(false);
                    this.form.markAsPristine();
                    this.workflowProgressService.triggerRefresh();
                    if (navigate) {
                        this.navigateToNextStep({entity}ID).then(() => {
                            this.alertService.pushAlert(new Alert("Updated successfully.", AlertContext.Success, true));
                        });
                    } else {
                        this.alertService.pushAlert(new Alert("Updated successfully.", AlertContext.Success, true));
                    }
                },
                error: (err) => {
                    this.isSaving = false;
                    const message = err?.error ?? err?.message ?? "Failed to update.";
                    this.alertService.pushAlert(new Alert(message, AlertContext.Danger, true));
                },
            });
        });
    }
}
```

**Key difference from non-first steps**: Non-first steps use `this.saveStep()` directly. The first step cannot because the create path has no projectID yet and needs special routing to the `/edit/:id` URL.

---

## 3. Route Configuration

**File**: `app.routes.ts`

### New Entity Route (first step only)

```typescript
{
    path: "{entities-kebab}/new",
    title: "New {Entity}",
    canActivate: [projectEditGuard],
    loadComponent: () =>
        import("./pages/{entities-kebab}/{entity-kebab}-create-workflow/{entity-kebab}-create-workflow-outlet.component").then(
            (m) => m.{Entity}CreateWorkflowOutletComponent
        ),
    children: [
        { path: "", redirectTo: "basics", pathMatch: "full" },
        {
            path: "basics",
            canDeactivate: [UnsavedChangesGuard],
            loadComponent: () =>
                import("./pages/{entities-kebab}/{entity-kebab}-create-workflow/steps/basics/basics.component").then(
                    (m) => m.BasicsComponent
                ),
        },
    ],
},
```

### Edit Entity Route (all steps)

```typescript
{
    path: "{entities-kebab}/edit/:{entity}ID",
    title: "Edit {Entity}",
    canActivate: [projectEditGuard],
    loadComponent: () =>
        import("./pages/{entities-kebab}/{entity-kebab}-create-workflow/{entity-kebab}-create-workflow-outlet.component").then(
            (m) => m.{Entity}CreateWorkflowOutletComponent
        ),
    children: [
        { path: "", redirectTo: "basics", pathMatch: "full" },
        {
            path: "basics",
            canDeactivate: [UnsavedChangesGuard],
            loadComponent: () =>
                import("./pages/{entities-kebab}/{entity-kebab}-create-workflow/steps/basics/basics.component").then(
                    (m) => m.BasicsComponent
                ),
        },
        {
            path: "step-two-route",
            canDeactivate: [UnsavedChangesGuard],
            loadComponent: () =>
                import("./pages/{entities-kebab}/{entity-kebab}-create-workflow/steps/step-two/step-two.component").then(
                    (m) => m.StepTwoComponent
                ),
        },
        // ... all remaining steps
    ],
},
```

### Route Guards

- `canActivate: [projectEditGuard]` (or appropriate auth guard) on both `/new` and `/edit/:id`
- `canDeactivate: [UnsavedChangesGuard]` on every step child route
- The outlet component receives the route param via `@Input() {entity}ID`
- Step components receive it via `@Input() {entity}ID` (passed down by router)

---

## 4. Checklist

- [ ] Outlet component with sidebar nav, step groups, progress observable
- [ ] `vm$` combining projectID + progress with `startWith({ isNewProject: true, progress: null })`
- [ ] Complete actions dropdown: {Entity} Details, Provide Feedback, Withdraw Proposal
- [ ] Complete footer: created by info + approval status label + action buttons
- [ ] Submit button disabled when `formDirty$ | async`
- [ ] All five state transition methods: Submit, Approve, Return, Reject, Withdraw
- [ ] Status helper methods: `canWithdraw`, `canApprove`, `canReturn`, `canReject`
- [ ] `onProjectCreated()` method for navigation after first step creates entity
- [ ] `FeedbackModalComponent` imported and wired to actions dropdown
- [ ] Per-step components extending `CreateWorkflowStepBase`
- [ ] Each step: `initProjectID()`, `trackFormDirty()` (or `setFormDirty()`)
- [ ] Each step: `onSave(navigate)` calling `this.saveStep()`
- [ ] First step: handles both create and save
- [ ] Map steps: `mapIsReady = signal(false)`
- [ ] Routes: `/new` with first step only, `/edit/:id` with all steps
- [ ] All routes have `canActivate` and step routes have `canDeactivate`
- [ ] No Bootstrap classes in templates
- [ ] `<form-field>` used for all form inputs
