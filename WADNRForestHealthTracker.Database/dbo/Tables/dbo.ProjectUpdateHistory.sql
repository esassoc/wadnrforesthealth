CREATE TABLE [dbo].[ProjectUpdateHistory](
    [ProjectUpdateHistoryID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectUpdateHistory_ProjectUpdateHistoryID] PRIMARY KEY,
    [ProjectUpdateID] [int] NOT NULL CONSTRAINT [FK_ProjectUpdateHistory_ProjectUpdate_ProjectUpdateID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdate]([ProjectUpdateID]),
    [ProjectUpdateHistoryDate] [datetime] NOT NULL,
    [ProjectUpdateHistoryContent] [dbo].[html] NULL
)
GO