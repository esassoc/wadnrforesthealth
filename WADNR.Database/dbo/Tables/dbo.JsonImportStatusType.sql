CREATE TABLE [dbo].[JsonImportStatusType](
    [JsonImportStatusTypeID] [int] NOT NULL CONSTRAINT [PK_JsonImportStatusType_JsonImportStatusTypeID] PRIMARY KEY,
    [JsonImportStatusTypeName] [varchar](100) NOT NULL,
    CONSTRAINT [AK_JsonImportStatusType_JsonImportStatusTypeName] UNIQUE ([JsonImportStatusTypeName])
)
GO
