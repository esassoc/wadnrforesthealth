DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0002 Add FirmaPage for Classifications'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
	PRINT @MigrationName;
	
	INSERT INTO dbo.FirmaPage (FirmaPageTypeID, firmaPageContent)
	SELECT 77, ClassificationSystemListPageContent AS firmaPageContent
	FROM dbo.ClassificationSystem
	WHERE ClassificationSystemID = 11


    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName)
    SELECT 'Mack Peters', @MigrationName;
 END