CREATE TABLE [dbo].[FundSourceAllocationSource](
    [FundSourceAllocationSourceID] [int] NOT NULL CONSTRAINT [PK_FundSourceAllocationSource_FundSourceAllocationSourceID] PRIMARY KEY,
    [FundSourceAllocationSourceName] [varchar](200) NOT NULL,
    [FundSourceAllocationSourceDisplayName] [varchar](200) NOT NULL,
    [SortOrder] [int] NOT NULL
)
GO
