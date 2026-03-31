DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0003 - Populate IsUser column on Person table'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    -- Backfill IsUser = 1 for persons who have a GlobalID or an entry in PersonAllowedAuthenticator
    UPDATE dbo.Person
    SET IsUser = 1
    WHERE (GlobalID IS NOT NULL AND GlobalID != '')
       OR PersonID IN (SELECT PersonID FROM dbo.PersonAllowedAuthenticator)

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'MackPeters', @MigrationName, 'Backfill IsUser column from GlobalID and PersonAllowedAuthenticator'
END
