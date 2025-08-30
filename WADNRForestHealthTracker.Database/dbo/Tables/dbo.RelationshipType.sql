CREATE TABLE [dbo].[RelationshipType](
    [RelationshipTypeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_RelationshipType_RelationshipTypeID] PRIMARY KEY,
    [RelationshipTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_RelationshipType_RelationshipTypeName] UNIQUE,
    [RelationshipTypeDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_RelationshipType_RelationshipTypeDisplayName] UNIQUE
)
GO
