CREATE TABLE dbo.ArcOnlineFinanceApiRawJsonImport
(
    ArcOnlineFinanceApiRawJsonImportID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ArcOnlineFinanceApiRawJsonImport_ArcOnlineFinanceApiRawJsonImportID PRIMARY KEY,
    ArcOnlineFinanceApiRawJsonImportTableTypeID int NOT NULL CONSTRAINT FK_ArcOnlineFinanceApiRawJsonImport_ArcOnlineFinanceApiRawJsonImportTableType_ArcOnlineFinanceApiRawJsonImportTableTypeID FOREIGN KEY REFERENCES dbo.ArcOnlineFinanceApiRawJsonImportTableType(ArcOnlineFinanceApiRawJsonImportTableTypeID),
    JsonRaw varchar(max) NOT NULL,
    ImportDate datetime NOT NULL
)
GO