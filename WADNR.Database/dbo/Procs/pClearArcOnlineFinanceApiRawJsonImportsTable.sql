

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