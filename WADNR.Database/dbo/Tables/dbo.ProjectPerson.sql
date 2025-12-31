CREATE TABLE [dbo].[ProjectPerson](
    [ProjectPersonID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectPerson_ProjectPersonID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectPerson_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [PersonID] [int] NOT NULL CONSTRAINT [FK_ProjectPerson_Person_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [ProjectPersonRelationshipTypeID] [int] NOT NULL CONSTRAINT [FK_ProjectPerson_ProjectPersonRelationshipType_ProjectPersonRelationshipTypeID] FOREIGN KEY REFERENCES [dbo].[ProjectPersonRelationshipType]([ProjectPersonRelationshipTypeID]),
    [CreatedAsPartOfBulkImport] [bit] NULL
)
GO
CREATE UNIQUE NONCLUSTERED INDEX [UNQ_ProjectPerson_ProjectPersonRelationshipTypeID] ON [dbo].[ProjectPerson]([ProjectID]) WHERE ([ProjectPersonRelationshipTypeID]=(1))
GO