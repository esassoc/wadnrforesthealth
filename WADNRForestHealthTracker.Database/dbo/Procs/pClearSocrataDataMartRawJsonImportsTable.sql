IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pClearSocrataDataMartRawJsonImportsTable'))
    DROP PROCEDURE dbo.pClearSocrataDataMartRawJsonImportsTable
GO

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