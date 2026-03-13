CREATE TABLE [dbo].[ProjectUpdateConfiguration](
    [ProjectUpdateConfigurationID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectUpdateConfiguration_ProjectUpdateConfigurationID] PRIMARY KEY,
    [ProjectUpdateKickOffDate] [date] NULL,
    [ProjectUpdateCloseOutDate] [date] NULL,
    [ProjectUpdateReminderInterval] [int] NULL,
    [EnableProjectUpdateReminders] [bit] NOT NULL,
    [SendPeriodicReminders] [bit] NOT NULL,
    [SendCloseOutNotification] [bit] NOT NULL,
    [ProjectUpdateKickOffIntroContent] [dbo].[html] NULL,
    [ProjectUpdateReminderIntroContent] [dbo].[html] NULL,
    [ProjectUpdateCloseOutIntroContent] [dbo].[html] NULL
)
GO
