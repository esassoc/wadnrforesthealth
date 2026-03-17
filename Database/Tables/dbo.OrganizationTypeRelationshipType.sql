SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrganizationTypeRelationshipType](
	[OrganizationTypeRelationshipTypeID] [int] IDENTITY(1,1) NOT NULL,
	[OrganizationTypeID] [int] NOT NULL,
	[RelationshipTypeID] [int] NOT NULL,
 CONSTRAINT [PK_OrganizationTypeRelationshipType_OrganizationTypeRelationshipTypeID] PRIMARY KEY CLUSTERED 
(
	[OrganizationTypeRelationshipTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[OrganizationTypeRelationshipType]  WITH CHECK ADD  CONSTRAINT [FK_OrganizationTypeRelationshipType_OrganizationType_OrganizationTypeID] FOREIGN KEY([OrganizationTypeID])
REFERENCES [dbo].[OrganizationType] ([OrganizationTypeID])
GO
ALTER TABLE [dbo].[OrganizationTypeRelationshipType] CHECK CONSTRAINT [FK_OrganizationTypeRelationshipType_OrganizationType_OrganizationTypeID]
GO
ALTER TABLE [dbo].[OrganizationTypeRelationshipType]  WITH CHECK ADD  CONSTRAINT [FK_OrganizationTypeRelationshipType_RelationshipType_RelationshipTypeID] FOREIGN KEY([RelationshipTypeID])
REFERENCES [dbo].[RelationshipType] ([RelationshipTypeID])
GO
ALTER TABLE [dbo].[OrganizationTypeRelationshipType] CHECK CONSTRAINT [FK_OrganizationTypeRelationshipType_RelationshipType_RelationshipTypeID]