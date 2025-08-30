CREATE TABLE [dbo].[ProjectInternalNote](
    [ProjectInternalNoteID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectInternalNote_ProjectInternalNoteID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectInternalNote_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [Note] [varchar](8000) NOT NULL,
    [CreatePersonID] [int] NULL CONSTRAINT [FK_ProjectInternalNote_Person_CreatePersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [CreateDate] [datetime] NOT NULL,
    [UpdatePersonID] [int] NULL CONSTRAINT [FK_ProjectInternalNote_Person_UpdatePersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [UpdateDate] [datetime] NULL
)
GO