

CREATE PROCEDURE dbo.pLatestSuccessfulJsonImportInfoForBienniumAndImportTableType
(
    @SocrataDataMartRawJsonImportTableTypeID int null,
    @OptionalBienniumFiscalYear int null = null
)
AS
begin

    select top 1
        ji.SocrataDataMartRawJsonImportTableTypeID,
        ji.BienniumFiscalYear,
        ji.JsonImportDate,
        ji.FinanceApiLastLoadDate
    from dbo.SocrataDataMartRawJsonImport as ji
    where 
    -- Type must be the one requested
    SocrataDataMartRawJsonImportTableTypeID = @SocrataDataMartRawJsonImportTableTypeID
    and
    -- Biennium Fiscal Year is optional
    (BienniumFiscalYear = @OptionalBienniumFiscalYear OR @OptionalBienniumFiscalYear is null)
    and
    -- Limit to known successful imports
    JsonImportStatusTypeID = 3 

    order by JsonImportDate desc

end

/*

exec dbo.pLatestSuccessfulJsonImportInfoForBienniumAndImportTableType @SocrataDataMartRawJsonImportTableTypeID=4, @OptionalBienniumFiscalYear = 2003
exec dbo.pLatestSuccessfulJsonImportInfoForBienniumAndImportTableType @SocrataDataMartRawJsonImportTableTypeID=4, @OptionalBienniumFiscalYear = 2007

*/

/*
delete from dbo.SocrataDataMartRawJsonImport 
select * from dbo.SocrataDataMartRawJsonImport 
*/