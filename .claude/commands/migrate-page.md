# Migrate Page Skill

When the user invokes `/migrate-page <EntityName>`:

## 1. Analyze Legacy Code

First, thoroughly examine the existing MVC implementation:

- Read the legacy controller: `Source/ProjectFirma.Web/Controllers/{Entity}Controller.cs`
- Read all views in: `Source/ProjectFirma.Web/Views/{Entity}/`
- Check for partials in: `Source/ProjectFirma.Web/Views/Shared/` that may be used
- Look for JavaScript in: `Source/ProjectFirma.Web/Scripts/` related to the entity

Identify and document:
- All CRUD operations (Index, Detail, New, Edit, Delete)
- Special/custom endpoints
- Bootstrap patterns used (panels, tables, forms, modals)
- Client-side validation or JavaScript behavior
- Related entities and their relationships
- Authorization/permission requirements

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
Location: `WADNR.Models/DataTransferObjects/{Entity}/`
- `{Entity}GridRowDto.cs`
- `{Entity}DetailDto.cs`
- `{Entity}UpsertRequestDto.cs`

### 3.2 Projections
Location: `WADNR.EFModels/Entities/{Entity}.DtoProjections.cs`
- Use `Expression<Func<Entity, DTO>>` pattern
- Include `AsGridRow` and `AsDetail` projections

### 3.3 Static Helpers
Location: `WADNR.EFModels/Entities/{Entity}.StaticHelpers.cs`
- `ListAsGridRowAsync()`
- `GetByIDAsDetailAsync()`
- `CreateAsync()`
- `UpdateAsync()`
- `DeleteAsync()`

### 3.4 Controller
Location: `WADNR.API/Controllers/{Entity}Controller.cs`
- Extend `SitkaController<T>` with primary constructor
- Implement endpoints matching the DTOs

## 4. Generate TypeScript

After API code is complete:

```powershell
# Build the API to generate swagger.json
dotnet build WADNR.API

# Generate TypeScript models
cd WADNR.Web
npm run gen-model
```

Verify the generated files in `WADNR.Web/src/app/shared/generated/`.

## 5. Create Angular Components

### 5.1 Component Files
Location: `WADNR.Web/src/app/pages/{entity}/`

Create:
- `{entity}.component.ts` - Main component with routing
- `{entity}.component.html` - Template (NO Bootstrap!)
- `{entity}.component.scss` - Styles if needed
- `{entity}-list/` subfolder for list view if separate
- `{entity}-detail/` subfolder for detail view if separate

### 5.2 Component Requirements
- Use standalone components with explicit imports
- Use `@Input()` with `BehaviorSubject` pattern for route params
- Use `grid-12` layout system, NOT Bootstrap
- Use `<wadnr-grid>` for tables
- Use `<icon>` component for icons

### 5.3 Route Configuration
Add route to `WADNR.Web/src/app/app.routes.ts`:

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
Location: `WADNR.API.Tests/Controllers/{Entity}ControllerTests.cs`
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
- Add `[Obsolete("Migrated to Angular - WADNR.Web")]` attribute to legacy controller
- Or remove legacy controller/views entirely if safe
- Update any internal links to point to new Angular routes

## 8. Run Tests and Verify

```powershell
# Run API tests
dotnet test

# Run Angular tests
cd WADNR.Web
npm test

# Manual verification
npm start
# Navigate to new pages and verify functionality
```

## Checklist Before Completion

- [ ] All CRUD operations migrated
- [ ] No Bootstrap classes used (grid-12, card system only)
- [ ] Route params use `@Input()` + `BehaviorSubject` pattern
- [ ] All queries use `.AsNoTracking().Select()` projections
- [ ] API tests pass
- [ ] Angular tests pass
- [ ] Legacy code deprecated/removed
- [ ] Manual testing complete
