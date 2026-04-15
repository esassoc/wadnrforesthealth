DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0006 - Add ApiKeyGeneratedDate to Person'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    PRINT @MigrationName;

    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Person' AND COLUMN_NAME = 'ApiKeyGeneratedDate')
    BEGIN
        ALTER TABLE dbo.Person ADD [ApiKeyGeneratedDate] [datetime] NULL;
    END

    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Tom Kamin', @MigrationName, 'Track when API keys were generated per Ray Cheng feedback'
END
