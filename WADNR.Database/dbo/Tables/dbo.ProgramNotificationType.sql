CREATE TABLE [dbo].[ProgramNotificationType](
    [ProgramNotificationTypeID] [int] NOT NULL CONSTRAINT [PK_ProgramNotificationType_ProgramNotificationTypeID] PRIMARY KEY,
    [ProgramNotificationTypeName] [varchar](100) NOT NULL,
    [ProgramNotificationTypeDisplayName] [varchar](100) NOT NULL
)
GO
