
CREATE TABLE dbo.FieldDefinition
(
    FieldDefinitionID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_FieldDefinition_FieldDefinitionID PRIMARY KEY,
    FieldDefinitionName varchar(100) NOT NULL,
    FieldDefinitionType varchar(50) NOT NULL,
    CONSTRAINT AK_FieldDefinition_FieldDefinitionName UNIQUE (FieldDefinitionName)
)
GO
