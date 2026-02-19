# .NET API Patterns

> **Scope**: backend
> **Applies when**: Working in WADNR.API, WADNR.EFModels, WADNR.Models, WADNR.Common, or SitkaCaptureService

## Cross-References

| If you're also doing... | Load |
|-------------------------|------|
| Database schema changes | `/database-patterns` |
| Writing API tests | `/write-tests` |
| Creating CRUD endpoints | `/crud-modal` |

---

## Controller Pattern

Controllers extend `SitkaController<T>` using primary constructor:

```csharp
[ApiController]
[Route("api/[controller]")]
public class EntityController(WADNRDbContext dbContext, ILogger<EntityController> logger)
    : SitkaController<EntityController>(dbContext, logger)
{
    [HttpGet]
    public async Task<ActionResult<List<EntityGridRowDto>>> List()
    {
        var entities = await Entities.ListAsGridRowAsync(DbContext);
        return Ok(entities);
    }

    [HttpGet("{entityID}")]
    public async Task<ActionResult<EntityDetailDto>> GetByID([FromRoute] int entityID)
    {
        var entity = await Entities.GetByIDAsDetailAsync(DbContext, entityID);
        return Ok(entity);
    }
}
```

---

## DTO Naming Conventions

| DTO Type | Purpose | Example |
|----------|---------|---------|
| `{Entity}GridRowDto` | List/grid views with minimal fields | `ProjectGridRowDto` |
| `{Entity}DetailDto` | Full detail views with related data | `ProjectDetailDto` |
| `{Entity}UpsertRequestDto` | Create/update requests | `ProjectUpsertRequestDto` |
| `{Entity}LookupItemDto` | Dropdown/select options | `ProjectLookupItemDto` |

Place DTOs in `WADNR.Models/DataTransferObjects/{Entity}/`.

---

## Method Naming Conventions

Use verb prefixes that indicate return cardinality:

| Prefix | Returns | Examples |
|--------|---------|----------|
| `Get...` | Single item (or null) | `GetByIDAsDetailAsync`, `GetByEmailAsDetailAsync` |
| `List...` | Collection (`List<T>`, `IEnumerable<T>`) | `ListAsGridRowAsync`, `ListForPersonAsGridRowAsync` |

**Always include the `As{DtoType}` suffix** to indicate what DTO is returned:
- `ListAsGridRowAsync` - returns grid row DTOs
- `ListForPersonAsGridRowAsync` - returns grid row DTOs filtered by person
- `GetByIDAsDetailAsync` - returns detail DTO

**Method placement**: Place methods in the **target entity's** StaticHelpers, not the filter entity's:

```csharp
// GOOD - Each entity owns its queries
var projects = await Projects.ListForPersonAsGridRowAsync(dbContext, personID);
var agreements = await Agreements.ListForPersonAsGridRowAsync(dbContext, personID);

// BAD - Person shouldn't know how to query other entities
var projects = await People.ListProjectsForPersonAsync(dbContext, personID);
```

---

## Static Helper Class Pattern

Create `{PluralEntity}.cs` in `WADNR.EFModels/Entities/`:

```csharp
public static class Entities
{
    public static async Task<List<EntityGridRowDto>> ListAsGridRowAsync(WADNRDbContext dbContext)
    {
        return await dbContext.Entities
            .AsNoTracking()
            .Select(EntityProjections.AsGridRow)
            .ToListAsync();
    }

    public static async Task<EntityDetailDto?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int entityID)
    {
        return await dbContext.Entities
            .AsNoTracking()
            .Where(x => x.EntityID == entityID)
            .Select(EntityProjections.AsDetail)
            .SingleOrDefaultAsync();
    }

    public static async Task<Entity> CreateAsync(WADNRDbContext dbContext, EntityUpsertRequestDto request)
    {
        var entity = new Entity { /* map from request */ };
        dbContext.Entities.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public static async Task UpdateAsync(WADNRDbContext dbContext, Entity entity, EntityUpsertRequestDto request)
    {
        // map from request to entity
        await dbContext.SaveChangesAsync();
    }

    public static async Task DeleteAsync(WADNRDbContext dbContext, Entity entity)
    {
        dbContext.Entities.Remove(entity);
        await dbContext.SaveChangesAsync();
    }
}
```

---

## Projection Class Pattern

Create `{Entity}Projections.cs` with `Expression<Func<Entity, DTO>>`:

```csharp
public static class EntityProjections
{
    public static Expression<Func<Entity, EntityGridRowDto>> AsGridRow => x => new EntityGridRowDto
    {
        EntityID = x.EntityID,
        Name = x.Name,
        // Only include fields needed for grid display
    };

    public static Expression<Func<Entity, EntityDetailDto>> AsDetail => x => new EntityDetailDto
    {
        EntityID = x.EntityID,
        Name = x.Name,
        Description = x.Description,
        // Include all fields and related data
        RelatedItems = x.RelatedEntities.Select(r => new RelatedItemDto { /* ... */ }).ToList()
    };
}
```

---

## Query Optimization Rules

### Rule 1: Always use `.AsNoTracking()` for read-only queries

```csharp
// GOOD
return await dbContext.Entities.AsNoTracking().Select(...).ToListAsync();

// BAD - unnecessary tracking overhead
return await dbContext.Entities.Select(...).ToListAsync();
```

### Rule 2: Always use `.Select(Projection)` instead of `.Include()`

```csharp
// GOOD - only fetches needed fields
return await dbContext.Entities
    .AsNoTracking()
    .Select(EntityProjections.AsDetail)
    .ToListAsync();

// BAD - fetches entire entity graph
return await dbContext.Entities
    .Include(e => e.RelatedEntities)
    .ToListAsync();
```

### Rule 3: Never return EF entities directly from controllers

Always project to DTOs.

### Rule 4: Never use `.Distinct()` on entities with geometry columns

SQL Server cannot compare spatial types. Select distinct IDs first:

```csharp
// BAD - Will fail if Entity has geometry columns
var items = await dbContext.JoinTable
    .Where(j => j.SomeID == id)
    .Select(j => j.Entity)
    .Distinct()  // ERROR: geometry cannot be compared
    .Select(EntityProjections.AsGridRow)
    .ToListAsync();

// GOOD - Get distinct IDs first, then query
var entityIDs = await dbContext.JoinTable
    .Where(j => j.SomeID == id)
    .Select(j => j.EntityID)
    .Distinct()
    .ToListAsync();

var items = await dbContext.Entities
    .Where(e => entityIDs.Contains(e.EntityID))
    .Select(EntityProjections.AsGridRow)
    .ToListAsync();
```

### Rule 5: Never use static lookup dictionaries in EF projections

EF Core cannot translate these to SQL. Resolve client-side after the query:

```csharp
// BAD - Will fail with "constant expression" error
public static readonly Expression<Func<Entity, EntityDetail>> AsDetail = x => new EntityDetail
{
    DivisionID = x.DivisionID,
    DivisionName = x.DivisionID != null
        ? Division.AllLookupDictionary[x.DivisionID.Value].DivisionDisplayName
        : null, // ERROR
};

// GOOD - Set to null in projection, resolve in static helper
public static readonly Expression<Func<Entity, EntityDetail>> AsDetail = x => new EntityDetail
{
    DivisionID = x.DivisionID,
    DivisionName = null, // Resolved client-side
};

// In static helper:
public static async Task<EntityDetail?> GetByIDAsDetailAsync(DbContext dbContext, int entityID)
{
    var detail = await dbContext.Entities
        .AsNoTracking()
        .Where(x => x.EntityID == entityID)
        .Select(EntityProjections.AsDetail)
        .SingleOrDefaultAsync();

    // Resolve static lookup values client-side
    if (detail?.DivisionID != null && Division.AllLookupDictionary.TryGetValue(detail.DivisionID.Value, out var division))
    {
        detail.DivisionName = division.DivisionDisplayName;
    }

    return detail;
}
```

---

## Authorization Attributes

All custom attributes live in `WADNR.API/Services/Authorization/` and extend `BaseAuthorizationAttribute` (unless noted).

| Attribute | Roles Granted | Use Case |
|-----------|---------------|----------|
| `[AllowAnonymous]` | No auth required | Public GET endpoints |
| `[LoggedInFeature]` | Any authenticated user | Basic logged-in access |
| `[LoggedInUnclassifiedFeature]` | Any authenticated user (direct `AuthorizeAttribute`) | Unclassified user actions |
| `[NormalUserFeature]` | Normal, ProjectSteward, Admin, EsaAdmin | Standard user features |
| `[ProjectViewFeature]` | Anonymous allowed (special attribute) | Public project viewing |
| `[ProjectEditFeature]` | Normal, ProjectSteward, Admin, EsaAdmin, CanEditProgram (supplemental) | Project create/edit |
| `[ProjectApproveFeature]` | ProjectSteward, Admin, EsaAdmin | Approve/return projects |
| `[ProjectPendingViewFeature]` | Normal, ProjectSteward, Admin, EsaAdmin, CanEditProgram (supplemental) | View pending projects |
| `[ProjectEditAsAdminFeature]` | ProjectSteward, Admin, EsaAdmin, CanEditProgram (supplemental) | Admin-level project edits |
| `[AdminFeature]` | Admin, EsaAdmin | Admin-only operations |
| `[AgreementManageFeature]` | Admin, EsaAdmin, CanManageFundSourcesAndAgreements (supplemental) | Agreement CRUD |
| `[FundSourceManageFeature]` | Admin, EsaAdmin, CanManageFundSourcesAndAgreements (supplemental) | Fund source CRUD |
| `[PageContentManageFeature]` | Admin, EsaAdmin, CanManagePageContent (supplemental) | CMS page editing |
| `[ProgramManageFeature]` | Admin, EsaAdmin, CanEditProgram (supplemental) | Program management |
| `[UserManageFeature]` | Admin, EsaAdmin, CanAddEditUsersContactsOrganizations (supplemental) | User/org management |
| `[VendorViewFeature]` | Admin, EsaAdmin, ProjectSteward | Vendor data access |

**Supplemental roles** are added via `SupplementalRoleEnum` — users get these in addition to their base role.
