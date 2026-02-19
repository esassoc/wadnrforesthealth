# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Variables

These variables are used by migration skills (`.claude/skills/`). When copying skills to a new project, update these values to match your project structure.

| Variable | Value | Description |
|----------|-------|-------------|
| `{ApiProject}` | WADNR.API | ASP.NET Core Web API project |
| `{FrontendProject}` | WADNR.Web | Angular frontend project |
| `{EFModelsProject}` | WADNR.EFModels | Entity Framework models project |
| `{ModelsProject}` | WADNR.Models | DTOs project |
| `{DbContext}` | WADNRDbContext | EF DbContext class name |
| `{LegacyPath}` | Source/ProjectFirma.Web | Legacy MVC project path |
| `{GridComponent}` | wadnr-grid | Grid component selector |
| `{MapComponent}` | wadnr-map | Map component selector |
| `{BaseController}` | SitkaController\<T\> | Base controller class |

---

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

## Architecture Overview

- **APIs**: Controller -> Static Helper -> Projection pattern. Controllers call static helper methods that use EF projections to return DTOs.
- **Frontend**: Standalone Angular components with route params via `@Input()` and `BehaviorSubject`. Uses custom grid system (`grid-12`/`g-col-*`), not Bootstrap.
- **Database**: Release scripts with idempotent checks in `WADNR.Database/Scripts/ReleaseScripts/`.
- **Code Gen**: Database changes -> `Scaffold.ps1` -> Build API -> `npm run gen-model`

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

## Rules (Auto-Loaded Conventions)

Rules in `.claude/rules/` are auto-loaded based on context. They define "how we do things."

| Rule | Applies When |
|------|-------------|
| `angular-patterns` | Working in WADNR.Web |
| `dotnet-patterns` | Working in WADNR.API, EFModels, Models, Common |
| `database-patterns` | Working with .sql files or schema changes |
| `write-tests` | Creating unit tests |

---

## Skills (On-Demand Workflows)

Skills in `.claude/skills/` are invoked via `/skill-name`. They define step-by-step procedures.

| Skill | When to Use |
|-------|-------------|
| `/migrate-page` | Full page migration from MVC to Angular |
| `/migrate-grid` | Data grids with `<wadnr-grid>` |
| `/migrate-map` | Maps with Leaflet |
| `/migrate-workflow` | Multi-step wizard workflows |
| `/crud-modal` | Create/Edit modal dialog forms |
| `/add-scrollspy-toc` | Scrollspy Table of Contents sidebar |
| `/check-migration-status` | MVC to Angular migration audit |
