CREATE TABLE [dbo].[GisCrossWalkDefault](
    [GisCrossWalkDefaultID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_GisCrossWalkDefault_GisCrossWalkDefaultID] PRIMARY KEY,
    [GisUploadSourceOrganizationID] [int] NOT NULL CONSTRAINT [FK_GisCrossWalkDefault_GisUploadSourceOrganization_GisUploadSourceOrganizationID] FOREIGN KEY REFERENCES [dbo].[GisUploadSourceOrganization]([GisUploadSourceOrganizationID]),
    [FieldDefinitionID] [int] NOT NULL CONSTRAINT [FK_GisCrossWalkDefault_FieldDefinition_FieldDefinitionID] FOREIGN KEY REFERENCES [dbo].[FieldDefinition]([FieldDefinitionID]),
    [GisCrossWalkSourceValue] [varchar](300) NOT NULL,
    [GisCrossWalkMappedValue] [varchar](300) NOT NULL
)
GO