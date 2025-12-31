CREATE TABLE [dbo].[FundSourceAllocationExpenditure](
    [FundSourceAllocationExpenditureID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceAllocationExpenditure_FundSourceAllocationExpenditureID] PRIMARY KEY,
    [FundSourceAllocationID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationExpenditure_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY REFERENCES [dbo].[FundSourceAllocation]([FundSourceAllocationID]),
    [CostTypeID] [int] NULL CONSTRAINT [FK_FundSourceAllocationExpenditure_CostType_CostTypeID] FOREIGN KEY REFERENCES [dbo].[CostType]([CostTypeID]),
    [Biennium] [int] NOT NULL,
    [FiscalMonth] [int] NOT NULL,
    [CalendarYear] [int] NOT NULL,
    [CalendarMonth] [int] NOT NULL,
    [ExpenditureAmount] [money] NOT NULL
)
GO