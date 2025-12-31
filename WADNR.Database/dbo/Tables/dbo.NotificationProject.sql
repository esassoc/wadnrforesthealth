CREATE TABLE [dbo].[NotificationProject](
    [NotificationProjectID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_NotificationProject_NotificationProjectID] PRIMARY KEY,
    [NotificationID] [int] NOT NULL CONSTRAINT [FK_NotificationProject_Notification_NotificationID] FOREIGN KEY REFERENCES [dbo].[Notification]([NotificationID]),
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_NotificationProject_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    CONSTRAINT [AK_NotificationProject_NotificationID_ProjectID] UNIQUE ([NotificationID], [ProjectID])
)
GO