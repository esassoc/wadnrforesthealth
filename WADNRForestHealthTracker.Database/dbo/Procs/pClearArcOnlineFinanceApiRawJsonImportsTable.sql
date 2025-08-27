IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pClearArcOnlineFinanceApiRawJsonImportsTable'))
    DROP PROCEDURE dbo.pClearArcOnlineFinanceApiRawJsonImportsTable
GO

CREATE PROCEDURE dbo.pClearArcOnlineFinanceApiRawJsonImportsTable
AS
begin

	-- ~ 
    truncate table dbo.ArcOnlineFinanceApiRawJsonImport;
	-- ~ 3 seconds
    --delete from dbo.ArcOnlineFinanceApiRawJsonImport;
end

/*

select * from dbo.ArcOnlineFinanceApiRawJsonImport
exec dbo.pClearArcOnlineFinanceApiRawJsonImportsTable

*/