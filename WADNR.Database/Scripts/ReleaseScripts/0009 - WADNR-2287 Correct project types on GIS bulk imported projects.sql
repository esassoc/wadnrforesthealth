DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0009 - WADNR-2287 Correct project types on GIS bulk imported projects'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    PRINT @MigrationName;

    -- WADNR-2287: GisBulkImports.ImportProjectsAsync assigned newly-created projects the *first*
    -- row of dbo.ProjectType when the source organization had no matching ProjectTypeDefaultName.
    -- With no ORDER BY that is the lowest ProjectTypeID, which in production is
    -- "Research and Monitoring". It also never honoured the source organization's
    -- AdjustProjectTypeBasedOnTreatmentTypes flag, which legacy applied after the treatment import.
    --
    -- DNR State Lands (ProgramID 1) has ProjectTypeDefaultName NULL and
    -- AdjustProjectTypeBasedOnTreatmentTypes = 1, so every project it created hit the bad fallback.
    -- In production this is 442 projects, all created by GisUploadAttempt 6011 and 6015 (2026-07-02).
    --
    -- Both scoping predicates are kept deliberately:
    --   * CreateGisUploadAttemptID IN (6011, 6015) — the damage originates at *creation* time; the
    --     update path never wrote ProjectTypeID, which is why later attempts (6124, 6159) re-touched
    --     these projects and left the wrong type in place.
    --   * ProjectTypeID = "Research and Monitoring" — makes this a no-op for anything corrected by
    --     hand (or by a re-upload) between now and deployment. No human-set "Research and
    --     Monitoring" project existed anywhere in production when this was scoped.
    --
    -- Project types are resolved BY NAME, never by ID: dbo.ProjectType is user-managed data rather
    -- than a seeded lookup, so the IDs differ between production, QA and dev.

    DECLARE @ResearchAndMonitoringProjectTypeID INT;
    DECLARE @CommercialProjectTypeID INT;
    DECLARE @NonCommercialProjectTypeID INT;
    DECLARE @PrescribedFireProjectTypeID INT;
    DECLARE @OtherProjectTypeID INT;

    SELECT @ResearchAndMonitoringProjectTypeID = ProjectTypeID FROM dbo.ProjectType WHERE LTRIM(RTRIM(ProjectTypeName)) = 'Research and Monitoring';
    SELECT @CommercialProjectTypeID           = ProjectTypeID FROM dbo.ProjectType WHERE LTRIM(RTRIM(ProjectTypeName)) = 'Commercial vegetation treatment';
    SELECT @NonCommercialProjectTypeID        = ProjectTypeID FROM dbo.ProjectType WHERE LTRIM(RTRIM(ProjectTypeName)) = 'Non-commercial vegetation treatment';
    SELECT @PrescribedFireProjectTypeID       = ProjectTypeID FROM dbo.ProjectType WHERE LTRIM(RTRIM(ProjectTypeName)) = 'Prescribed fire treatment';
    SELECT @OtherProjectTypeID                = ProjectTypeID FROM dbo.ProjectType WHERE LTRIM(RTRIM(ProjectTypeName)) = 'Other';

    DECLARE @CommercialTreatmentTypeID     INT = (SELECT TreatmentTypeID FROM dbo.TreatmentType WHERE TreatmentTypeName = 'Commercial');
    DECLARE @NonCommercialTreatmentTypeID  INT = (SELECT TreatmentTypeID FROM dbo.TreatmentType WHERE TreatmentTypeName = 'NonCommercial');
    DECLARE @PrescribedFireTreatmentTypeID INT = (SELECT TreatmentTypeID FROM dbo.TreatmentType WHERE TreatmentTypeName = 'PrescribedFire');

    IF @ResearchAndMonitoringProjectTypeID IS NULL OR @OtherProjectTypeID IS NULL
    BEGIN
        -- Nothing to repair here (or the environment lacks the project types this keys off).
        PRINT 'Skipping: could not resolve the "Research and Monitoring" and/or "Other" project types by name.';
    END
    ELSE
    BEGIN
        -- Projects created by the two bad attempts that are still on the wrong type, carrying the
        -- source organization's ImportIsFlattened flag so the treatment filter matches the runtime
        -- rule in GisBulkImports.ApplyProjectTypeFromTreatmentTypesAsync.
        DECLARE @AffectedProject TABLE (ProjectID INT PRIMARY KEY, ImportIsFlattened BIT NOT NULL);

        INSERT INTO @AffectedProject (ProjectID, ImportIsFlattened)
        SELECT P.ProjectID,
               CASE WHEN GUSO.ImportIsFlattened = 1 THEN 1 ELSE 0 END
        FROM dbo.Project P
        INNER JOIN dbo.GisUploadAttempt GUA ON GUA.GisUploadAttemptID = P.CreateGisUploadAttemptID
        INNER JOIN dbo.GisUploadSourceOrganization GUSO ON GUSO.GisUploadSourceOrganizationID = GUA.GisUploadSourceOrganizationID
        WHERE P.CreateGisUploadAttemptID IN (6011, 6015)
          AND P.ProjectTypeID = @ResearchAndMonitoringProjectTypeID;

        DECLARE @AffectedProjectCount INT = (SELECT COUNT(*) FROM @AffectedProject);
        PRINT 'Found ' + CAST(@AffectedProjectCount AS VARCHAR(10)) + ' project(s) to re-type.';

        -- Each project's sole treatment type, if it has exactly one. Projects with a mixed or empty
        -- treatment set get no row here and fall through to "Other" (the legacy default) below.
        DECLARE @SoleTreatmentType TABLE (ProjectID INT PRIMARY KEY, TreatmentTypeID INT NOT NULL);

        INSERT INTO @SoleTreatmentType (ProjectID, TreatmentTypeID)
        SELECT T.ProjectID, MIN(T.TreatmentTypeID)
        FROM dbo.Treatment T
        INNER JOIN @AffectedProject AP ON AP.ProjectID = T.ProjectID
        WHERE AP.ImportIsFlattened = 0
           OR ISNULL(T.TreatmentTreatedAcres, 0) > 0
        GROUP BY T.ProjectID
        HAVING COUNT(DISTINCT T.TreatmentTypeID) = 1;

        UPDATE P
        SET ProjectTypeID =
            COALESCE(
                CASE
                    WHEN STT.TreatmentTypeID = @CommercialTreatmentTypeID     THEN @CommercialProjectTypeID
                    WHEN STT.TreatmentTypeID = @NonCommercialTreatmentTypeID  THEN @NonCommercialProjectTypeID
                    WHEN STT.TreatmentTypeID = @PrescribedFireTreatmentTypeID THEN @PrescribedFireProjectTypeID
                END,
                -- No treatments, a mixed treatment set, an "Other" treatment type, or a target
                -- project type that doesn't exist in this environment: fall back the way legacy did.
                @OtherProjectTypeID)
        FROM dbo.Project P
        INNER JOIN @AffectedProject AP ON AP.ProjectID = P.ProjectID
        LEFT JOIN @SoleTreatmentType STT ON STT.ProjectID = P.ProjectID;

        DECLARE @ReTypedCount INT = @@ROWCOUNT;
        PRINT 'Re-typed ' + CAST(@ReTypedCount AS VARCHAR(10)) + ' project(s).';
    END

    /*
        Part 2 - backfill DNR Service Forestry Regional Coordinators.

        The rewrite also dropped legacy's AddProjectCoordinators, so Landowner Assistance projects
        created by a GIS upload since the migration never got the coordinator for the DNR Upland
        Region they landed in. Restoring the code fixes this going forward, but only for *newly
        created* projects — which is what legacy did, and deliberately so: assigning on the update
        path would silently re-add a coordinator a steward had removed, on every nightly run.

        That leaves the existing projects needing a one-time backfill, which is this.

        Scoped to Landowner Assistance projects created by a GIS upload on or after 2026-03-01 (the
        migration cutover) whose region has a coordinator and which have no coordinator record.
        Idempotent via NOT EXISTS, so it is safe alongside any nightly run that lands first.
    */
    DECLARE @LandownerAssistanceProgramID INT = 3;
    DECLARE @CoordinatorRelationshipTypeID INT =
        (SELECT ProjectPersonRelationshipTypeID FROM dbo.ProjectPersonRelationshipType
         WHERE ProjectPersonRelationshipTypeName = 'ServiceForestryRegionalCoordinator');

    IF @CoordinatorRelationshipTypeID IS NULL
    BEGIN
        PRINT 'Skipping coordinator backfill: ServiceForestryRegionalCoordinator relationship type not found.';
    END
    ELSE
    BEGIN
        INSERT INTO dbo.ProjectPerson (ProjectID, PersonID, ProjectPersonRelationshipTypeID, CreatedAsPartOfBulkImport)
        SELECT DISTINCT P.ProjectID, R.DNRUplandRegionCoordinatorID, @CoordinatorRelationshipTypeID, 1
        FROM dbo.Project P
        INNER JOIN dbo.GisUploadAttempt A ON A.GisUploadAttemptID = P.CreateGisUploadAttemptID
        INNER JOIN dbo.GisUploadSourceOrganization G ON G.GisUploadSourceOrganizationID = A.GisUploadSourceOrganizationID
        INNER JOIN dbo.ProjectProgram PP ON PP.ProjectID = P.ProjectID AND PP.ProgramID = @LandownerAssistanceProgramID
        INNER JOIN dbo.ProjectRegion PR ON PR.ProjectID = P.ProjectID
        INNER JOIN dbo.DNRUplandRegion R ON R.DNRUplandRegionID = PR.DNRUplandRegionID
        WHERE A.GisUploadAttemptCreateDate >= '2026-03-01'
          AND R.DNRUplandRegionCoordinatorID IS NOT NULL
          AND NOT EXISTS (
                SELECT 1 FROM dbo.ProjectPerson EX
                WHERE EX.ProjectID = P.ProjectID
                  AND EX.PersonID = R.DNRUplandRegionCoordinatorID
                  AND EX.ProjectPersonRelationshipTypeID = @CoordinatorRelationshipTypeID);

        DECLARE @CoordinatorsAdded INT = @@ROWCOUNT;
        PRINT 'Backfilled ' + CAST(@CoordinatorsAdded AS VARCHAR(10)) + ' regional coordinator record(s).';
    END

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'mpeters', @MigrationName, 'WADNR-2287 - correct project types on projects the GIS bulk import mis-typed as "Research and Monitoring"'
END
