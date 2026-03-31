# 02 — Database Artifacts

> **Purpose**: All database artifacts for both Create and Update workflows.

---

## 1. Lookup Tables

### ProjectApprovalStatus (Create workflow)

Used for the Create workflow lifecycle. Populated via MERGE script in `WADNR.Database/Scripts/LookupTables/`.

| ID | Name | Display Name |
|----|------|-------------|
| 1 | Draft | Draft |
| 2 | PendingApproval | Pending Approval |
| 3 | Approved | Approved |
| 4 | Returned | Returned |
| 5 | Rejected | Rejected |

**Custom entity**: Replace `ProjectApprovalStatus` with `{Entity}ApprovalStatus`. Adjust statuses to match your entity's lifecycle.

### ProjectUpdateState (Update workflow)

Used for the Update workflow batch lifecycle.

| ID | Name | Display Name |
|----|------|-------------|
| 1 | Created | Created |
| 2 | Submitted | Submitted |
| 3 | Returned | Returned |
| 4 | Approved | Approved |

**Custom entity**: Replace `ProjectUpdateState` with `{Entity}UpdateState`.

---

## 2. Update Batch Table

The central table for the Update workflow. One batch per entity update cycle.

**Pattern from**: `WADNR.Database/dbo/Tables/ProjectUpdateBatch.sql`

**Key columns**:

| Column | Type | Purpose |
|--------|------|---------|
| `{Entity}UpdateBatchID` | `int IDENTITY` PK | Primary key |
| `{Entity}ID` | `int` FK | The entity being updated |
| `{Entity}UpdateStateID` | `int` FK | Current batch state |
| `LastUpdateDate` | `datetime` | Last modification timestamp |
| `LastUpdatePersonID` | `int` FK | Who last modified |
| **Per-step comment columns** | `varchar(max) NULL` | Reviewer comments per section |
| **Per-step diff log columns** | `varchar(max) NULL` | HTML diffs for audit trail |
| `StructuredDiffLogJson` | `varchar(max) NULL` | JSON structured diffs |
| **Per-step explanation columns** | `varchar(max) NULL` | "No X because..." explanations |

**Per-step comment columns** (one per reviewable step):
```sql
BasicsComment varchar(max) NULL,
LocationSimpleComment varchar(max) NULL,
LocationDetailedComment varchar(max) NULL,
ExpectedFundingComment varchar(max) NULL,
ContactsComment varchar(max) NULL,
OrganizationsComment varchar(max) NULL,
```

**Per-step diff log columns**:
```sql
BasicsDiffLog varchar(max) NULL,
OrganizationsDiffLog varchar(max) NULL,
ExternalLinksDiffLog varchar(max) NULL,
NotesDiffLog varchar(max) NULL,
ExpectedFundingDiffLog varchar(max) NULL,
StructuredDiffLogJson varchar(max) NULL,
```

**Per-step explanation columns** (for "no X because..." patterns):
```sql
NoPriorityLandscapesExplanation varchar(max) NULL,
NoRegionsExplanation varchar(max) NULL,
NoCountiesExplanation varchar(max) NULL,
```

---

## 3. Update Mirror Tables

For each related entity table, create a mirror table with:
- Same data columns as the original
- FK to `{Entity}UpdateBatch` instead of `{Entity}`
- Columns renamed where they'd conflict (e.g., `ProjectLocationUpdateGeometry` instead of `ProjectLocationGeometry`)

### Mirror Table Pattern

```sql
CREATE TABLE dbo.{RelatedEntity}Update (
    {RelatedEntity}UpdateID int IDENTITY(1,1) NOT NULL,
    {Entity}UpdateBatchID int NOT NULL,  -- FK to batch, NOT to entity
    -- Same data columns as {RelatedEntity} table
    -- Rename conflicting columns with "Update" suffix
    CONSTRAINT PK_{RelatedEntity}Update_{RelatedEntity}UpdateID PRIMARY KEY ({RelatedEntity}UpdateID),
    CONSTRAINT FK_{RelatedEntity}Update_{Entity}UpdateBatch_{Entity}UpdateBatchID
        FOREIGN KEY ({Entity}UpdateBatchID) REFERENCES dbo.{Entity}UpdateBatch({Entity}UpdateBatchID)
)
```

### Scalar Mirror Table

The main entity's scalar fields go in a dedicated Update table:

```sql
CREATE TABLE dbo.{Entity}Update (
    {Entity}UpdateID int IDENTITY(1,1) NOT NULL,
    {Entity}UpdateBatchID int NOT NULL,
    -- Same scalar columns as {Entity} table (not PKs, not audit columns)
    -- Geometry columns renamed: e.g., ProjectLocationPoint stays the same
    CONSTRAINT PK_{Entity}Update_{Entity}UpdateID PRIMARY KEY ({Entity}UpdateID),
    CONSTRAINT FK_{Entity}Update_{Entity}UpdateBatch_{Entity}UpdateBatchID
        FOREIGN KEY ({Entity}UpdateBatchID) REFERENCES dbo.{Entity}UpdateBatch({Entity}UpdateBatchID)
)
```

### Tables to Mirror (Project example)

| Original Table | Mirror Table | Notes |
|---------------|-------------|-------|
| `Project` (scalars) | `ProjectUpdate` | Stage, dates, description, location point, etc. |
| `ProjectLocation` | `ProjectLocationUpdate` | Detailed locations with geometry |
| `ProjectLocationStaging` | `ProjectLocationStagingUpdate` | GDB import staging |
| `Treatment` | `TreatmentUpdate` | Treatments on locations |
| `ProjectCounty` | `ProjectCountyUpdate` | County join |
| `ProjectRegion` | `ProjectRegionUpdate` | Region join |
| `ProjectPriorityLandscape` | `ProjectPriorityLandscapeUpdate` | Priority landscape join |
| `ProjectOrganization` | `ProjectOrganizationUpdate` | Organization join |
| `ProjectPerson` | `ProjectPersonUpdate` | Contact join |
| `ProjectFundingSource` | `ProjectFundingSourceUpdate` | Funding source join |
| `ProjectFundSourceAllocationRequest` | `ProjectFundSourceAllocationRequestUpdate` | Allocation request |
| `ProjectImage` | `ProjectImageUpdate` | Photos (shares FileResource) |
| `ProjectExternalLink` | `ProjectExternalLinkUpdate` | External links |
| `ProjectDocument` | `ProjectDocumentUpdate` | Documents (shares FileResource) |
| `ProjectNote` | `ProjectNoteUpdate` | Notes |
| `ProjectProgram` | `ProjectUpdateProgram` | Program join |

---

## 4. Update History Table

Audit trail for batch state transitions.

```sql
CREATE TABLE dbo.{Entity}UpdateHistory (
    {Entity}UpdateHistoryID int IDENTITY(1,1) NOT NULL,
    {Entity}UpdateBatchID int NOT NULL,
    {Entity}UpdateStateID int NOT NULL,
    UpdatePersonID int NOT NULL,
    TransitionDate datetime NOT NULL,
    CONSTRAINT PK_{Entity}UpdateHistory PRIMARY KEY ({Entity}UpdateHistoryID),
    CONSTRAINT FK_{Entity}UpdateHistory_{Entity}UpdateBatch
        FOREIGN KEY ({Entity}UpdateBatchID) REFERENCES dbo.{Entity}UpdateBatch({Entity}UpdateBatchID),
    CONSTRAINT FK_{Entity}UpdateHistory_{Entity}UpdateState
        FOREIGN KEY ({Entity}UpdateStateID) REFERENCES dbo.{Entity}UpdateState({Entity}UpdateStateID),
    CONSTRAINT FK_{Entity}UpdateHistory_Person
        FOREIGN KEY (UpdatePersonID) REFERENCES dbo.Person(PersonID)
)
```

---

## 5. Stored Procedure: pStart{Entity}UpdateBatch

Creates a new batch and copies all data from live tables to Update tables in a single database call.

**Pattern from**: `WADNR.Database/dbo/Procs/dbo.pStartProjectUpdateBatch.sql`

### Safety Guards

```sql
-- 1. Entity must be in Approved status
IF NOT EXISTS (SELECT 1 FROM dbo.{Entity} WHERE {Entity}ID = @{Entity}ID
    AND {Entity}ApprovalStatusID = @ApprovedStatusID)
BEGIN
    RAISERROR('Entity must be in Approved status to start an update.', 16, 1)
    RETURN
END

-- 2. No active (non-approved) batch for this entity
IF EXISTS (SELECT 1 FROM dbo.{Entity}UpdateBatch
    WHERE {Entity}ID = @{Entity}ID AND {Entity}UpdateStateID != @ApprovedStateID)
BEGIN
    RAISERROR('An update batch is already in progress.', 16, 1)
    RETURN
END
```

### Batch Creation

```sql
INSERT INTO dbo.{Entity}UpdateBatch (
    {Entity}ID, {Entity}UpdateStateID, LastUpdateDate, LastUpdatePersonID,
    -- Copy explanation columns from entity
    NoPriorityLandscapesExplanation, NoRegionsExplanation, NoCountiesExplanation
)
SELECT
    {Entity}ID, @CreatedStateID, GETDATE(), @CallingPersonID,
    NoPriorityLandscapesExplanation, NoRegionsExplanation, NoCountiesExplanation
FROM dbo.{Entity}
WHERE {Entity}ID = @{Entity}ID

SET @BatchID = SCOPE_IDENTITY()
```

### Data Copy Pattern

For each related table, INSERT-SELECT from live to Update:

```sql
-- Simple join tables (no geometry): straight copy
INSERT INTO dbo.{RelatedEntity}Update ({Entity}UpdateBatchID, ColumnA, ColumnB, ...)
SELECT @BatchID, ColumnA, ColumnB, ...
FROM dbo.{RelatedEntity}
WHERE {Entity}ID = @{Entity}ID
```

### Special Cases

1. **Scalar mirror**: Copy entity fields to `{Entity}Update`
2. **File resources**: Share `FileResourceID` (don't duplicate files). Set `FileResourceID` in Update table pointing to same resource.
3. **Treatments**: Match by location name + geometry to find corresponding `ProjectLocationUpdate`:
   ```sql
   INSERT INTO dbo.TreatmentUpdate (ProjectUpdateBatchID, ProjectLocationUpdateID, ...)
   SELECT @BatchID, plu.ProjectLocationUpdateID, ...
   FROM dbo.Treatment t
   INNER JOIN dbo.ProjectLocation pl ON t.ProjectLocationID = pl.ProjectLocationID
   INNER JOIN dbo.ProjectLocationUpdate plu
       ON plu.ProjectUpdateBatchID = @BatchID
       AND plu.ProjectLocationUpdateName = pl.ProjectLocationName
       AND plu.ProjectLocationUpdateGeometry.STEquals(pl.ProjectLocationGeometry) = 1
   WHERE pl.ProjectID = @{Entity}ID
   ```
4. **History entry**: Insert initial Created history record

---

## 6. FK Indexes

Create indexes on ALL foreign key columns in Update tables. EF does not auto-create these.

```sql
CREATE INDEX IX_{Table}_{Column} ON dbo.{Table}({Column})
```

Essential indexes:
- `{Entity}UpdateBatchID` on every Update table
- `{Entity}UpdateStateID` on `{Entity}UpdateBatch`
- `{Entity}ID` on `{Entity}UpdateBatch`
- `UpdatePersonID` on `{Entity}UpdateHistory`

---

## 7. Release Script Wrapper

All schema changes should be wrapped in an idempotent release script:

```sql
DECLARE @MigrationName VARCHAR(200)
SET @MigrationName = '0001 - Add {Entity} Update workflow tables'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    -- All CREATE TABLE statements here
    -- All CREATE INDEX statements here
    -- All MERGE (lookup table) statements here
    -- Stored procedure creation

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'AuthorName', @MigrationName, 'Add update workflow infrastructure for {Entity}'
END
```

---

## 8. Post-Database Steps

After creating all database artifacts:

```powershell
cd Build
.\DatabaseBuild.ps1    # Deploy schema changes
.\Scaffold.ps1         # Regenerate EF entities
```

Verify that `Scaffold.ps1` generates:
- `{Entity}UpdateBatch.cs` in `WADNR.EFModels/Entities/Generated/`
- `{Entity}Update.cs`
- All `{RelatedEntity}Update.cs` files
- `{Entity}UpdateHistory.cs`
- `{Entity}UpdateState.cs` (lookup enum)
- Extension methods in `Generated/ExtensionMethods/`
