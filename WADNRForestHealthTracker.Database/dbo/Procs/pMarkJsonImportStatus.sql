

CREATE PROCEDURE dbo.pMarkJsonImportStatus
(
    @SocrataDataMartRawJsonImportID int null,
    @JsonImportStatusTypeID int null
)
AS
begin
    update dbo.SocrataDataMartRawJsonImport
    set JsonImportStatusTypeID = @JsonImportStatusTypeID,
        JsonImportDate = GETDATE()
    where SocrataDataMartRawJsonImportID = @SocrataDataMartRawJsonImportID

end
go



/*

select * from  dbo.SocrataDataMartRawJsonImport

exec pMarkJsonImportStatus @SocrataDataMartRawJsonImportID = 2, @JsonImportStatusTypeID = 1

*/