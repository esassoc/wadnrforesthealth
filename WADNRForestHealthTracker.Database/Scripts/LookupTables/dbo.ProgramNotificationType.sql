merge into dbo.ProgramNotificationType as Target
using (values
(1, 'CompletedProjectsMaintenanceReminder', 'Completed Projects Maintenance Reminder')
) as Source (ProgramNotificationTypeID, ProgramNotificationTypeName, ProgramNotificationTypeDisplayName)
on Target.ProgramNotificationTypeID = Source.ProgramNotificationTypeID
when matched then
    update set
        ProgramNotificationTypeName = Source.ProgramNotificationTypeName,
        ProgramNotificationTypeDisplayName = Source.ProgramNotificationTypeDisplayName
when not matched by target then
    insert (ProgramNotificationTypeID, ProgramNotificationTypeName, ProgramNotificationTypeDisplayName)
    values (ProgramNotificationTypeID, ProgramNotificationTypeName, ProgramNotificationTypeDisplayName)
when not matched by source then
    delete;