SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectUpdateConfiguration](
	[ProjectUpdateConfigurationID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectUpdateKickOffDate] [datetime] NULL,
	[ProjectUpdateCloseOutDate] [datetime] NULL,
	[ProjectUpdateReminderInterval] [int] NULL,
	[EnableProjectUpdateReminders] [bit] NOT NULL,
	[SendPeriodicReminders] [bit] NOT NULL,
	[SendCloseOutNotification] [bit] NOT NULL,
	[ProjectUpdateKickOffIntroContent] [dbo].[html] NULL,
	[ProjectUpdateReminderIntroContent] [dbo].[html] NULL,
	[ProjectUpdateCloseOutIntroContent] [dbo].[html] NULL,
 CONSTRAINT [PK_ProjectUpdateConfiguration_ProjectUpdateConfigurationID] PRIMARY KEY CLUSTERED 
(
	[ProjectUpdateConfigurationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
