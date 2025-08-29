CREATE TABLE dbo.FieldDefinitionDataImage
(
    FieldDefinitionDataImageID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_FieldDefinitionDataImage_FieldDefinitionDataImageID PRIMARY KEY,
    FieldDefinitionDataID int NOT NULL CONSTRAINT FK_FieldDefinitionDataImage_FieldDefinitionData_FieldDefinitionDataID FOREIGN KEY REFERENCES dbo.FieldDefinitionData(FieldDefinitionDataID),
    ImageUrl varchar(256) NOT NULL,
    AltText varchar(256) NULL
)
GO