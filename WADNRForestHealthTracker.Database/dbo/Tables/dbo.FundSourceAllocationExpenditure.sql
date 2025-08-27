SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundSourceAllocationExpenditure](
	[FundSourceAllocationExpenditureID] [int] IDENTITY(1,1) NOT NULL,
	[FundSourceAllocationID] [int] NOT NULL,
	[CostTypeID] [int] NULL,
	[Biennium] [int] NOT NULL,
	[FiscalMonth] [int] NOT NULL,
	[CalendarYear] [int] NOT NULL,
	[CalendarMonth] [int] NOT NULL,
	[ExpenditureAmount] [money] NOT NULL,
 CONSTRAINT [PK_FundSourceAllocationExpenditure_FundSourceAllocationExpenditureID] PRIMARY KEY CLUSTERED 
(
	[FundSourceAllocationExpenditureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[FundSourceAllocationExpenditure]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationExpenditure_CostType_CostTypeID] FOREIGN KEY([CostTypeID])
REFERENCES [dbo].[CostType] ([CostTypeID])
GO
ALTER TABLE [dbo].[FundSourceAllocationExpenditure] CHECK CONSTRAINT [FK_FundSourceAllocationExpenditure_CostType_CostTypeID]
GO
ALTER TABLE [dbo].[FundSourceAllocationExpenditure]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationExpenditure_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY([FundSourceAllocationID])
REFERENCES [dbo].[FundSourceAllocation] ([FundSourceAllocationID])
GO
ALTER TABLE [dbo].[FundSourceAllocationExpenditure] CHECK CONSTRAINT [FK_FundSourceAllocationExpenditure_FundSourceAllocation_FundSourceAllocationID]