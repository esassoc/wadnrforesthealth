

CREATE PROCEDURE dbo.pArcOnlineMarkJsonImportStatus
(
    @ArcOnlineFinanceApiRawJsonImportID int null,
    @JsonImportStatusTypeID int null
)
AS
begin
    update dbo.ArcOnlineFinanceApiRawJsonImport
    set JsonImportStatusTypeID = @JsonImportStatusTypeID,
        JsonImportDate = GETDATE()
    where ArcOnlineFinanceApiRawJsonImportID = @ArcOnlineFinanceApiRawJsonImportID

end
go



/*

select * from  dbo.ArcOnlineFinanceApiRawJsonImport

exec pArcOnlineMarkJsonImportStatus @ArcOnlineFinanceApiRawJsonImportID = 2, @JsonImportStatusTypeID = 1

*/