SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GisDefaultMapping](
	[GisDefaultMappingID] [int] IDENTITY(1,1) NOT NULL,
	[GisUploadSourceOrganizationID] [int] NOT NULL,
	[FieldDefinitionID] [int] NOT NULL,
	[GisDefaultMappingColumnName] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_GisDefaultMapping_GisDefaultMappingID] PRIMARY KEY CLUSTERED 
(
	[GisDefaultMappingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[GisDefaultMapping]  WITH CHECK ADD  CONSTRAINT [FK_GisDefaultMapping_FieldDefinition_FieldDefinitionID] FOREIGN KEY([FieldDefinitionID])
REFERENCES [dbo].[FieldDefinition] ([FieldDefinitionID])
GO
ALTER TABLE [dbo].[GisDefaultMapping] CHECK CONSTRAINT [FK_GisDefaultMapping_FieldDefinition_FieldDefinitionID]
GO
ALTER TABLE [dbo].[GisDefaultMapping]  WITH CHECK ADD  CONSTRAINT [FK_GisDefaultMapping_GisUploadSourceOrganization_GisUploadSourceOrganizationID] FOREIGN KEY([GisUploadSourceOrganizationID])
REFERENCES [dbo].[GisUploadSourceOrganization] ([GisUploadSourceOrganizationID])
GO
ALTER TABLE [dbo].[GisDefaultMapping] CHECK CONSTRAINT [FK_GisDefaultMapping_GisUploadSourceOrganization_GisUploadSourceOrganizationID]