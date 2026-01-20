# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

WA DNR Forest Health Tracker is a conservation and natural resource project tracking application. It tracks project lifecycles from planning through design to final reporting for Washington State Department of Natural Resources.

## Technology Stack

- **Backend**: ASP.NET Core 10 Web API (WADNR.API)
- **Frontend**: Angular 21 (WADNR.Web)
- **Database**: SQL Server with Entity Framework Core 10 (spatial support via NetTopologySuite)
- **Background Jobs**: Hangfire
- **Maps**: Leaflet with Esri layers, GeoServer integration

## Solution Structure

The main solution is `WADNR.sln` containing:
- **WADNR.API** - ASP.NET Core Web API with controllers, services, and Swagger documentation
- **WADNR.EFModels** - Entity Framework Core database context and auto-generated entities
- **WADNR.Models** - Data Transfer Objects (DTOs)
- **WADNR.Common** - Shared utilities and email services
- **WADNR.Database** - SQL Server database project (.sqlproj)
- **SitkaCaptureService** - Screenshot/capture service

Legacy code exists in `Source/WADNRForestHealth.sln` (ProjectFirma.Web - ASP.NET MVC) but new development uses the WADNR.* projects.

## Common Commands

### Frontend (WADNR.Web)
```powershell
cd WADNR.Web
npm install              # Install dependencies
npm start                # Dev server at https://wadnr.localhost.esassoc.com:3215
npm run build            # Development build
npm run build-qa         # QA build
npm run build-prod       # Production build
npm run lint             # Run ESLint
npm run lint-fix         # Fix ESLint issues
npm run gen-model        # Regenerate TypeScript models from swagger.json
```
Node version: v22.17.0 (use nvm)

### Backend (.NET)
```powershell
dotnet build WADNR.sln   # Build all projects
dotnet run --project WADNR.API  # Run the API
```

### Database & Code Generation (from Build/ directory)
```powershell
.\DatabaseDownload.ps1   # Download database backup from Azure
.\DatabaseRestore.ps1    # Restore database from backup
.\DatabaseBuild.ps1      # Build and deploy database project
.\Scaffold.ps1           # Regenerate EF models and DTOs from database
.\DownloadRestoreBuildScaffold.ps1  # Full pipeline
```

### Docker
```powershell
docker-compose -f docker-compose/docker-compose.yml up
```

## Architecture Patterns

### API Layer
Controllers are in `WADNR.API/Controllers/`. Each controller typically maps to a domain entity (e.g., `ProjectController.cs`, `AgreementController.cs`). The API generates `swagger.json` on Debug builds via post-build event.

### Code Generation Pipeline
1. Database changes go in `WADNR.Database/dbo/Tables/` or release scripts
2. Run `Build/Scaffold.ps1` to regenerate:
   - EF entities in `WADNR.EFModels/Entities/Generated/`
   - Extension methods in `WADNR.EFModels/Entities/Generated/ExtensionMethods/`
   - TypeScript enums in `WADNR.Web/src/app/shared/generated/enum/`
3. Build the API to regenerate `swagger.json`
4. Run `npm run gen-model` to regenerate TypeScript API clients in `WADNR.Web/src/app/shared/generated/`

### Database Migrations
Use release scripts in `WADNR.Database/Scripts/ReleaseScripts/` or `PreReleaseScripts/`:
```sql
DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0001 - Description'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    -- Migration logic here
    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'AuthorName', @MigrationName, 'Reason'
END
```

### Frontend Structure
- `WADNR.Web/src/app/pages/` - Page components organized by feature
- `WADNR.Web/src/app/shared/` - Shared components, services, pipes, directives
- `WADNR.Web/src/app/shared/generated/` - Auto-generated API clients and models (do not edit)
- `WADNR.Web/src/app/shared/components/leaflet/` - Map components

## Configuration

- `Build/build.ini` - Database connection, paths, code generation settings
- `WADNR.API/appsettings.json` and `appsecrets.json` - API configuration
- `WADNR.Web/angular.json` - Angular build configuration
- `WADNR.Web/proxy.conf.dev.json` - Dev server proxy configuration

---

## MVC-to-Angular Migration

This project is undergoing a page-by-page migration from ASP.NET MVC (ProjectFirma.Web) to Angular 21 + ASP.NET Core 10 API.

### Page Migration Workflow

Follow these steps when migrating any entity/page:

1. **Analyze Legacy MVC Code**
   - Read controller: `Source/ProjectFirma.Web/Controllers/{Entity}Controller.cs`
   - Review Razor views: `Source/ProjectFirma.Web/Views/{Entity}/`
   - Identify CRUD operations, special endpoints, Bootstrap patterns used

2. **Create API Artifacts**
   - Controller extending `SitkaController<T>`
   - DTOs in `WADNR.Models/DataTransferObjects/{Entity}/`
   - Static helpers in `WADNR.EFModels/Entities/{Entity}.StaticHelpers.cs`
   - Projections in `WADNR.EFModels/Entities/{Entity}.DtoProjections.cs`

3. **Run Code Generation Pipeline**
   - Build API to regenerate `swagger.json`
   - Run `npm run gen-model` in WADNR.Web

4. **Create Angular Page Components**
   - Component files in `WADNR.Web/src/app/pages/{entity}/`
   - Add route to `app.routes.ts`
   - Use `@Input()` with `BehaviorSubject` for route params

5. **Write Unit Tests**
   - MSTest for API controllers
   - Jasmine for Angular components

6. **Remove/Deprecate Legacy Code**
   - Mark legacy controller/views as deprecated or remove

7. **Run Tests**
   - `dotnet test` for API
   - `npm test` for Angular

---

## API Patterns

### Controller Pattern

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

### DTO Naming Conventions

- `{Entity}GridRowDto` - List/grid views with minimal fields
- `{Entity}DetailDto` - Full detail views with related data
- `{Entity}UpsertRequestDto` - Create/update requests
- `{Entity}LookupItemDto` - Dropdown/select options

### Static Helper Class Pattern

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

### Projection Class Pattern

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

### Query Optimization Rules

- **ALWAYS** use `.AsNoTracking()` for read-only queries
- **ALWAYS** use `.Select(Projection)` instead of `.Include()` for DTOs
- **NEVER** return EF entities directly from controllers
- **AVOID** N+1 queries by projecting related data in the same query

---

## Angular Patterns

### Standalone Components

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

### Route Params with withComponentInputBinding()

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

### Template Pattern

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

### Grid Columns

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

### Grid System Examples

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

## Testing Patterns

### API Tests (MSTest)

Create tests in `WADNR.API.Tests` project:

```csharp
[TestClass]
public class EntityControllerTests
{
    private WADNRDbContext _dbContext;
    private EntityController _controller;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<WADNRDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new WADNRDbContext(options);
        _controller = new EntityController(_dbContext, Mock.Of<ILogger<EntityController>>());
    }

    [TestMethod]
    public async Task List_ReturnsAllEntities()
    {
        // Arrange
        _dbContext.Entities.Add(new Entity { Name = "Test" });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _controller.List();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result;
        var entities = (List<EntityGridRowDto>)okResult.Value;
        Assert.AreEqual(1, entities.Count);
    }

    [TestMethod]
    public async Task GetByID_ReturnsEntity_WhenExists()
    {
        // Arrange
        var entity = new Entity { EntityID = 1, Name = "Test" };
        _dbContext.Entities.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _controller.GetByID(1);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    public async Task GetByID_ReturnsNotFound_WhenNotExists()
    {
        // Act
        var result = await _controller.GetByID(999);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }
}
```

### Angular Tests (Jasmine)

Component tests in `*.spec.ts` files:

```typescript
describe('EntityDetailComponent', () => {
  let component: EntityDetailComponent;
  let fixture: ComponentFixture<EntityDetailComponent>;
  let entityServiceSpy: jasmine.SpyObj<EntityService>;

  beforeEach(async () => {
    entityServiceSpy = jasmine.createSpyObj('EntityService', ['getByID']);

    await TestBed.configureTestingModule({
      imports: [EntityDetailComponent],
      providers: [
        { provide: EntityService, useValue: entityServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EntityDetailComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load entity when entityID is set', fakeAsync(() => {
    const mockEntity = { entityID: 1, name: 'Test Entity' };
    entityServiceSpy.getByID.and.returnValue(of(mockEntity));

    component.entityID = '1';
    tick();

    component.entity$.subscribe(entity => {
      expect(entity).toEqual(mockEntity);
    });

    expect(entityServiceSpy.getByID).toHaveBeenCalledWith(1);
  }));

  it('should display entity name in template', fakeAsync(() => {
    const mockEntity = { entityID: 1, name: 'Test Entity' };
    entityServiceSpy.getByID.and.returnValue(of(mockEntity));

    component.entityID = '1';
    fixture.detectChanges();
    tick();
    fixture.detectChanges();

    const compiled = fixture.nativeElement;
    expect(compiled.querySelector('.card-title').textContent).toContain('Test Entity');
  }));
});
```
