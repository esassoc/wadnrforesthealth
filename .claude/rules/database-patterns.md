# Database Patterns

> **Scope**: database
> **Applies when**: Working with .sql files, schema changes, or migrations in WADNR.Database

## Cross-References

| After schema changes... | Load |
|-------------------------|------|
| Creating API endpoints | `/dotnet-patterns` |
| Regenerating frontend models | Run `npm run gen-model` in WADNR.Web |
| Writing database tests | `/write-tests` |

---

## Migration Script Template

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

**Naming convention**: Scripts are numbered sequentially (0001, 0002, etc.) with a short description.

---

## Code Generation Pipeline

After making database changes, run this pipeline:

1. **Add/modify tables** in `WADNR.Database/dbo/Tables/`
2. **Run `Build/Scaffold.ps1`** to regenerate:
   - EF entities in `WADNR.EFModels/Entities/Generated/`
   - Extension methods in `WADNR.EFModels/Entities/Generated/ExtensionMethods/`
   - TypeScript enums in `WADNR.Web/src/app/shared/generated/enum/`
3. **Build the API** to regenerate `swagger.json`
4. **Run `npm run gen-model`** in WADNR.Web to regenerate TypeScript API clients

### Commands

```powershell
# From Build/ directory
.\Scaffold.ps1                    # Regenerate EF models
.\DatabaseBuild.ps1               # Build and deploy database project
.\DownloadRestoreBuildScaffold.ps1  # Full pipeline (download, restore, build, scaffold)
```

---

## Table Categories

| Category | Purpose | Examples |
|----------|---------|----------|
| Domain | Core business entities | `Project`, `Agreement`, `FocusArea` |
| Lookup | Enum-like reference tables with fixed IDs | `ProjectStage`, `Role`, `AgreementStatus` |
| Join | Many-to-many relationships | `ProjectCounty`, `ProjectOrganization` |
| Staging | Temporary import/processing data | `ProjectCodeImportStaging` |

---

## Column Naming Conventions

| Convention | Pattern | Examples |
|------------|---------|----------|
| Primary key | `{TableName}ID` | `ProjectID`, `AgreementID` |
| Foreign key | Match referenced table's PK name | `ProjectID`, `PersonID` |
| Boolean | `Is{Adjective}` | `IsActive`, `IsComplete`, `IsApproved` |
| Audit (create) | `CreateDate`, `CreatePersonID` | — |
| Audit (update) | `UpdateDate`, `UpdatePersonID` | — |
| Money | `decimal(18,2)` or `money` | `TotalCost`, `FederalFunding` |
| Spatial | `geometry` (NTS type) | `ProjectLocationGeometry`, `FocusAreaBoundary` |
| Soft delete | `IsActive` flag | — |

---

## Constraint Naming

| Constraint | Pattern | Example |
|------------|---------|---------|
| Primary key | `PK_{Table}_{Column}` | `PK_Project_ProjectID` |
| Foreign key | `FK_{Table}_{RefTable}_{Column}` | `FK_Project_ProjectStage_ProjectStageID` |
| Unique/alternate | `AK_{Table}_{Column}` | `AK_Person_Email` |
| Default | `DF_{Table}_{Column}` | `DF_Project_CreateDate` |

---

## Lookup Table Pattern

Lookup tables use explicit IDs (not IDENTITY) and are populated via MERGE scripts.

**Location**: `WADNR.Database/Scripts/LookupTables/`

```sql
-- Example: ProjectStage lookup table
MERGE INTO dbo.ProjectStage AS Target
USING (VALUES
    (1, 'Planning', 'Planning', 10),
    (2, 'Design', 'Design', 20),
    (3, 'Implementation', 'Implementation', 30),
    (4, 'Completed', 'Completed', 40)
) AS Source (ProjectStageID, ProjectStageName, ProjectStageDisplayName, SortOrder)
ON Target.ProjectStageID = Source.ProjectStageID
WHEN MATCHED THEN UPDATE SET
    ProjectStageName = Source.ProjectStageName,
    ProjectStageDisplayName = Source.ProjectStageDisplayName,
    SortOrder = Source.SortOrder
WHEN NOT MATCHED THEN INSERT (ProjectStageID, ProjectStageName, ProjectStageDisplayName, SortOrder)
    VALUES (Source.ProjectStageID, Source.ProjectStageName, Source.ProjectStageDisplayName, Source.SortOrder);
```

**Code generation**: `Scaffold.ps1` auto-generates C# enums and TypeScript enums for lookup tables.

---

## Index Guidelines

- **Always** create indexes on FK columns (EF does not auto-create these)
- Use `SPATIAL INDEX` for geometry columns used in spatial queries
- Add `UNIQUE` constraints on lookup table name columns
- Composite indexes: put the most selective column first

---

## View Conventions

- GeoServer views: prefix with `vGeoServer` (e.g., `vGeoServerProjectLocationDetailed`)
- Views are **excluded** from `Scaffold.ps1` code generation by default
- Define views in `WADNR.Database/dbo/Views/`

---

## Adding a New Table

1. Create table definition in `WADNR.Database/dbo/Tables/{TableName}.sql`
2. Add any foreign key constraints
3. Create indexes on FK columns
4. Create release script for initial data if needed
5. Run `Build/DatabaseBuild.ps1` to deploy
6. Run `Build/Scaffold.ps1` to generate EF models
7. Create static helpers and projections (see `/dotnet-patterns`)
