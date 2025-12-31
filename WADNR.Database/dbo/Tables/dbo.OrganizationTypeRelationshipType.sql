CREATE TABLE [dbo].[OrganizationTypeRelationshipType](
    [OrganizationTypeRelationshipTypeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_OrganizationTypeRelationshipType_OrganizationTypeRelationshipTypeID] PRIMARY KEY,
    [OrganizationTypeID] [int] NOT NULL CONSTRAINT [FK_OrganizationTypeRelationshipType_OrganizationType_OrganizationTypeID] FOREIGN KEY REFERENCES [dbo].[OrganizationType]([OrganizationTypeID]),
    [RelationshipTypeID] [int] NOT NULL CONSTRAINT [FK_OrganizationTypeRelationshipType_RelationshipType_RelationshipTypeID] FOREIGN KEY REFERENCES [dbo].[RelationshipType]([RelationshipTypeID])
)
GO