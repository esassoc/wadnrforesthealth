CREATE TABLE [dbo].[GisDefaultMapping](
    [GisDefaultMappingID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_GisDefaultMapping_GisDefaultMappingID] PRIMARY KEY,
    [GisUploadSourceOrganizationID] [int] NOT NULL CONSTRAINT [FK_GisDefaultMapping_GisUploadSourceOrganization_GisUploadSourceOrganizationID] FOREIGN KEY REFERENCES [dbo].[GisUploadSourceOrganization]([GisUploadSourceOrganizationID]),
    [FieldDefinitionID] [int] NOT NULL CONSTRAINT [FK_GisDefaultMapping_FieldDefinition_FieldDefinitionID] FOREIGN KEY REFERENCES [dbo].[FieldDefinition]([FieldDefinitionID]),
    [GisDefaultMappingColumnName] [varchar](300) NOT NULL
)
GO