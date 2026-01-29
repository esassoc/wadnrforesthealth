# Angular Patterns

Load this skill when working in WADNR.Web.

## Cross-References

| If you're also doing... | Load |
|-------------------------|------|
| Writing component tests | `/write-tests` |
| Creating data grids | `/migrate-grid` |
| Creating maps | `/migrate-map` |

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
