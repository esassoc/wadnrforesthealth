CREATE TABLE [dbo].[FundSourceStatus](
    [FundSourceStatusID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceStatus_FundSourceStatusID] PRIMARY KEY,
    [FundSourceStatusName] [varchar](100) NOT NULL CONSTRAINT [AK_FundSourceStatus_FundSourceStatusName] UNIQUE
)
GO
