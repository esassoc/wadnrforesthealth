IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pClearAuditLogTable'))
    DROP PROCEDURE dbo.pClearAuditLogTable
GO

CREATE PROCEDURE dbo.pClearAuditLogTable
AS
begin

    truncate table dbo.AuditLog;

end

/*

exec dbo.pClearAuditLogTable

*/