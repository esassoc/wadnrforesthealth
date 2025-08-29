CREATE TABLE dbo.AuditLogEventType
(
    AuditLogEventTypeID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditLogEventType_AuditLogEventTypeID PRIMARY KEY,
    AuditLogEventTypeName varchar(100) NOT NULL,
    AuditLogEventTypeDisplayName varchar(100) NOT NULL,
    CONSTRAINT AK_AuditLogEventType_AuditLogEventTypeName UNIQUE (AuditLogEventTypeName)
)
GO
