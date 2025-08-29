CREATE TABLE [dbo].[ProgramNotificationConfiguration](
    [ProgramNotificationConfigurationID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProgramNotificationConfiguration_ProgramNotificationConfigurationID] PRIMARY KEY,
    [ProgramID] [int] NOT NULL CONSTRAINT [FK_ProgramNotificationConfiguration_Program_ProgramID] FOREIGN KEY REFERENCES [dbo].[Program]([ProgramID]),
    [ProgramNotificationTypeID] [int] NOT NULL CONSTRAINT [FK_ProgramNotificationConfiguration_ProgramNotificationType_ProgramNotificationTypeID] FOREIGN KEY REFERENCES [dbo].[ProgramNotificationType]([ProgramNotificationTypeID]),
    [RecurrenceIntervalID] [int] NOT NULL CONSTRAINT [FK_ProgramNotificationConfiguration_RecurrenceInterval_RecurrenceIntervalID] FOREIGN KEY REFERENCES [dbo].[RecurrenceInterval]([RecurrenceIntervalID]),
    [NotificationEmailText] [dbo].[html] NOT NULL
)
GO