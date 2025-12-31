CREATE TABLE [dbo].[FundSourceType](
    [FundSourceTypeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceType_FundSourceTypeID] PRIMARY KEY,
    [FundSourceTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_FundSourceType_FundSourceTypeName] UNIQUE
)
GO
