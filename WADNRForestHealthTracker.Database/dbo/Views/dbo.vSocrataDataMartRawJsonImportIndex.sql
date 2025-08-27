if exists (select * from dbo.sysobjects where id = object_id('dbo.vSocrataDataMartRawJsonImportIndex'))
    drop view dbo.vSocrataDataMartRawJsonImportIndex
go

create view dbo.vSocrataDataMartRawJsonImportIndex
as
select 
    rj.SocrataDataMartRawJsonImportID,
    rj.CreateDate,
    rj.SocrataDataMartRawJsonImportTableTypeID,
    tt.SocrataDataMartRawJsonImportTableTypeName,
    rj.BienniumFiscalYear,
    rj.FinanceApiLastLoadDate,
    rj.JsonImportStatusTypeID,
    jist.JsonImportStatusTypeName,
    rj.JsonImportDate,
    DATALENGTH(rj.RawJsonString) as RawJsonStringLength
from dbo.SocrataDataMartRawJsonImport as rj
inner join SocrataDataMartRawJsonImportTableType as tt on rj.SocrataDataMartRawJsonImportTableTypeID = tt.SocrataDataMartRawJsonImportTableTypeID
inner join JsonImportStatusType as jist on rj.JsonImportStatusTypeID = jist.JsonImportStatusTypeID

/*

select * from dbo.SocrataDataMartRawJsonImport
select * from dbo.vSocrataDataMartRawJsonImportIndex

*/