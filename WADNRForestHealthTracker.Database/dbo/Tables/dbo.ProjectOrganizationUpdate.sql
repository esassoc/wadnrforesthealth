SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectOrganizationUpdate](
	[ProjectOrganizationUpdateID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectUpdateBatchID] [int] NOT NULL,
	[OrganizationID] [int] NOT NULL,
	[RelationshipTypeID] [int] NOT NULL,
 CONSTRAINT [PK_ProjectOrganizationUpdate_ProjectOrganizationUpdateID] PRIMARY KEY CLUSTERED 
(
	[ProjectOrganizationUpdateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProjectOrganizationUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectOrganizationUpdate_Organization_OrganizationID] FOREIGN KEY([OrganizationID])
REFERENCES [dbo].[Organization] ([OrganizationID])
GO
ALTER TABLE [dbo].[ProjectOrganizationUpdate] CHECK CONSTRAINT [FK_ProjectOrganizationUpdate_Organization_OrganizationID]
GO
ALTER TABLE [dbo].[ProjectOrganizationUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectOrganizationUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY([ProjectUpdateBatchID])
REFERENCES [dbo].[ProjectUpdateBatch] ([ProjectUpdateBatchID])
GO
ALTER TABLE [dbo].[ProjectOrganizationUpdate] CHECK CONSTRAINT [FK_ProjectOrganizationUpdate_ProjectUpdateBatch_ProjectUpdateBatchID]
GO
ALTER TABLE [dbo].[ProjectOrganizationUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectOrganizationUpdate_RelationshipType_RelationshipTypeID] FOREIGN KEY([RelationshipTypeID])
REFERENCES [dbo].[RelationshipType] ([RelationshipTypeID])
GO
ALTER TABLE [dbo].[ProjectOrganizationUpdate] CHECK CONSTRAINT [FK_ProjectOrganizationUpdate_RelationshipType_RelationshipTypeID]