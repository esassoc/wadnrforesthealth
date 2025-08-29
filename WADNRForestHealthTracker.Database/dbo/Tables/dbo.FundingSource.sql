CREATE TABLE [dbo].[FundingSource](
    [FundingSourceID] [int] NOT NULL CONSTRAINT [PK_FundingSource_FundingSourceID] PRIMARY KEY,
    [FundingSourceName] [varchar](150) NOT NULL,
    [FundingSourceDisplayName] [varchar](150) NOT NULL
)
GO
