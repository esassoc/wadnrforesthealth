# Migrate Grid Skill

When the user invokes `/migrate-grid <EntityName> <GridName>`:

## Overview

This skill guides the migration of data grids from legacy MVC views to Angular using the `WADNRGridComponent`, ensuring complete column parity with the legacy implementation.

---

## 1. Analyze Legacy Grid Implementation

First, thoroughly examine the legacy MVC grid:

### Find Legacy Grid Views

- **Index views**: `Source/ProjectFirma.Web/Views/{Entity}/Index.cshtml`
- **Detail views**: `Source/ProjectFirma.Web/Views/{Entity}/Detail.cshtml`
- **Partial views**: `Source/ProjectFirma.Web/Views/{Entity}/*Grid*.cshtml`
- **Shared partials**: `Source/ProjectFirma.Web/Views/Shared/*Grid*.cshtml`

### Document Every Column

Create a column inventory table:

| # | Header | Field/Expression | Type | Link? | Filter? | Notes |
|---|--------|------------------|------|-------|---------|-------|
| 1 | Name | ProjectName | Text | Yes, to detail | Yes | - |
| 2 | Status | StatusName | Text | No | Dropdown | - |
| 3 | Date | CreatedDate | Date | No | Date range | Format: M/d/yyyy |
| 4 | Amount | TotalAmount | Currency | No | Number | Format: $X,XXX |
| 5 | Actions | - | Actions | Yes | No | Edit, Delete |

### Identify Data Source

- Look for `GridSpec` classes or `@model` declarations
- Check controller actions that return grid data
- Note any complex joins or calculations

---

## 2. Design Grid Row DTO

### Naming Convention

- Grid DTOs: `{Entity}{Context}GridRowDto`
- Examples:
  - `ProjectGridRowDto` - main project grid
  - `ProjectOrganizationDetailGridRowDto` - projects on org detail page
  - `AgreementGridRowDto` - agreements grid

### DTO Structure

```csharp
// In WADNR.Models/DataTransferObjects/{Entity}/{Entity}GridRowDto.cs
namespace WADNR.Models.DataTransferObjects;

public class EntityGridRowDto
{
    // Primary key for row identification
    public int EntityID { get; set; }

    // Simple text fields
    public string Name { get; set; }
    public string Description { get; set; }

    // Nested objects for linked columns
    public RelatedEntitySimpleDto? RelatedEntity { get; set; }

    // Lists for multi-link columns
    public List<TagSimpleDto> Tags { get; set; } = new();

    // Formatted/calculated fields
    public decimal? TotalAmount { get; set; }
    public DateTime? CreatedDate { get; set; }

    // Boolean fields
    public bool IsActive { get; set; }
}
```

### Nested Object Pattern

For columns that link to other entities:

```csharp
// Simple nested object for link columns
public class OrganizationSimpleDto
{
    public int OrganizationID { get; set; }
    public string OrganizationName { get; set; }
}
```

---

## 3. Create Projection Expression

```csharp
// In WADNR.EFModels/Entities/{Entity}.DtoProjections.cs
public static class EntityProjections
{
    public static Expression<Func<Entity, EntityGridRowDto>> AsGridRow => x => new EntityGridRowDto
    {
        EntityID = x.EntityID,
        Name = x.Name,
        Description = x.Description,

        // Nested object projection
        RelatedEntity = x.RelatedEntity == null ? null : new RelatedEntitySimpleDto
        {
            RelatedEntityID = x.RelatedEntity.RelatedEntityID,
            RelatedEntityName = x.RelatedEntity.Name
        },

        // List projection
        Tags = x.EntityTags.Select(et => new TagSimpleDto
        {
            TagID = et.Tag.TagID,
            TagName = et.Tag.TagName
        }).ToList(),

        // Calculated fields
        TotalAmount = x.LineItems.Sum(li => li.Amount),
        CreatedDate = x.CreateDate,
        IsActive = x.IsActive ?? false
    };
}
```

---

## 4. Create Static Helper Method

```csharp
// In WADNR.EFModels/Entities/{PluralEntity}.cs
public static class Entities
{
    public static async Task<List<EntityGridRowDto>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        return await dbContext.Entities
            .AsNoTracking()
            .Select(EntityProjections.AsGridRow)
            .ToListAsync();
    }

    // For filtered grids (e.g., entities for a specific parent)
    public static async Task<List<EntityGridRowDto>> ListByParentIDAsGridRowAsync(
        WADNRDbContext dbContext, int parentID)
    {
        return await dbContext.Entities
            .AsNoTracking()
            .Where(x => x.ParentID == parentID)
            .Select(EntityProjections.AsGridRow)
            .ToListAsync();
    }
}
```

---

## 5. Add API Endpoint

```csharp
// In WADNR.API/Controllers/{Entity}Controller.cs
[HttpGet]
public async Task<ActionResult<List<EntityGridRowDto>>> List()
{
    var entities = await Entities.ListAsGridRowAsync(DbContext);
    return Ok(entities);
}

// For child grids on parent detail pages
[HttpGet("{parentID}/entities")]
public async Task<ActionResult<List<EntityGridRowDto>>> ListByParent([FromRoute] int parentID)
{
    var entities = await Entities.ListByParentIDAsGridRowAsync(DbContext, parentID);
    return Ok(entities);
}
```

---

## 6. Create Angular Column Definitions

### Import UtilityFunctionsService

```typescript
import { UtilityFunctionsService } from "src/app/services/utility-functions.service";
import { ColDef } from "ag-grid-community";
import { EntityGridRowDto } from "src/app/shared/generated/model/entity-grid-row-dto";

constructor(private utilityFunctions: UtilityFunctionsService) {}

public columnDefs: ColDef<EntityGridRowDto>[] = [];

ngOnInit(): void {
    this.columnDefs = this.createColumnDefs();
}
```

### Column Definition Methods Reference

| Method | Use Case | Example |
|--------|----------|---------|
| `createBasicColumnDef` | Text, nested object fields | Name, Description |
| `createLinkColumnDef` | Single link to another entity | Project Name → Project Detail |
| `createMultiLinkColumnDef` | Array of links | Tags, Programs |
| `createDateColumnDef` | Dates with format | Created Date |
| `createDecimalColumnDef` | Numbers with decimals | Acres (2 decimal places) |
| `createCurrencyColumnDef` | Money values | Amount ($X,XXX) |
| `createBooleanColumnDef` | Yes/No values (displays "Yes"/"No", not true/false) | Is Active |
| `createYearColumnDef` | Year numbers | Fiscal Year |
| `createJoinedBasicColumnDef` | Comma-separated array text | Category Names |
| `createActionsColumnDef` | Edit/Delete buttons | Row actions |
| `createPhoneColumnDef` | Phone number formatting | Contact Phone |
| `createPercentColumnDef` | Percentage values | Completion % |

### Column Definition Examples

```typescript
private createColumnDefs(): ColDef<EntityGridRowDto>[] {
    return [
        // Basic text column
        this.utilityFunctions.createBasicColumnDef("Name", "Name", {
            FieldDefinitionType: "EntityName"
        }),

        // Nested object field (dot notation)
        this.utilityFunctions.createBasicColumnDef("Category", "Category.CategoryName", {
            CustomDropdownFilterField: "Category.CategoryName"
        }),

        // Link column (single link)
        this.utilityFunctions.createLinkColumnDef("Project", "ProjectName", "ProjectID", {
            InRouterLink: "/projects/",
            FieldDefinitionType: "Project"
        }),

        // Link column with nested object
        this.utilityFunctions.createLinkColumnDef(
            "Organization",
            "Organization.OrganizationName",
            "Organization.OrganizationID",
            {
                InRouterLink: "/organizations/",
                CustomDropdownFilterField: "Organization.OrganizationName"
            }
        ),

        // Multi-link column (array of links)
        this.utilityFunctions.createMultiLinkColumnDef(
            "Programs",           // Header
            "Programs",           // List field
            "ProgramID",          // ID field in list item
            "ProgramName",        // Display field in list item
            { InRouterLink: "/programs/" }
        ),

        // Date column
        this.utilityFunctions.createDateColumnDef("Start Date", "StartDate", "M/d/yyyy", {
            FieldDefinitionType: "StartDate"
        }),

        // Currency column
        this.utilityFunctions.createCurrencyColumnDef("Amount", "TotalAmount", {
            MaxDecimalPlacesToDisplay: 0,
            FieldDefinitionType: "Amount"
        }),

        // Decimal column
        this.utilityFunctions.createDecimalColumnDef("Acres", "TotalAcres", {
            MaxDecimalPlacesToDisplay: 2,
            FieldDefinitionType: "TotalAcres"
        }),

        // Boolean column
        this.utilityFunctions.createBooleanColumnDef("Active", "IsActive"),

        // Joined text column (array to comma-separated string)
        this.utilityFunctions.createJoinedBasicColumnDef("Categories", "Categories.CategoryName", {
            Distinct: true,
            SortValues: true
        }),

        // Actions column
        this.utilityFunctions.createActionsColumnDef(
            (params) => ({
                items: [
                    { name: "Edit", icon: "Edit", link: `/entities/${params.data.EntityID}/edit` },
                    { name: "Delete", icon: "Trash", callback: () => this.deleteEntity(params.data) }
                ]
            })
        )
    ];
}
```

### LtinfoColumnDefParams Options

```typescript
{
    Width?: number;                    // Explicit column width
    MaxWidth?: number;                 // Maximum column width
    Hide?: boolean;                    // Hide column by default
    FieldDefinitionType?: string;      // Field definition for tooltip
    FieldDefinitionLabelOverride?: string; // Override the label shown in tooltip
    CustomDropdownFilterField?: string; // Enable dropdown filter on this field
    ColumnContainsMultipleValues?: boolean; // For comma-separated values
    Sort?: 'asc' | 'desc';            // Default sort direction
    Editable?: boolean;               // Make column editable
    ValueGetter?: (params) => any;    // Custom value getter for complex fields
}
```

### Dropdown Filters for Boolean and Lookup Columns

**Always use `CustomDropdownFilterField`** for columns with a fixed set of values to enable multi-checkbox filtering:

1. **Boolean columns** (Yes/No, Active/Inactive)
2. **Lookup columns** (Status, Stage, Type, Category, etc.)
3. **Any column with enumerated values**

This provides a better filtering experience with checkboxes instead of text search.

```typescript
// Boolean column with dropdown filter
// Note: Both the cell display AND filter dropdown show "Yes"/"No" instead of true/false
this.utilityFunctions.createBooleanColumnDef("Is Active", "IsActive", {
    CustomDropdownFilterField: "IsActive",
}),

// Lookup column (e.g., Status, Stage, Type)
this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStage.ProjectStageName", {
    FieldDefinitionType: "ProjectStage",
    CustomDropdownFilterField: "ProjectStage.ProjectStageName",
}),

// Another lookup example
this.utilityFunctions.createBasicColumnDef("Agreement Type", "AgreementTypeName", {
    FieldDefinitionType: "AgreementType",
    CustomDropdownFilterField: "AgreementTypeName",
}),
```

**When to use dropdown filters:**
| Column Type | Use Dropdown Filter? | Example |
|-------------|---------------------|---------|
| Boolean (Yes/No) | ✅ Yes | IsActive, IsComplete |
| Status/Stage | ✅ Yes | ProjectStage, AgreementStatus |
| Type/Category | ✅ Yes | OrganizationType, FundSourceType |
| Linked entity name | ✅ Yes | Organization.OrganizationName |
| Free-form text | ❌ No | Description, Notes |
| Numbers | ❌ No | Amount, Count |
| Dates | ❌ No | StartDate, CreatedDate |

---

### Column Formatting and Alignment Guidelines

**Right-aligned columns**: The following column types should ALWAYS be right-aligned:
- Numbers (counts, IDs displayed as numbers)
- Dates
- Percentages
- Currency/Money values

**Number formatting**: Numbers should display with comma separators for thousands (e.g., 1,234,567 not 1234567).

The built-in utility methods handle this automatically:
- `createCurrencyColumnDef` - Right-aligned, formatted as $X,XXX
- `createDecimalColumnDef` - Right-aligned, with specified decimal places and comma separators
- `createPercentColumnDef` - Right-aligned, formatted as X.XX%
- `createDateColumnDef` - Right-aligned, formatted per the format string
- `createYearColumnDef` - Right-aligned

For custom number columns using `createBasicColumnDef`, add alignment and formatting:

```typescript
// Custom number column with right alignment and comma separators
this.utilityFunctions.createBasicColumnDef("Project Count", "ProjectCount", {
    ValueFormatter: (params) => params.value?.toLocaleString() ?? "",
    CellStyle: { textAlign: 'right' }
}),
```

**Text columns**: Left-aligned (default behavior, no changes needed).

### Understanding Legacy Column Headers and Field Definitions

When analyzing legacy grid specs, pay close attention to how column headers are defined:

**Pattern 1: `ToGridHeaderString()` - NO override parameter**
```csharp
// Legacy uses field definition's default label
Add(Models.FieldDefinition.ProjectStage.ToGridHeaderString(), ...)
Add(Models.FieldDefinition.FundSource.ToGridHeaderString(), ...)
```
→ In Angular: Use `FieldDefinitionType` WITHOUT `FieldDefinitionLabelOverride`. The column header should match the field definition's label.

```typescript
// Angular - let field definition control the header
this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStage.ProjectStageName", {
    FieldDefinitionType: "ProjectStage",  // No override - uses field def label
}),
```

**Pattern 2: `ToGridHeaderString("Override")` - WITH override parameter**
```csharp
// Legacy overrides the field definition label
Add(Models.FieldDefinition.AgreementType.ToGridHeaderString("Type"), ...)
Add(Models.FieldDefinition.AgreementStartDate.ToGridHeaderString("Start Date"), ...)
```
→ In Angular: Use `FieldDefinitionType` WITH `FieldDefinitionLabelOverride` to match the override.

```typescript
// Angular - override the field definition label
this.utilityFunctions.createBasicColumnDef("Type", "AgreementTypeAbbrev", {
    FieldDefinitionType: "AgreementType",
    FieldDefinitionLabelOverride: "Type",  // Matches legacy override
}),
```

**Pattern 3: Hardcoded header (no field definition)**
```csharp
// Legacy uses a hardcoded string, no field definition
Add("# of Photos", x => x.ProjectImages.Count, ...)
Add("Short Name", a => a.ProgramShortName, ...)
```
→ In Angular: Use the same hardcoded header, no `FieldDefinitionType`.

```typescript
// Angular - hardcoded header, no field definition
this.utilityFunctions.createBasicColumnDef("# of Photos", "PhotoCount"),
this.utilityFunctions.createBasicColumnDef("Short Name", "ProgramShortName"),
```

### Migration Decision Table

| Legacy Pattern | Angular Implementation |
|----------------|----------------------|
| `FieldDef.X.ToGridHeaderString()` | Use `FieldDefinitionType: "X"`, NO override |
| `FieldDef.X.ToGridHeaderString("Label")` | Use `FieldDefinitionType: "X"` AND `FieldDefinitionLabelOverride: "Label"` |
| `"Hardcoded Header"` (no field def) | Use hardcoded header string, no FieldDefinitionType |

### Common Mistakes to Avoid

❌ **Wrong**: Always adding `FieldDefinitionLabelOverride` when using `FieldDefinitionType`
```typescript
// DON'T do this if legacy uses ToGridHeaderString() without override
this.utilityFunctions.createBasicColumnDef("Stage", "ProjectStage", {
    FieldDefinitionType: "ProjectStage",
    FieldDefinitionLabelOverride: "Stage",  // WRONG - unnecessary override
}),
```

✅ **Correct**: Only add override when legacy explicitly overrides
```typescript
// DO this - let field definition control the label
this.utilityFunctions.createBasicColumnDef("Project Stage", "ProjectStage", {
    FieldDefinitionType: "ProjectStage",  // Uses field def's default label
}),
```

---

## 7. Angular Template Pattern

### Basic Grid

```html
<div class="card">
    <div class="card-header"><span class="card-title">Entities</span></div>
    <div class="card-body">
        <wadnr-grid
            [rowData]="entities$ | async"
            [columnDefs]="columnDefs"
            [downloadFileName]="'entities'">
        </wadnr-grid>
    </div>
</div>
```

### Grid with Height and Totals Row

```html
<wadnr-grid
    [rowData]="entities$ | async"
    [columnDefs]="columnDefs"
    [height]="'400px'"
    [pinnedTotalsRow]="pinnedTotalsRow"
    [downloadFileName]="'entities'">
</wadnr-grid>
```

```typescript
public pinnedTotalsRow = {
    fields: ["TotalAmount", "EstimatedCost"],
    label: "Total:",
    labelField: "Name",
    filteredOnly: true
};
```

### Grid with Row Selection

```html
<wadnr-grid
    [rowData]="entities$ | async"
    [columnDefs]="columnDefs"
    [defaultRowSelection]="'multiRow'"
    (selectionChanged)="onSelectionChanged($event)">
</wadnr-grid>
```

---

## 8. WADNRGridComponent Input Reference

| Input | Type | Default | Description |
|-------|------|---------|-------------|
| `rowData` | `any[]` | - | Grid data array |
| `columnDefs` | `ColDef[]` | - | Column definitions |
| `height` | string | `'500px'` | Grid height |
| `width` | string | `'100%'` | Grid width |
| `downloadFileName` | string | `'grid-data'` | CSV export filename |
| `defaultRowSelection` | `'singleRow' \| 'multiRow'` | - | Row selection mode |
| `pinnedTotalsRow` | object | - | Pinned totals row config |
| `hideDownloadButton` | boolean | `false` | Hide CSV download button |
| `hideGlobalFilter` | boolean | `false` | Hide search box |
| `sizeColumnsToFitGrid` | boolean | `false` | Fit columns to grid width |
| `pagination` | boolean | `false` | Enable pagination |
| `paginationPageSize` | number | `100` | Rows per page |

---

## 9. Column Parity Checklist

Before considering migration complete, verify:

- [ ] All legacy columns are present in Angular grid
- [ ] Column headers match (or are intentionally improved)
- [ ] Data formats match:
  - [ ] Dates display in same format (M/d/yyyy)
  - [ ] Currency displays with proper formatting ($X,XXX)
  - [ ] Decimals show correct precision
  - [ ] Numbers have comma separators (1,234,567)
  - [ ] Booleans show Yes/No
- [ ] Column alignment:
  - [ ] Numbers are right-aligned
  - [ ] Dates are right-aligned
  - [ ] Currency is right-aligned
  - [ ] Percentages are right-aligned
  - [ ] Text is left-aligned (default)
- [ ] Links navigate to correct routes
- [ ] Dropdown filters configured for:
  - [ ] Boolean columns (Yes/No)
  - [ ] Lookup columns (Status, Stage, Type, etc.)
  - [ ] Linked entity name columns
- [ ] Field definition tooltips are present on headers
- [ ] Column sort works correctly
- [ ] Column widths are appropriate
- [ ] Actions column has all required actions

---

## 10. Common Patterns

### Multiple Grids on One Page

```typescript
public projectColumnDefs: ColDef<ProjectGridRowDto>[] = [];
public agreementColumnDefs: ColDef<AgreementGridRowDto>[] = [];

ngOnInit(): void {
    this.projectColumnDefs = this.createProjectColumnDefs();
    this.agreementColumnDefs = this.createAgreementColumnDefs();
}
```

### Refreshing Grid Data

```typescript
private refreshData$ = new Subject<void>();

this.entities$ = combineLatest([this.entityID$, this.refreshData$.pipe(startWith(undefined))]).pipe(
    switchMap(([id]) => this.entityService.listByParent(id)),
    shareReplay({ bufferSize: 1, refCount: true })
);

refreshGrid(): void {
    this.refreshData$.next();
}
```

### Grid with Action Callbacks

```typescript
private createColumnDefs(): ColDef[] {
    return [
        // ... other columns
        this.utilityFunctions.createActionsColumnDef((params) => ({
            items: [
                {
                    name: "Edit",
                    icon: "Edit",
                    callback: () => this.openEditModal(params.data)
                },
                {
                    name: "Delete",
                    icon: "Trash",
                    callback: () => this.confirmDelete(params.data)
                }
            ]
        }))
    ];
}

openEditModal(entity: EntityGridRowDto): void {
    // Open modal logic
}

confirmDelete(entity: EntityGridRowDto): void {
    // Delete confirmation logic
}
```

---

## 11. Migration Checklist

- [ ] Documented all legacy grid columns
- [ ] Created GridRow DTO with all required fields
- [ ] Created projection expression
- [ ] Created static helper method(s)
- [ ] Added API endpoint(s)
- [ ] Ran `dotnet build WADNR.API` to generate swagger.json
- [ ] Ran `npm run gen-model` to generate TypeScript models
- [ ] Created column definitions using UtilityFunctionsService
- [ ] Added WADNRGridComponent to template
- [ ] Verified column parity with legacy grid
- [ ] Verified filtering works
- [ ] Verified sorting works
- [ ] Verified links navigate correctly
- [ ] Verified CSV export works
- [ ] Verified field definition tooltips appear
