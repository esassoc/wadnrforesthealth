CREATE TABLE [dbo].[ProjectNoteUpdate](
    [ProjectNoteUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectNoteUpdate_ProjectNoteUpdateID] PRIMARY KEY,
    [ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectNoteUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
    [Note] [varchar](8000) NOT NULL,
    [CreatePersonID] [int] NULL CONSTRAINT [FK_ProjectNoteUpdate_Person_CreatePersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [CreateDate] [datetime] NOT NULL,
    [UpdatePersonID] [int] NULL CONSTRAINT [FK_ProjectNoteUpdate_Person_UpdatePersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [UpdateDate] [datetime] NULL
)
GO