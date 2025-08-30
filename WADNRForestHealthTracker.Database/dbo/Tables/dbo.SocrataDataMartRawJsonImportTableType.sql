CREATE TABLE [dbo].[SocrataDataMartRawJsonImportTableType](
    [SocrataDataMartRawJsonImportTableTypeID] [int] NOT NULL CONSTRAINT [PK_SocrataDataMartRawJsonImportTableType_SocrataDataMartRawJsonImportTableTypeID] PRIMARY KEY,
    [SocrataDataMartRawJsonImportTableTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_SocrataDataMartRawJsonImportTableType_SocrataDataMartRawJsonImportTableTypeName] UNIQUE
)
GO
