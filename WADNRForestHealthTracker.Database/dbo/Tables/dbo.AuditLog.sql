CREATE TABLE dbo.AuditLog
(
    AuditLogID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AuditLog_AuditLogID PRIMARY KEY,
    PersonID int NOT NULL CONSTRAINT FK_AuditLog_Person_PersonID FOREIGN KEY REFERENCES dbo.Person(PersonID),
    AuditLogDate datetime NOT NULL,
    AuditLogEventTypeID int NOT NULL CONSTRAINT FK_AuditLog_AuditLogEventType_AuditLogEventTypeID FOREIGN KEY REFERENCES dbo.AuditLogEventType(AuditLogEventTypeID),
    TableName varchar(500) NOT NULL,
    RecordID int NOT NULL,
    ColumnName varchar(500) NOT NULL,
    OriginalValue varchar(max) NULL,
    NewValue varchar(max) NOT NULL,
    AuditDescription varchar(max) NULL,
    ProjectID int NULL
)
GO