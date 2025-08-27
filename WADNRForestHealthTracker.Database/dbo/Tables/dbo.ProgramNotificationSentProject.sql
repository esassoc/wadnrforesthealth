SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProgramNotificationSentProject](
	[ProgramNotificationSentProjectID] [int] IDENTITY(1,1) NOT NULL,
	[ProgramNotificationSentID] [int] NOT NULL,
	[ProjectID] [int] NOT NULL,
 CONSTRAINT [PK_ProgramNotificationSentProject_ProgramNotificationSentProjectID] PRIMARY KEY CLUSTERED 
(
	[ProgramNotificationSentProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProgramNotificationSentProject]  WITH CHECK ADD  CONSTRAINT [FK_ProgramNotificationSentProject_ProgramNotificationSent_ProgramNotificationSentID] FOREIGN KEY([ProgramNotificationSentID])
REFERENCES [dbo].[ProgramNotificationSent] ([ProgramNotificationSentID])
GO
ALTER TABLE [dbo].[ProgramNotificationSentProject] CHECK CONSTRAINT [FK_ProgramNotificationSentProject_ProgramNotificationSent_ProgramNotificationSentID]
GO
ALTER TABLE [dbo].[ProgramNotificationSentProject]  WITH CHECK ADD  CONSTRAINT [FK_ProgramNotificationSentProject_Project_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[ProgramNotificationSentProject] CHECK CONSTRAINT [FK_ProgramNotificationSentProject_Project_ProjectID]