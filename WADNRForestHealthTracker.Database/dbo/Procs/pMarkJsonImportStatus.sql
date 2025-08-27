IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pMarkJsonImportStatus'))
	DROP PROCEDURE dbo.pMarkJsonImportStatus
GO

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