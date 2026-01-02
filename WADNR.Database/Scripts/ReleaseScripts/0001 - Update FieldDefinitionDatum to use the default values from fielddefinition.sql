DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0001 - Update FieldDefinitionDatum to use the default values from fielddefinition'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
	PRINT @MigrationName;
	
	UPDATE fdd
    SET fdd.FieldDefinitionDatumValue = fd.DefaultDefinition
    FROM dbo.FieldDefinitionDatum AS fdd
    JOIN dbo.FieldDefinition AS fd
        ON fd.FieldDefinitionID = fdd.FieldDefinitionID
    WHERE fdd.FieldDefinitionDatumValue IS NULL
      AND fd.DefaultDefinition IS NOT NULL

    INSERT INTO dbo.FieldDefinitionDatum
    (
        FieldDefinitionID,
        FieldDefinitionDatumValue,
        FieldDefinitionLabel
    )
    SELECT
        fd.FieldDefinitionID,
        fd.DefaultDefinition AS FieldDefinitionDatumValue,
        NULL AS FieldDefinitionLabel
    FROM dbo.FieldDefinition AS fd
    LEFT JOIN dbo.FieldDefinitionDatum AS fdd
        ON fdd.FieldDefinitionID = fd.FieldDefinitionID
    WHERE fdd.FieldDefinitionID IS NULL


    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName)
    SELECT 'Mack Peters', @MigrationName;
 END