SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectPersonRelationshipType](
	[ProjectPersonRelationshipTypeID] [int] NOT NULL,
	[ProjectPersonRelationshipTypeName] [varchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ProjectPersonRelationshipTypeDisplayName] [varchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[FieldDefinitionID] [int] NOT NULL,
	[IsRequired] [bit] NOT NULL,
	[IsRestrictedToAdminAndProjectStewardAndCanViewLandownerInfo] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_ProjectPersonRelationshipType_ProjectPersonRelationshipTypeID] PRIMARY KEY CLUSTERED 
(
	[ProjectPersonRelationshipTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_ProjectPersonRelationshipType_ProjectPersonRelationshipTypeDisplayName] UNIQUE NONCLUSTERED 
(
	[ProjectPersonRelationshipTypeDisplayName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_ProjectPersonRelationshipType_ProjectPersonRelationshipTypeName] UNIQUE NONCLUSTERED 
(
	[ProjectPersonRelationshipTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProjectPersonRelationshipType]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPersonRelationshipType_FieldDefinition_FieldDefinitionID] FOREIGN KEY([FieldDefinitionID])
REFERENCES [dbo].[FieldDefinition] ([FieldDefinitionID])
GO
ALTER TABLE [dbo].[ProjectPersonRelationshipType] CHECK CONSTRAINT [FK_ProjectPersonRelationshipType_FieldDefinition_FieldDefinitionID]