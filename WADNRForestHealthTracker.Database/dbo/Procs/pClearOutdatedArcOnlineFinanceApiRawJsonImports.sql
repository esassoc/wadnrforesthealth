IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pClearOutdatedArcOnlineFinanceApiRawJsonImports'))
    DROP PROCEDURE dbo.pClearOutdatedArcOnlineFinanceApiRawJsonImports
GO

CREATE PROCEDURE dbo.pClearOutdatedArcOnlineFinanceApiRawJsonImports
    (
        @daysOldToRemove int
    )
AS

begin

-- Figure out the current maximum API Load date. Note that this might be successful, or failed - we don't care
declare @maxFinanceApiLastLoadDate datetime
set @maxFinanceApiLastLoadDate = (select MAX(FinanceApiLastLoadDate) from dbo.ArcOnlineFinanceApiRawJsonImport)

-- For clarity. There's no work to do if we have no records in the table.
if @maxFinanceApiLastLoadDate is null return;

-- Calculate a cutoff date, before which we will delete records
declare @cutoffFinanceApiLastLoadDate datetime
set @cutoffFinanceApiLastLoadDate = DATEADD(DAY, -@daysOldToRemove, @maxFinanceApiLastLoadDate)

-- Delete records that were loaded before the cutoff date
delete from dbo.ArcOnlineFinanceApiRawJsonImport
where FinanceApiLastLoadDate < @cutoffFinanceApiLastLoadDate

end
GO


/*

exec dbo.pClearOutdatedArcOnlineFinanceApiRawJsonImports 2

*/



