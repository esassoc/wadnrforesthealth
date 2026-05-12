DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0007 - WADNR-2263 Backfill FHT Project Numbers with year prefix'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    PRINT @MigrationName;

    -- Bug-introduced rows since commit c5d4fc3ca (2026-03-14): start with 'FHT-' but lack the
    -- 'FHT-YYYY-' year segment. Free-form GIS identifiers don't start with 'FHT-' and are excluded.
    -- Correctly-formatted legacy rows like 'FHT-2019-001' match the year-segment pattern and are excluded.
    -- The Project table has no CreateDate column, so derive the year from:
    --   1. GisUploadAttempt.GisUploadAttemptCreateDate for bulk-imported projects
    --   2. Project.ProposingDate for manually-created projects
    --   3. 2026 as a hard fallback (regression first appeared 2026-03-14, no bug rows pre-date that)
    ;WITH BadProjects AS (
        SELECT
            P.ProjectID,
            COALESCE(
                YEAR(GUA.GisUploadAttemptCreateDate),
                YEAR(P.ProposingDate),
                2026
            ) AS CreateYear,
            COALESCE(GUA.GisUploadAttemptCreateDate, P.ProposingDate, '2026-03-14') AS OrderingDate
        FROM dbo.Project P
        LEFT JOIN dbo.GisUploadAttempt GUA ON GUA.GisUploadAttemptID = P.CreateGisUploadAttemptID
        WHERE P.FhtProjectNumber LIKE 'FHT-%'
          AND P.FhtProjectNumber NOT LIKE 'FHT-[0-9][0-9][0-9][0-9]-%'
    ),
    YearStarts AS (
        -- For each year with bad rows, find the max counter already in use by legitimate rows for that year
        -- so backfilled rows continue past them and never collide.
        SELECT DISTINCT
            B.CreateYear,
            COALESCE((
                SELECT MAX(TRY_CAST(SUBSTRING(P.FhtProjectNumber, 10, 20) AS INT))
                FROM dbo.Project P
                WHERE P.FhtProjectNumber LIKE 'FHT-' + CAST(B.CreateYear AS VARCHAR(4)) + '-[0-9]%'
            ), 0) AS StartCounter
        FROM BadProjects B
    ),
    Numbered AS (
        SELECT
            B.ProjectID,
            B.CreateYear,
            ROW_NUMBER() OVER (PARTITION BY B.CreateYear ORDER BY B.OrderingDate, B.ProjectID) + Y.StartCounter AS NewCounter
        FROM BadProjects B
        JOIN YearStarts Y ON Y.CreateYear = B.CreateYear
    )
    UPDATE P
    SET FhtProjectNumber =
        'FHT-' + CAST(N.CreateYear AS VARCHAR(4)) + '-' +
        RIGHT('00000' + CAST(N.NewCounter AS VARCHAR(10)), 5)
    FROM dbo.Project P
    JOIN Numbered N ON N.ProjectID = P.ProjectID;

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Tom Kamin', @MigrationName, 'WADNR-2263: rebuild project numbers that regressed without year segment after commit c5d4fc3ca'
END
