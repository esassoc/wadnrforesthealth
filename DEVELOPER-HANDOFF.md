# WA DNR Forest Health Tracker - Developer Handoff Guide

**For developers transitioning from the legacy ProjectFirma MVC stack to the modern Angular + ASP.NET Core stack.**

---

## Table of Contents

1. [What Changed (Legacy vs Modern at a Glance)](#1-what-changed-legacy-vs-modern-at-a-glance)
2. [Prerequisites & Setup](#2-prerequisites--setup)
3. [Config Files You Need (Not in Git)](#3-config-files-you-need-not-in-git)
4. [Running the App Locally](#4-running-the-app-locally)
5. [Solution Structure](#5-solution-structure)
6. [Backend: ASP.NET Core API Patterns](#6-backend-aspnet-core-api-patterns)
7. [Frontend: Angular Patterns](#7-frontend-angular-patterns)
8. [Database & Code Generation Pipeline](#8-database--code-generation-pipeline)
9. [Helm Charts & Environment Variable Pipeline](#9-helm-charts--environment-variable-pipeline)
10. [Authentication & Authorization](#10-authentication--authorization)
11. [Common Gotchas](#11-common-gotchas)
12. [Quick Reference Commands](#12-quick-reference-commands)

---

## 1. What Changed (Legacy vs Modern at a Glance)

| Aspect | Legacy (ProjectFirma) | Modern (WADNR.*) |
|--------|----------------------|-------------------|
| **Framework** | .NET Framework 4.8, ASP.NET MVC 5 | .NET 10, ASP.NET Core 10 |
| **Frontend** | Razor views (.cshtml), jQuery, server-rendered HTML | Angular 21 SPA, TypeScript, standalone components |
| **ORM** | Entity Framework 6 | Entity Framework Core 10 |
| **Data access** | Direct EF queries in controllers, ViewModels | Static Helpers + Projections returning DTOs |
| **Package mgmt** | NuGet packages.config | NuGet PackageReference (.csproj), npm (package.json) |
| **Auth** | OWIN cookie auth, ADFS/SAW SAML | Auth0 JWT bearer tokens |
| **Background jobs** | Hangfire (OWIN pipeline) | Hangfire (ASP.NET Core middleware) |
| **Maps** | Leaflet (server-side rendered) | Leaflet (Angular components) |
| **Charts** | Google Charts (server-side data) | Vega-Lite (client-side, declarative specs) |
| **Grids** | Custom GridSpec classes, server HTML | ag-Grid via `<wadnr-grid>` component |
| **CSS** | Bootstrap 3 | Custom grid system (`grid-12`/`g-col-*`), BEM SCSS, CSS custom properties |
| **Routing** | MVC route table + areas | Angular Router with lazy-loaded components |
| **Config** | Web.config, Global.asax | appsettings.json, appsecrets.json, Startup.cs |
| **Running locally** | IIS Express (Visual Studio F5) | Docker Compose (Visual Studio F5) + `npm start` (Angular dev server) |
| **API style** | MVC controller returning ViewResult | REST API returning JSON (Swagger/OpenAPI) |

### Key Mindset Shifts

- **No more Razor views.** The API only returns JSON. The Angular SPA renders everything.
- **No more ViewModels.** Instead, you have DTOs (Data Transfer Objects) that the API returns, and Angular components that consume them.
- **No more server-side rendering.** The API is stateless REST. The Angular app handles all UI state, routing, and rendering.
- **Multiple services running locally.** Docker Compose runs the API, Scalar (API docs), GDAL API (geospatial processing), GeoServer (map tiles), and SitkaCapture (screenshot service). The Angular dev server runs separately via `npm start` and proxies `/api/*` requests to the containerized API.
- **Docker Compose replaces IIS Express.** Instead of F5 launching IIS Express, you set the `docker-compose` project as your startup project in Visual Studio and F5 launches all backend services in containers.

---

## 2. Prerequisites & Setup

### Required Software

| Tool | Version | Notes |
|------|---------|-------|
| **.NET SDK** | 10.0 | [Download](https://dotnet.microsoft.com/download) - check with `dotnet --version` |
| **Node.js** | v22.17.0 | Use [nvm-windows](https://github.com/coreybutler/nvm-windows). The `.nvmrc` file specifies the version. |
| **npm** | Comes with Node | |
| **SQL Server** | Local instance (`.\`) | SQL Server Developer Edition or Express. Must support spatial types. |
| **Docker Desktop** | Latest | Required to run the backend services (API, GeoServer, GDAL, etc.) |
| **Git** | Latest | |
| **Visual Studio 2022** | Latest | Set `docker-compose` as startup project for F5 debugging |
| **VS Code** | Latest (optional) | Useful for Angular development alongside VS 2022 |
| **OpenSSL** | Any | For generating dev SSL certs (usually bundled with Git for Windows) |

### First-Time Setup

#### 1. Clone the repo

```powershell
git clone <repo-url> C:\git\esassoc\wadnrforesthealth
cd C:\git\esassoc\wadnrforesthealth
```

#### 2. Set up the database

You need a local SQL Server instance. The database name defaults to `WADNRDB` (configured in `Build/build.ini`).

```powershell
cd Build

# Option A: Download from Azure, restore, build, and scaffold (full pipeline)
# Requires Azure credentials in Build/secrets.ini (copy from secrets.ini.template)
.\DownloadRestoreBuildScaffold.ps1

# Option B: If you already have a .bak file
.\DatabaseRestore.ps1
.\DatabaseBuild.ps1      # Deploys the .dacpac (schema + release scripts)
.\Scaffold.ps1           # Regenerates EF models from the database
```

**Azure credentials**: Copy `Build/secrets.ini.template` to `Build/secrets.ini` and fill in the Azure storage connection string to download database backups.

**build.ini**: Contains database connection info. Default server is `.\` (local default instance). Change `Server` if your SQL Server instance name differs (e.g., `.\SQLEXPRESS`).

#### 3. Set up the API

```powershell
# From repo root
dotnet build WADNR.sln
```

The API needs config files that aren't in git. See [Section 3: Config Files You Need](#3-config-files-you-need-not-in-git) for the complete list and contents of every file.

#### 4. Set up the frontend

```powershell
cd WADNR.Web

# Install and use the correct Node version
nvm install 22.17.0
nvm use 22.17.0

# Install dependencies
npm install
```

#### 5. Set up SSL certificate

The Angular dev server runs on HTTPS. On first `npm start`, the `prestart` script automatically:
1. Installs the correct Node version via nvm
2. Generates a self-signed SSL cert (`server.crt` / `server.key`)
3. Trusts it in the Windows certificate store

If you need to do this manually:
```powershell
cd WADNR.Web
npm run create-dev-cert
npm run trust-dev-cert
```

#### 6. Hosts file entry

Add this to `C:\Windows\System32\drivers\etc\hosts`:
```
127.0.0.1 wadnr.localhost.esassoc.com
```

This is needed because the Angular dev server binds to `wadnr.localhost.esassoc.com:3215`.

---

## 3. Config Files You Need (Not in Git)

Four config files contain secrets and are **gitignored**. Get copies from a teammate to get up and running quickly.

| File | Template | Purpose | When you need it |
|------|----------|---------|------------------|
| `docker-compose/.env` | `docker-compose/.env.template` | Docker Compose environment variables — service URLs, email config, GeoServer password, feature flags, external API endpoints (DNR GIS, ArcGIS) | Always (Docker Compose won't start without it) |
| `WADNR.API/appsecrets.json` | None | API secrets — database connection string, SendGrid API key, Azure Blob Storage key, Hangfire credentials, ArcGIS client credentials | Always (API needs DB + service keys) |
| `WADNR.Scalar/appsecrets.json` | None | Scalar API docs DB connection (same connection string as the API) | Always (Scalar service needs DB access) |
| `Build/secrets.ini` | `Build/secrets.ini.template` | Azure Storage connection string for downloading database backups | Only for `DatabaseDownload.ps1` |

**Note on Docker DB connections:** The `appsecrets.json` files use `host.docker.internal` as the Data Source instead of `localhost` — that's how Docker containers reach SQL Server on your host machine. Your SQL Server also needs a SQL login (not just Windows auth) since Docker containers can't use Windows authentication. Make sure SQL Server has **mixed mode authentication** enabled and **TCP/IP** turned on in SQL Server Configuration Manager.

---

## 4. Running the App Locally

### Docker Compose (Backend Services)

The backend runs via **Docker Compose** in Visual Studio — this is the primary way to run the API and supporting services.

#### Setup

1. **Set startup project**: In Visual Studio, right-click the `docker-compose` project in Solution Explorer → "Set as Startup Project"
2. **Create `.env` file**: Copy `docker-compose/.env.template` to `docker-compose/.env` and fill in the values (ask the team for the current values)
3. **Press F5**: Visual Studio builds the Docker images and starts all services

#### What Docker Compose Runs

The `docker-compose.yml` + `docker-compose.override.yml` define these services:

| Service | Image | Ports | Purpose |
|---------|-------|-------|---------|
| **wadnr.api** | `wadnr/api` | `3211:8080` (HTTP), `3212:8081` (HTTPS) | The main REST API |
| **wadnr.scalar** | `wadnr/scalar` | `5611:8080`, `5612:8081` | API documentation (Scalar UI) |
| **wadnr.gdalapi** | `wadnr/gdalapi` | `3213:8080` | Geospatial processing (GDAL) |
| **geoserver** | `kartoza/geoserver:2.28.0` | `3280:8080` | Map tile server (WMS/WFS) |
| **sitkacapture** | `containersesaqa.azurecr.io/sitkacapture:latest` | `3216:3216` | Screenshot/PDF capture service |

**Legacy comparison**: In the old stack, you just hit F5 and IIS Express served everything as one process. Now F5 launches a fleet of Docker containers, each responsible for a different concern.

#### The `.env` File

The `.env` file provides environment variables to Docker Compose. Key settings:

```ini
# Secrets (paths inside the container)
SECRET_PATH_API=/run/secrets/appsecrets
SECRET_PATH_SCALAR=/run/secrets/appsecrets-scalar

# Email
SitkaEmailRedirect=your.email@example.com    # Set blank for prod
SitkaSupportEmail=rocket.team@sitkatech.com

# Feature flags
EnableE2ETestAuth=true   # Enables test auth for local dev

# External service URLs (Finance API, ArcGIS, etc.)
ProjectCodeJsonApiBaseUrl=...
VendorJsonApiBaseUrl=...
ArcGisAuthUrl=...

# Display
WebsiteDisplayName=WA DNR Forest Health Tracker
```

#### GeoServer Volume Mounts

GeoServer expects two volume mounts:
- `c:/git/esassoc/wadnrforesthealth/WADNR.GeoServer/data_dir` → `/opt/geoserver/data_dir`
- `c:/sitka/WADNRForestHealth/GeoServer` → `/app/config` (environment properties)

Make sure the `c:/sitka/WADNRForestHealth/GeoServer` directory exists with the GeoServer environment config. Ask the team for the `geoserver-environment.properties` file.

#### Debugging the API in Docker

When you F5 with the `docker-compose` project, Visual Studio attaches the debugger to the `wadnr.api` container automatically. You can set breakpoints in controller code and they'll hit just like with IIS Express.

To view API logs, use the "Containers" window in Visual Studio (View → Other Windows → Containers) or `docker logs`.

### Angular Dev Server (Frontend)

The Angular frontend runs **outside** Docker, in a separate terminal:

```powershell
cd C:\git\esassoc\wadnrforesthealth\WADNR.Web
npm start
```

The Angular app starts on `https://wadnr.localhost.esassoc.com:3215`.

### How the Proxy Works

The Angular dev server proxies API requests to the Docker containers. When the Angular app makes a request to `/api/projects`, the dev server (configured in `proxy.conf.dev.json`) forwards it to `https://localhost:3212/projects` (stripping the `/api` prefix and hitting the containerized API on port 3212).

```json
// proxy.conf.dev.json
{
    "/api": {
        "target": "https://localhost:3212",
        "secure": false,
        "changeOrigin": true,
        "pathRewrite": { "^/api": "" }
    },
    "/hangfire": {
        "target": "https://localhost:3212",
        "secure": false,
        "changeOrigin": true
    }
}
```

### Running Without Docker (Alternative)

If you need to run the API outside Docker (e.g., for quick iteration without rebuilding the image):

```powershell
dotnet run --project WADNR.API
```

This starts the API directly on `https://localhost:3212`. The Angular proxy config works the same way. However, you won't have GeoServer, GDAL API, or SitkaCapture running — only the main API.

---

## 5. Solution Structure

```
wadnrforesthealth/
├── WADNR.API/                  # ASP.NET Core Web API
│   ├── Controllers/            # REST endpoints (one per entity)
│   ├── Services/               # Auth, middleware, config, helpers
│   │   └── Authorization/      # Feature-based auth attributes
│   ├── Hangfire/               # Background job definitions
│   ├── ExcelSpecs/             # Excel export specifications
│   ├── Startup.cs              # DI container, middleware pipeline
│   ├── Program.cs              # Host builder entry point
│   ├── appsettings.json        # Non-secret config
│   └── swagger.json            # Auto-generated OpenAPI spec
│
├── WADNR.EFModels/             # Entity Framework Core models
│   └── Entities/
│       ├── Generated/          # ⚠️ AUTO-GENERATED - do not edit!
│       │   └── ExtensionMethods/ # Auto-generated extension methods
│       ├── {Entity}.StaticHelpers.cs    # Data access methods
│       └── {Entity}.DtoProjections.cs   # EF → DTO projections
│
├── WADNR.Models/               # Data Transfer Objects
│   └── DataTransferObjects/
│       └── {Entity}/           # DTOs grouped by entity
│
├── WADNR.Common/               # Shared utilities, email service
│
├── WADNR.Database/             # SQL Server database project
│   ├── dbo/Tables/             # Table definitions (.sql)
│   ├── dbo/Views/              # View definitions (.sql)
│   └── Scripts/
│       ├── ReleaseScripts/     # Idempotent migration scripts
│       └── LookupTables/       # MERGE scripts for reference data
│
├── WADNR.API.Tests/            # MSTest unit/integration tests
│
├── WADNR.Scalar/               # Scalar API documentation UI (separate service)
│
├── WADNR.GDALAPI/              # Geospatial processing API (GDAL-based)
│
├── WADNR.GeoServer/            # GeoServer config and data directory
│   └── data_dir/               # Volume-mounted into GeoServer container
│
├── SitkaCaptureService/        # Screenshot/PDF capture service
│
├── docker-compose/             # Docker Compose orchestration
│   ├── docker-compose.yml      # Service definitions
│   ├── docker-compose.override.yml  # Dev environment config (ports, env vars)
│   ├── docker-compose.dcproj   # VS startup project for F5
│   ├── .env.template           # Template for environment variables
│   └── .env                    # ⚠️ Local only - not committed (copy from template)
│
├── WADNR.Web/                  # Angular 21 SPA
│   └── src/app/
│       ├── pages/              # Page components (one folder per feature)
│       └── shared/
│           ├── components/     # Reusable components (grid, charts, map, etc.)
│           ├── generated/      # ⚠️ AUTO-GENERATED - do not edit!
│           │   ├── api/        # TypeScript API service classes
│           │   ├── model/      # TypeScript interfaces for DTOs
│           │   └── enum/       # TypeScript enums from lookup tables
│           ├── guards/         # Route guards (auth, unsaved changes)
│           ├── services/       # Shared Angular services
│           └── directives/     # Loading spinners, etc.
│
├── Build/                      # Build scripts & code generation
│   ├── build.ini               # Database & scaffold config
│   ├── secrets.ini.template    # Azure credentials template
│   ├── Scaffold.ps1            # Regenerate EF models from DB
│   ├── DatabaseBuild.ps1       # Build & deploy .dacpac
│   ├── DatabaseDownload.ps1    # Download backup from Azure
│   ├── DatabaseRestore.ps1     # Restore backup locally
│   └── typescript-angular/     # OpenAPI codegen templates
│
└── Source/                     # ⚠️ LEGACY - ProjectFirma MVC code (reference only)
```

### Legacy comparison

| Legacy | Modern |
|--------|--------|
| `Controllers/ProjectController.cs` returning `ViewResult` | `WADNR.API/Controllers/ProjectController.cs` returning `ActionResult<ProjectDetail>` |
| `Views/Project/Detail.cshtml` (Razor) | `WADNR.Web/src/app/pages/projects/project-detail/project-detail.component.ts` (Angular) |
| `Models/Project.cs` (EF entity + business logic mixed) | `WADNR.EFModels/Entities/Generated/Project.cs` (pure EF entity, auto-generated) + `Projects.StaticHelpers.cs` (data access) + `WADNR.Models/DataTransferObjects/Project/ProjectDetail.cs` (DTO) |
| `Views/Shared/ProjectControls/EditProject.cshtml` | `WADNR.Web/src/app/pages/projects/project-modal/project-modal.component.ts` (Angular modal) |

---

## 6. Backend: ASP.NET Core API Patterns

### Controllers

Controllers extend `SitkaController<T>` and use **primary constructors** (a C# 12 feature):

```csharp
[ApiController]
[Route("projects")]
public class ProjectController(
    WADNRDbContext dbContext,
    ILogger<ProjectController> logger,
    IOptions<WADNRConfiguration> configuration)
    : SitkaController<ProjectController>(dbContext, logger, configuration)
{
    [HttpGet]
    [ProjectViewFeature]
    public async Task<ActionResult<IEnumerable<ProjectGridRow>>> List()
    {
        var projects = await Projects.ListAsGridRowForUserAsync(DbContext, CallingUser);
        return Ok(projects);
    }

    [HttpGet("{projectID}")]
    [ProjectViewFeature]
    public async Task<ActionResult<ProjectDetail>> GetByID([FromRoute] int projectID)
    {
        var project = await Projects.GetByIDAsDetailAsync(DbContext, projectID);
        return RequireNotNullThrowNotFound(project, "Project", projectID);
    }
}
```

**Key differences from legacy MVC controllers:**
- `[ApiController]` + `[Route("projects")]` instead of MVC convention routing
- Returns `ActionResult<T>` (JSON) instead of `ViewResult` (HTML)
- `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]` attributes for REST verbs
- No `ViewBag`, no `ViewData`, no `PartialViewResult`
- Auth attributes (`[ProjectViewFeature]`) work similarly to legacy but are on the method/class level

**The base class** (`SitkaController<T>`) provides:
- `DbContext` - the EF Core database context
- `Logger` - Serilog logger
- `Configuration` - app configuration
- `CallingUser` - the authenticated user (from JWT token)
- `RequireNotNullThrowNotFound()` - standard null-check helper
- `ExcelFileResult()` - for Excel downloads

### Static Helpers (Data Access Layer)

Instead of querying EF directly in controllers, all data access goes through **static helper classes** in `WADNR.EFModels/Entities/`:

```csharp
// Classification.StaticHelpers.cs → class name is "Classifications" (plural)
public static class Classifications
{
    public static async Task<List<ClassificationGridRow>> ListAsGridRowAsync(WADNRDbContext dbContext)
        => await ClassificationProjections.AsGridRow(dbContext.Classifications.AsNoTracking())
            .OrderBy(x => x.ClassificationSortOrder).ThenBy(x => x.DisplayName)
            .ToListAsync();

    public static async Task<ClassificationDetail?> GetByIDAsDetailAsync(WADNRDbContext dbContext, int id)
        => await ClassificationProjections.AsDetail(dbContext.Classifications.AsNoTracking().Where(x => x.ClassificationID == id))
            .SingleOrDefaultAsync();

    public static async Task<ClassificationDetail?> CreateAsync(WADNRDbContext dbContext, ClassificationUpsertRequest dto)
    {
        var entity = new Classification { /* map from dto */ };
        dbContext.Classifications.Add(entity);
        await dbContext.SaveChangesAsync();
        return await GetByIDAsDetailAsync(dbContext, entity.ClassificationID);
    }
}
```

**Naming conventions:**
- `ListAs{DtoType}Async()` - returns a list
- `GetByIDAs{DtoType}Async()` - returns a single item
- Class name is **plural** (e.g., `Classifications`, `Projects`, `Agreements`)

### Projections (DTO Mapping)

Instead of AutoMapper or manual mapping in controllers, **projection classes** define how EF entities map to DTOs using `IQueryable`:

```csharp
// Classification.DtoProjections.cs
public static class ClassificationProjections
{
    public static IQueryable<ClassificationGridRow> AsGridRow(IQueryable<Classification> query)
        => query.Select(c => new ClassificationGridRow
        {
            ClassificationID = c.ClassificationID,
            DisplayName = c.DisplayName,
            ProjectCount = c.ProjectClassifications.Select(pc => pc.ProjectID).Distinct().Count()
        });
}
```

**Why this matters:** Projections run as SQL — only the fields you select get queried. This is much more efficient than loading entire entities.

### DTOs

DTOs live in `WADNR.Models/DataTransferObjects/{Entity}/`:

| DTO Type | Purpose |
|----------|---------|
| `{Entity}GridRow` | Minimal fields for list/grid views |
| `{Entity}Detail` | Full detail view with related data |
| `{Entity}UpsertRequest` | Create/update request body |
| `{Entity}LookupItem` | Dropdown/select options |

**Note:** DTO class names do NOT have a `Dto` suffix (e.g., `ProjectDetail`, not `ProjectDetailDto`).

### How a Request Flows

```
Browser → Angular component → Generated API service → /api/projects/123
    ↓ (proxy strips /api)
API Controller (ProjectController.GetByID)
    ↓
Static Helper (Projects.GetByIDAsDetailAsync)
    ↓
Projection (ProjectProjections.AsDetail) → SQL query
    ↓
DTO (ProjectDetail) → JSON response
    ↓
Angular component renders the data
```

---

## 7. Frontend: Angular Patterns

### Component Anatomy

Every feature lives in `WADNR.Web/src/app/pages/{feature}/`. Components are **standalone** (no NgModules):

```typescript
@Component({
    selector: 'app-classification-detail',
    standalone: true,
    imports: [CommonModule, RouterModule, WadnrGridComponent, IconComponent],
    templateUrl: './classification-detail.component.html',
    styleUrl: './classification-detail.component.scss'
})
export class ClassificationDetailComponent {
    // Route param bound automatically via withComponentInputBinding()
    @Input() set classificationID(value: string) {
        this._classificationID$.next(Number(value));
    }
    private _classificationID$ = new BehaviorSubject<number | null>(null);

    // Reactive data loading
    classification$ = this._classificationID$.pipe(
        filter((id): id is number => id != null),
        switchMap(id => this.classificationService.getByID(id)),
        shareReplay({ bufferSize: 1, refCount: true })
    );

    constructor(private classificationService: ClassificationService) {}
}
```

**Legacy comparison:**
- In legacy, the controller loaded data and passed it to the Razor view via `ViewData`. Here, the Angular component loads its own data from the API.
- Route params come in via `@Input()` setters (bound by the Angular Router), not `RouteData`.
- Data is reactive (RxJS Observables) — the template subscribes with `| async`.

### Template Syntax

Angular uses its own template syntax instead of Razor:

```html
<!-- Razor (old) -->
@if (Model.Classification != null) {
    <div class="panel-heading">
        <span>@Model.Classification.DisplayName</span>
    </div>
}

<!-- Angular (new) -->
@if (classification$ | async; as classification) {
    <div class="card-header">
        <span class="card-title">{{ classification.DisplayName }}</span>
    </div>
} @else {
    <app-loading-spinner></app-loading-spinner>
}
```

Key syntax differences:
- `{{ value }}` for interpolation (like `@Model.Value`)
- `@if / @else` for conditionals (like `@if`)
- `@for (item of items; track item.id)` for loops (like `@foreach`)
- `(click)="doSomething()"` for events (like `onclick`)
- `[property]="value"` for binding (like `value="@Model.X"`)
- `| async` pipe for subscribing to Observables

### Routing

Routes are defined in `WADNR.Web/src/app/app.routes.ts` with lazy loading:

```typescript
{
    path: "classifications/:classificationID",
    title: "Classification Detail",
    canActivate: [authGuard],
    loadComponent: () => import("./pages/classifications/classification-detail.component")
        .then(m => m.ClassificationDetailComponent),
}
```

**Every route that requires authentication must have `canActivate: [authGuard]`.** Check the API controller's auth attribute to know which guard to use.

### Generated Code (Do Not Edit!)

The `WADNR.Web/src/app/shared/generated/` directory is **auto-generated** from the API's `swagger.json`. It contains:

- `api/` - TypeScript service classes (one per controller, e.g., `classification.service.ts`)
- `model/` - TypeScript interfaces matching C# DTOs (e.g., `classification-detail.ts`)
- `enum/` - TypeScript enums from database lookup tables

**To regenerate after API changes:**
```powershell
# 1. Build the API to update swagger.json
dotnet build WADNR.API

# 2. Regenerate TypeScript
cd WADNR.Web
npm run gen-model
```

### Grids

The legacy stack used custom `GridSpec` classes that rendered server-side HTML tables. The modern stack uses **ag-Grid** via the `<wadnr-grid>` component:

```html
<wadnr-grid
    [columns]="columns"
    [rowData]="classifications$ | async"
    [gridOptions]="{ pagination: true }">
</wadnr-grid>
```

Column definitions use `UtilityFunctionsService`:

```typescript
this.columns = [
    this.utilityFunctionsService.createLinkColumnDef('Name', 'displayName', 'classificationID', 'ClassificationDetail'),
    this.utilityFunctionsService.createTextColumnDef('Description', 'classificationDescription'),
];
```

### Modals

The legacy stack used partial views rendered in Bootstrap modals. The modern stack uses **@ngneat/dialog**:

```typescript
// Opening a modal
this.dialog.open(ClassificationEditModalComponent, {
    data: { classificationID: id }
});

// Modal component
export class ClassificationEditModalComponent {
    constructor(public dialogRef: DialogRef<{ classificationID: number }>) {
        const data = this.dialogRef.data;
    }
}
```

### SCSS / Styling

**No Bootstrap!** The project uses its own grid system and CSS custom properties.

| Bootstrap (old) | Modern replacement |
|------------------|--------------------|
| `row` | `grid-12` |
| `col-6` | `g-col-6` |
| `panel` / `panel-heading` | `card` / `card-header` |
| `glyphicon` | `<icon [icon]="'IconName'">` |
| `btn btn-default` | `btn btn-secondary` |
| Bootstrap utility classes | Custom utilities in `src/scss/utilities/` |

SCSS uses **BEM naming** with the `&` nesting operator:

```scss
.classification-detail {
    &__header { /* ... */ }
    &__body { /* ... */ }
    &__item--active { /* ... */ }
}
```

**Always use CSS custom properties** from the theme (`src/scss/base/_theme.scss`):

```scss
// Good
color: var(--primary);
padding: var(--spacing-400);

// Bad
color: #3e72b0;
padding: 1rem;
```

---

## 8. Database & Code Generation Pipeline

### The Full Pipeline

When you make a database schema change, here's the complete flow:

```
1. Edit table .sql file in WADNR.Database/dbo/Tables/
         ↓
2. Run Build/DatabaseBuild.ps1  (builds .dacpac, deploys to local DB)
         ↓
3. Run Build/Scaffold.ps1       (regenerates EF entities + TypeScript enums)
         ↓
4. Create/update StaticHelpers, DtoProjections, DTOs (manual)
         ↓
5. Create/update API controller endpoints (manual)
         ↓
6. dotnet build WADNR.API       (regenerates swagger.json)
         ↓
7. cd WADNR.Web && npm run gen-model  (regenerates TypeScript API services + models)
         ↓
8. Create/update Angular components (manual)
```

### What Scaffold.ps1 Does

1. Runs `dotnet ef dbcontext scaffold` to regenerate EF entity classes in `WADNR.EFModels/Entities/Generated/`
2. Runs the custom `EFCorePOCOGenerator` tool that generates:
   - Extension methods in `WADNR.EFModels/Entities/Generated/ExtensionMethods/`
   - TypeScript enums in `WADNR.Web/src/app/shared/generated/enum/`

**Configuration** is in `Build/build.ini`:
- `Server` / `DatabaseName` - where to scaffold from
- `TableExcludeList` - tables to skip (staging tables, HangFire tables, views, etc.)
- `TypescriptEnumsPath` - where to output TypeScript enum files

### Release Scripts (Migrations)

There's no EF Migrations. Instead, the project uses **idempotent release scripts**:

```sql
-- WADNR.Database/Scripts/ReleaseScripts/0042 - Add IsActive to Widget.sql
DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0042 - Add IsActive to Widget'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    ALTER TABLE dbo.Widget ADD IsActive BIT NOT NULL DEFAULT 1;

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'YourName', @MigrationName, 'Add IsActive flag to Widget for soft deletes'
END
```

Scripts are numbered sequentially. The `IF NOT EXISTS` check makes them safe to run repeatedly.

### Lookup Tables

Reference data (enums) is managed via MERGE scripts in `WADNR.Database/Scripts/LookupTables/`:

```sql
MERGE INTO dbo.ProjectStage AS Target
USING (VALUES
    (1, 'Planning', 'Planning', 10),
    (2, 'Design', 'Design', 20),
    (3, 'Implementation', 'Implementation', 30)
) AS Source (ProjectStageID, ProjectStageName, ProjectStageDisplayName, SortOrder)
ON Target.ProjectStageID = Source.ProjectStageID
WHEN MATCHED THEN UPDATE SET ...
WHEN NOT MATCHED THEN INSERT ...;
```

When you `Scaffold.ps1`, these become:
- C# enum: `ProjectStageEnum.cs` in `WADNR.EFModels/Entities/Generated/`
- TypeScript enum: `project-stage-enum.ts` in `WADNR.Web/src/app/shared/generated/enum/`

### Adding a New Table (Step by Step)

1. Create `WADNR.Database/dbo/Tables/Widget.sql`:
   ```sql
   CREATE TABLE dbo.Widget (
       WidgetID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Widget_WidgetID PRIMARY KEY,
       WidgetName VARCHAR(200) NOT NULL,
       IsActive BIT NOT NULL CONSTRAINT DF_Widget_IsActive DEFAULT 1,
       CreateDate DATETIME NOT NULL CONSTRAINT DF_Widget_CreateDate DEFAULT GETDATE(),
       CreatePersonID INT NOT NULL CONSTRAINT FK_Widget_Person_CreatePersonID REFERENCES dbo.Person(PersonID)
   );
   ```
2. Run `Build/DatabaseBuild.ps1`
3. Run `Build/Scaffold.ps1`
4. Create `WADNR.Models/DataTransferObjects/Widget/WidgetGridRow.cs` and other DTOs
5. Create `WADNR.EFModels/Entities/Widget.DtoProjections.cs`
6. Create `WADNR.EFModels/Entities/Widget.StaticHelpers.cs`
7. Create `WADNR.API/Controllers/WidgetController.cs`
8. Build the API: `dotnet build WADNR.API`
9. Regenerate TypeScript: `cd WADNR.Web && npm run gen-model`
10. Create Angular components

---

## 9. Helm Charts & Environment Variable Pipeline

### Overview

The app deploys to **Azure Kubernetes Service (AKS)** via **Helm charts**. The Helm charts live in `charts/wadnr/` and define how each service is deployed, configured, and exposed across environments (QA, prod).

Understanding this pipeline is critical because it's how configuration and secrets propagate differently per environment.

### Folder Structure

```
charts/wadnr/
├── Chart.yaml                          # Parent chart metadata
├── values.yaml                         # Default values (secrets, domains, image repos)
└── charts/
    ├── wadnr-api/                      # API service
    │   └── templates/
    │       ├── deployment.yaml         # Pod spec, resource limits, probes, envFrom
    │       ├── configmap-qa.yaml       # QA-specific env vars (non-secret)
    │       ├── configmap-prod.yaml     # Prod-specific env vars (non-secret)
    │       ├── wadnr-api-secrets.yaml  # Secret values (DB password, API keys) → mounted as file
    │       ├── ingress.yaml            # External routing via Azure App Gateway
    │       ├── service.yaml
    │       └── serviceaccount.yaml
    ├── wadnr-web/                      # Angular frontend (same template pattern)
    │   └── templates/
    │       ├── configmap-qa.yaml
    │       ├── configmap-prod.yaml
    │       └── ...
    ├── wadnr-scalar/                   # Scalar API docs
    │   └── templates/
    │       ├── wadnr-scalar-secrets.yaml
    │       └── ...
    ├── wadnr-geoserver/                # GeoServer
    │   └── templates/
    │       ├── geoserver-secrets.yaml
    │       ├── geoserver-azure-file-secret.yaml
    │       └── ...
    ├── wadnr-gdalapi/                  # GDAL geospatial API
    │   └── templates/
    │       └── ...
    └── sitkacapture/                   # Screenshot service
        └── templates/
            └── ...
```

### How Configuration Flows Per Environment

There are **two types** of configuration, and they flow differently:

#### Non-Secret Config (ConfigMaps)

Non-secret environment variables (service URLs, email addresses, Auth0 domains) are defined in **per-environment ConfigMap templates**:

- `configmap-qa.yaml` — used when `global.env.name == "qa"`
- `configmap-prod.yaml` — used when `global.env.name == "prod"`

The API deployment mounts the correct one via `envFrom`:

```yaml
# deployment.yaml
envFrom:
- configMapRef:
    name: {{ include "api.fullname" . }}-{{ .Values.global.env.name }}-configmap
```

So the pod gets environment variables like `SitkaEmailRedirect`, `WebUrl`, `Auth0__Authority`, etc. — and each environment has its own values.

**Key differences between QA and Prod ConfigMaps:**

| Variable | QA | Prod |
|----------|-----|------|
| `ASPNETCORE_ENVIRONMENT` | `Staging` | `Production` |
| `SitkaEmailRedirect` | `notifications-qa@sitkatech.com` | `` (empty — real emails sent) |
| `Auth0__Authority` | `https://wadnr-qa.us.auth0.com/` | `https://wadnr.us.auth0.com/` |
| `WebUrl` | `https://wadnr.esa-qa.sitkatech.com` | `https://wadnr.esa-prod.sitkatech.com` |

#### Secrets

Sensitive values (DB passwords, API keys, SendGrid key) are defined in **Secret templates** that pull from `values.yaml`:

```yaml
# wadnr-api-secrets.yaml
stringData:
  wadnrApiSecrets: |
    {
      "DatabaseConnectionString": "...{{ .Values.global.secrets.apiSqlPassword }}...",
      "SendGridApiKey": "{{ .Values.global.secrets.sendGridApiKey }}",
      "ArcGisClientId": "{{ .Values.global.secrets.arcGisClientId }}",
      ...
    }
```

This secret is mounted as a file at `/app/secrets/wadnrApiSecrets` in the container. The API reads it via the `SECRET_PATH` environment variable — the same pattern as local dev with `appsecrets.json`.

Secret values in `values.yaml` are **placeholder defaults** — the real values come from **Azure Key Vault** at deploy time, injected by the CI/CD pipeline.

### Adding a New Environment Variable (Step by Step)

Here's the full pipeline for adding a new config value that the API needs:

#### If it's a non-secret (URL, feature flag, display name):

1. **Add to `WADNRConfiguration.cs`**:
   ```csharp
   public string MyNewServiceUrl { get; set; }
   ```

2. **Add to both Helm ConfigMaps** (QA and Prod values will differ):
   - `charts/wadnr/charts/wadnr-api/templates/configmap-qa.yaml`:
     ```yaml
     MyNewServiceUrl: "https://qa.example.com/api"
     ```
   - `charts/wadnr/charts/wadnr-api/templates/configmap-prod.yaml`:
     ```yaml
     MyNewServiceUrl: "https://prod.example.com/api"
     ```

3. **Add to `docker-compose/.env`** for local dev:
   ```ini
   MyNewServiceUrl=https://dev.example.com/api
   ```

4. **Add to `docker-compose/docker-compose.override.yml`** so Docker passes it to the container:
   ```yaml
   wadnr.api:
     environment:
       - MyNewServiceUrl=${MyNewServiceUrl}
   ```

5. **Use it in your code** via `Configuration.MyNewServiceUrl`

#### If it's a secret (password, API key, connection string):

1. **Add to `WADNRConfiguration.cs`**:
   ```csharp
   public string MyNewApiKey { get; set; }
   ```

2. **Add to `values.yaml`** under `global.secrets`:
   ```yaml
   global:
     secrets:
       myNewApiKey: "placeholderhere"
   ```

3. **Add to the secrets template** (`wadnr-api-secrets.yaml`):
   ```yaml
   "MyNewApiKey": "{{ .Values.global.secrets.myNewApiKey }}"
   ```

4. **Add to `WADNR.API/appsecrets.json`** for local dev:
   ```json
   "MyNewApiKey": "dev-key-here"
   ```

5. **Add to Azure Key Vault** for QA/Prod (the CI/CD pipeline pulls secrets from Key Vault and overrides `values.yaml` at deploy time)

6. **Use it in your code** via `Configuration.MyNewApiKey`

### How It All Connects

```
Local Dev                          Deployed (QA/Prod)
─────────                          ──────────────────
docker-compose/.env                values.yaml (defaults)
        ↓                                  ↓
docker-compose.override.yml        Azure Key Vault (overrides secrets)
        ↓                                  ↓
Container env vars                 Helm template rendering
        ↓                                  ↓
appsecrets.json (secrets)          ConfigMap (non-secret env vars)
        ↓                          Secret (mounted as /app/secrets/wadnrApiSecrets)
        ↓                                  ↓
        └──────── Both read by ────────────┘
                      ↓
          WADNRConfiguration.cs
          (ASP.NET Core config binding)
                      ↓
          Configuration.MyNewValue
          (used in controllers/services)
```

ASP.NET Core's configuration system merges all sources (appsettings.json, environment variables, secret files) — so the same `WADNRConfiguration` property works regardless of whether the value came from a ConfigMap env var or a mounted secrets file.

### Angular Frontend Config

The Angular app handles per-environment config differently — it uses **compile-time file replacement**, not runtime env vars:

```
src/environments/
├── environment.ts          # Local dev (default)
├── environment.qa.ts       # QA (swapped in during `npm run build-qa`)
└── environment.prod.ts     # Prod (swapped in during `npm run build-prod`)
```

Each file exports an `environment` object with API URLs, Auth0 config, and feature flags. The Angular build replaces the file at compile time based on the build configuration in `angular.json`.

To add a new frontend environment variable:
1. Add the property to all three `environment*.ts` files
2. Reference it in your component via `import { environment } from "src/environments/environment"`

---

## 10. Authentication & Authorization

### Legacy vs Modern Auth

| Legacy | Modern |
|--------|--------|
| OWIN cookie auth | Auth0 JWT bearer tokens |
| ADFS/SAW SAML federation | Auth0 social/enterprise connections |
| `[FirmaAdminFeature]` attributes | `[AdminFeature]` attributes (similar pattern) |
| Session-based | Stateless (JWT in Authorization header) |

### How It Works

1. User logs in via Auth0 (hosted login page)
2. Auth0 returns a JWT access token to the Angular app
3. Angular includes the token in every API request (`Authorization: Bearer <token>`)
4. The API validates the JWT and resolves the user from the database
5. Feature attributes check the user's role

### Authorization Attributes

Applied at the controller or method level:

```csharp
[HttpGet]
[NormalUserFeature]     // Requires Normal, ProjectSteward, Admin, or EsaAdmin role
public async Task<ActionResult<List<ProjectGridRow>>> List() { }

[HttpPost]
[AdminFeature]          // Requires Admin or EsaAdmin role
public async Task<ActionResult> Create([FromBody] ProjectUpsertRequest dto) { }

[HttpGet("public")]
[AllowAnonymous]        // No auth required, no user context populated
public async Task<ActionResult<List<ProjectSummary>>> ListPublic() { }
```

Key attributes: `[AllowAnonymous]`, `[NormalUserFeature]`, `[AdminFeature]`, `[ProjectEditFeature]`, `[ProjectViewFeature]`, `[AgreementManageFeature]`, `[ProgramManageFeature]`.

### Anonymous-With-User-Context Pattern (`[ProjectViewFeature]` / `[ProgramViewFeature]`)

Sometimes you need an endpoint that is **publicly accessible** but also **knows who the user is** if they happen to be logged in. For example, a project list page that anyone can view, but authenticated users see additional projects (like draft/pending ones).

`[AllowAnonymous]` won't work here because it skips authentication middleware entirely — `CallingUser` would always be the anonymous placeholder. Instead, use the **ViewFeature pattern**:

```csharp
[HttpGet]
[ProjectViewFeature]  // ← Allows anonymous AND populates user if authenticated
public async Task<ActionResult<IEnumerable<ProjectGridRow>>> List()
{
    // CallingUser is either the real user (if authenticated) or an anonymous
    // placeholder with PersonID = Person.AnonymousPersonID
    var projects = await Projects.ListAsGridRowForUserAsync(DbContext, CallingUser);
    return Ok(projects);
}
```

**How it works under the hood:**

`ProjectViewFeature` (in `WADNR.API/Services/Authorization/ProjectViewFeature.cs`) implements both `IAuthorizationFilter` and `IAllowAnonymous`:

```csharp
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class ProjectViewFeature : Attribute, IAuthorizationFilter, IAllowAnonymous
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Intentionally does NOT block any requests.
        // Allows anonymous access while ensuring that if a user IS authenticated,
        // their claims are available via HttpContext.User.
    }
}
```

The key difference from `[AllowAnonymous]`:
- `IAllowAnonymous` tells the authorization middleware not to reject unauthenticated requests
- But authentication middleware **still runs**, so if a JWT token is present, `HttpContext.User` gets populated with claims
- `CallingUser` (via `UserContext.GetUserAsDetailFromHttpContext`) then resolves the user from those claims, or falls back to an anonymous placeholder (`Person.AnonymousPersonID`) if no claims exist

**When to use which:**

| Attribute | Anonymous allowed? | User populated if logged in? | Use case |
|-----------|-------------------|------------------------------|----------|
| `[AllowAnonymous]` | Yes | No (skips auth entirely) | Truly public endpoints that never need user context (health checks, public summaries) |
| `[ProjectViewFeature]` / `[ProgramViewFeature]` | Yes | Yes (falls back to anonymous placeholder) | Public pages that show different data based on auth status |
| `[NormalUserFeature]` | No (401) | Yes | Standard authenticated endpoints |

**Creating a new ViewFeature:** If you need this pattern for a new entity, create a new attribute following the same pattern — implement `Attribute, IAuthorizationFilter, IAllowAnonymous` with an empty `OnAuthorization` body. See `ProjectViewFeature.cs` or `ProgramViewFeature.cs` as templates.

### Frontend Route Guards

Every Angular route that calls an authenticated API must have a guard:

```typescript
// In app.routes.ts
{
    path: "classifications",
    canActivate: [authGuard],  // ← Required!
    loadComponent: () => import("./pages/classifications/..."),
}
```

If you forget the guard, unauthenticated users will see a broken page with 401 errors.

For routes using ViewFeature endpoints (anonymous-with-user-context), **do not add `canActivate: [authGuard]`** — the page should be accessible to everyone. The Angular component should handle both states (logged-in user sees more data, anonymous user sees public data).

---

## 11. Common Gotchas

### 1. "I changed the database but nothing updated"

You need to run the full pipeline:
```powershell
cd Build
.\DatabaseBuild.ps1   # Deploy schema changes
.\Scaffold.ps1        # Regenerate EF models
cd ..\WADNR.Web
# Then build the API and run gen-model (see Section 7)
```

### 2. "I added an API endpoint but the Angular service doesn't have it"

After adding/changing API endpoints:
```powershell
dotnet build WADNR.API        # Regenerates swagger.json
cd WADNR.Web
npm run gen-model              # Regenerates TypeScript services
```

### 3. "The Angular app compiles but the page is blank / shows a spinner forever"

Check the browser console (F12). Common causes:
- Docker containers not running (check with `docker ps` or start via F5 in Visual Studio)
- API container crashed (check `docker logs` for errors)
- CORS or proxy misconfiguration
- Auth token expired (log out and back in)
- Database connection string in appsecrets.json uses `localhost` instead of `host.docker.internal` (containers can't reach `localhost` on the host)

### 4. "I edited a file in shared/generated/ and my changes disappeared"

Files in `WADNR.Web/src/app/shared/generated/` and `WADNR.EFModels/Entities/Generated/` are **auto-generated**. Never edit them manually. Make changes to the source (API endpoints, database schema) and regenerate.

### 5. "Bootstrap classes aren't working"

This project does **not** use Bootstrap. See the [SCSS/Styling section](#scss--styling) for the replacement map. Use `grid-12`/`g-col-*` instead of `row`/`col-*`, `card` instead of `panel`, etc.

### 6. "I need to add a dropdown of enum values"

For lookup table enums, use the pre-generated `AsSelectDropdownOptions` export:

```typescript
import { ProjectStagesAsSelectDropdownOptions } from "src/app/shared/generated/enum/project-stage-enum";

public options = ProjectStagesAsSelectDropdownOptions;
```

Don't manually map lookup table arrays.

### 7. ".Distinct() throws an error about geometry"

SQL Server can't compare spatial types. Get distinct IDs first, then query:

```csharp
// Wrong
var items = query.Select(j => j.Entity).Distinct().ToList();

// Right
var ids = query.Select(j => j.EntityID).Distinct().ToList();
var items = dbContext.Entities.Where(e => ids.Contains(e.EntityID)).ToList();
```

### 8. "Where do I put my new feature?"

- **New API endpoint** → `WADNR.API/Controllers/{Entity}Controller.cs`
- **New DTO** → `WADNR.Models/DataTransferObjects/{Entity}/`
- **New data access** → `WADNR.EFModels/Entities/{Entity}.StaticHelpers.cs`
- **New projection** → `WADNR.EFModels/Entities/{Entity}.DtoProjections.cs`
- **New page** → `WADNR.Web/src/app/pages/{feature}/`
- **New shared component** → `WADNR.Web/src/app/shared/components/{component}/`
- **New route** → `WADNR.Web/src/app/app.routes.ts`
- **Database schema change** → `WADNR.Database/dbo/Tables/`
- **Data migration** → `WADNR.Database/Scripts/ReleaseScripts/`

### 9. "How do I debug?"

- **API**: Set `docker-compose` as startup project in Visual Studio and press F5. The debugger attaches to the API container automatically. Breakpoints work normally.
- **Angular**: Use browser DevTools (F12). The Angular dev server includes source maps. You can set breakpoints in the Sources tab.
- **API requests**: Check the Network tab in DevTools, or use the Scalar API docs UI at `https://localhost:5612`
- **Container logs**: Use Visual Studio's Containers window (View → Other Windows → Containers) or `docker logs wadnr.api`

### 10. "The prestart script failed"

The `npm start` prestart script tries to install Node via nvm and generate SSL certs. If it fails:
- Make sure nvm-windows is installed
- Run the steps manually:
  ```powershell
  nvm install 22.17.0
  nvm use 22.17.0
  cd WADNR.Web
  npm run create-dev-cert
  npm run trust-dev-cert   # Needs admin PowerShell
  ```

---

## 12. Quick Reference Commands

### Daily Development

```powershell
# Start backend services: Set docker-compose as startup project in VS → F5
# Or from command line:
docker-compose -f docker-compose/docker-compose.yml -f docker-compose/docker-compose.override.yml up

# Start Angular (separate terminal)
cd WADNR.Web
npm start

# Build everything
dotnet build WADNR.sln

# Run API tests
dotnet test WADNR.API.Tests

# Run Angular tests
cd WADNR.Web
npm test

# Lint Angular code
cd WADNR.Web
npm run lint
npm run lint-fix          # Auto-fix
```

### After Database Changes

```powershell
cd Build
.\DatabaseBuild.ps1       # Deploy .dacpac to local DB
.\Scaffold.ps1            # Regenerate EF models + TypeScript enums
cd ..
dotnet build WADNR.API    # Rebuild API → regenerate swagger.json
cd WADNR.Web
npm run gen-model          # Regenerate TypeScript API services
```

### Full Database Reset

```powershell
cd Build
# Copy secrets.ini.template → secrets.ini and fill in Azure connection string
.\DownloadRestoreBuildScaffold.ps1
```

### Useful URLs (Local Dev)

| URL | Purpose |
|-----|---------|
| `https://wadnr.localhost.esassoc.com:3215` | Angular app (frontend dev server) |
| `https://localhost:3212` | API (HTTPS, via Docker) |
| `http://localhost:3211` | API (HTTP, via Docker) |
| `https://localhost:5612` | Scalar API documentation UI |
| `https://localhost:3212/hangfire` | Background job dashboard |
| `http://localhost:3280/geoserver` | GeoServer admin UI (user: `geomaster`) |
| `http://localhost:3213` | GDAL API (geospatial processing) |
| `http://localhost:3216` | SitkaCapture (screenshot service) |

---

## Need Help?

- **CLAUDE.md** in the repo root has the complete project conventions
- **`.claude/rules/`** has detailed patterns for Angular, .NET, database, and testing
- The legacy code is still in the repo at `Source/` for reference during migration
- Check the `Build/` directory README or scripts for database tooling questions
