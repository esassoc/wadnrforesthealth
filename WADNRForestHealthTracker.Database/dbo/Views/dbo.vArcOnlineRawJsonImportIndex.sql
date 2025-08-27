if exists (select * from dbo.sysobjects where id = object_id('dbo.vArcOnlineRawJsonImportIndex'))
    drop view dbo.vArcOnlineRawJsonImportIndex
go

create view dbo.vArcOnlineRawJsonImportIndex
as
select 
    rj.ArcOnlineFinanceApiRawJsonImportID,
    rj.CreateDate,
    rj.ArcOnlineFinanceApiRawJsonImportTableTypeID,
    tt.ArcOnlineFinanceApiRawJsonImportTableTypeName,
    rj.BienniumFiscalYear,
    rj.FinanceApiLastLoadDate,
    rj.JsonImportStatusTypeID,
    jist.JsonImportStatusTypeName,
    rj.JsonImportDate,
    DATALENGTH(rj.RawJsonString) as RawJsonStringLength
from dbo.ArcOnlineFinanceApiRawJsonImport as rj
inner join ArcOnlineFinanceApiRawJsonImportTableType as tt on rj.ArcOnlineFinanceApiRawJsonImportTableTypeID = tt.ArcOnlineFinanceApiRawJsonImportTableTypeID
inner join JsonImportStatusType as jist on rj.JsonImportStatusTypeID = jist.JsonImportStatusTypeID

/*

select * from dbo.ArcOnlineFinanceApiRawJsonImport
select * from dbo.vArcOnlineFinanceApiRawJsonImportIndex

*/