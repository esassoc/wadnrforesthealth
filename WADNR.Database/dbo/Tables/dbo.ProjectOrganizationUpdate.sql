CREATE TABLE [dbo].[ProjectOrganizationUpdate](
	[ProjectOrganizationUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectOrganizationUpdate_ProjectOrganizationUpdateID] PRIMARY KEY,
	[ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectOrganizationUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
	[OrganizationID] [int] NOT NULL CONSTRAINT [FK_ProjectOrganizationUpdate_Organization_OrganizationID] FOREIGN KEY REFERENCES [dbo].[Organization]([OrganizationID]),
	[RelationshipTypeID] [int] NOT NULL CONSTRAINT [FK_ProjectOrganizationUpdate_RelationshipType_RelationshipTypeID] FOREIGN KEY REFERENCES [dbo].[RelationshipType]([RelationshipTypeID])
)
GO