CREATE TABLE [dbo].[FundSourceAllocationBudgetLineItem](
    [FundSourceAllocationBudgetLineItemID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceAllocationBudgetLineItem_FundSourceAllocationBudgetLineItemID] PRIMARY KEY,
    [FundSourceAllocationID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationBudgetLineItem_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY REFERENCES [dbo].[FundSourceAllocation]([FundSourceAllocationID]),
    [CostTypeID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationBudgetLineItem_CostType_CostTypeID] FOREIGN KEY REFERENCES [dbo].[CostType]([CostTypeID]),
    [FundSourceAllocationBudgetLineItemAmount] [money] NOT NULL,
    [FundSourceAllocationBudgetLineItemNote] [varchar](max) NULL,
    CONSTRAINT [AK_FundSourceAllocationBudgetLineItem_FundSourceAllocationID_CostTypeID] UNIQUE ([FundSourceAllocationID], [CostTypeID])
)
GO