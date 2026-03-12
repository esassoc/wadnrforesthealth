# Migrate Page Skill

> **Scope**: fullstack
> **Prereqs**: Load `/dotnet-patterns` and `/angular-patterns` first

When the user invokes `/migrate-page <EntityName>`:

## 1. Analyze Legacy Code (THOROUGH — parity depends on this)

The #1 goal of migration is **absolute parity** with the legacy app. Every visual element,
every data field, every label, every conditional must be reproduced exactly. This step
determines success — do not rush it.

Read EVERY file completely (not skimming):

- Read the legacy controller: `{LegacyPath}/Controllers/{Entity}Controller.cs`
  - Note every action method, what data it fetches, and how it transforms it
  - Note the `*Feature` attribute on each action (maps to authorization)
- Read ALL views in: `{LegacyPath}/Views/{Entity}/` — read each .cshtml file fully
  - For each view, inventory every visual element: panels/cards, tables/grids, forms, buttons, links, labels, conditional sections
  - Note the exact text of every label, title, header, and button
  - Note any `@if` / `@Html.Raw` / role-check conditionals
- Check for partials: search each view for `@Html.RenderPartial`, `@Html.Partial`, `Html.RenderPartialView` and read those partial views too
- Look for JavaScript in: `{LegacyPath}/Scripts/` related to the entity
- Check for ViewData / ViewBag usage that passes data to views

Create a parity inventory — for each view, list:
- All CRUD operations (Index, Detail, New, Edit, Delete)
- Every card/panel with its title text
- Every grid with its column headers (in order)
- Every form with its field labels (in order)
- Every button with its text and action
- Every link with its text and destination
- Conditional visibility rules (what shows for admin vs normal user, what hides when data is empty)
- Special/custom endpoints
- Client-side validation or JavaScript behavior
- Related entities and their relationships
- Authorization/permission requirements

This inventory is your checklist — every item must appear in the Angular output.

### Identify Components Requiring Specialized Skills

Based on your analysis, note which of these components the page contains:

| Component Found | Specialized Skill | When to Use |
|-----------------|-------------------|-------------|
| Data tables/grids | `/migrate-grid` | Index pages, detail pages with related entity lists |
| Maps with boundaries/locations | `/migrate-map` | Pages showing spatial data |
| Create/Edit forms | `/crud-modal` | Pages with New/Edit functionality |

Document which skills you'll need for this migration.

## 2. Plan API Layer

Before writing code, plan the artifacts needed:

**DTOs to create:**
- `{Entity}GridRowDto` - Fields needed for list/index view
- `{Entity}DetailDto` - Fields for detail view including related data
- `{Entity}UpsertRequestDto` - Fields for create/edit forms
- `{Entity}LookupItemDto` - If used in dropdowns elsewhere

**Identify:**
- Which related entities need to be included in projections
- Query optimization opportunities (avoid N+1)
- Validation rules to implement

## 3. Create API Files

Create the following files in order:

### 3.1 DTOs
Location: `{ModelsProject}/DataTransferObjects/{Entity}/`
- `{Entity}GridRowDto.cs`
- `{Entity}DetailDto.cs`
- `{Entity}UpsertRequestDto.cs`

### 3.2 Projections
Location: `{EFModelsProject}/Entities/{Entity}.DtoProjections.cs`
- Use `Expression<Func<Entity, DTO>>` pattern
- Include `AsGridRow` and `AsDetail` projections

### 3.3 Static Helpers
Location: `{EFModelsProject}/Entities/{Entity}.StaticHelpers.cs`
- `ListAsGridRowAsync()`
- `GetByIDAsDetailAsync()`
- `CreateAsync()`
- `UpdateAsync()`
- `DeleteAsync()`

### 3.4 Controller
Location: `{ApiProject}/Controllers/{Entity}Controller.cs`
- Extend `{BaseController}` with primary constructor
- Implement endpoints matching the DTOs

### 3.5 Authorization

Apply authorization attributes to controller endpoints based on legacy `*Feature` attributes.

**Step 1: Identify legacy features**
Search for `Feature]` in the legacy controller to find which features protect each action:
```bash
grep -E "Feature\]" {LegacyPath}/Controllers/{Entity}Controller.cs
```

**Step 2: Check legacy feature definitions**
Read the feature class to see which roles are allowed:
```bash
cat {LegacyPath}/Security/{FeatureName}.cs
```

**Step 3: Map to new authorization attributes**

| Legacy Feature Pattern | New Attribute | Roles |
|------------------------|---------------|-------|
| `AnonymousUnclassifiedFeature` or inherits from it | `[AllowAnonymous]` | No auth required |
| Admin + EsaAdmin only | `[AdminFeature]` | Admin, EsaAdmin |
| Normal + ProjectSteward + Admin + EsaAdmin + CanEditProgram | `[ProjectEditFeature]` | Project editors |
| Admin + EsaAdmin + CanManageFundSourcesAndAgreements | `[AgreementManageFeature]` or `[FundSourceManageFeature]` | Fund/Agreement managers |
| Admin + EsaAdmin + CanAddEditUsersContactsOrganizations | `[UserManageFeature]` | User managers |
| Admin + EsaAdmin + CanManagePageContent | `[PageContentManageFeature]` | Content managers |
| Admin + EsaAdmin + CanEditProgram | `[ProgramManageFeature]` | Program managers |
| Any authenticated user | `[LoggedInFeature]` | All logged-in users |
| Normal + ProjectSteward + Admin + EsaAdmin | `[NormalUserFeature]` | Standard users |

**Step 4: Apply attributes**
- Add `using WADNR.API.Services.Authorization;` to the controller
- Add `using Microsoft.AspNetCore.Authorization;` if using `[AllowAnonymous]`
- Apply appropriate attribute to each endpoint:
  - GET endpoints (public data): Usually `[AllowAnonymous]`
  - POST/PUT/DELETE: Apply the feature attribute that matches the legacy feature

**Example:**
```csharp
[HttpGet]
[AllowAnonymous]  // Legacy: AgreementsViewFeature (extends AnonymousUnclassifiedFeature)
public async Task<ActionResult<List<EntityGridRow>>> List() { ... }

[HttpPost]
[AgreementManageFeature]  // Legacy: AgreementCreateFeature (Admin, EsaAdmin, CanManageFundSourcesAndAgreements)
public async Task<ActionResult<EntityDetail>> Create([FromBody] EntityUpsertRequest dto) { ... }
```

**Creating new Feature attributes:**
If no existing attribute matches, create a new one in `{ApiProject}/Services/Authorization/`:
```csharp
using WADNR.EFModels.Entities;

namespace WADNR.API.Services.Authorization;

public class {Entity}ManageFeature : BaseAuthorizationAttribute
{
    public {Entity}ManageFeature() : base([
        RoleEnum.Admin,
        RoleEnum.EsaAdmin,
        // Add other roles from legacy feature
    ])
    {
    }
}
```

## 4. Generate TypeScript

After API code is complete:

```powershell
# Build the API to generate swagger.json
dotnet build {ApiProject}

# Generate TypeScript models
cd {FrontendProject}
npm run gen-model
```

Verify the generated files in `{FrontendProject}/src/app/shared/generated/`.

## 5. Create Angular Components

### 5.1 Component Structure
Location: `{FrontendProject}/src/app/pages/{entity}/`

Create the folder structure based on identified components:
- `{entity}-list/` - If page has a main grid (use `/migrate-grid`)
- `{entity}-detail/` - If page has a detail view
- `{entity}-modal/` - If page needs create/edit forms (use `/crud-modal`)

All components must be standalone with explicit imports. Use `@Input()` with `BehaviorSubject` pattern for route params.

### 5.2 Grids

If the page contains data grids, follow the `/migrate-grid` skill for:
- Column definition patterns
- DTO design for grid rows
- Projection expressions
- Grid component usage

### 5.3 Maps

If the page contains maps, follow the `/migrate-map` skill for:
- GeoJSON endpoint patterns
- Map component integration
- Layer component usage
- Bounding box handling

### 5.4 CRUD Modals

If the page needs create/edit/delete functionality, follow the `/crud-modal` skill for:
- Modal component structure
- Form creation with generated helpers
- Validation patterns
- Delete confirmation patterns

### 5.5 Route Configuration
Add route to `{FrontendProject}/src/app/app.routes.ts`:

```typescript
{
  path: 'entities',
  children: [
    { path: '', component: EntityListComponent },
    { path: ':entityID', component: EntityDetailComponent },
    { path: ':entityID/edit', component: EntityEditComponent },
    { path: 'new', component: EntityEditComponent }
  ]
}
```

## 6. Write Tests

### 6.1 API Tests (MSTest)
Location: `{ApiProject}.Tests/Controllers/{Entity}ControllerTests.cs`
- Test all controller actions
- Use in-memory database
- Test success and error cases

### 6.2 Angular Tests (Jasmine)
Location: Same folder as component with `.spec.ts` extension
- Test component creation
- Test data loading
- Test user interactions
- Mock services with `jasmine.createSpyObj`

## 7. Remove/Deprecate Legacy Code

After migration is complete and tested:
- Add `[Obsolete("Migrated to Angular - {FrontendProject}")]` attribute to legacy controller
- Or remove legacy controller/views entirely if safe
- Update any internal links to point to new Angular routes

## 8. Run Tests and Verify

```powershell
# Run API tests
dotnet test

# Run Angular tests
cd {FrontendProject}
npm test

# Manual verification
npm start
# Navigate to new pages and verify functionality
```

## Checklist Before Completion

### Legacy Parity (MOST IMPORTANT — verify element by element)
- [ ] Every card/section from legacy views exists in Angular with the same title text
- [ ] Every grid has the same columns in the same order with the same header text
- [ ] Every form has the same fields in the same order with the same labels
- [ ] Every button exists with the same text and equivalent action
- [ ] Every link exists with the same text and navigates to the equivalent route
- [ ] Conditional visibility matches legacy (role-based, data-state-based)
- [ ] Empty states match legacy (hidden sections, placeholder messages)
- [ ] No legacy visual elements were omitted or renamed
- [ ] No new UI elements were added that don't exist in legacy

### Core Migration
- [ ] All CRUD operations migrated
- [ ] No Bootstrap classes used (grid-12, card system only)
- [ ] Route params use `@Input()` + `BehaviorSubject` pattern
- [ ] All queries use `.AsNoTracking().Select()` projections

### Authorization
- [ ] Legacy `*Feature` attributes identified for each action
- [ ] Appropriate authorization attributes applied to all endpoints
- [ ] Public GET endpoints marked with `[AllowAnonymous]`
- [ ] Create/Edit/Delete endpoints protected with feature attributes

### If Page Has Grids (per /migrate-grid)
- [ ] Column parity verified with legacy
- [ ] Filtering/sorting works
- [ ] Links navigate correctly

### If Page Has Maps (per /migrate-map)
- [ ] All layers display correctly
- [ ] Bounds/zoom behavior correct
- [ ] Layer toggle works (if applicable)

### If Page Has CRUD (per /crud-modal)
- [ ] Create flow works
- [ ] Edit flow works
- [ ] Delete confirmation works
- [ ] Form validation displays correctly

### Final
- [ ] API tests pass
- [ ] Angular tests pass
- [ ] Legacy code deprecated/removed
- [ ] Manual testing complete

---

## Cross-References

| If you're also doing... | Load |
|-------------------------|------|
| Creating data grids | `/migrate-grid` |
| Creating maps | `/migrate-map` |
| Creating CRUD modals | `/crud-modal` |
| Writing tests | `/write-tests` |
