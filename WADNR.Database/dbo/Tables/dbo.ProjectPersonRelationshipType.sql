CREATE TABLE [dbo].[ProjectPersonRelationshipType](
    [ProjectPersonRelationshipTypeID] [int] NOT NULL CONSTRAINT [PK_ProjectPersonRelationshipType_ProjectPersonRelationshipTypeID] PRIMARY KEY,
    [ProjectPersonRelationshipTypeName] [varchar](150) NOT NULL CONSTRAINT [AK_ProjectPersonRelationshipType_ProjectPersonRelationshipTypeName] UNIQUE,
    [ProjectPersonRelationshipTypeDisplayName] [varchar](150) NOT NULL CONSTRAINT [AK_ProjectPersonRelationshipType_ProjectPersonRelationshipTypeDisplayName] UNIQUE,
    [FieldDefinitionID] [int] NOT NULL CONSTRAINT [FK_ProjectPersonRelationshipType_FieldDefinition_FieldDefinitionID] FOREIGN KEY REFERENCES [dbo].[FieldDefinition]([FieldDefinitionID]),
    [IsRequired] [bit] NOT NULL,
    [IsRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo] [bit] NOT NULL,
    [SortOrder] [int] NOT NULL
)
GO