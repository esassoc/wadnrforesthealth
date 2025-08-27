SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProgramNotificationSent](
	[ProgramNotificationSentID] [int] IDENTITY(1,1) NOT NULL,
	[ProgramNotificationConfigurationID] [int] NOT NULL,
	[SentToPersonID] [int] NOT NULL,
	[ProgramNotificationSentDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ProgramNotificationSent_ProgramNotificationSentID] PRIMARY KEY CLUSTERED 
(
	[ProgramNotificationSentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProgramNotificationSent]  WITH CHECK ADD  CONSTRAINT [FK_ProgramNotificationSent_Person_SentToPersonID_PersonID] FOREIGN KEY([SentToPersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[ProgramNotificationSent] CHECK CONSTRAINT [FK_ProgramNotificationSent_Person_SentToPersonID_PersonID]
GO
ALTER TABLE [dbo].[ProgramNotificationSent]  WITH CHECK ADD  CONSTRAINT [FK_ProgramNotificationSent_ProgramNotificationConfiguration_ProgramNotificationConfigurationID] FOREIGN KEY([ProgramNotificationConfigurationID])
REFERENCES [dbo].[ProgramNotificationConfiguration] ([ProgramNotificationConfigurationID])
GO
ALTER TABLE [dbo].[ProgramNotificationSent] CHECK CONSTRAINT [FK_ProgramNotificationSent_ProgramNotificationConfiguration_ProgramNotificationConfigurationID]