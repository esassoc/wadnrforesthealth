DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = 'Pre - 0002 - WADNR-2289 Remove Agreements'

-- ============================================================================
-- WADNR-2289: Remove Agreements.
--
-- The Agreement tables were removed from the project, so the schema compare
-- drops them during deployment. This PRE-deployment script runs before that
-- drop and:
--   * deletes all Agreement data (link tables first, then Agreement)
--   * deletes the FileResource rows that were Agreement documents (DNR confirmed
--     these do not need to be preserved; they would otherwise be orphaned)
--   * clears dependents of the removed Agreement LOOKUP rows so the
--     post-deployment Lookup-Table MERGE can delete them:
--       - FirmaPageType 60   (FullAgreementList)
--       - FieldDefinition    295, 298-303, 305, 306
--
-- Guarded on dbo.DatabaseMigration existing: on a fresh from-scratch build the
-- schema is not yet deployed at pre-deployment time (and there is nothing to
-- clean up), so the whole block is skipped. The Agreement block is also guarded
-- on dbo.Agreement existing.
-- ============================================================================

IF OBJECT_ID('dbo.DatabaseMigration', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    PRINT @MigrationName;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    -- ---- Agreement data + Agreement documents ----
    IF OBJECT_ID('dbo.Agreement', 'U') IS NOT NULL
    BEGIN
        DECLARE @FileResourceIDs TABLE (FileResourceID INT PRIMARY KEY);

        INSERT INTO @FileResourceIDs (FileResourceID)
        EXEC('SELECT DISTINCT AgreementFileResourceID FROM dbo.Agreement WHERE AgreementFileResourceID IS NOT NULL');

        EXEC('DELETE FROM dbo.AgreementFundSourceAllocation');
        EXEC('DELETE FROM dbo.AgreementPerson');
        EXEC('DELETE FROM dbo.AgreementProject');
        EXEC('DELETE FROM dbo.Agreement');

        DELETE FROM dbo.FileResource WHERE FileResourceID IN (SELECT FileResourceID FROM @FileResourceIDs);
    END

    -- ---- FirmaPageType 60 (FullAgreementList) dependents ----
    -- FirmaPageImage -> FirmaPage
    DELETE fpi
    FROM dbo.FirmaPageImage fpi
    INNER JOIN dbo.FirmaPage fp ON fp.FirmaPageID = fpi.FirmaPageID
    WHERE fp.FirmaPageTypeID = 60;

    -- FirmaPage -> FirmaPageType
    DELETE FROM dbo.FirmaPage WHERE FirmaPageTypeID = 60;

    -- ---- FieldDefinition 295, 298-303, 305, 306 (Agreement) dependents ----
    -- FieldDefinitionDatumImage -> FieldDefinitionDatum
    DELETE fddi
    FROM dbo.FieldDefinitionDatumImage fddi
    INNER JOIN dbo.FieldDefinitionDatum fdd ON fdd.FieldDefinitionDatumID = fddi.FieldDefinitionDatumID
    WHERE fdd.FieldDefinitionID IN (295, 298, 299, 300, 301, 302, 303, 305, 306);

    -- FieldDefinitionDatum -> FieldDefinition (admin-customized label overrides)
    DELETE FROM dbo.FieldDefinitionDatum WHERE FieldDefinitionID IN (295, 298, 299, 300, 301, 302, 303, 305, 306);

    -- GIS import mappings -> FieldDefinition
    DELETE FROM dbo.GisDefaultMapping WHERE FieldDefinitionID IN (295, 298, 299, 300, 301, 302, 303, 305, 306);
    DELETE FROM dbo.GisCrossWalkDefault WHERE FieldDefinitionID IN (295, 298, 299, 300, 301, 302, 303, 305, 306);

    -- Parent FieldDefinition and FirmaPageType rows are removed by the
    -- post-deployment Lookup-Table MERGE now that their dependents are gone.

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Tom Kamin', @MigrationName, 'WADNR-2289 Remove Agreements: delete Agreement data and documents, and clear Agreement lookup-row dependents so the Lookup-Table MERGE can delete FieldDefinition 295/298-303/305/306 and FirmaPageType 60'

    COMMIT TRANSACTION;
END
