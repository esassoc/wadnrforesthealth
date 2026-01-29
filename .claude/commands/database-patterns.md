# Database Patterns

Load this skill when working with `.sql` files, database schema changes, or migrations in WADNR.Database.

## Cross-References

| After schema changes... | Load |
|-------------------------|------|
| Creating API endpoints | `/dotnet-patterns` |
| Regenerating frontend models | Run `npm run gen-model` in WADNR.Web |

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

## Table Conventions

- Primary keys: `{TableName}ID` (e.g., `ProjectID`, `AgreementID`)
- Foreign keys: Match the referenced table's primary key name
- Audit columns: `CreateDate`, `CreatePersonID`, `UpdateDate`, `UpdatePersonID` where needed
- Soft delete: Use `IsActive` or similar flag rather than hard deletes where appropriate

---

## Adding a New Table

1. Create table definition in `WADNR.Database/dbo/Tables/{TableName}.sql`
2. Add any foreign key constraints
3. Create release script for initial data if needed
4. Run `Build/DatabaseBuild.ps1` to deploy
5. Run `Build/Scaffold.ps1` to generate EF models
6. Create static helpers and projections (see `/dotnet-patterns`)
