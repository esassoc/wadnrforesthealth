SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundSourceAllocationBudgetLineItem](
	[FundSourceAllocationBudgetLineItemID] [int] IDENTITY(1,1) NOT NULL,
	[FundSourceAllocationID] [int] NOT NULL,
	[CostTypeID] [int] NOT NULL,
	[FundSourceAllocationBudgetLineItemAmount] [money] NOT NULL,
	[FundSourceAllocationBudgetLineItemNote] [nvarchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_FundSourceAllocationBudgetLineItem_FundSourceAllocationBudgetLineItemID] PRIMARY KEY CLUSTERED 
(
	[FundSourceAllocationBudgetLineItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_FundSourceAllocationBudgetLineItem_FundSourceAllocationID_CostTypeID] UNIQUE NONCLUSTERED 
(
	[FundSourceAllocationID] ASC,
	[CostTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[FundSourceAllocationBudgetLineItem]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationBudgetLineItem_CostType_CostTypeID] FOREIGN KEY([CostTypeID])
REFERENCES [dbo].[CostType] ([CostTypeID])
GO
ALTER TABLE [dbo].[FundSourceAllocationBudgetLineItem] CHECK CONSTRAINT [FK_FundSourceAllocationBudgetLineItem_CostType_CostTypeID]
GO
ALTER TABLE [dbo].[FundSourceAllocationBudgetLineItem]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationBudgetLineItem_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY([FundSourceAllocationID])
REFERENCES [dbo].[FundSourceAllocation] ([FundSourceAllocationID])
GO
ALTER TABLE [dbo].[FundSourceAllocationBudgetLineItem] CHECK CONSTRAINT [FK_FundSourceAllocationBudgetLineItem_FundSourceAllocation_FundSourceAllocationID]