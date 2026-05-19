DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0008 - WADNR-2264 Delete blocked projects imported in error'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    PRINT @MigrationName;

    -- Compute the set of ProjectIDs to delete: projects created after 2026-04-01
    -- that match a ProjectImportBlockList entry for one of their linked programs.
    -- Match criteria mirror the runtime block-list check in
    -- GisBulkImport.StaticHelpers.cs::ImportProjectsAsync (case-insensitive, trimmed,
    -- on either ProjectGisIdentifier or ProjectName).
    DECLARE @ProjectIDs TABLE (ProjectID INT PRIMARY KEY);

    INSERT INTO @ProjectIDs (ProjectID)
    SELECT DISTINCT P.ProjectID
    FROM dbo.Project P
    INNER JOIN dbo.GisUploadAttempt GUA ON GUA.GisUploadAttemptID = P.CreateGisUploadAttemptID
    INNER JOIN dbo.ProjectProgram PP ON PP.ProjectID = P.ProjectID
    INNER JOIN dbo.ProjectImportBlockList BL ON BL.ProgramID = PP.ProgramID
    WHERE GUA.GisUploadAttemptCreateDate > '2026-04-01'
      AND (
            (BL.ProjectGisIdentifier IS NOT NULL
              AND P.ProjectGisIdentifier IS NOT NULL
              AND UPPER(LTRIM(RTRIM(BL.ProjectGisIdentifier))) = UPPER(LTRIM(RTRIM(P.ProjectGisIdentifier))))
         OR (BL.ProjectName IS NOT NULL
              AND P.ProjectName IS NOT NULL
              AND UPPER(LTRIM(RTRIM(BL.ProjectName))) = UPPER(LTRIM(RTRIM(P.ProjectName))))
          );

    DECLARE @BlockedProjectCount INT = (SELECT COUNT(*) FROM @ProjectIDs);
    PRINT 'Deleting ' + CAST(@BlockedProjectCount AS VARCHAR(10)) + ' blocked projects.';

    -- Preserve the block-list entries themselves so future imports remain blocked.
    -- Null out the optional ProjectID back-reference on any block-list rows that linked to these projects.
    UPDATE dbo.ProjectImportBlockList SET ProjectID = NULL
    WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);

    -- Collect ProjectUpdateBatchIDs and FileResourceIDs that will become orphaned.
    DECLARE @BatchIDs TABLE (ProjectUpdateBatchID INT PRIMARY KEY);
    INSERT INTO @BatchIDs (ProjectUpdateBatchID)
    SELECT ProjectUpdateBatchID FROM dbo.ProjectUpdateBatch
    WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);

    DECLARE @InvoicePaymentRequestIDs TABLE (InvoicePaymentRequestID INT PRIMARY KEY);
    INSERT INTO @InvoicePaymentRequestIDs (InvoicePaymentRequestID)
    SELECT InvoicePaymentRequestID FROM dbo.InvoicePaymentRequest
    WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);

    DECLARE @FileResourceIDs TABLE (FileResourceID INT PRIMARY KEY);
    INSERT INTO @FileResourceIDs (FileResourceID)
    SELECT FileResourceID FROM dbo.ProjectImage
    WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs)
    UNION
    SELECT FileResourceID FROM dbo.ProjectDocument
    WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs)
    UNION
    SELECT FileResourceID FROM dbo.ProjectImageUpdate
    WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs)
      AND FileResourceID IS NOT NULL
    UNION
    SELECT FileResourceID FROM dbo.ProjectDocumentUpdate
    WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs)
    UNION
    SELECT InvoiceFileResourceID FROM dbo.Invoice
    WHERE InvoicePaymentRequestID IN (SELECT InvoicePaymentRequestID FROM @InvoicePaymentRequestIDs)
      AND InvoiceFileResourceID IS NOT NULL;

    -- Mirror the dependency-deletion order of Project.StaticHelpers.cs::DeleteAsync.

    -- Layer 1: Deepest children of InvoicePaymentRequest and ProjectUpdateBatch
    DELETE FROM dbo.Invoice
      WHERE InvoicePaymentRequestID IN (SELECT InvoicePaymentRequestID FROM @InvoicePaymentRequestIDs);

    DELETE FROM dbo.TreatmentUpdate                       WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectUpdate                         WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectUpdateProgram                  WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectCountyUpdate                   WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectRegionUpdate                   WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectPriorityLandscapeUpdate        WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectLocationStagingUpdate          WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectLocationUpdate                 WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectPersonUpdate                   WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectOrganizationUpdate             WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectFundingSourceUpdate            WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectFundSourceAllocationRequestUpdate WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectImageUpdate                    WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectExternalLinkUpdate             WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectDocumentUpdate                 WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectNoteUpdate                     WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);
    DELETE FROM dbo.ProjectUpdateHistory                  WHERE ProjectUpdateBatchID IN (SELECT ProjectUpdateBatchID FROM @BatchIDs);

    -- Layer 2: Parents of layer 1
    DELETE FROM dbo.InvoicePaymentRequest WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.Treatment             WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectUpdateBatch    WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);

    -- Layer 3: Direct Project children
    DELETE FROM dbo.AgreementProject              WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.InteractionEventProject       WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.NotificationProject           WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProgramNotificationSentProject WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectClassification         WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectCounty                 WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectDocument               WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectExternalLink           WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectFundSourceAllocationRequest WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectFundingSource          WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectImage                  WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    -- (ProjectImportBlockList intentionally preserved — ProjectID FK was nulled above.)
    DELETE FROM dbo.ProjectInternalNote           WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectLocation               WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectLocationStaging        WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectNote                   WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectOrganization           WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectPerson                 WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectPriorityLandscape      WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectProgram                WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectRegion                 WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);
    DELETE FROM dbo.ProjectTag                    WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);

    -- Layer 4: FileResources (now safe — all referencing rows are deleted)
    DELETE FROM dbo.FileResource
      WHERE FileResourceID IN (SELECT FileResourceID FROM @FileResourceIDs);

    -- Layer 5: The Project itself
    DELETE FROM dbo.Project WHERE ProjectID IN (SELECT ProjectID FROM @ProjectIDs);

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'tkamin', @MigrationName, 'WADNR-2264 - delete projects imported after 2026-04-01 that match a Project Import Block List entry'
END
