CREATE TABLE [dbo].[FieldDefinitionDatum](
    [FieldDefinitionDatumID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_FieldDefinitionDatum_FieldDefinitionDatumID PRIMARY KEY,
    [FieldDefinitionID] [int] NOT NULL CONSTRAINT FK_FieldDefinitionDatum_FieldDefinition_FieldDefinitionID FOREIGN KEY REFERENCES [dbo].[FieldDefinition]([FieldDefinitionID]),
    [FieldDefinitionDatumValue] [dbo].[html] NULL,
    [FieldDefinitionLabel] [varchar](300) NULL
)
GO