SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectOrganization](
	[ProjectOrganizationID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[OrganizationID] [int] NOT NULL,
	[RelationshipTypeID] [int] NOT NULL,
 CONSTRAINT [PK_ProjectOrganization_ProjectOrganizationID] PRIMARY KEY CLUSTERED 
(
	[ProjectOrganizationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProjectOrganization]  WITH CHECK ADD  CONSTRAINT [FK_ProjectOrganization_Organization_OrganizationID] FOREIGN KEY([OrganizationID])
REFERENCES [dbo].[Organization] ([OrganizationID])
GO
ALTER TABLE [dbo].[ProjectOrganization] CHECK CONSTRAINT [FK_ProjectOrganization_Organization_OrganizationID]
GO
ALTER TABLE [dbo].[ProjectOrganization]  WITH CHECK ADD  CONSTRAINT [FK_ProjectOrganization_Project_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[ProjectOrganization] CHECK CONSTRAINT [FK_ProjectOrganization_Project_ProjectID]
GO
ALTER TABLE [dbo].[ProjectOrganization]  WITH CHECK ADD  CONSTRAINT [FK_ProjectOrganization_RelationshipType_RelationshipTypeID] FOREIGN KEY([RelationshipTypeID])
REFERENCES [dbo].[RelationshipType] ([RelationshipTypeID])
GO
ALTER TABLE [dbo].[ProjectOrganization] CHECK CONSTRAINT [FK_ProjectOrganization_RelationshipType_RelationshipTypeID]