CREATE TABLE [dbo].[ProjectOrganization](
    [ProjectOrganizationID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectOrganization_ProjectOrganizationID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectOrganization_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [OrganizationID] [int] NOT NULL CONSTRAINT [FK_ProjectOrganization_Organization_OrganizationID] FOREIGN KEY REFERENCES [dbo].[Organization]([OrganizationID]),
    [RelationshipTypeID] [int] NOT NULL CONSTRAINT [FK_ProjectOrganization_RelationshipType_RelationshipTypeID] FOREIGN KEY REFERENCES [dbo].[RelationshipType]([RelationshipTypeID])
)
GO