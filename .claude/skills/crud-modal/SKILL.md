# CRUD Modal Skill

> **Scope**: fullstack
> **Prereqs**: Load `/dotnet-patterns` and `/angular-patterns` first

When the user invokes `/crud-modal <EntityName>`:

## Contents

1. [Analyze Legacy Forms](#1-analyze-legacy-forms)
2. [Create Upsert Request DTO](#2-create-upsert-request-dto)
3. [Add API Endpoints](#3-add-api-endpoints)
4. [Create Static Helper Methods](#4-create-static-helper-methods)
5. [Create Modal Component](#5-create-modal-component)
6. [FormFieldType Reference](#6-formfieldtype-reference)
7. [Opening the Modal](#7-opening-the-modal)
8. [Delete Confirmation Pattern](#8-delete-confirmation-pattern)
9. [Adding Edit/Delete Buttons](#9-adding-editdelete-buttons)
10. [Permission Checks (Frontend)](#10-permission-checks-frontend)
11. [Form Validation Display](#11-form-validation-display)
12. [Migration Checklist](#12-migration-checklist)
13. [Common Issues and Solutions](#13-common-issues-and-solutions)

## Overview

This skill guides the creation of CRUD (Create, Read, Update, Delete) modals with forms, validation, and permission checks for the Angular application.

---

## 1. Analyze Legacy Forms

First, examine the legacy MVC implementation:

### Find Legacy Forms

- **Edit views**: `{LegacyPath}/Views/{Entity}/Edit.cshtml`
- **New views**: `{LegacyPath}/Views/{Entity}/New.cshtml`
- **Modal partials**: `{LegacyPath}/Views/{Entity}/*Modal*.cshtml`
- **ViewModels**: `{LegacyPath}/Models/{Entity}ViewModel.cs`

### Document Form Fields

Create a field inventory:

| # | Field Label | Field Name | Type | Required | Validation | Notes |
|---|-------------|------------|------|----------|------------|-------|
| 1 | Name | EntityName | Text | Yes | Max 200 chars | - |
| 2 | Description | Description | Textarea | No | Max 4000 chars | - |
| 3 | Start Date | StartDate | Date | Yes | Must be valid date | - |
| 4 | Category | CategoryID | Dropdown | Yes | Must exist | FK to Category |
| 5 | Amount | Amount | Currency | No | Min 0, Max 9999999 | 2 decimals |
| 6 | Is Active | IsActive | Checkbox | No | - | Default: true |

### Identify Validation Rules

- Look for `[Required]`, `[StringLength]`, `[Range]` attributes
- Check for custom validation logic in controller or ViewModel
- Note any conditional validation rules

---

## 2. Create Upsert Request DTO

### DTO with Validation Attributes

```csharp
// In {ModelsProject}/DataTransferObjects/{Entity}/{Entity}UpsertRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace {ModelsProject}.DataTransferObjects;

public class EntityUpsertRequestDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string EntityName { get; set; }

    [StringLength(4000, ErrorMessage = "Description cannot exceed 4000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Start Date is required")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public int CategoryID { get; set; }

    [Range(0, 9999999.99, ErrorMessage = "Amount must be between 0 and 9,999,999.99")]
    public decimal? Amount { get; set; }

    public bool IsActive { get; set; } = true;

    // For creating related records
    public List<int>? RelatedEntityIDs { get; set; }
}
```

---

## 3. Add API Endpoints

### Controller Endpoints

```csharp
// In {ApiProject}/Controllers/{Entity}Controller.cs

[HttpPost]
[LoggedInUnclassifiedFeature]
public async Task<ActionResult<EntityDetailDto>> Create([FromBody] EntityUpsertRequestDto dto)
{
    var entity = await Entities.CreateAsync(DbContext, dto);
    var detail = await Entities.GetByIDAsDetailAsync(DbContext, entity.EntityID);
    return Ok(detail);
}

[HttpPut("{entityID}")]
[LoggedInUnclassifiedFeature]
public async Task<ActionResult<EntityDetailDto>> Update(
    [FromRoute] int entityID,
    [FromBody] EntityUpsertRequestDto dto)
{
    var entity = await DbContext.Entities.FindAsync(entityID);
    if (entity == null)
        return NotFound();

    await Entities.UpdateAsync(DbContext, entity, dto);
    var detail = await Entities.GetByIDAsDetailAsync(DbContext, entity.EntityID);
    return Ok(detail);
}

[HttpDelete("{entityID}")]
[LoggedInUnclassifiedFeature]
public async Task<IActionResult> Delete([FromRoute] int entityID)
{
    var entity = await DbContext.Entities.FindAsync(entityID);
    if (entity == null)
        return NotFound();

    await Entities.DeleteAsync(DbContext, entity);
    return Ok();
}
```

### Permission Attributes

| Attribute | Description |
|-----------|-------------|
| `[LoggedInUnclassifiedFeature]` | Any logged-in user |
| `[SitkaAdminFeature]` | Admin users only |
| `[FirmaBaseFeature]` | Standard application users |

---

## 4. Create Static Helper Methods

```csharp
// In {EFModelsProject}/Entities/{PluralEntity}.cs

public static async Task<Entity> CreateAsync({DbContext} dbContext, EntityUpsertRequestDto dto)
{
    var entity = new Entity
    {
        EntityName = dto.EntityName,
        Description = dto.Description,
        StartDate = dto.StartDate,
        CategoryID = dto.CategoryID,
        Amount = dto.Amount,
        IsActive = dto.IsActive,
        CreateDate = DateTime.UtcNow,
        CreatePersonID = dbContext.GetCurrentPersonID()
    };

    dbContext.Entities.Add(entity);
    await dbContext.SaveChangesAsync();

    // Handle related entities if needed
    if (dto.RelatedEntityIDs?.Any() == true)
    {
        foreach (var relatedID in dto.RelatedEntityIDs)
        {
            dbContext.EntityRelations.Add(new EntityRelation
            {
                EntityID = entity.EntityID,
                RelatedEntityID = relatedID
            });
        }
        await dbContext.SaveChangesAsync();
    }

    return entity;
}

public static async Task UpdateAsync({DbContext} dbContext, Entity entity, EntityUpsertRequestDto dto)
{
    entity.EntityName = dto.EntityName;
    entity.Description = dto.Description;
    entity.StartDate = dto.StartDate;
    entity.CategoryID = dto.CategoryID;
    entity.Amount = dto.Amount;
    entity.IsActive = dto.IsActive;
    entity.UpdateDate = DateTime.UtcNow;
    entity.UpdatePersonID = dbContext.GetCurrentPersonID();

    await dbContext.SaveChangesAsync();
}

public static async Task DeleteAsync({DbContext} dbContext, Entity entity)
{
    // Remove related records first if needed
    var relations = await dbContext.EntityRelations
        .Where(x => x.EntityID == entity.EntityID)
        .ToListAsync();
    dbContext.EntityRelations.RemoveRange(relations);

    dbContext.Entities.Remove(entity);
    await dbContext.SaveChangesAsync();
}
```

---

## 5. Create Modal Component

### Component Files

Create the following files:
- `{FrontendProject}/src/app/pages/{entity}/{entity}-modal/{entity}-modal.component.ts`
- `{FrontendProject}/src/app/pages/{entity}/{entity}-modal/{entity}-modal.component.html`
- `{FrontendProject}/src/app/pages/{entity}/{entity}-modal/{entity}-modal.component.scss`

### Generated Form Helpers

The OpenAPI code generator produces reactive form helpers for every UpsertRequest DTO:
- `{Entity}UpsertRequest` - Class with constructor that accepts form values
- `{Entity}UpsertRequestForm` - Interface with typed FormControls
- `{Entity}UpsertRequestFormControls` - Static factory methods for creating FormControls

**Benefits of using generated helpers:**

| Aspect | Manual | Generated |
|--------|--------|-----------|
| Type safety | Manual, can drift | Automatic, tied to DTO |
| Boilerplate | High | Low |
| Maintenance | Update in 2 places | Update in 1 place (C# DTO) |
| Mapping | Manual field-by-field | Constructor: `new Dto(form.value)` |
| Refactoring | Error-prone | Safe |

### TypeScript Component

```typescript
// {entity}-modal.component.ts
import { Component, inject, OnInit } from "@angular/core";
import { DialogRef } from "@ngneat/dialog";
import { FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { FormFieldComponent, FormFieldType } from "src/app/shared/components/forms/form-field/form-field.component";
import { ModalAlertsComponent } from "src/app/shared/components/modal/modal-alerts.component";
import { BaseModal } from "src/app/shared/components/modal/base-modal";
import { AlertService } from "src/app/shared/services/alert.service";
import { AlertContext } from "src/app/shared/models/enums/alert-context.enum";

import { EntityService } from "src/app/shared/generated/api/entity.service";
import { EntityDetail } from "src/app/shared/generated/model/entity-detail";
import { CategorySimple } from "src/app/shared/generated/model/category-simple";

// Import generated form helpers - Form interface and FormControl factory class
import {
    EntityUpsertRequest,
    EntityUpsertRequestForm,
    EntityUpsertRequestFormControls
} from "src/app/shared/generated/model/entity-upsert-request";

export interface EntityModalData {
    mode: "create" | "edit";
    entity?: EntityDetail;
    categories: CategorySimple[];
}

@Component({
    selector: "entity-modal",
    standalone: true,
    imports: [ReactiveFormsModule, FormFieldComponent, ModalAlertsComponent],
    templateUrl: "./entity-modal.component.html",
    styleUrls: ["./entity-modal.component.scss"]
})
export class EntityModalComponent extends BaseModal implements OnInit {
    public ref: DialogRef<EntityModalData, EntityDetail | null> = inject(DialogRef);

    public FormFieldType = FormFieldType;
    public mode: "create" | "edit" = "create";
    public entity?: EntityDetail;
    public categories: CategorySimple[] = [];
    public isSubmitting = false;

    // Use generated form interface for type safety
    public form = new FormGroup<EntityUpsertRequestForm>({
        // Use generated FormControl factories - add validators as needed
        EntityName: EntityUpsertRequestFormControls.EntityName("", {
            validators: [Validators.required, Validators.maxLength(200)]
        }),
        Description: EntityUpsertRequestFormControls.Description(""),
        StartDate: EntityUpsertRequestFormControls.StartDate(null, {
            validators: [Validators.required]
        }),
        CategoryID: EntityUpsertRequestFormControls.CategoryID(null, {
            validators: [Validators.required]
        }),
        Amount: EntityUpsertRequestFormControls.Amount(null, {
            validators: [Validators.min(0), Validators.max(9999999.99)]
        }),
        IsActive: EntityUpsertRequestFormControls.IsActive(true),
    });

    constructor(
        private entityService: EntityService,
        alertService: AlertService
    ) {
        super(alertService);
    }

    ngOnInit(): void {
        const data = this.ref.data;
        this.mode = data?.mode ?? "create";
        this.entity = data?.entity;
        this.categories = data?.categories ?? [];

        if (this.mode === "edit" && this.entity) {
            // patchValue works directly with the typed form
            this.form.patchValue({
                EntityName: this.entity.EntityName,
                Description: this.entity.Description,
                StartDate: this.entity.StartDate ? new Date(this.entity.StartDate) : null,
                CategoryID: this.entity.Category?.CategoryID,
                Amount: this.entity.Amount,
                IsActive: this.entity.IsActive
            });
        }
    }

    get modalTitle(): string {
        return this.mode === "create" ? "New Entity" : "Edit Entity";
    }

    save(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.isSubmitting = true;
        this.localAlerts = [];

        // Use constructor for direct mapping - no manual field-by-field mapping!
        const dto = new EntityUpsertRequest(this.form.value);

        const request$ = this.mode === "create"
            ? this.entityService.createEntity(dto)
            : this.entityService.updateEntity(this.entity!.EntityID, dto);

        request$.subscribe({
            next: (result) => {
                const message = this.mode === "create"
                    ? "Entity created successfully."
                    : "Entity updated successfully.";
                this.pushGlobalSuccess(message);
                this.ref.close(result);
            },
            error: (err) => {
                this.isSubmitting = false;
                const message = err?.error?.message ?? err?.message ?? "An error occurred.";
                this.addLocalAlert(message, AlertContext.Danger, true);
            }
        });
    }

    cancel(): void {
        this.ref.close(null);
    }
}
```

### HTML Template

```html
<!-- {entity}-modal.component.html -->
<div class="modal-header">
    <h3>{{ modalTitle }}</h3>
</div>
<div class="modal-body">
    <!-- Modal-local alerts -->
    <modal-alerts [alerts]="localAlerts" (onClosed)="removeLocalAlert($event)"></modal-alerts>

    <form [formGroup]="form">
        <!-- Text input -->
        <form-field
            [formControl]="form.controls.EntityName"
            fieldLabel="Name"
            [type]="FormFieldType.Text"
            [required]="true"
            placeholder="Enter name">
        </form-field>

        <!-- Textarea -->
        <form-field
            [formControl]="form.controls.Description"
            fieldLabel="Description"
            [type]="FormFieldType.Textarea"
            placeholder="Enter description">
        </form-field>

        <!-- Date input -->
        <form-field
            [formControl]="form.controls.StartDate"
            fieldLabel="Start Date"
            [type]="FormFieldType.Date"
            [required]="true">
        </form-field>

        <!-- Dropdown/Select -->
        <form-field
            [formControl]="form.controls.CategoryID"
            fieldLabel="Category"
            [type]="FormFieldType.Select"
            [required]="true"
            [selectOptions]="categories"
            selectLabelField="CategoryName"
            selectValueField="CategoryID"
            placeholder="Select a category">
        </form-field>

        <!-- Number input -->
        <form-field
            [formControl]="form.controls.Amount"
            fieldLabel="Amount"
            [type]="FormFieldType.Number"
            placeholder="0.00">
        </form-field>

        <!-- Check (boolean) -->
        <form-field
            [formControl]="form.controls.IsActive"
            fieldLabel="Active"
            [type]="FormFieldType.Check">
        </form-field>
    </form>
</div>
<div class="modal-footer">
    <button
        class="btn btn-primary"
        (click)="save()"
        [disabled]="isSubmitting">
        {{ isSubmitting ? 'Saving...' : 'Save' }}
    </button>
    <button
        class="btn btn-secondary"
        (click)="cancel()"
        [disabled]="isSubmitting">
        Cancel
    </button>
</div>
```

---

## 6. FormFieldType Reference

```typescript
export enum FormFieldType {
    Text = "text",
    Textarea = "textarea",
    Check = "check",
    Toggle = "toggle",
    Date = "date",
    Select = "select",
    Number = "number",
    Radio = "radio",
    RTE = "rte",
    File = "file",
}
```

### FormInputOption Interface

```typescript
export interface FormInputOption {
    Value: any;
    Label: string;
    SortOrder?: number | null | undefined;
    Group?: string | null | undefined;
    disabled: boolean | null | undefined;
}
```

### Key `<form-field>` @Input() Properties

| Input | Type | Description |
|-------|------|-------------|
| `formControl` | `FormControl` | FormControl binding |
| `fieldLabel` | `string` | Label text |
| `placeholder` | `string` | Placeholder text |
| `type` | `FormFieldType` | Input type (default: `Text`) |
| `formInputOptions` | `FormInputOption[]` | Options for Select/Radio |
| `formInputOptionLabel` | `string` | Property name for option labels (default: `"Label"`) |
| `formInputOptionValue` | `string` | Property name for option values (default: `"Value"`) |
| `multiple` | `boolean` | Allow multiple selections (default: `false`) |
| `fieldDefinitionName` | `string` | Field definition tooltip |
| `fieldDefinitionLabelOverride` | `string` | Override label in tooltip |
| `checkLabel` | `string` | Label for checkbox |
| `toggleTrue` / `toggleFalse` | `string` | Text for toggle states |
| `units` | `string` | Unit text (e.g., `"acres"`) |
| `mask` | `string` | Input mask pattern (ngx-mask) |
| `horizontal` | `boolean` | Horizontal layout (default: `false`) |
| `readOnly` | `boolean` | Read-only mode (default: `false`) |
| `uploadFileAccepts` | `string` | File accept types (e.g., `".docx,.doc"`) |

---

## 7. Opening the Modal

### From Parent Component

```typescript
import { DialogService } from "@ngneat/dialog";
import { EntityModalComponent, EntityModalData } from "./entity-modal/entity-modal.component";

constructor(
    private dialogService: DialogService,
    private entityService: EntityService
) {}

// For creating new entity
openCreateModal(): void {
    // First, load any dropdown data needed
    this.categoryService.listCategories().subscribe(categories => {
        const dialogRef = this.dialogService.open(EntityModalComponent, {
            data: {
                mode: "create",
                categories: categories
            } as EntityModalData,
            size: "md"
        });

        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshData();
            }
        });
    });
}

// For editing existing entity
openEditModal(entity: EntityDetailDto): void {
    this.categoryService.listCategories().subscribe(categories => {
        const dialogRef = this.dialogService.open(EntityModalComponent, {
            data: {
                mode: "edit",
                entity: entity,
                categories: categories
            } as EntityModalData,
            size: "md"
        });

        dialogRef.afterClosed$.subscribe(result => {
            if (result) {
                this.refreshData();
            }
        });
    });
}
```

### Dialog Sizes

| Size | Description |
|------|-------------|
| `"sm"` | Small modal (~300px) |
| `"md"` | Medium modal (~500px) |
| `"lg"` | Large modal (~800px) |
| `"fullScreen"` | Full screen modal |

---

## 8. Delete Confirmation Pattern

### Using ConfirmService

```typescript
import { ConfirmService } from "src/app/shared/services/confirm/confirm.service";

constructor(private confirmService: ConfirmService) {}

async confirmDelete(entity: EntityDetailDto): Promise<void> {
    const confirmed = await this.confirmService.confirm({
        title: "Delete Entity",
        message: `Are you sure you want to delete "${entity.EntityName}"? This action cannot be undone.`,
        buttonTextYes: "Delete",
        buttonClassYes: "btn-danger",
        buttonTextNo: "Cancel"
    });

    if (confirmed) {
        this.entityService.deleteEntity(entity.EntityID).subscribe({
            next: () => {
                this.alertService.pushAlert(new Alert(
                    "Entity deleted successfully.",
                    AlertContext.Success,
                    true
                ));
                this.refreshData();
            },
            error: (err) => {
                this.alertService.pushAlert(new Alert(
                    err?.error?.message ?? "Failed to delete entity.",
                    AlertContext.Danger,
                    true
                ));
            }
        });
    }
}
```

---

## 9. Adding Edit/Delete Buttons

### In Detail Page Header

```html
<page-header pageTitle="{{ entity.EntityName }}">
    <div class="header-actions">
        @if (canEdit) {
            <button class="btn btn-primary" (click)="openEditModal()">
                <icon [icon]="'Edit'"></icon> Edit
            </button>
        }
        @if (canDelete) {
            <button class="btn btn-danger" (click)="confirmDelete()">
                <icon [icon]="'Trash'"></icon> Delete
            </button>
        }
    </div>
</page-header>
```

### In Grid Actions Column

```typescript
this.utilityFunctions.createActionsColumnDef((params) => ({
    items: [
        {
            name: "Edit",
            icon: "Edit",
            callback: () => this.openEditModal(params.data),
            visible: this.canEdit
        },
        {
            name: "Delete",
            icon: "Trash",
            callback: () => this.confirmDelete(params.data),
            visible: this.canDelete
        }
    ].filter(item => item.visible !== false)
}))
```

---

## 10. Permission Checks (Frontend)

### Using AuthenticationService

```typescript
import { AuthenticationService } from "src/app/shared/services/authentication.service";

constructor(private authService: AuthenticationService) {}

public canEdit = false;
public canDelete = false;

ngOnInit(): void {
    this.authService.getCurrentUser().subscribe(user => {
        // Check user roles or permissions
        this.canEdit = user?.RoleID >= RoleEnum.Editor;
        this.canDelete = user?.RoleID >= RoleEnum.Admin;
    });
}
```

### Common Permission Patterns

```typescript
// Any logged-in user
const canView = !!user;

// User with specific role
const isAdmin = user?.RoleID === RoleEnum.Admin;

// User who owns the record
const isOwner = user?.PersonID === entity.CreatePersonID;

// Combined permissions
const canEdit = isAdmin || isOwner;
```

---

## 11. Form Validation Display

The `form-field` component automatically displays validation errors. For custom error messages:

```typescript
// Custom validators with generated FormControl factory
EntityName: EntityUpsertRequestFormControls.EntityName("", {
    validators: [
        Validators.required,
        Validators.maxLength(200),
        this.uniqueNameValidator()
    ]
})

private uniqueNameValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const value = control.value;
        if (this.existingNames.includes(value)) {
            return { uniqueName: true };
        }
        return null;
    };
}
```

### Displaying Custom Errors in Template

```html
<form-field
    [formControl]="form.controls.EntityName"
    fieldLabel="Name"
    [type]="FormFieldType.Text"
    [required]="true"
    [customErrors]="{ uniqueName: 'This name already exists' }">
</form-field>
```

---

## 12. Migration Checklist

### Backend

- [ ] Created `{Entity}UpsertRequestDto` with validation attributes
- [ ] Added `Create` endpoint to controller
- [ ] Added `Update` endpoint to controller
- [ ] Added `Delete` endpoint to controller
- [ ] Created `CreateAsync` static helper
- [ ] Created `UpdateAsync` static helper
- [ ] Created `DeleteAsync` static helper
- [ ] Added appropriate permission attributes
- [ ] Ran `dotnet build {ApiProject}` to generate swagger.json

### Frontend

- [ ] Ran `npm run gen-model` to generate TypeScript models
- [ ] Created modal component files
- [ ] Implemented form with all fields
- [ ] Implemented save logic for create/edit modes
- [ ] Implemented cancel logic
- [ ] Added modal-local error handling
- [ ] Added loading/submitting state
- [ ] Added delete confirmation (if applicable)
- [ ] Added Edit/Delete buttons with permission checks
- [ ] Tested create flow
- [ ] Tested edit flow
- [ ] Tested delete flow
- [ ] Tested validation errors display
- [ ] Tested permission checks

---

## 13. Common Issues and Solutions

### Modal doesn't close after save
- Ensure `this.ref.close(result)` is called in the success handler
- Check for errors in the API response

### Form validation not triggering
- Call `this.form.markAllAsTouched()` before checking validity
- Ensure validators are properly configured

### Dropdown not populating
- Verify the data is loaded before opening the modal
- Check `selectLabelField` and `selectValueField` match DTO property names

### Error alerts not showing
- Ensure component extends `BaseModal`
- Add `<modal-alerts>` to template
- Use `this.addLocalAlert()` for modal-local errors

### Permission checks not working
- Verify user is loaded before checking permissions
- Check role/permission values match expected values
