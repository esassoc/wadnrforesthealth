IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pLatestSuccessfulJsonImportInfoForBienniumAndImportTableTypeFromArcOnlineFinanceApi'))
    DROP PROCEDURE dbo.pLatestSuccessfulJsonImportInfoForBienniumAndImportTableTypeFromArcOnlineFinanceApi
GO

CREATE PROCEDURE dbo.pLatestSuccessfulJsonImportInfoForBienniumAndImportTableTypeFromArcOnlineFinanceApi
(
    @ArcOnlineFinanceApiRawJsonImportTableTypeID int null,
    @OptionalBienniumFiscalYear int null = null
)
AS
begin

    select top 1
        ji.ArcOnlineFinanceApiRawJsonImportTableTypeID,
        ji.BienniumFiscalYear,
        ji.JsonImportDate,
        ji.FinanceApiLastLoadDate
    from dbo.ArcOnlineFinanceApiRawJsonImport as ji
    where 
    -- Type must be the one requested
    ArcOnlineFinanceApiRawJsonImportTableTypeID = @ArcOnlineFinanceApiRawJsonImportTableTypeID
    and
    -- Biennium Fiscal Year is optional
    (BienniumFiscalYear = @OptionalBienniumFiscalYear OR @OptionalBienniumFiscalYear is null)
    and
    -- Limit to known successful imports
    JsonImportStatusTypeID = 3 

    order by JsonImportDate desc

end

/*

exec dbo.pLatestSuccessfulJsonImportInfoForBienniumAndImportTableTypeFromArcOnlineFinanceApi @ArcOnlineFinanceApiRawJsonImportTableTypeID=4, @OptionalBienniumFiscalYear = 2003
exec dbo.pLatestSuccessfulJsonImportInfoForBienniumAndImportTableTypeFromArcOnlineFinanceApi @ArcOnlineFinanceApiRawJsonImportTableTypeID=4, @OptionalBienniumFiscalYear = 2007

*/

