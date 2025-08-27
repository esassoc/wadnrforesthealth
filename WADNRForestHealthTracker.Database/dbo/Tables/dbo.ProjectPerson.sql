SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectPerson](
	[ProjectPersonID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[PersonID] [int] NOT NULL,
	[ProjectPersonRelationshipTypeID] [int] NOT NULL,
	[CreatedAsPartOfBulkImport] [bit] NULL,
 CONSTRAINT [PK_ProjectPerson_ProjectPersonID] PRIMARY KEY CLUSTERED 
(
	[ProjectPersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
CREATE UNIQUE NONCLUSTERED INDEX [UNQ_ProjectPerson_ProjectPersonRelationshipTypeID] ON [dbo].[ProjectPerson]
(
	[ProjectID] ASC
)
WHERE ([ProjectPersonRelationshipTypeID]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ProjectPerson]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPerson_Person_PersonID] FOREIGN KEY([PersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[ProjectPerson] CHECK CONSTRAINT [FK_ProjectPerson_Person_PersonID]
GO
ALTER TABLE [dbo].[ProjectPerson]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPerson_Project_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[ProjectPerson] CHECK CONSTRAINT [FK_ProjectPerson_Project_ProjectID]
GO
ALTER TABLE [dbo].[ProjectPerson]  WITH CHECK ADD  CONSTRAINT [FK_ProjectPerson_ProjectPersonRelationshipType_ProjectPersonRelationshipTypeID] FOREIGN KEY([ProjectPersonRelationshipTypeID])
REFERENCES [dbo].[ProjectPersonRelationshipType] ([ProjectPersonRelationshipTypeID])
GO
ALTER TABLE [dbo].[ProjectPerson] CHECK CONSTRAINT [FK_ProjectPerson_ProjectPersonRelationshipType_ProjectPersonRelationshipTypeID]