CREATE TABLE [dbo].[FundSourceAllocationPriority](
    [FundSourceAllocationPriorityID] [int] NOT NULL CONSTRAINT [PK_FundSourceAllocationPriority_FundSourceAllocationPriorityID] PRIMARY KEY,
    [FundSourceAllocationPriorityNumber] [int] NOT NULL,
    [FundSourceAllocationPriorityColor] [varchar](8) NOT NULL
)
GO
