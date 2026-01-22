# Migrate Page Skill

When the user invokes `/migrate-page <EntityName>`:

## 1. Analyze Legacy Code

First, thoroughly examine the existing MVC implementation:

- Read the legacy controller: `{LegacyPath}/Controllers/{Entity}Controller.cs`
- Read all views in: `{LegacyPath}/Views/{Entity}/`
- Check for partials in: `{LegacyPath}/Views/Shared/` that may be used
- Look for JavaScript in: `{LegacyPath}/Scripts/` related to the entity

Identify and document:
- All CRUD operations (Index, Detail, New, Edit, Delete)
- Special/custom endpoints
- Bootstrap patterns used (panels, tables, forms, modals)
- Client-side validation or JavaScript behavior
- Related entities and their relationships
- Authorization/permission requirements

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

### Core Migration
- [ ] All CRUD operations migrated
- [ ] No Bootstrap classes used (grid-12, card system only)
- [ ] Route params use `@Input()` + `BehaviorSubject` pattern
- [ ] All queries use `.AsNoTracking().Select()` projections

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
