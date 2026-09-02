DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = 'Pre - 0001 - WADNR-2290 Remove FocusArea lookup dependents'

-- ============================================================================
-- WADNR-2290: Remove DNR LOA Focus Areas.
--
-- The FocusArea entity/pages/columns were already removed. This clears the
-- remaining FocusArea LOOKUP rows:
--   * FirmaPageType 55  (FocusAreasList)
--   * FieldDefinition   276 (FocusArea) and 310-314 (Focus Area closeout/report labels)
--
-- Those rows were removed from the Lookup-Table seed scripts, so the
-- post-deployment Lookup-Table MERGE (which uses WHEN NOT MATCHED BY SOURCE
-- THEN DELETE) will delete the parent rows themselves. That MERGE runs BEFORE
-- the post-deployment release scripts, so it would hit foreign-key violations
-- unless the dependent rows are gone first. This PRE-deployment script deletes
-- those dependents in FK-safe order so the MERGE can succeed.
--
-- Guarded on dbo.DatabaseMigration existing: on a fresh from-scratch build the
-- schema is not yet deployed at pre-deployment time (and there is nothing to
-- clean up), so the whole block is skipped.
-- ============================================================================

IF OBJECT_ID('dbo.DatabaseMigration', 'U') IS NOT NULL
   AND NOT EXISTS (SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    PRINT @MigrationName;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    -- ---- FirmaPageType 55 (FocusAreasList) dependents ----
    -- FirmaPageImage -> FirmaPage
    DELETE fpi
    FROM dbo.FirmaPageImage fpi
    INNER JOIN dbo.FirmaPage fp ON fp.FirmaPageID = fpi.FirmaPageID
    WHERE fp.FirmaPageTypeID = 55;

    -- FirmaPage -> FirmaPageType
    DELETE FROM dbo.FirmaPage WHERE FirmaPageTypeID = 55;

    -- ---- FieldDefinition 276, 310-314 (FocusArea) dependents ----
    -- FieldDefinitionDatumImage -> FieldDefinitionDatum
    DELETE fddi
    FROM dbo.FieldDefinitionDatumImage fddi
    INNER JOIN dbo.FieldDefinitionDatum fdd ON fdd.FieldDefinitionDatumID = fddi.FieldDefinitionDatumID
    WHERE fdd.FieldDefinitionID IN (276, 310, 311, 312, 313, 314);

    -- FieldDefinitionDatum -> FieldDefinition (admin-customized label overrides)
    DELETE FROM dbo.FieldDefinitionDatum WHERE FieldDefinitionID IN (276, 310, 311, 312, 313, 314);

    -- GIS import mappings -> FieldDefinition (any FocusArea column mapping)
    DELETE FROM dbo.GisDefaultMapping WHERE FieldDefinitionID IN (276, 310, 311, 312, 313, 314);
    DELETE FROM dbo.GisCrossWalkDefault WHERE FieldDefinitionID IN (276, 310, 311, 312, 313, 314);

    -- Parent FieldDefinition (276, 310-314) and FirmaPageType (55) rows are removed
    -- by the post-deployment Lookup-Table MERGE now that their dependents are gone.

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Tom Kamin', @MigrationName, 'WADNR-2290 Remove DNR LOA Focus Areas: clear FocusArea lookup-row dependents so the Lookup-Table MERGE can delete FieldDefinition 276/310-314 and FirmaPageType 55'

    COMMIT TRANSACTION;
END
