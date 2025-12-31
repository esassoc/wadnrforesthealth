CREATE TABLE [dbo].[NotificationType](
	[NotificationTypeID] [int] NOT NULL CONSTRAINT [PK_NotificationType_NotificationTypeID] PRIMARY KEY,
	[NotificationTypeName] [varchar](100) NOT NULL,
	[NotificationTypeDisplayName] [varchar](100) NOT NULL,
 CONSTRAINT [AK_NotificationType_NotificationTypeDisplayName] UNIQUE ([NotificationTypeDisplayName]),
 CONSTRAINT [AK_NotificationType_NotificationTypeName] UNIQUE ([NotificationTypeName])
)
GO
