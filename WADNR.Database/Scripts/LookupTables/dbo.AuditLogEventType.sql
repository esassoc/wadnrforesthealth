merge into dbo.AuditLogEventType as Target
using (values
(1, 'Added', 'Added'),
(2, 'Deleted', 'Deleted'),
(3, 'Modified', 'Modified')
) as Source (AuditLogEventTypeID, AuditLogEventTypeName, AuditLogEventTypeDisplayName)
on Target.AuditLogEventTypeID = Source.AuditLogEventTypeID
when matched then
    update set
        AuditLogEventTypeName = Source.AuditLogEventTypeName,
        AuditLogEventTypeDisplayName = Source.AuditLogEventTypeDisplayName
when not matched by target then
    insert (AuditLogEventTypeID, AuditLogEventTypeName, AuditLogEventTypeDisplayName)
    values (AuditLogEventTypeID, AuditLogEventTypeName, AuditLogEventTypeDisplayName)
when not matched by source then
    delete;

