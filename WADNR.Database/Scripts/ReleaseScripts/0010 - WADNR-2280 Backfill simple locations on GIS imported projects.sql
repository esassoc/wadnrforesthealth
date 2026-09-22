DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0010 - WADNR-2280 Backfill simple locations on GIS imported projects'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    PRINT @MigrationName;

    -- WADNR-2280: GIS-imported projects on the treatment path (e.g. Landowner Assistance /
    -- Service Forestry) could be left with a NULL Project.ProjectLocationPoint, so they never
    -- appeared on the projects map. The point was only ever set by the final UPDATE in
    -- dbo.procImportTreatmentsFromGisUploadAttempt, which is gated to newly-created projects
    -- (CreateGisUploadAttemptID = the running attempt) and does not run at all when the treatment
    -- import throws. The code fix heals this going forward for created AND updated projects; this
    -- is the one-time backfill for projects imported before the fix.
    --
    -- Scope: every GIS-imported project (CreateGisUploadAttemptID IS NOT NULL) with no simple
    -- location point that has ProjectArea geometry to derive a centroid from. Interactive projects
    -- (including those a steward set to "No location") have a NULL CreateGisUploadAttemptID and are
    -- never touched. The centroid is the union of the project's ProjectArea locations, matching the
    -- runtime code path (GisBulkImports.ApplySimpleLocationFromProjectAreasAsync) and the proc's
    -- geometry::UnionAggregate(...).STCentroid().
    --
    -- ProjectLocationType is resolved BY NAME: its IDs are consistent in this project's lookup data,
    -- but resolving by name keeps the script robust the way 0009 resolves its lookups by name.
    -- Idempotent: the ProjectLocationPoint IS NULL predicate plus the migration guard make a re-run a
    -- no-op, so it is safe alongside a nightly import that lands first.

    DECLARE @ProjectAreaProjectLocationTypeID INT =
        (SELECT ProjectLocationTypeID FROM dbo.ProjectLocationType
         WHERE LTRIM(RTRIM(ProjectLocationTypeName)) = 'ProjectArea');

    IF @ProjectAreaProjectLocationTypeID IS NULL
    BEGIN
        PRINT 'Skipping: could not resolve the "ProjectArea" project location type by name.';
    END
    ELSE
    BEGIN
        -- .MakeValid() every geometry before the union: geometry::UnionAggregate throws
        -- (SQL 24144 "the instance is not valid") on a single invalid instance, and historical
        -- ProjectArea rows predate the import's MakeValid-on-ingest normalization
        -- (GisBulkImports.BuildLocation). This matches that same normalization, so the centroid here
        -- is the one the corrected import would produce.
        UPDATE P
        SET P.ProjectLocationPoint = Y.SimplePoint,
            P.ProjectLocationSimpleTypeID = 1 -- PointOnMap
        FROM dbo.Project P
        INNER JOIN (
            SELECT PL.ProjectID,
                   geometry::UnionAggregate(PL.ProjectLocationGeometry.MakeValid()).STCentroid() AS SimplePoint
            FROM dbo.ProjectLocation PL
            WHERE PL.ProjectLocationTypeID = @ProjectAreaProjectLocationTypeID
              AND PL.ProjectLocationGeometry IS NOT NULL
            GROUP BY PL.ProjectID
        ) Y ON Y.ProjectID = P.ProjectID
        WHERE P.ProjectLocationPoint IS NULL
          AND P.CreateGisUploadAttemptID IS NOT NULL
          AND Y.SimplePoint IS NOT NULL;

        DECLARE @BackfilledCount INT = @@ROWCOUNT;
        PRINT 'Backfilled simple location on ' + CAST(@BackfilledCount AS VARCHAR(10)) + ' GIS-imported project(s).';
    END

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'tkamin', @MigrationName, 'WADNR-2280 - backfill Project.ProjectLocationPoint on GIS-imported projects the import left without a simple location'
END
