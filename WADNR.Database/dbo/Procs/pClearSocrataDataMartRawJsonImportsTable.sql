

CREATE PROCEDURE dbo.pClearSocrataDataMartRawJsonImportsTable
AS
begin

	-- ~ 
    truncate table dbo.SocrataDataMartRawJsonImport;
	-- ~ 3 seconds
    --delete from dbo.SocrataDataMartRawJsonImport;
end

/*

select * from dbo.SocrataDataMartRawJsonImport
exec dbo.pClearSocrataDataMartRawJsonImportsTable

*/