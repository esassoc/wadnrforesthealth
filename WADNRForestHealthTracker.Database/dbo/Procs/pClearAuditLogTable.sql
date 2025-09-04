

CREATE PROCEDURE dbo.pClearAuditLogTable
AS
begin

    truncate table dbo.AuditLog;

end

/*

exec dbo.pClearAuditLogTable

*/