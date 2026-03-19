/*
================================================================================
AUDIT LOGGING REQUIREMENT
================================================================================
This stored procedure bypasses Entity Framework and therefore does NOT trigger
automatic audit logging.

*** ANY DATA MODIFICATION IN THIS PROCEDURE MUST HAVE CORRESPONDING AUDIT LOGGING ***

When adding new tables or fields:
1. Add the DELETE/INSERT/UPDATE operation
2. IMMEDIATELY add the corresponding audit INSERT (see patterns below)
3. Test by checking AuditLog table after running the workflow

Audit Event Types:
  1 = Added
  2 = Deleted
  3 = Modified
================================================================================
*/
CREATE PROCEDURE dbo.pCommitProjectUpdateToProject
    @ProjectUpdateBatchID int,
    @CallingPersonID int
AS
BEGIN
    SET NOCOUNT ON;

    -- Resolve ProjectID from the batch
    DECLARE @ProjectID int;
    SELECT @ProjectID = ProjectID FROM dbo.ProjectUpdateBatch WHERE ProjectUpdateBatchID = @ProjectUpdateBatchID;

    IF @ProjectID IS NULL
        RETURN;

    -- Capture current timestamp for all audit records
    DECLARE @AuditDate datetime = GETUTCDATE();

    BEGIN TRANSACTION;

    BEGIN TRY
        -- ============================================================================
        -- SECTION 1: Project Scalar Fields
        -- AUDIT: Each modified field must be logged individually
        -- ============================================================================

        -- Capture original values BEFORE update (including geometry as EWKT string for comparison)
        SELECT
            ProjectDescription,
            ProjectStageID,
            PlannedDate,
            CompletionDate,
            ExpirationDate,
            EstimatedTotalCost,
            FocusAreaID,
            PercentageMatch,
            ProjectLocationSimpleTypeID,
            ProjectLocationNotes,
            ProjectFundingSourceNotes,
            NoPriorityLandscapesExplanation,
            NoRegionsExplanation,
            NoCountiesExplanation,
            CASE WHEN ProjectLocationPoint IS NOT NULL
                 THEN 'SRID=' + CAST(ProjectLocationPoint.STSrid as varchar(10)) + ';' + ProjectLocationPoint.STAsText()
                 ELSE NULL END AS ProjectLocationPointEWKT
        INTO #ProjectBefore
        FROM dbo.Project WHERE ProjectID = @ProjectID;

        -- Perform the UPDATE
        UPDATE p SET
            p.ProjectDescription = pu.ProjectDescription,
            p.ProjectStageID = pu.ProjectStageID,
            p.PlannedDate = pu.PlannedDate,
            p.CompletionDate = pu.CompletionDate,
            p.ExpirationDate = pu.ExpirationDate,
            p.EstimatedTotalCost = pu.EstimatedTotalCost,
            p.FocusAreaID = pu.FocusAreaID,
            p.PercentageMatch = pu.PercentageMatch,
            p.ProjectLocationPoint = pu.ProjectLocationPoint,
            p.ProjectLocationSimpleTypeID = pu.ProjectLocationSimpleTypeID,
            p.ProjectLocationNotes = pu.ProjectLocationNotes,
            p.ProjectFundingSourceNotes = pu.ProjectFundingSourceNotes,
            p.NoPriorityLandscapesExplanation = b.NoPriorityLandscapesExplanation,
            p.NoRegionsExplanation = b.NoRegionsExplanation,
            p.NoCountiesExplanation = b.NoCountiesExplanation
        FROM dbo.Project p
        INNER JOIN dbo.ProjectUpdate pu ON pu.ProjectUpdateBatchID = @ProjectUpdateBatchID
        INNER JOIN dbo.ProjectUpdateBatch b ON b.ProjectUpdateBatchID = @ProjectUpdateBatchID
        WHERE p.ProjectID = @ProjectID;

        -- AUDIT: Log each changed scalar field
        -- ProjectDescription
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'ProjectDescription',
               b.ProjectDescription, ISNULL(p.ProjectDescription, ''), @ProjectID
        FROM dbo.Project p CROSS JOIN #ProjectBefore b
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(CAST(b.ProjectDescription as nvarchar(max)), '') <> ISNULL(CAST(p.ProjectDescription as nvarchar(max)), '');

        -- ProjectStageID (with AuditDescription for human-readable change)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'ProjectStageID',
               CAST(b.ProjectStageID as varchar(max)), ISNULL(CAST(p.ProjectStageID as varchar(max)), ''),
               'Project Stage: ' + ISNULL(ps_old.ProjectStageDisplayName, '') + ' changed to ' + ISNULL(ps_new.ProjectStageDisplayName, ''),
               @ProjectID
        FROM dbo.Project p
        CROSS JOIN #ProjectBefore b
        LEFT JOIN dbo.ProjectStage ps_old ON ps_old.ProjectStageID = b.ProjectStageID
        LEFT JOIN dbo.ProjectStage ps_new ON ps_new.ProjectStageID = p.ProjectStageID
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(b.ProjectStageID, -1) <> ISNULL(p.ProjectStageID, -1);

        -- PlannedDate (date type - use date-only format)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'PlannedDate',
               ISNULL(FORMAT(b.PlannedDate, 'M/d/yyyy', 'en-US'), ''), ISNULL(FORMAT(p.PlannedDate, 'M/d/yyyy', 'en-US'), ''), @ProjectID
        FROM dbo.Project p CROSS JOIN #ProjectBefore b
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(b.PlannedDate, '1900-01-01') <> ISNULL(p.PlannedDate, '1900-01-01');

        -- CompletionDate (date type - use date-only format)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'CompletionDate',
               ISNULL(FORMAT(b.CompletionDate, 'M/d/yyyy', 'en-US'), ''), ISNULL(FORMAT(p.CompletionDate, 'M/d/yyyy', 'en-US'), ''), @ProjectID
        FROM dbo.Project p CROSS JOIN #ProjectBefore b
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(b.CompletionDate, '1900-01-01') <> ISNULL(p.CompletionDate, '1900-01-01');

        -- ExpirationDate (date type - use date-only format)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'ExpirationDate',
               ISNULL(FORMAT(b.ExpirationDate, 'M/d/yyyy', 'en-US'), ''), ISNULL(FORMAT(p.ExpirationDate, 'M/d/yyyy', 'en-US'), ''), @ProjectID
        FROM dbo.Project p CROSS JOIN #ProjectBefore b
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(b.ExpirationDate, '1900-01-01') <> ISNULL(p.ExpirationDate, '1900-01-01');

        -- EstimatedTotalCost
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'EstimatedTotalCost',
               CAST(b.EstimatedTotalCost as varchar(max)), ISNULL(CAST(p.EstimatedTotalCost as varchar(max)), ''), @ProjectID
        FROM dbo.Project p CROSS JOIN #ProjectBefore b
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(b.EstimatedTotalCost, -1) <> ISNULL(p.EstimatedTotalCost, -1);

        -- FocusAreaID
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'FocusAreaID',
               CAST(b.FocusAreaID as varchar(max)), ISNULL(CAST(p.FocusAreaID as varchar(max)), ''), @ProjectID
        FROM dbo.Project p CROSS JOIN #ProjectBefore b
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(b.FocusAreaID, -1) <> ISNULL(p.FocusAreaID, -1);

        -- PercentageMatch
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'PercentageMatch',
               CAST(b.PercentageMatch as varchar(max)), ISNULL(CAST(p.PercentageMatch as varchar(max)), ''), @ProjectID
        FROM dbo.Project p CROSS JOIN #ProjectBefore b
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(b.PercentageMatch, -1) <> ISNULL(p.PercentageMatch, -1);

        -- ProjectLocationSimpleTypeID
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'ProjectLocationSimpleTypeID',
               CAST(b.ProjectLocationSimpleTypeID as varchar(max)), ISNULL(CAST(p.ProjectLocationSimpleTypeID as varchar(max)), ''), @ProjectID
        FROM dbo.Project p CROSS JOIN #ProjectBefore b
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(b.ProjectLocationSimpleTypeID, -1) <> ISNULL(p.ProjectLocationSimpleTypeID, -1);

        -- ProjectLocationNotes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'ProjectLocationNotes',
               b.ProjectLocationNotes, ISNULL(p.ProjectLocationNotes, ''), @ProjectID
        FROM dbo.Project p CROSS JOIN #ProjectBefore b
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(CAST(b.ProjectLocationNotes as nvarchar(max)), '') <> ISNULL(CAST(p.ProjectLocationNotes as nvarchar(max)), '');

        -- ProjectFundingSourceNotes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'ProjectFundingSourceNotes',
               b.ProjectFundingSourceNotes, ISNULL(p.ProjectFundingSourceNotes, ''), @ProjectID
        FROM dbo.Project p CROSS JOIN #ProjectBefore b
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(CAST(b.ProjectFundingSourceNotes as nvarchar(max)), '') <> ISNULL(CAST(p.ProjectFundingSourceNotes as nvarchar(max)), '');

        -- NoPriorityLandscapesExplanation
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'NoPriorityLandscapesExplanation',
               b.NoPriorityLandscapesExplanation, ISNULL(p.NoPriorityLandscapesExplanation, ''), @ProjectID
        FROM dbo.Project p CROSS JOIN #ProjectBefore b
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(CAST(b.NoPriorityLandscapesExplanation as nvarchar(max)), '') <> ISNULL(CAST(p.NoPriorityLandscapesExplanation as nvarchar(max)), '');

        -- NoRegionsExplanation
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'NoRegionsExplanation',
               b.NoRegionsExplanation, ISNULL(p.NoRegionsExplanation, ''), @ProjectID
        FROM dbo.Project p CROSS JOIN #ProjectBefore b
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(CAST(b.NoRegionsExplanation as nvarchar(max)), '') <> ISNULL(CAST(p.NoRegionsExplanation as nvarchar(max)), '');

        -- NoCountiesExplanation
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'NoCountiesExplanation',
               b.NoCountiesExplanation, ISNULL(p.NoCountiesExplanation, ''), @ProjectID
        FROM dbo.Project p CROSS JOIN #ProjectBefore b
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(CAST(b.NoCountiesExplanation as nvarchar(max)), '') <> ISNULL(CAST(p.NoCountiesExplanation as nvarchar(max)), '');

        -- ProjectLocationPoint (geometry stored as EWKT string for audit comparison)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'Project', @ProjectID, 'ProjectLocationPoint',
               b.ProjectLocationPointEWKT,
               CASE WHEN p.ProjectLocationPoint IS NOT NULL
                    THEN 'SRID=' + CAST(p.ProjectLocationPoint.STSrid as varchar(10)) + ';' + p.ProjectLocationPoint.STAsText()
                    ELSE '' END,
               @ProjectID
        FROM dbo.Project p CROSS JOIN #ProjectBefore b
        WHERE p.ProjectID = @ProjectID
          AND ISNULL(b.ProjectLocationPointEWKT, '') <>
              ISNULL(CASE WHEN p.ProjectLocationPoint IS NOT NULL
                          THEN 'SRID=' + CAST(p.ProjectLocationPoint.STSrid as varchar(10)) + ';' + p.ProjectLocationPoint.STAsText()
                          ELSE NULL END, '');

        DROP TABLE #ProjectBefore;

        -- ============================================================================
        -- SECTION 2: Compute orphaned FileResource IDs (images/docs removed during update)
        -- Must compute BEFORE deleting the project-side rows.
        -- ============================================================================
        DECLARE @OrphanedFileResourceIDs TABLE (FileResourceID int);

        -- Orphaned image file resources
        INSERT INTO @OrphanedFileResourceIDs (FileResourceID)
        SELECT pi.FileResourceID
        FROM dbo.ProjectImage pi
        WHERE pi.ProjectID = @ProjectID
            AND pi.FileResourceID NOT IN (
                SELECT piu.FileResourceID FROM dbo.ProjectImageUpdate piu
                WHERE piu.ProjectUpdateBatchID = @ProjectUpdateBatchID AND piu.FileResourceID IS NOT NULL
            );

        -- Orphaned document file resources
        INSERT INTO @OrphanedFileResourceIDs (FileResourceID)
        SELECT pd.FileResourceID
        FROM dbo.ProjectDocument pd
        WHERE pd.ProjectID = @ProjectID
            AND pd.FileResourceID NOT IN (
                SELECT pdu.FileResourceID FROM dbo.ProjectDocumentUpdate pdu
                WHERE pdu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            );

        -- ============================================================================
        -- SECTION 3: Clear ProjectImageUpdate.ProjectImageID FK for images about to be deleted
        -- (Legacy MVC batches populated this FK; modern workflow does not use it.)
        -- Only clear for images being REMOVED (FileResourceID not in update set)
        -- ============================================================================
        UPDATE piu SET piu.ProjectImageID = NULL
        FROM dbo.ProjectImageUpdate piu
        WHERE piu.ProjectImageID IS NOT NULL
            AND piu.ProjectImageID IN (
                SELECT pi.ProjectImageID FROM dbo.ProjectImage pi
                WHERE pi.ProjectID = @ProjectID
                  AND NOT EXISTS (
                      SELECT 1 FROM dbo.ProjectImageUpdate piu2
                      WHERE piu2.ProjectUpdateBatchID = @ProjectUpdateBatchID
                        AND piu2.FileResourceID = pi.FileResourceID
                  )
            );

        -- ============================================================================
        -- SECTION 4: DELETE in FK-safe order
        -- AUDIT: Log deletions BEFORE the DELETE
        -- ============================================================================

        -- Treatment (FK to ProjectLocation, must delete first)
        -- Change detection: only delete treatments for locations being REMOVED
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 2, 'Treatment', t.TreatmentID, '*ALL', NULL, 'Deleted',
            'Treatment: deleted ' + ISNULL(tt.TreatmentTypeDisplayName, '') + ISNULL(' at ' + pl.ProjectLocationName, ''),
            @ProjectID
        FROM dbo.Treatment t
        LEFT JOIN dbo.TreatmentType tt ON t.TreatmentTypeID = tt.TreatmentTypeID
        LEFT JOIN dbo.ProjectLocation pl ON t.ProjectLocationID = pl.ProjectLocationID
        WHERE t.ProjectID = @ProjectID
          AND (t.ProjectLocationID IS NULL
               OR NOT EXISTS (
                   SELECT 1 FROM dbo.ProjectLocationUpdate plu
                   WHERE plu.ProjectUpdateBatchID = @ProjectUpdateBatchID
                     AND plu.ProjectLocationUpdateName = pl.ProjectLocationName
               ));

        DELETE t FROM dbo.Treatment t
        LEFT JOIN dbo.ProjectLocation pl ON t.ProjectLocationID = pl.ProjectLocationID
        WHERE t.ProjectID = @ProjectID
          AND (t.ProjectLocationID IS NULL
               OR NOT EXISTS (
                   SELECT 1 FROM dbo.ProjectLocationUpdate plu
                   WHERE plu.ProjectUpdateBatchID = @ProjectUpdateBatchID
                     AND plu.ProjectLocationUpdateName = pl.ProjectLocationName
               ));

        -- ProjectLocation (change detection: only delete removed locations)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 2, 'ProjectLocation', pl.ProjectLocationID, '*ALL', NULL, 'Deleted',
            'Location: deleted ' + ISNULL(pl.ProjectLocationName, '(unnamed)'), @ProjectID
        FROM dbo.ProjectLocation pl
        WHERE pl.ProjectID = @ProjectID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectLocationUpdate plu
              WHERE plu.ProjectUpdateBatchID = @ProjectUpdateBatchID
                AND plu.ProjectLocationUpdateName = pl.ProjectLocationName
          );

        DELETE pl FROM dbo.ProjectLocation pl
        WHERE pl.ProjectID = @ProjectID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectLocationUpdate plu
              WHERE plu.ProjectUpdateBatchID = @ProjectUpdateBatchID
                AND plu.ProjectLocationUpdateName = pl.ProjectLocationName
          );

        -- ProjectProgram (change detection: only delete removed, only insert new)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 2, 'ProjectProgram', pp.ProjectProgramID, '*ALL', NULL, 'Deleted',
            'Program: deleted ' + p.ProgramName, @ProjectID
        FROM dbo.ProjectProgram pp
        JOIN dbo.Program p ON pp.ProgramID = p.ProgramID
        WHERE pp.ProjectID = @ProjectID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectUpdateProgram pup
            WHERE pup.ProjectUpdateBatchID = @ProjectUpdateBatchID
              AND pup.ProgramID = pp.ProgramID
          );

        DELETE pp FROM dbo.ProjectProgram pp
        WHERE pp.ProjectID = @ProjectID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectUpdateProgram pup
            WHERE pup.ProjectUpdateBatchID = @ProjectUpdateBatchID
              AND pup.ProgramID = pp.ProgramID
          );

        -- ProjectPriorityLandscape (change detection: only delete removed, only insert new)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 2, 'ProjectPriorityLandscape', ppl.ProjectPriorityLandscapeID, '*ALL', NULL, 'Deleted',
            'Priority Landscape: deleted ' + pl.PriorityLandscapeName, @ProjectID
        FROM dbo.ProjectPriorityLandscape ppl
        JOIN dbo.PriorityLandscape pl ON ppl.PriorityLandscapeID = pl.PriorityLandscapeID
        WHERE ppl.ProjectID = @ProjectID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectPriorityLandscapeUpdate pplu
            WHERE pplu.ProjectUpdateBatchID = @ProjectUpdateBatchID
              AND pplu.PriorityLandscapeID = ppl.PriorityLandscapeID
          );

        DELETE ppl FROM dbo.ProjectPriorityLandscape ppl
        WHERE ppl.ProjectID = @ProjectID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectPriorityLandscapeUpdate pplu
            WHERE pplu.ProjectUpdateBatchID = @ProjectUpdateBatchID
              AND pplu.PriorityLandscapeID = ppl.PriorityLandscapeID
          );

        -- ProjectRegion (change detection: only delete removed, only insert new)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 2, 'ProjectRegion', pr.ProjectRegionID, '*ALL', NULL, 'Deleted',
            'Region: deleted ' + r.DNRUplandRegionName, @ProjectID
        FROM dbo.ProjectRegion pr
        JOIN dbo.DNRUplandRegion r ON pr.DNRUplandRegionID = r.DNRUplandRegionID
        WHERE pr.ProjectID = @ProjectID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectRegionUpdate pru
            WHERE pru.ProjectUpdateBatchID = @ProjectUpdateBatchID
              AND pru.DNRUplandRegionID = pr.DNRUplandRegionID
          );

        DELETE pr FROM dbo.ProjectRegion pr
        WHERE pr.ProjectID = @ProjectID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectRegionUpdate pru
            WHERE pru.ProjectUpdateBatchID = @ProjectUpdateBatchID
              AND pru.DNRUplandRegionID = pr.DNRUplandRegionID
          );

        -- ProjectCounty (change detection: only delete removed, only insert new)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 2, 'ProjectCounty', pc.ProjectCountyID, '*ALL', NULL, 'Deleted',
            'County: deleted ' + c.CountyName, @ProjectID
        FROM dbo.ProjectCounty pc
        JOIN dbo.County c ON pc.CountyID = c.CountyID
        WHERE pc.ProjectID = @ProjectID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectCountyUpdate pcu
            WHERE pcu.ProjectUpdateBatchID = @ProjectUpdateBatchID
              AND pcu.CountyID = pc.CountyID
          );

        DELETE pc FROM dbo.ProjectCounty pc
        WHERE pc.ProjectID = @ProjectID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectCountyUpdate pcu
            WHERE pcu.ProjectUpdateBatchID = @ProjectUpdateBatchID
              AND pcu.CountyID = pc.CountyID
          );

        -- ProjectOrganization (change detection: only delete removed, only insert new)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 2, 'ProjectOrganization', po.ProjectOrganizationID, '*ALL', NULL, 'Deleted',
            'Organization: deleted ' + o.OrganizationName + ' (' + rt.RelationshipTypeName + ')', @ProjectID
        FROM dbo.ProjectOrganization po
        JOIN dbo.Organization o ON po.OrganizationID = o.OrganizationID
        JOIN dbo.RelationshipType rt ON po.RelationshipTypeID = rt.RelationshipTypeID
        WHERE po.ProjectID = @ProjectID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectOrganizationUpdate pou
            WHERE pou.ProjectUpdateBatchID = @ProjectUpdateBatchID
              AND pou.OrganizationID = po.OrganizationID
              AND pou.RelationshipTypeID = po.RelationshipTypeID
          );

        DELETE po FROM dbo.ProjectOrganization po
        WHERE po.ProjectID = @ProjectID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectOrganizationUpdate pou
            WHERE pou.ProjectUpdateBatchID = @ProjectUpdateBatchID
              AND pou.OrganizationID = po.OrganizationID
              AND pou.RelationshipTypeID = po.RelationshipTypeID
          );

        -- ProjectPerson (contacts) - NOT audited, matches legacy EF IgnoredTables list
        DELETE FROM dbo.ProjectPerson WHERE ProjectID = @ProjectID;

        -- ProjectFundingSource (change detection: only delete removed, only insert new)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 2, 'ProjectFundingSource', pfs.ProjectFundingSourceID, '*ALL', NULL, 'Deleted',
            'Funding Source: deleted ' + fs.FundingSourceDisplayName, @ProjectID
        FROM dbo.ProjectFundingSource pfs
        JOIN dbo.FundingSource fs ON pfs.FundingSourceID = fs.FundingSourceID
        WHERE pfs.ProjectID = @ProjectID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectFundingSourceUpdate pfsu
            WHERE pfsu.ProjectUpdateBatchID = @ProjectUpdateBatchID
              AND pfsu.FundingSourceID = pfs.FundingSourceID
          );

        DELETE pfs FROM dbo.ProjectFundingSource pfs
        WHERE pfs.ProjectID = @ProjectID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectFundingSourceUpdate pfsu
            WHERE pfsu.ProjectUpdateBatchID = @ProjectUpdateBatchID
              AND pfsu.FundingSourceID = pfs.FundingSourceID
          );

        -- ProjectFundSourceAllocationRequest (change detection: only delete removed, by FundSourceAllocationID)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 2, 'ProjectFundSourceAllocationRequest', pfsar.ProjectFundSourceAllocationRequestID, '*ALL', NULL, 'Deleted',
            'Allocation Request: deleted ' + ISNULL(fsa.FundSourceAllocationName, CAST(pfsar.FundSourceAllocationID as varchar(20))),
            @ProjectID
        FROM dbo.ProjectFundSourceAllocationRequest pfsar
        LEFT JOIN dbo.FundSourceAllocation fsa ON pfsar.FundSourceAllocationID = fsa.FundSourceAllocationID
        WHERE pfsar.ProjectID = @ProjectID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectFundSourceAllocationRequestUpdate pfsaru
              WHERE pfsaru.ProjectUpdateBatchID = @ProjectUpdateBatchID
                AND pfsaru.FundSourceAllocationID = pfsar.FundSourceAllocationID
          );

        DELETE pfsar FROM dbo.ProjectFundSourceAllocationRequest pfsar
        WHERE pfsar.ProjectID = @ProjectID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectFundSourceAllocationRequestUpdate pfsaru
              WHERE pfsaru.ProjectUpdateBatchID = @ProjectUpdateBatchID
                AND pfsaru.FundSourceAllocationID = pfsar.FundSourceAllocationID
          );

        -- ProjectImage (change detection: only delete removed, by FileResourceID)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 2, 'ProjectImage', pi.ProjectImageID, '*ALL', NULL, 'Deleted',
            'Image: deleted ' + ISNULL(pi.Caption, '(no caption)'), @ProjectID
        FROM dbo.ProjectImage pi
        WHERE pi.ProjectID = @ProjectID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectImageUpdate piu
              WHERE piu.ProjectUpdateBatchID = @ProjectUpdateBatchID
                AND piu.FileResourceID = pi.FileResourceID
          );

        DELETE pi FROM dbo.ProjectImage pi
        WHERE pi.ProjectID = @ProjectID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectImageUpdate piu
              WHERE piu.ProjectUpdateBatchID = @ProjectUpdateBatchID
                AND piu.FileResourceID = pi.FileResourceID
          );

        -- ProjectExternalLink (change detection: only delete removed, by Label+URL)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 2, 'ProjectExternalLink', pel.ProjectExternalLinkID, '*ALL', NULL, 'Deleted',
            'External Link: deleted ' + ISNULL(pel.ExternalLinkLabel, '(no label)'), @ProjectID
        FROM dbo.ProjectExternalLink pel
        WHERE pel.ProjectID = @ProjectID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectExternalLinkUpdate pelu
              WHERE pelu.ProjectUpdateBatchID = @ProjectUpdateBatchID
                AND ISNULL(pelu.ExternalLinkLabel, '') = ISNULL(pel.ExternalLinkLabel, '')
                AND ISNULL(pelu.ExternalLinkUrl, '') = ISNULL(pel.ExternalLinkUrl, '')
          );

        DELETE pel FROM dbo.ProjectExternalLink pel
        WHERE pel.ProjectID = @ProjectID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectExternalLinkUpdate pelu
              WHERE pelu.ProjectUpdateBatchID = @ProjectUpdateBatchID
                AND ISNULL(pelu.ExternalLinkLabel, '') = ISNULL(pel.ExternalLinkLabel, '')
                AND ISNULL(pelu.ExternalLinkUrl, '') = ISNULL(pel.ExternalLinkUrl, '')
          );

        -- ProjectDocument (change detection: only delete removed, by FileResourceID)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 2, 'ProjectDocument', pd.ProjectDocumentID, '*ALL', NULL, 'Deleted',
            'Document: deleted ' + ISNULL(pd.DisplayName, '(unnamed)'), @ProjectID
        FROM dbo.ProjectDocument pd
        WHERE pd.ProjectID = @ProjectID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectDocumentUpdate pdu
              WHERE pdu.ProjectUpdateBatchID = @ProjectUpdateBatchID
                AND pdu.FileResourceID = pd.FileResourceID
          );

        DELETE pd FROM dbo.ProjectDocument pd
        WHERE pd.ProjectID = @ProjectID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectDocumentUpdate pdu
              WHERE pdu.ProjectUpdateBatchID = @ProjectUpdateBatchID
                AND pdu.FileResourceID = pd.FileResourceID
          );

        -- ProjectNote (change detection: only delete removed, by CreatePersonID+CreateDate)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 2, 'ProjectNote', pn.ProjectNoteID, '*ALL', NULL, 'Deleted',
            'Note: deleted ' + ISNULL(LEFT(pn.Note, 100), '(empty)'), @ProjectID
        FROM dbo.ProjectNote pn
        WHERE pn.ProjectID = @ProjectID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectNoteUpdate pnu
              WHERE pnu.ProjectUpdateBatchID = @ProjectUpdateBatchID
                AND pnu.CreatePersonID = pn.CreatePersonID
                AND pnu.CreateDate = pn.CreateDate
          );

        DELETE pn FROM dbo.ProjectNote pn
        WHERE pn.ProjectID = @ProjectID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectNoteUpdate pnu
              WHERE pnu.ProjectUpdateBatchID = @ProjectUpdateBatchID
                AND pnu.CreatePersonID = pn.CreatePersonID
                AND pnu.CreateDate = pn.CreateDate
          );

        -- ============================================================================
        -- SECTION 4b: AUDIT diffs then UPDATE matched rows with changed properties
        -- Critical: Audit BEFORE update to capture old values
        -- ============================================================================

        -- ---- ProjectLocation: matched by ProjectLocationName ----
        -- Audit ProjectLocationTypeID changes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectLocation', pl.ProjectLocationID, 'ProjectLocationTypeID',
               CAST(pl.ProjectLocationTypeID as varchar(max)), CAST(plu.ProjectLocationTypeID as varchar(max)),
               'Location Type: ' + ISNULL(plt_old.ProjectLocationTypeDisplayName, '') + ' changed to ' + ISNULL(plt_new.ProjectLocationTypeDisplayName, ''),
               @ProjectID
        FROM dbo.ProjectLocation pl
        INNER JOIN dbo.ProjectLocationUpdate plu
            ON plu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND plu.ProjectLocationUpdateName = pl.ProjectLocationName
        LEFT JOIN dbo.ProjectLocationType plt_old ON plt_old.ProjectLocationTypeID = pl.ProjectLocationTypeID
        LEFT JOIN dbo.ProjectLocationType plt_new ON plt_new.ProjectLocationTypeID = plu.ProjectLocationTypeID
        WHERE pl.ProjectID = @ProjectID
          AND ISNULL(pl.ProjectLocationTypeID, -1) <> ISNULL(plu.ProjectLocationTypeID, -1);

        -- Audit ProjectLocationGeometry changes (compare via STEquals)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectLocation', pl.ProjectLocationID, 'ProjectLocationGeometry',
               CASE WHEN pl.ProjectLocationGeometry IS NOT NULL
                    THEN 'SRID=' + CAST(pl.ProjectLocationGeometry.STSrid as varchar(10)) + ';' + pl.ProjectLocationGeometry.MakeValid().STAsText()
                    ELSE NULL END,
               CASE WHEN plu.ProjectLocationUpdateGeometry IS NOT NULL
                    THEN 'SRID=' + CAST(plu.ProjectLocationUpdateGeometry.STSrid as varchar(10)) + ';' + plu.ProjectLocationUpdateGeometry.MakeValid().STAsText()
                    ELSE NULL END,
               'Location Geometry: changed for ' + ISNULL(pl.ProjectLocationName, '(unnamed)'),
               @ProjectID
        FROM dbo.ProjectLocation pl
        INNER JOIN dbo.ProjectLocationUpdate plu
            ON plu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND plu.ProjectLocationUpdateName = pl.ProjectLocationName
        WHERE pl.ProjectID = @ProjectID
          AND (
              (pl.ProjectLocationGeometry IS NULL AND plu.ProjectLocationUpdateGeometry IS NOT NULL)
              OR (pl.ProjectLocationGeometry IS NOT NULL AND plu.ProjectLocationUpdateGeometry IS NULL)
              OR (pl.ProjectLocationGeometry IS NOT NULL AND plu.ProjectLocationUpdateGeometry IS NOT NULL
                  AND pl.ProjectLocationGeometry.MakeValid().STEquals(plu.ProjectLocationUpdateGeometry.MakeValid()) = 0)
          );

        -- Audit ProjectLocationNotes changes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectLocation', pl.ProjectLocationID, 'ProjectLocationNotes',
               pl.ProjectLocationNotes, plu.ProjectLocationUpdateNotes,
               'Location Notes: changed for ' + ISNULL(pl.ProjectLocationName, '(unnamed)'),
               @ProjectID
        FROM dbo.ProjectLocation pl
        INNER JOIN dbo.ProjectLocationUpdate plu
            ON plu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND plu.ProjectLocationUpdateName = pl.ProjectLocationName
        WHERE pl.ProjectID = @ProjectID
          AND ISNULL(CAST(pl.ProjectLocationNotes as nvarchar(max)), '') <> ISNULL(CAST(plu.ProjectLocationUpdateNotes as nvarchar(max)), '');

        -- Update ProjectLocation matched rows
        UPDATE pl SET
            pl.ProjectLocationTypeID = plu.ProjectLocationTypeID,
            pl.ProjectLocationGeometry = plu.ProjectLocationUpdateGeometry,
            pl.ProjectLocationNotes = plu.ProjectLocationUpdateNotes,
            pl.ArcGisObjectID = plu.ArcGisObjectID,
            pl.ArcGisGlobalID = plu.ArcGisGlobalID
        FROM dbo.ProjectLocation pl
        INNER JOIN dbo.ProjectLocationUpdate plu
            ON plu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND plu.ProjectLocationUpdateName = pl.ProjectLocationName
        WHERE pl.ProjectID = @ProjectID;

        -- ---- ProjectImage: matched by FileResourceID ----
        -- Audit Caption changes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectImage', pi.ProjectImageID, 'Caption',
               pi.Caption, piu.Caption,
               'Image Caption: ' + ISNULL(pi.Caption, '(none)') + ' changed to ' + ISNULL(piu.Caption, '(none)'),
               @ProjectID
        FROM dbo.ProjectImage pi
        INNER JOIN dbo.ProjectImageUpdate piu
            ON piu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND piu.FileResourceID = pi.FileResourceID
        WHERE pi.ProjectID = @ProjectID
          AND ISNULL(CAST(pi.Caption as nvarchar(max)), '') <> ISNULL(CAST(piu.Caption as nvarchar(max)), '');

        -- Audit Credit changes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectImage', pi.ProjectImageID, 'Credit',
               pi.Credit, piu.Credit,
               'Image Credit: ' + ISNULL(pi.Credit, '(none)') + ' changed to ' + ISNULL(piu.Credit, '(none)'),
               @ProjectID
        FROM dbo.ProjectImage pi
        INNER JOIN dbo.ProjectImageUpdate piu
            ON piu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND piu.FileResourceID = pi.FileResourceID
        WHERE pi.ProjectID = @ProjectID
          AND ISNULL(CAST(pi.Credit as nvarchar(max)), '') <> ISNULL(CAST(piu.Credit as nvarchar(max)), '');

        -- Audit IsKeyPhoto changes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectImage', pi.ProjectImageID, 'IsKeyPhoto',
               CAST(pi.IsKeyPhoto as varchar(5)), CAST(piu.IsKeyPhoto as varchar(5)),
               'Image Key Photo: ' + CASE WHEN pi.IsKeyPhoto = 1 THEN 'Yes' ELSE 'No' END + ' changed to ' + CASE WHEN piu.IsKeyPhoto = 1 THEN 'Yes' ELSE 'No' END,
               @ProjectID
        FROM dbo.ProjectImage pi
        INNER JOIN dbo.ProjectImageUpdate piu
            ON piu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND piu.FileResourceID = pi.FileResourceID
        WHERE pi.ProjectID = @ProjectID
          AND pi.IsKeyPhoto <> piu.IsKeyPhoto;

        -- Audit ExcludeFromFactSheet changes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectImage', pi.ProjectImageID, 'ExcludeFromFactSheet',
               CAST(pi.ExcludeFromFactSheet as varchar(5)), CAST(piu.ExcludeFromFactSheet as varchar(5)),
               'Image Exclude From Fact Sheet: ' + CASE WHEN pi.ExcludeFromFactSheet = 1 THEN 'Yes' ELSE 'No' END + ' changed to ' + CASE WHEN piu.ExcludeFromFactSheet = 1 THEN 'Yes' ELSE 'No' END,
               @ProjectID
        FROM dbo.ProjectImage pi
        INNER JOIN dbo.ProjectImageUpdate piu
            ON piu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND piu.FileResourceID = pi.FileResourceID
        WHERE pi.ProjectID = @ProjectID
          AND pi.ExcludeFromFactSheet <> piu.ExcludeFromFactSheet;

        -- Audit ProjectImageTimingID changes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectImage', pi.ProjectImageID, 'ProjectImageTimingID',
               CAST(pi.ProjectImageTimingID as varchar(max)), CAST(piu.ProjectImageTimingID as varchar(max)),
               'Image Timing: ' + ISNULL(pit_old.ProjectImageTimingDisplayName, '') + ' changed to ' + ISNULL(pit_new.ProjectImageTimingDisplayName, ''),
               @ProjectID
        FROM dbo.ProjectImage pi
        INNER JOIN dbo.ProjectImageUpdate piu
            ON piu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND piu.FileResourceID = pi.FileResourceID
        LEFT JOIN dbo.ProjectImageTiming pit_old ON pit_old.ProjectImageTimingID = pi.ProjectImageTimingID
        LEFT JOIN dbo.ProjectImageTiming pit_new ON pit_new.ProjectImageTimingID = piu.ProjectImageTimingID
        WHERE pi.ProjectID = @ProjectID
          AND ISNULL(pi.ProjectImageTimingID, -1) <> ISNULL(piu.ProjectImageTimingID, -1);

        -- Update ProjectImage matched rows
        UPDATE pi SET
            pi.Caption = piu.Caption,
            pi.Credit = piu.Credit,
            pi.IsKeyPhoto = piu.IsKeyPhoto,
            pi.ExcludeFromFactSheet = piu.ExcludeFromFactSheet,
            pi.ProjectImageTimingID = piu.ProjectImageTimingID
        FROM dbo.ProjectImage pi
        INNER JOIN dbo.ProjectImageUpdate piu
            ON piu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND piu.FileResourceID = pi.FileResourceID
        WHERE pi.ProjectID = @ProjectID;

        -- ---- ProjectFundSourceAllocationRequest: matched by FundSourceAllocationID ----
        -- Audit TotalAmount changes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectFundSourceAllocationRequest', pfsar.ProjectFundSourceAllocationRequestID, 'TotalAmount',
               CAST(pfsar.TotalAmount as varchar(max)), CAST(pfsaru.TotalAmount as varchar(max)),
               'Allocation Request Total: ' + ISNULL(fsa.FundSourceAllocationName, CAST(pfsar.FundSourceAllocationID as varchar(20))) + ' changed from ' + ISNULL(CAST(pfsar.TotalAmount as varchar(20)), '(none)') + ' to ' + ISNULL(CAST(pfsaru.TotalAmount as varchar(20)), '(none)'),
               @ProjectID
        FROM dbo.ProjectFundSourceAllocationRequest pfsar
        INNER JOIN dbo.ProjectFundSourceAllocationRequestUpdate pfsaru
            ON pfsaru.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND pfsaru.FundSourceAllocationID = pfsar.FundSourceAllocationID
        LEFT JOIN dbo.FundSourceAllocation fsa ON pfsar.FundSourceAllocationID = fsa.FundSourceAllocationID
        WHERE pfsar.ProjectID = @ProjectID
          AND ISNULL(pfsar.TotalAmount, -1) <> ISNULL(pfsaru.TotalAmount, -1);

        -- Audit MatchAmount changes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectFundSourceAllocationRequest', pfsar.ProjectFundSourceAllocationRequestID, 'MatchAmount',
               CAST(pfsar.MatchAmount as varchar(max)), CAST(pfsaru.MatchAmount as varchar(max)),
               'Allocation Request Match: ' + ISNULL(fsa.FundSourceAllocationName, CAST(pfsar.FundSourceAllocationID as varchar(20))) + ' changed from ' + ISNULL(CAST(pfsar.MatchAmount as varchar(20)), '(none)') + ' to ' + ISNULL(CAST(pfsaru.MatchAmount as varchar(20)), '(none)'),
               @ProjectID
        FROM dbo.ProjectFundSourceAllocationRequest pfsar
        INNER JOIN dbo.ProjectFundSourceAllocationRequestUpdate pfsaru
            ON pfsaru.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND pfsaru.FundSourceAllocationID = pfsar.FundSourceAllocationID
        LEFT JOIN dbo.FundSourceAllocation fsa ON pfsar.FundSourceAllocationID = fsa.FundSourceAllocationID
        WHERE pfsar.ProjectID = @ProjectID
          AND ISNULL(pfsar.MatchAmount, -1) <> ISNULL(pfsaru.MatchAmount, -1);

        -- Audit PayAmount changes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectFundSourceAllocationRequest', pfsar.ProjectFundSourceAllocationRequestID, 'PayAmount',
               CAST(pfsar.PayAmount as varchar(max)), CAST(pfsaru.PayAmount as varchar(max)),
               'Allocation Request Pay: ' + ISNULL(fsa.FundSourceAllocationName, CAST(pfsar.FundSourceAllocationID as varchar(20))) + ' changed from ' + ISNULL(CAST(pfsar.PayAmount as varchar(20)), '(none)') + ' to ' + ISNULL(CAST(pfsaru.PayAmount as varchar(20)), '(none)'),
               @ProjectID
        FROM dbo.ProjectFundSourceAllocationRequest pfsar
        INNER JOIN dbo.ProjectFundSourceAllocationRequestUpdate pfsaru
            ON pfsaru.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND pfsaru.FundSourceAllocationID = pfsar.FundSourceAllocationID
        LEFT JOIN dbo.FundSourceAllocation fsa ON pfsar.FundSourceAllocationID = fsa.FundSourceAllocationID
        WHERE pfsar.ProjectID = @ProjectID
          AND ISNULL(pfsar.PayAmount, -1) <> ISNULL(pfsaru.PayAmount, -1);

        -- Update ProjectFundSourceAllocationRequest matched rows
        UPDATE pfsar SET
            pfsar.TotalAmount = pfsaru.TotalAmount,
            pfsar.MatchAmount = pfsaru.MatchAmount,
            pfsar.PayAmount = pfsaru.PayAmount,
            pfsar.UpdateDate = pfsaru.UpdateDate
        FROM dbo.ProjectFundSourceAllocationRequest pfsar
        INNER JOIN dbo.ProjectFundSourceAllocationRequestUpdate pfsaru
            ON pfsaru.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND pfsaru.FundSourceAllocationID = pfsar.FundSourceAllocationID
        WHERE pfsar.ProjectID = @ProjectID;

        -- ---- ProjectDocument: matched by FileResourceID ----
        -- Audit DisplayName changes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectDocument', pd.ProjectDocumentID, 'DisplayName',
               pd.DisplayName, pdu.DisplayName,
               'Document Name: ' + ISNULL(pd.DisplayName, '(none)') + ' changed to ' + ISNULL(pdu.DisplayName, '(none)'),
               @ProjectID
        FROM dbo.ProjectDocument pd
        INNER JOIN dbo.ProjectDocumentUpdate pdu
            ON pdu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND pdu.FileResourceID = pd.FileResourceID
        WHERE pd.ProjectID = @ProjectID
          AND ISNULL(CAST(pd.DisplayName as nvarchar(max)), '') <> ISNULL(CAST(pdu.DisplayName as nvarchar(max)), '');

        -- Audit Description changes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectDocument', pd.ProjectDocumentID, 'Description',
               pd.Description, pdu.Description,
               'Document Description: changed for ' + ISNULL(pd.DisplayName, '(unnamed)'),
               @ProjectID
        FROM dbo.ProjectDocument pd
        INNER JOIN dbo.ProjectDocumentUpdate pdu
            ON pdu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND pdu.FileResourceID = pd.FileResourceID
        WHERE pd.ProjectID = @ProjectID
          AND ISNULL(CAST(pd.Description as nvarchar(max)), '') <> ISNULL(CAST(pdu.Description as nvarchar(max)), '');

        -- Audit ProjectDocumentTypeID changes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectDocument', pd.ProjectDocumentID, 'ProjectDocumentTypeID',
               CAST(pd.ProjectDocumentTypeID as varchar(max)), CAST(pdu.ProjectDocumentTypeID as varchar(max)),
               'Document Type: ' + ISNULL(dt_old.ProjectDocumentTypeName, '') + ' changed to ' + ISNULL(dt_new.ProjectDocumentTypeName, ''),
               @ProjectID
        FROM dbo.ProjectDocument pd
        INNER JOIN dbo.ProjectDocumentUpdate pdu
            ON pdu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND pdu.FileResourceID = pd.FileResourceID
        LEFT JOIN dbo.ProjectDocumentType dt_old ON dt_old.ProjectDocumentTypeID = pd.ProjectDocumentTypeID
        LEFT JOIN dbo.ProjectDocumentType dt_new ON dt_new.ProjectDocumentTypeID = pdu.ProjectDocumentTypeID
        WHERE pd.ProjectID = @ProjectID
          AND ISNULL(pd.ProjectDocumentTypeID, -1) <> ISNULL(pdu.ProjectDocumentTypeID, -1);

        -- Update ProjectDocument matched rows
        UPDATE pd SET
            pd.DisplayName = pdu.DisplayName,
            pd.Description = pdu.Description,
            pd.ProjectDocumentTypeID = pdu.ProjectDocumentTypeID
        FROM dbo.ProjectDocument pd
        INNER JOIN dbo.ProjectDocumentUpdate pdu
            ON pdu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND pdu.FileResourceID = pd.FileResourceID
        WHERE pd.ProjectID = @ProjectID;

        -- ---- ProjectNote: matched by CreatePersonID + CreateDate ----
        -- Audit Note changes
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 3, 'ProjectNote', pn.ProjectNoteID, 'Note',
               LEFT(pn.Note, 500), LEFT(pnu.Note, 500),
               'Note: changed ' + ISNULL(LEFT(pn.Note, 100), '(empty)'),
               @ProjectID
        FROM dbo.ProjectNote pn
        INNER JOIN dbo.ProjectNoteUpdate pnu
            ON pnu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND pnu.CreatePersonID = pn.CreatePersonID
            AND pnu.CreateDate = pn.CreateDate
        WHERE pn.ProjectID = @ProjectID
          AND ISNULL(CAST(pn.Note as nvarchar(max)), '') <> ISNULL(CAST(pnu.Note as nvarchar(max)), '');

        -- Update ProjectNote matched rows
        UPDATE pn SET
            pn.Note = pnu.Note,
            pn.UpdatePersonID = pnu.UpdatePersonID,
            pn.UpdateDate = pnu.UpdateDate
        FROM dbo.ProjectNote pn
        INNER JOIN dbo.ProjectNoteUpdate pnu
            ON pnu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND pnu.CreatePersonID = pn.CreatePersonID
            AND pnu.CreateDate = pn.CreateDate
        WHERE pn.ProjectID = @ProjectID;

        -- ============================================================================
        -- SECTION 5: INSERT from update tables back to project tables
        -- AUDIT: Log additions AFTER the INSERT to capture the new PK values
        -- ============================================================================

        -- Programs (change detection: only insert new)
        DECLARE @NewProjectPrograms TABLE (ProjectProgramID int, ProgramID int);
        INSERT INTO dbo.ProjectProgram (ProjectID, ProgramID)
        OUTPUT inserted.ProjectProgramID, inserted.ProgramID INTO @NewProjectPrograms
        SELECT @ProjectID, up.ProgramID
        FROM dbo.ProjectUpdateProgram up
        WHERE up.ProjectUpdateBatchID = @ProjectUpdateBatchID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectProgram pp
            WHERE pp.ProjectID = @ProjectID AND pp.ProgramID = up.ProgramID
          );

        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 1, 'ProjectProgram', np.ProjectProgramID, 'ProgramID', NULL, CAST(np.ProgramID as varchar(max)),
            'Program: added ' + p.ProgramName, @ProjectID
        FROM @NewProjectPrograms np
        JOIN dbo.Program p ON np.ProgramID = p.ProgramID;

        -- Priority Landscapes (change detection: only insert new)
        DECLARE @NewProjectPriorityLandscapes TABLE (ProjectPriorityLandscapeID int, PriorityLandscapeID int);
        INSERT INTO dbo.ProjectPriorityLandscape (ProjectID, PriorityLandscapeID)
        OUTPUT inserted.ProjectPriorityLandscapeID, inserted.PriorityLandscapeID INTO @NewProjectPriorityLandscapes
        SELECT @ProjectID, upl.PriorityLandscapeID
        FROM dbo.ProjectPriorityLandscapeUpdate upl
        WHERE upl.ProjectUpdateBatchID = @ProjectUpdateBatchID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectPriorityLandscape ppl
            WHERE ppl.ProjectID = @ProjectID AND ppl.PriorityLandscapeID = upl.PriorityLandscapeID
          );

        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 1, 'ProjectPriorityLandscape', np.ProjectPriorityLandscapeID, 'PriorityLandscapeID', NULL, CAST(np.PriorityLandscapeID as varchar(max)),
            'Priority Landscape: added ' + pl.PriorityLandscapeName, @ProjectID
        FROM @NewProjectPriorityLandscapes np
        JOIN dbo.PriorityLandscape pl ON np.PriorityLandscapeID = pl.PriorityLandscapeID;

        -- Regions (change detection: only insert new)
        DECLARE @NewProjectRegions TABLE (ProjectRegionID int, DNRUplandRegionID int);
        INSERT INTO dbo.ProjectRegion (ProjectID, DNRUplandRegionID)
        OUTPUT inserted.ProjectRegionID, inserted.DNRUplandRegionID INTO @NewProjectRegions
        SELECT @ProjectID, ur.DNRUplandRegionID
        FROM dbo.ProjectRegionUpdate ur
        WHERE ur.ProjectUpdateBatchID = @ProjectUpdateBatchID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectRegion pr
            WHERE pr.ProjectID = @ProjectID AND pr.DNRUplandRegionID = ur.DNRUplandRegionID
          );

        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 1, 'ProjectRegion', nr.ProjectRegionID, 'DNRUplandRegionID', NULL, CAST(nr.DNRUplandRegionID as varchar(max)),
            'Region: added ' + r.DNRUplandRegionName, @ProjectID
        FROM @NewProjectRegions nr
        JOIN dbo.DNRUplandRegion r ON nr.DNRUplandRegionID = r.DNRUplandRegionID;

        -- Counties (change detection: only insert new)
        DECLARE @NewProjectCounties TABLE (ProjectCountyID int, CountyID int);
        INSERT INTO dbo.ProjectCounty (ProjectID, CountyID)
        OUTPUT inserted.ProjectCountyID, inserted.CountyID INTO @NewProjectCounties
        SELECT @ProjectID, uc.CountyID
        FROM dbo.ProjectCountyUpdate uc
        WHERE uc.ProjectUpdateBatchID = @ProjectUpdateBatchID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectCounty pc
            WHERE pc.ProjectID = @ProjectID AND pc.CountyID = uc.CountyID
          );

        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 1, 'ProjectCounty', nc.ProjectCountyID, 'CountyID', NULL, CAST(nc.CountyID as varchar(max)),
            'County: added ' + c.CountyName, @ProjectID
        FROM @NewProjectCounties nc
        JOIN dbo.County c ON nc.CountyID = c.CountyID;

        -- Locations (change detection: only insert new, by ProjectLocationName)
        DECLARE @NewProjectLocations TABLE (ProjectLocationID int, ProjectLocationName varchar(200));
        INSERT INTO dbo.ProjectLocation (ProjectID, ProjectLocationTypeID, ProjectLocationGeometry,
            ProjectLocationNotes, ProjectLocationName, ArcGisObjectID, ArcGisGlobalID)
        OUTPUT inserted.ProjectLocationID, inserted.ProjectLocationName INTO @NewProjectLocations
        SELECT @ProjectID, ul.ProjectLocationTypeID, ul.ProjectLocationUpdateGeometry,
            ul.ProjectLocationUpdateNotes, ul.ProjectLocationUpdateName, ul.ArcGisObjectID, ul.ArcGisGlobalID
        FROM dbo.ProjectLocationUpdate ul
        WHERE ul.ProjectUpdateBatchID = @ProjectUpdateBatchID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectLocation pl
              WHERE pl.ProjectID = @ProjectID
                AND pl.ProjectLocationName = ul.ProjectLocationUpdateName
          );

        -- Audit: Single summary entry per new location
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 1, 'ProjectLocation', nl.ProjectLocationID, 'ProjectLocationName', NULL, nl.ProjectLocationName,
            'Location: added ' + ISNULL(nl.ProjectLocationName, '(unnamed)'), @ProjectID
        FROM @NewProjectLocations nl;

        -- Organizations (change detection: only insert new)
        DECLARE @NewProjectOrganizations TABLE (ProjectOrganizationID int, OrganizationID int, RelationshipTypeID int);
        INSERT INTO dbo.ProjectOrganization (ProjectID, OrganizationID, RelationshipTypeID)
        OUTPUT inserted.ProjectOrganizationID, inserted.OrganizationID, inserted.RelationshipTypeID INTO @NewProjectOrganizations
        SELECT @ProjectID, uo.OrganizationID, uo.RelationshipTypeID
        FROM dbo.ProjectOrganizationUpdate uo
        WHERE uo.ProjectUpdateBatchID = @ProjectUpdateBatchID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectOrganization po
            WHERE po.ProjectID = @ProjectID
              AND po.OrganizationID = uo.OrganizationID
              AND po.RelationshipTypeID = uo.RelationshipTypeID
          );

        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 1, 'ProjectOrganization', no_.ProjectOrganizationID, 'OrganizationID', NULL, CAST(no_.OrganizationID as varchar(max)),
            'Organization: added ' + o.OrganizationName + ' (' + rt.RelationshipTypeName + ')', @ProjectID
        FROM @NewProjectOrganizations no_
        JOIN dbo.Organization o ON no_.OrganizationID = o.OrganizationID
        JOIN dbo.RelationshipType rt ON no_.RelationshipTypeID = rt.RelationshipTypeID;

        -- Contacts - NOT audited, matches legacy EF IgnoredTables list
        INSERT INTO dbo.ProjectPerson (ProjectID, PersonID, ProjectPersonRelationshipTypeID)
        SELECT @ProjectID, up.PersonID, up.ProjectPersonRelationshipTypeID
        FROM dbo.ProjectPersonUpdate up
        WHERE up.ProjectUpdateBatchID = @ProjectUpdateBatchID;

        -- Funding Sources (change detection: only insert new)
        DECLARE @NewProjectFundingSources TABLE (ProjectFundingSourceID int, FundingSourceID int);
        INSERT INTO dbo.ProjectFundingSource (ProjectID, FundingSourceID)
        OUTPUT inserted.ProjectFundingSourceID, inserted.FundingSourceID INTO @NewProjectFundingSources
        SELECT @ProjectID, ufs.FundingSourceID
        FROM dbo.ProjectFundingSourceUpdate ufs
        WHERE ufs.ProjectUpdateBatchID = @ProjectUpdateBatchID
          AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectFundingSource pfs
            WHERE pfs.ProjectID = @ProjectID AND pfs.FundingSourceID = ufs.FundingSourceID
          );

        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 1, 'ProjectFundingSource', nfs.ProjectFundingSourceID, 'FundingSourceID', NULL, CAST(nfs.FundingSourceID as varchar(max)),
            'Funding Source: added ' + fs.FundingSourceDisplayName, @ProjectID
        FROM @NewProjectFundingSources nfs
        JOIN dbo.FundingSource fs ON nfs.FundingSourceID = fs.FundingSourceID;

        -- Allocation Requests (change detection: only insert new, by FundSourceAllocationID)
        DECLARE @NewAllocRequests TABLE (ProjectFundSourceAllocationRequestID int, FundSourceAllocationID int);
        INSERT INTO dbo.ProjectFundSourceAllocationRequest (ProjectID, FundSourceAllocationID,
            TotalAmount, MatchAmount, PayAmount, CreateDate, UpdateDate, ImportedFromTabularData)
        OUTPUT inserted.ProjectFundSourceAllocationRequestID, inserted.FundSourceAllocationID INTO @NewAllocRequests
        SELECT @ProjectID, uar.FundSourceAllocationID,
            uar.TotalAmount, uar.MatchAmount, uar.PayAmount, uar.CreateDate, uar.UpdateDate, uar.ImportedFromTabularData
        FROM dbo.ProjectFundSourceAllocationRequestUpdate uar
        WHERE uar.ProjectUpdateBatchID = @ProjectUpdateBatchID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectFundSourceAllocationRequest pfsar
              WHERE pfsar.ProjectID = @ProjectID
                AND pfsar.FundSourceAllocationID = uar.FundSourceAllocationID
          );

        -- Audit: Single summary entry per new allocation request (with display name)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 1, 'ProjectFundSourceAllocationRequest', nar.ProjectFundSourceAllocationRequestID, 'FundSourceAllocationID', NULL, CAST(nar.FundSourceAllocationID as varchar(max)),
            'Allocation Request: added ' + ISNULL(fsa.FundSourceAllocationName, CAST(nar.FundSourceAllocationID as varchar(20))),
            @ProjectID
        FROM @NewAllocRequests nar
        LEFT JOIN dbo.FundSourceAllocation fsa ON nar.FundSourceAllocationID = fsa.FundSourceAllocationID;

        -- Images (change detection: only insert new, by FileResourceID)
        DECLARE @NewProjectImages TABLE (ProjectImageID int, Caption varchar(500));
        INSERT INTO dbo.ProjectImage (ProjectID, FileResourceID, Caption, Credit, IsKeyPhoto, ExcludeFromFactSheet, ProjectImageTimingID)
        OUTPUT inserted.ProjectImageID, inserted.Caption INTO @NewProjectImages
        SELECT @ProjectID, ui.FileResourceID, ui.Caption, ui.Credit, ui.IsKeyPhoto, ui.ExcludeFromFactSheet, ui.ProjectImageTimingID
        FROM dbo.ProjectImageUpdate ui
        WHERE ui.ProjectUpdateBatchID = @ProjectUpdateBatchID AND ui.FileResourceID IS NOT NULL
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectImage pi
              WHERE pi.ProjectID = @ProjectID
                AND pi.FileResourceID = ui.FileResourceID
          );

        -- Audit: Single summary entry per new image
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 1, 'ProjectImage', ni.ProjectImageID, 'Caption', NULL, ni.Caption,
            'Image: added ' + ISNULL(ni.Caption, '(no caption)'), @ProjectID
        FROM @NewProjectImages ni;

        -- External Links (change detection: only insert new, by Label+URL)
        DECLARE @NewExternalLinks TABLE (ProjectExternalLinkID int, ExternalLinkLabel varchar(300));
        INSERT INTO dbo.ProjectExternalLink (ProjectID, ExternalLinkLabel, ExternalLinkUrl)
        OUTPUT inserted.ProjectExternalLinkID, inserted.ExternalLinkLabel INTO @NewExternalLinks
        SELECT @ProjectID, uel.ExternalLinkLabel, uel.ExternalLinkUrl
        FROM dbo.ProjectExternalLinkUpdate uel
        WHERE uel.ProjectUpdateBatchID = @ProjectUpdateBatchID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectExternalLink pel
              WHERE pel.ProjectID = @ProjectID
                AND ISNULL(pel.ExternalLinkLabel, '') = ISNULL(uel.ExternalLinkLabel, '')
                AND ISNULL(pel.ExternalLinkUrl, '') = ISNULL(uel.ExternalLinkUrl, '')
          );

        -- Audit: Single summary entry per new external link
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 1, 'ProjectExternalLink', nel.ProjectExternalLinkID, 'ExternalLinkLabel', NULL, nel.ExternalLinkLabel,
            'External Link: added ' + ISNULL(nel.ExternalLinkLabel, '(no label)'), @ProjectID
        FROM @NewExternalLinks nel;

        -- Documents (change detection: only insert new, by FileResourceID)
        DECLARE @NewProjectDocuments TABLE (ProjectDocumentID int, DisplayName varchar(200), ProjectDocumentTypeID int);
        INSERT INTO dbo.ProjectDocument (ProjectID, FileResourceID, DisplayName, Description, ProjectDocumentTypeID)
        OUTPUT inserted.ProjectDocumentID, inserted.DisplayName, inserted.ProjectDocumentTypeID INTO @NewProjectDocuments
        SELECT @ProjectID, ud.FileResourceID, ud.DisplayName, ud.Description, ud.ProjectDocumentTypeID
        FROM dbo.ProjectDocumentUpdate ud
        WHERE ud.ProjectUpdateBatchID = @ProjectUpdateBatchID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectDocument pd
              WHERE pd.ProjectID = @ProjectID
                AND pd.FileResourceID = ud.FileResourceID
          );

        -- Audit: Single summary entry per new document (with display name and type)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 1, 'ProjectDocument', nd.ProjectDocumentID, 'DisplayName', NULL, nd.DisplayName,
            'Document: added ' + ISNULL(nd.DisplayName, '(unnamed)') + ISNULL(' (' + dt.ProjectDocumentTypeName + ')', ''),
            @ProjectID
        FROM @NewProjectDocuments nd
        LEFT JOIN dbo.ProjectDocumentType dt ON nd.ProjectDocumentTypeID = dt.ProjectDocumentTypeID;

        -- Notes (change detection: only insert new, by CreatePersonID+CreateDate)
        DECLARE @NewProjectNotes TABLE (ProjectNoteID int, Note varchar(max));
        INSERT INTO dbo.ProjectNote (ProjectID, Note, CreatePersonID, CreateDate, UpdatePersonID, UpdateDate)
        OUTPUT inserted.ProjectNoteID, inserted.Note INTO @NewProjectNotes
        SELECT @ProjectID, un.Note, un.CreatePersonID, un.CreateDate, un.UpdatePersonID, un.UpdateDate
        FROM dbo.ProjectNoteUpdate un
        WHERE un.ProjectUpdateBatchID = @ProjectUpdateBatchID
          AND NOT EXISTS (
              SELECT 1 FROM dbo.ProjectNote pn
              WHERE pn.ProjectID = @ProjectID
                AND pn.CreatePersonID = un.CreatePersonID
                AND pn.CreateDate = un.CreateDate
          );

        -- Audit: Single summary entry per new note
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 1, 'ProjectNote', nn.ProjectNoteID, 'Note', NULL, LEFT(nn.Note, 500),
            'Note: added ' + ISNULL(LEFT(nn.Note, 100), '(empty)'), @ProjectID
        FROM @NewProjectNotes nn;

        -- ============================================================================
        -- SECTION 6: Treatments (match to new project locations by name + geometry)
        -- ============================================================================

        -- Treatments with a location update (only insert for NEW locations — unchanged locations keep their treatments)
        DECLARE @NewTreatmentsWithLocation TABLE (TreatmentID int, TreatmentTypeID int);
        INSERT INTO dbo.Treatment (ProjectID, ProjectLocationID, TreatmentTypeID, TreatmentDetailedActivityTypeID,
            TreatmentCodeID, TreatmentStartDate, TreatmentEndDate, TreatmentFootprintAcres, TreatmentTreatedAcres,
            TreatmentNotes, CostPerAcre, TreatmentTypeImportedText, TreatmentDetailedActivityTypeImportedText,
            ProgramID, ImportedFromGis, CreateGisUploadAttemptID, UpdateGisUploadAttemptID)
        OUTPUT inserted.TreatmentID, inserted.TreatmentTypeID
        INTO @NewTreatmentsWithLocation
        SELECT @ProjectID, pl.ProjectLocationID, tu.TreatmentTypeID, tu.TreatmentDetailedActivityTypeID,
            tu.TreatmentCodeID, tu.TreatmentStartDate, tu.TreatmentEndDate, tu.TreatmentFootprintAcres, tu.TreatmentTreatedAcres,
            tu.TreatmentNotes, tu.CostPerAcre, tu.TreatmentTypeImportedText, tu.TreatmentDetailedActivityTypeImportedText,
            tu.ProgramID, tu.ImportedFromGis, tu.CreateGisUploadAttemptID, tu.UpdateGisUploadAttemptID
        FROM dbo.TreatmentUpdate tu
        INNER JOIN dbo.ProjectLocationUpdate plu ON tu.ProjectLocationUpdateID = plu.ProjectLocationUpdateID
        INNER JOIN @NewProjectLocations npl ON npl.ProjectLocationName = plu.ProjectLocationUpdateName
        INNER JOIN dbo.ProjectLocation pl
            ON pl.ProjectID = @ProjectID
            AND pl.ProjectLocationName = plu.ProjectLocationUpdateName
        WHERE tu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND tu.ProjectLocationUpdateID IS NOT NULL;

        -- Audit: Single summary entry per new treatment
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 1, 'Treatment', nt.TreatmentID, 'TreatmentTypeID', NULL, CAST(nt.TreatmentTypeID as varchar(max)),
            'Treatment: added ' + ISNULL(tt.TreatmentTypeDisplayName, ''),
            @ProjectID
        FROM @NewTreatmentsWithLocation nt
        LEFT JOIN dbo.TreatmentType tt ON nt.TreatmentTypeID = tt.TreatmentTypeID;

        -- Treatments with no location (these are always recreated since they have no stable key)
        DECLARE @NewTreatmentsNoLocation TABLE (TreatmentID int, TreatmentTypeID int);
        INSERT INTO dbo.Treatment (ProjectID, ProjectLocationID, TreatmentTypeID, TreatmentDetailedActivityTypeID,
            TreatmentCodeID, TreatmentStartDate, TreatmentEndDate, TreatmentFootprintAcres, TreatmentTreatedAcres,
            TreatmentNotes, CostPerAcre, TreatmentTypeImportedText, TreatmentDetailedActivityTypeImportedText,
            ProgramID, ImportedFromGis, CreateGisUploadAttemptID, UpdateGisUploadAttemptID)
        OUTPUT inserted.TreatmentID, inserted.TreatmentTypeID
        INTO @NewTreatmentsNoLocation
        SELECT @ProjectID, NULL, tu.TreatmentTypeID, tu.TreatmentDetailedActivityTypeID,
            tu.TreatmentCodeID, tu.TreatmentStartDate, tu.TreatmentEndDate, tu.TreatmentFootprintAcres, tu.TreatmentTreatedAcres,
            tu.TreatmentNotes, tu.CostPerAcre, tu.TreatmentTypeImportedText, tu.TreatmentDetailedActivityTypeImportedText,
            tu.ProgramID, tu.ImportedFromGis, tu.CreateGisUploadAttemptID, tu.UpdateGisUploadAttemptID
        FROM dbo.TreatmentUpdate tu
        WHERE tu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND tu.ProjectLocationUpdateID IS NULL;

        -- Audit: Single summary entry per new treatment (no location)
        INSERT INTO dbo.AuditLog (PersonID, AuditLogDate, AuditLogEventTypeID, TableName, RecordID, ColumnName, OriginalValue, NewValue, AuditDescription, ProjectID)
        SELECT @CallingPersonID, @AuditDate, 1, 'Treatment', nt.TreatmentID, 'TreatmentTypeID', NULL, CAST(nt.TreatmentTypeID as varchar(max)),
            'Treatment: added ' + ISNULL(tt.TreatmentTypeDisplayName, ''),
            @ProjectID
        FROM @NewTreatmentsNoLocation nt
        LEFT JOIN dbo.TreatmentType tt ON nt.TreatmentTypeID = tt.TreatmentTypeID;

        -- ============================================================================
        -- SECTION 7: Delete orphaned FileResources (images/docs removed during update)
        -- Note: FileResource is in the ignored tables list, so we don't audit these deletions
        -- Only delete if truly unreferenced (other stale batches may still hold FK refs)
        -- ============================================================================
        DELETE fr FROM dbo.FileResource fr
        INNER JOIN @OrphanedFileResourceIDs o ON fr.FileResourceID = o.FileResourceID
        WHERE NOT EXISTS (
            SELECT 1 FROM dbo.ProjectDocumentUpdate pdu WHERE pdu.FileResourceID = fr.FileResourceID
        )
        AND NOT EXISTS (
            SELECT 1 FROM dbo.ProjectImageUpdate piu WHERE piu.FileResourceID = fr.FileResourceID
        );

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
