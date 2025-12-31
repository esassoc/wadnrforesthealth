CREATE TABLE [dbo].[SocrataDataMartRawJsonImport](
    [SocrataDataMartRawJsonImportID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_SocrataDataMartRawJsonImport_SocrataDataMartRawJsonImportID] PRIMARY KEY,
    [CreateDate] [datetime] NOT NULL,
    [SocrataDataMartRawJsonImportTableTypeID] [int] NOT NULL CONSTRAINT [FK_SocrataDataMartRawJsonImport_SocrataDataMartRawJsonImportTableType_SocrataDataMartRawJsonImportTableTypeID] FOREIGN KEY REFERENCES [dbo].[SocrataDataMartRawJsonImportTableType]([SocrataDataMartRawJsonImportTableTypeID]),
    [BienniumFiscalYear] [int] NULL,
    [FinanceApiLastLoadDate] [datetime] NULL,
    [RawJsonString] [varchar](max) NOT NULL,
    [JsonImportDate] [datetime] NULL,
    [JsonImportStatusTypeID] [int] NOT NULL CONSTRAINT [FK_SocrataDataMartRawJsonImport_JsonImportStatusType_JsonImportStatusTypeID] FOREIGN KEY REFERENCES [dbo].[JsonImportStatusType]([JsonImportStatusTypeID])
)
GO