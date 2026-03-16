# Angular Patterns

> **Scope**: frontend
> **Applies when**: Working in WADNR.Web

## Cross-References

| If you're also doing... | Load |
|-------------------------|------|
| Writing component tests | `/write-tests` |
| Creating data grids | `/migrate-grid` |
| Creating maps | `/migrate-map` |
| Creating CRUD modals | `/crud-modal` |
| Adding scrollspy TOC | `/add-scrollspy-toc` |

---

## Standalone Components

All new components must be standalone with explicit imports:

```typescript
@Component({
  selector: 'app-entity-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    WadnrGridComponent,
    IconComponent,
    // explicit imports for all dependencies
  ],
  templateUrl: './entity-detail.component.html',
  styleUrl: './entity-detail.component.scss'
})
export class EntityDetailComponent { }
```

---

## Route Params with withComponentInputBinding()

Use `@Input()` decorators matching route param names with `BehaviorSubject` for reactive state:

```typescript
@Component({...})
export class EntityDetailComponent {
  // Route param bound via withComponentInputBinding()
  @Input() set entityID(value: string) {
    this._entityID$.next(Number(value));
  }

  private _entityID$ = new BehaviorSubject<number | null>(null);

  entity$ = this._entityID$.pipe(
    filter((id): id is number => id != null),
    switchMap(id => this.entityService.getByID(id)),
    shareReplay({ bufferSize: 1, refCount: true })
  );

  constructor(private entityService: EntityService) {}
}
```

---

## Route Guards

**Every route MUST have a `canActivate` guard that matches the API controller's authorization attribute.**

When adding a route to `app.routes.ts`, check the corresponding API controller's auth decorator and apply the correct guard:

| API Controller Attribute | Route Guard |
|--------------------------|-------------|
| `[AllowAnonymous]` | None (public) |
| `[Authorize]`, `[NormalUserFeature]`, `[VendorViewFeature]`, `[PageContentManageFeature]`, `[AdminFeature]`, or any other auth attribute | `canActivate: [authGuard]` |
| Project create/edit workflows | `canActivate: [projectEditGuard]` |

Guards are in `WADNR.Web/src/app/shared/guards/`.

```typescript
// CORRECT: API controller has [NormalUserFeature] — route is guarded
{
    path: "focus-areas",
    title: "Focus Areas",
    canActivate: [authGuard],
    loadComponent: () => import("./pages/focus-areas/focus-areas.component").then((m) => m.FocusAreasComponent),
},

// WRONG: API controller has [NormalUserFeature] but route has no guard
// Unauthenticated users will see a broken page with 401/403 errors
{
    path: "focus-areas",
    title: "Focus Areas",
    loadComponent: () => import("./pages/focus-areas/focus-areas.component").then((m) => m.FocusAreasComponent),
},
```

### UnsavedChangesGuard

For routes with forms, add `canDeactivate: [UnsavedChangesGuard]` to prevent accidental navigation away from unsaved changes.

**Guard**: `WADNR.Web/src/app/shared/guards/unsaved-changes.guard.ts`

**Interface**: Components must implement `IDeactivateComponent`:

```typescript
import { IDeactivateComponent } from "src/app/shared/guards/unsaved-changes.guard";

export class MyFormComponent implements IDeactivateComponent {
    canExit(): boolean {
        return !this.form.dirty;
    }
}
```

**Route config**:

```typescript
{
    path: "entities/edit/:entityID",
    canActivate: [authGuard],
    canDeactivate: [UnsavedChangesGuard],
    loadComponent: () => import("./pages/entities/entity-edit.component").then((m) => m.EntityEditComponent),
},
```

**Workflow steps**: Both `CreateWorkflowStepBase` and `UpdateWorkflowStepBase` already implement `IDeactivateComponent` — track dirty state with `trackFormDirty(form)` and call `form.markAsPristine()` after save.

---

## Template Pattern

Use `@if` with async pipe for observable data:

```html
@if (entity$ | async; as entity) {
  <div class="card">
    <div class="card-header">
      <span class="card-title">{{ entity.name }}</span>
    </div>
    <div class="card-body">
      <!-- content -->
    </div>
  </div>
} @else {
  <app-loading-spinner></app-loading-spinner>
}
```

---

## Grid Columns

Use `UtilityFunctionsService` for column definitions:

```typescript
this.columns = [
  this.utilityFunctionsService.createLinkColumnDef('Name', 'name', 'entityID', 'EntityDetail'),
  this.utilityFunctionsService.createTextColumnDef('Description', 'description'),
  this.utilityFunctionsService.createDateColumnDef('Created', 'createDate'),
  this.utilityFunctionsService.createActionsColumnDef([
    { icon: 'Edit', routerLink: (row) => ['/entities', row.entityID, 'edit'] }
  ])
];
```

---

## Enum Dropdowns

**ALWAYS use the `{Enum}AsSelectDropdownOptions` export when populating dropdowns from generated enums.**

Generated enum files in `WADNR.Web/src/app/shared/generated/enum/` export three things:
1. `{Enum}Enum` - The TypeScript enum
2. `{Enums}` - Lookup table array (`LookupTableEntry[]`)
3. `{Enums}AsSelectDropdownOptions` - Pre-mapped dropdown options (`SelectDropdownOption[]`)

```typescript
// CORRECT: Use the AsSelectDropdownOptions export
import { ProjectStagesAsSelectDropdownOptions } from "src/app/shared/generated/enum/project-stage-enum";

public projectStageOptions: SelectDropdownOption[] = ProjectStagesAsSelectDropdownOptions;

// WRONG: Don't manually map the lookup table
import { ProjectStages } from "src/app/shared/generated/enum/project-stage-enum";

public projectStageOptions = ProjectStages.map(x => ({ Value: x.Value, Label: x.DisplayName }));
```

---

## Form Fields

**ALWAYS check `<form-field>` component before adding raw HTML form elements.**

Location: `WADNR.Web/src/app/shared/components/form-field/`

Before adding any form input (text, select, checkbox, textarea, date picker, etc.) to a template:

1. **Read the `form-field` component** to see what field types it supports
2. **Use `<form-field>` if the element type exists** - it provides consistent styling, labels, validation display, and accessibility
3. **Only use raw HTML** if the component doesn't support your specific need

```html
<!-- CORRECT: Use form-field component -->
<form-field
  [label]="'Project Name'"
  [required]="true"
  [control]="form.controls.projectName"
  fieldType="text">
</form-field>

<!-- WRONG: Don't use raw HTML when form-field supports it -->
<div class="form-group">
  <label>Project Name</label>
  <input type="text" class="form-control" formControlName="projectName">
</div>
```

---

## BEM SCSS Convention

**All component SCSS must use BEM naming with SCSS `&` nesting.**

- **Block**: Component's root concept, kebab-case (usually matches the folder name). Example: `image-gallery`, `project-detail`, `fact-sheet`
- **Element**: `&__element` nested inside the block. Flat kebab children become elements: `.photo-item` → `.image-gallery__item`
- **Modifier**: `&--modifier` for variants/states: `.image-gallery__item--key-photo`

```scss
.image-gallery {
    // block styles

    &__item {
        // element styles

        &--key-photo {
            // modifier styles
        }

        &:hover {
            // pseudo-class (not a modifier)
        }
    }

    &__thumbnail {
        img {
            // bare element selectors OK inside BEM elements
        }
    }

    // ::ng-deep for third-party overrides stays nested under block
    ::ng-deep .leaflet-control {
        // leave external class names alone
    }
}
```

### What NOT to rename to BEM

- **Global classes**: `.card`, `.card-header`, `.card-body`, `.card-footer`, `.card-title`, `.btn`, `.btn-primary`, `.btn-secondary`, `.grid-12`, `.g-col-*`, `.flex`, `.flex-between`, `.flex-end`, `.flex-center`, `.m-*`, `.p-*`, `.alert`, `.field`, `.modal`, `.modal-header`, `.modal-body`, `.modal-footer`, `.page-body`, `.sidebar-item`, `.sidebar-link`, `.badge`, `.table`
- **Third-party classes** inside `::ng-deep` (Leaflet `.leaflet-*`, ag-grid `.ag-*`, ng-select `.ng-*`)
- **Directive-toggled classes**: `.active`, `.disabled`, `.scrollspy-fixed`, `.required`, `.fade`

### CSS Custom Properties

**Always use CSS custom properties from `src/scss/base/_theme.scss` instead of raw values.** The theme defines a full design token system:

| Category | Pattern | Examples |
|----------|---------|----------|
| Colors | `var(--{color}-{shade})` | `var(--blue-default)`, `var(--gray-200)`, `var(--teal-dark)` |
| Brand colors | `var(--wadnr-{color})` | `var(--wadnr-blue)`, `var(--wadnr-orange)`, `var(--wadnr-green)` |
| Semantic colors | `var(--primary)`, `var(--secondary)` | `var(--primary)`, `var(--danger)`, `var(--muted)` |
| Spacing | `var(--spacing-{size})` | `var(--spacing-200)` = 0.5rem, `var(--spacing-400)` = 1rem |
| Type sizes | `var(--type-size-{size})` | `var(--type-size-200)` (body), `var(--type-size-400)` (large) |
| Shadows | `var(--shadow-{size})` | `var(--shadow-100)`, `var(--shadow-200)` |
| Border radius | `var(--border-radius-{size})` | `var(--border-radius-200)` = 0.25rem |
| Card tokens | `var(--card-{part})` | `var(--card-header-bg-color)`, `var(--card-body-bg-color)` |
| Button tokens | `var(--btn-{variant}-{part})` | `var(--btn-primary-bg-color)`, `var(--btn-primary-hover-bg-color)` |

```scss
// CORRECT: Use theme tokens
.my-component {
    &__header {
        color: var(--primary);
        padding: var(--spacing-400);
        font-size: var(--type-size-300);
        border-radius: var(--border-radius-200);
        box-shadow: var(--shadow-100);
    }
}

// WRONG: Raw values
.my-component {
    &__header {
        color: #3e72b0;
        padding: 1rem;
        font-size: 1.13rem;
        border-radius: 0.25rem;
        box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    }
}
```

---

## Bootstrap Replacement Map

**Do NOT use Bootstrap classes.** Use the custom SCSS grid system:

| Bootstrap Class | Replacement |
|----------------|-------------|
| `row` | `grid-12` |
| `col-{n}` | `g-col-{n}` |
| `col-sm-{n}` | `g-col-{n}` (use media queries for responsive) |
| `col-md-{n}` | `g-col-{n}` |
| `col-lg-{n}` | `g-col-{n}` |
| `panel` | `card` |
| `panel-default` | `card` |
| `panel-heading` | `card-header` |
| `panel-title` | `span.card-title` |
| `panel-body` | `card-body` |
| `panel-footer` | `card-footer` |
| `glyphicon glyphicon-*` | `<icon [icon]="'IconName'">` |
| `<table class="table">` | `<wadnr-grid>` component |
| `btn btn-primary` | `btn btn-primary` (kept) |
| `btn btn-default` | `btn btn-secondary` |
| `form-group` | Standard form layout |
| `form-control` | Standard input styling |
| `well` | `card` or custom container |
| `alert alert-*` | `<alert-box [alertType]="'...'">` |

---

## Grid System Examples

```html
<!-- Two equal columns -->
<div class="grid-12">
  <div class="g-col-6">Left column</div>
  <div class="g-col-6">Right column</div>
</div>

<!-- Sidebar layout -->
<div class="grid-12">
  <div class="g-col-3">Sidebar</div>
  <div class="g-col-9">Main content</div>
</div>

<!-- Card layout -->
<div class="card">
  <div class="card-header">
    <span class="card-title">Title</span>
  </div>
  <div class="card-body">
    Content here
  </div>
</div>
```

---

## Card Header Guidelines

**Card titles should use hardcoded text**, not `<field-definition>` components:

```html
<!-- CORRECT: Hardcoded title -->
<div class="card-header">
  <span class="card-title">Projects</span>
</div>

<!-- WRONG: Don't use field-definition for card titles -->
<div class="card-header">
  <span class="card-title">
    <field-definition fieldDefinition="Project"></field-definition>s
  </span>
</div>
```

The `<field-definition>` component adds a help icon with popover, which is appropriate for form field labels but not for card/section titles.

---

## Modal Template Structure

**All modals MUST wrap their content in `<div class="modal">`** for the CSS flex layout (`_modal.scss`) to work. Without it, tall modals overflow instead of scrolling.

Use the standard three-section structure inside the wrapper:

```html
<!-- CORRECT: modal wrapper with standard sections -->
<div class="modal">
  <div class="modal-header">
    <h4>Edit Entity</h4>
  </div>
  <div class="modal-body">
    <!-- form fields, content -->
  </div>
  <div class="modal-footer">
    <button class="btn btn-primary" (click)="save()">Save</button>
    <button class="btn btn-secondary" (click)="close()">Cancel</button>
  </div>
</div>
```

```html
<!-- WRONG: missing modal wrapper — tall content will overflow instead of scroll -->
<div class="modal-header">
  <h4>Edit Entity</h4>
</div>
<div class="modal-body">
  <!-- content -->
</div>
<div class="modal-footer">
  <button class="btn btn-primary" (click)="save()">Save</button>
  <button class="btn btn-secondary" (click)="close()">Cancel</button>
</div>
```

**Button order**: Confirmation/action buttons (Save, Upload, Delete) go on the **left**, abandon buttons (Cancel) go on the **right**.

All modals use `@ngneat/dialog` with `DialogRef<InputData, OutputData>`. Use `ViewEncapsulation.None` when the modal contains injected innerHTML (like diff HTML) that needs global styles.

---

## Loading States

**Prefer the built-in loading directives over hand-rolled spinner markup.**

### `[loadingSpinner]` — Content area loading overlay

Location: `WADNR.Web/src/app/shared/directives/loading.directive.ts`

Attribute directive that adds an animated overlay spinner to any container. Takes an `ILoadingSpinnerOptions` object:
- `isLoading: boolean` — show/hide the spinner
- `loadingHeight?: number` — minimum height (px) while loading
- `opacity?: number` — background overlay opacity

```html
<!-- CORRECT: Use loadingSpinner directive -->
<div class="modal-body" [loadingSpinner]="{ isLoading, loadingHeight: 200 }">
  <!-- content -->
</div>

<div class="card-body" [loadingSpinner]="{ isLoading: isLoading$ | async, loadingHeight: 100 }">
  <!-- content -->
</div>
```

### `[buttonLoading]` — Button spinner

Location: `WADNR.Web/src/app/shared/directives/button-loading.directive.ts`

Attribute directive that prepends a spinning FA icon to a button when true. Takes a `boolean`.

```html
<!-- CORRECT: Use buttonLoading directive -->
<button class="btn btn-primary" (click)="save()" [buttonLoading]="isSubmitting">Save</button>

<!-- WRONG: Don't hand-roll spinner icons -->
<button class="btn btn-primary" (click)="save()">
  <span *ngIf="isSubmitting" class="fas fa-spinner fa-spin"></span>
  Save
</button>

<!-- WRONG: Don't use conditional button text instead of a spinner -->
<button class="btn btn-primary" (click)="save()">
  {{ isSubmitting ? 'Saving...' : 'Save' }}
</button>
```

---

## CSS Utility Classes

**Always check `WADNR.Web/src/scss/utilities/` for available utility classes before using any CSS class for layout, spacing, or display.** This project does NOT use Bootstrap utility classes — it has its own.

Key utility files and classes:

### Spacing (`_margin.scss`)

Uses `m/mt/mr/mb/ml/mx/my` and `p/pt/pr/pb/pl/px/py` with levels 1–6:

| Class | Size |
|-------|------|
| `mr-1`, `ml-1`, etc. | 0.25rem |
| `mr-2`, `ml-2`, etc. | 0.5rem |
| `mr-3`, `ml-3`, etc. | 1rem |
| `mr-4`, `ml-4`, etc. | 2rem |
| `mr-5`, `ml-5`, etc. | 3rem |
| `mr-6`, `ml-6`, etc. | 4rem |

```html
<!-- CORRECT: Project's own margin classes -->
<button class="btn btn-secondary mr-2">First</button>
<button class="btn btn-primary">Second</button>

<!-- WRONG: Bootstrap 5 margin-end classes don't exist here -->
<button class="btn btn-secondary me-2">First</button>
```

### Flexbox (`_flex.scss`)

| Class | Effect |
|-------|--------|
| `flex` | Flex row, centered items, gap |
| `flex-start` | Flex row, justify start |
| `flex-between` | Flex row, space-between |
| `flex-end` | Flex row, justify end |
| `flex-center` | Flex row, centered |
| `no-wrap` | `flex-wrap: nowrap` |
| `fill` | `flex: 1` |
| `ai-fs` | `align-items: flex-start` |
| `ai-fe` | `align-items: flex-end` |
