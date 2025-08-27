SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProgramNotificationConfiguration](
	[ProgramNotificationConfigurationID] [int] IDENTITY(1,1) NOT NULL,
	[ProgramID] [int] NOT NULL,
	[ProgramNotificationTypeID] [int] NOT NULL,
	[RecurrenceIntervalID] [int] NOT NULL,
	[NotificationEmailText] [dbo].[html] NOT NULL,
 CONSTRAINT [PK_ProgramNotificationConfiguration_ProgramNotificationConfigurationID] PRIMARY KEY CLUSTERED 
(
	[ProgramNotificationConfigurationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProgramNotificationConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_ProgramNotificationConfiguration_Program_ProgramID] FOREIGN KEY([ProgramID])
REFERENCES [dbo].[Program] ([ProgramID])
GO
ALTER TABLE [dbo].[ProgramNotificationConfiguration] CHECK CONSTRAINT [FK_ProgramNotificationConfiguration_Program_ProgramID]
GO
ALTER TABLE [dbo].[ProgramNotificationConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_ProgramNotificationConfiguration_ProgramNotificationType_ProgramNotificationTypeID] FOREIGN KEY([ProgramNotificationTypeID])
REFERENCES [dbo].[ProgramNotificationType] ([ProgramNotificationTypeID])
GO
ALTER TABLE [dbo].[ProgramNotificationConfiguration] CHECK CONSTRAINT [FK_ProgramNotificationConfiguration_ProgramNotificationType_ProgramNotificationTypeID]
GO
ALTER TABLE [dbo].[ProgramNotificationConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_ProgramNotificationConfiguration_RecurrenceInterval_RecurrenceIntervalID] FOREIGN KEY([RecurrenceIntervalID])
REFERENCES [dbo].[RecurrenceInterval] ([RecurrenceIntervalID])
GO
ALTER TABLE [dbo].[ProgramNotificationConfiguration] CHECK CONSTRAINT [FK_ProgramNotificationConfiguration_RecurrenceInterval_RecurrenceIntervalID]