CREATE TABLE [dbo].[ProjectUpdateHistory](
    [ProjectUpdateHistoryID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_ProjectUpdateHistory_ProjectUpdateHistoryID PRIMARY KEY,
    [ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT FK_ProjectUpdateHistory_ProjectUpdateBatch_ProjectUpdateBatchID FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
    [ProjectUpdateStateID] [int] NOT NULL CONSTRAINT FK_ProjectUpdateHistory_ProjectUpdateState_ProjectUpdateStateID FOREIGN KEY REFERENCES [dbo].[ProjectUpdateState]([ProjectUpdateStateID]),
    [UpdatePersonID] [int] NOT NULL CONSTRAINT FK_ProjectUpdateHistory_Person_UpdatePersonID_PersonID FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [TransitionDate] [datetime] NOT NULL
)
GO