CREATE TABLE [dbo].[FieldDefinitionDatumImage](
	[FieldDefinitionDatumImageID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_FieldDefinitionDatumImage_FieldDefinitionDatumImageID PRIMARY KEY,
	[FieldDefinitionDatumID] [int] NOT NULL CONSTRAINT FK_FieldDefinitionDatumImage_FieldDefinitionDatum_FieldDefinitionDatumID FOREIGN KEY REFERENCES [dbo].[FieldDefinitionDatum]([FieldDefinitionDatumID]),
	[FileResourceID] [int] NOT NULL CONSTRAINT FK_FieldDefinitionDatumImage_FileResource_FileResourceID FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID])
)
GO