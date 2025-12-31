CREATE TABLE [dbo].[ProgramNotificationSentProject](
	[ProgramNotificationSentProjectID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProgramNotificationSentProject_ProgramNotificationSentProjectID] PRIMARY KEY,
	[ProgramNotificationSentID] [int] NOT NULL CONSTRAINT [FK_ProgramNotificationSentProject_ProgramNotificationSent_ProgramNotificationSentID] FOREIGN KEY REFERENCES [dbo].[ProgramNotificationSent] ([ProgramNotificationSentID]),
	[ProjectID] [int] NOT NULL CONSTRAINT [FK_ProgramNotificationSentProject_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project] ([ProjectID])
)
GO