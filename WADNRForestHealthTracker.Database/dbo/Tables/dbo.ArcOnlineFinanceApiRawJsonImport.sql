CREATE TABLE [dbo].[ArcOnlineFinanceApiRawJsonImport](
    [ArcOnlineFinanceApiRawJsonImportID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_ArcOnlineFinanceApiRawJsonImport_ArcOnlineFinanceApiRawJsonImportID PRIMARY KEY,
    [CreateDate] [datetime] NOT NULL,
    [ArcOnlineFinanceApiRawJsonImportTableTypeID] [int] NOT NULL CONSTRAINT FK_ArcOnlineFinanceApiRawJsonImport_ArcOnlineFinanceApiRawJsonImportTableType_ArcOnlineFinanceApiRawJsonImportTableTypeID FOREIGN KEY REFERENCES [dbo].[ArcOnlineFinanceApiRawJsonImportTableType]([ArcOnlineFinanceApiRawJsonImportTableTypeID]),
    [BienniumFiscalYear] [int] NULL,
    [FinanceApiLastLoadDate] [datetime] NULL,
    [RawJsonString] [varchar](max) NOT NULL,
    [JsonImportDate] [datetime] NULL,
    [JsonImportStatusTypeID] [int] NOT NULL CONSTRAINT FK_ArcOnlineFinanceApiRawJsonImport_JsonImportStatusType_JsonImportStatusTypeID FOREIGN KEY REFERENCES [dbo].[JsonImportStatusType]([JsonImportStatusTypeID])
)
GO