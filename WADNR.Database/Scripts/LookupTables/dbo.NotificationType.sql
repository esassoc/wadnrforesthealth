merge into dbo.NotificationType as Target
using (values
(1, 'ProjectUpdateReminder', 'Project Update Reminder'),
(2, 'ProjectUpdateSubmitted', 'Project Update Submitted'),
(3, 'ProjectUpdateReturned', 'Project Update Returned'),
(4, 'ProjectUpdateApproved', 'Project Update Approved'),
(5, 'Custom', 'Custom Notification'),
(6, 'ProjectSubmitted', 'Project Submitted'),
(7, 'ProjectApproved', 'Project Approved'),
(8, 'ProjectReturned', 'Project Returned')
) as Source (NotificationTypeID, NotificationTypeName, NotificationTypeDisplayName)
on Target.NotificationTypeID = Source.NotificationTypeID
when matched then
    update set
        NotificationTypeName = Source.NotificationTypeName,
        NotificationTypeDisplayName = Source.NotificationTypeDisplayName
when not matched by target then
    insert (NotificationTypeID, NotificationTypeName, NotificationTypeDisplayName)
    values (NotificationTypeID, NotificationTypeName, NotificationTypeDisplayName)
when not matched by source then
    delete;