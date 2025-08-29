CREATE TABLE dbo.FieldDefinitionData
(
    FieldDefinitionDataID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_FieldDefinitionData_FieldDefinitionDataID PRIMARY KEY,
    FieldDefinitionID int NOT NULL CONSTRAINT FK_FieldDefinitionData_FieldDefinition_FieldDefinitionID FOREIGN KEY REFERENCES dbo.FieldDefinition(FieldDefinitionID),
    DataValue varchar(256) NOT NULL
)
GO