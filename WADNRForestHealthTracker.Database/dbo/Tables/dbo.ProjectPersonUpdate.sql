CREATE TABLE [dbo].[ProjectPersonUpdate](
    [ProjectPersonUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectPersonUpdate_ProjectPersonUpdateID] PRIMARY KEY,
    [ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectPersonUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
    [PersonID] [int] NOT NULL CONSTRAINT [FK_ProjectPersonUpdate_Person_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [ProjectPersonRelationshipTypeID] [int] NOT NULL CONSTRAINT [FK_ProjectPersonUpdate_ProjectPersonRelationshipType_ProjectPersonRelationshipTypeID] FOREIGN KEY REFERENCES [dbo].[ProjectPersonRelationshipType]([ProjectPersonRelationshipTypeID])
)
GO