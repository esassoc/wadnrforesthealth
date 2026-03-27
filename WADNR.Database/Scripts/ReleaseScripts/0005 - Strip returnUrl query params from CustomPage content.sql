DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0005 - Strip returnUrl query params from CustomPage content'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    PRINT @MigrationName;

    UPDATE dbo.CustomPage
    SET CustomPageContent = REPLACE(
        CAST(CustomPageContent AS NVARCHAR(MAX)),
        '?returnUrl=https%3a%2f%2fforesthealthtracker.dnr.wa.gov%2f',
        ''
    )
    WHERE CAST(CustomPageContent AS NVARCHAR(MAX)) LIKE '%?returnUrl=https%3a%2f%2fforesthealthtracker.dnr.wa.gov%2f%';

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Mack Peters', @MigrationName, 'Remove returnUrl query params with production domain from CustomPage HTML links to fix 403s in QA caused by WAF blocking encoded URLs'
END
