CREATE TABLE dbo.ArcOnlineFinanceApiRawJsonImportTableType
(
    ArcOnlineFinanceApiRawJsonImportTableTypeID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ArcOnlineFinanceApiRawJsonImportTableType_ArcOnlineFinanceApiRawJsonImportTableTypeID PRIMARY KEY,
    ArcOnlineFinanceApiRawJsonImportTableTypeName varchar(100) NOT NULL,
    CONSTRAINT AK_ArcOnlineFinanceApiRawJsonImportTableType_ArcOnlineFinanceApiRawJsonImportTableTypeName UNIQUE (ArcOnlineFinanceApiRawJsonImportTableTypeName)
)
GO
