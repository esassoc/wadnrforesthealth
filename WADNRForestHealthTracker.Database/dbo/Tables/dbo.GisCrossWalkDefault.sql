SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GisCrossWalkDefault](
	[GisCrossWalkDefaultID] [int] IDENTITY(1,1) NOT NULL,
	[GisUploadSourceOrganizationID] [int] NOT NULL,
	[FieldDefinitionID] [int] NOT NULL,
	[GisCrossWalkSourceValue] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[GisCrossWalkMappedValue] [varchar](300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_GisCrossWalkDefault_GisCrossWalkDefaultID] PRIMARY KEY CLUSTERED 
(
	[GisCrossWalkDefaultID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[GisCrossWalkDefault]  WITH CHECK ADD  CONSTRAINT [FK_GisCrossWalkDefault_FieldDefinition_FieldDefinitionID] FOREIGN KEY([FieldDefinitionID])
REFERENCES [dbo].[FieldDefinition] ([FieldDefinitionID])
GO
ALTER TABLE [dbo].[GisCrossWalkDefault] CHECK CONSTRAINT [FK_GisCrossWalkDefault_FieldDefinition_FieldDefinitionID]
GO
ALTER TABLE [dbo].[GisCrossWalkDefault]  WITH CHECK ADD  CONSTRAINT [FK_GisCrossWalkDefault_GisUploadSourceOrganization_GisUploadSourceOrganizationID] FOREIGN KEY([GisUploadSourceOrganizationID])
REFERENCES [dbo].[GisUploadSourceOrganization] ([GisUploadSourceOrganizationID])
GO
ALTER TABLE [dbo].[GisCrossWalkDefault] CHECK CONSTRAINT [FK_GisCrossWalkDefault_GisUploadSourceOrganization_GisUploadSourceOrganizationID]