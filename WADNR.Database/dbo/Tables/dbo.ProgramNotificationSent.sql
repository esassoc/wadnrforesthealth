CREATE TABLE [dbo].[ProgramNotificationSent](
    [ProgramNotificationSentID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProgramNotificationSent_ProgramNotificationSentID] PRIMARY KEY,
    [ProgramNotificationConfigurationID] [int] NOT NULL CONSTRAINT [FK_ProgramNotificationSent_ProgramNotificationConfiguration_ProgramNotificationConfigurationID] FOREIGN KEY REFERENCES [dbo].[ProgramNotificationConfiguration]([ProgramNotificationConfigurationID]),
    [SentToPersonID] [int] NOT NULL CONSTRAINT [FK_ProgramNotificationSent_Person_SentToPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [ProgramNotificationSentDate] [datetime] NOT NULL
)
GO