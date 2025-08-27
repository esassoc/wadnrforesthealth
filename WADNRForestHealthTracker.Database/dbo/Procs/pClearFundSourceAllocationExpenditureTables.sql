IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pClearFundSourceAllocationExpenditureTables'))
    DROP PROCEDURE dbo.pClearFundSourceAllocationExpenditureTables
GO
CREATE PROCEDURE dbo.pClearFundSourceAllocationExpenditureTables
(
	@bienniumFiscalYear int
)
AS
begin

    delete from dbo.FundSourceAllocationExpenditureJsonStage
	where Biennium = @bienniumFiscalYear
    --DBCC CHECKIDENT ('dbo.FundSourceAllocationExpenditureJsonStage', RESEED, 0);

    delete from dbo.FundSourceAllocationExpenditure
	where Biennium = @bienniumFiscalYear
    --DBCC CHECKIDENT ('dbo.FundSourceAllocationExpenditure', RESEED, 0);
	
end
go



/*

set statistics time on

exec pClearFundSourceAllocationExpenditureTables 2013

set statistics time off

select * from [dbo].[SocrataDataMartRawJsonImport]
select * from dbo.FundSourceAllocationExpenditureJsonStage
select * from dbo.FundSourceAllocationExpenditure

*/