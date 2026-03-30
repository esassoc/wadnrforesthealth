CREATE TABLE [dbo].[ProjectFundSourceAllocationRequestUpdate](
    [ProjectFundSourceAllocationRequestUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectFundSourceAllocationRequestUpdate_ProjectFundSourceAllocationRequestUpdateID] PRIMARY KEY,
    [ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectFundSourceAllocationRequestUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
    [FundSourceAllocationID] [int] NOT NULL CONSTRAINT [FK_ProjectFundSourceAllocationRequestUpdate_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY REFERENCES [dbo].[FundSourceAllocation]([FundSourceAllocationID]),
    [TotalAmount] [money] NULL,
    [MatchAmount] [money] NULL,
    [PayAmount] [money] NULL,
    [CreateDate] [datetime] NOT NULL,
    [UpdateDate] [datetime] NULL,
    [ImportedFromTabularData] [bit] NOT NULL,
    CONSTRAINT [AK_ProjectFundSourceAllocationRequestUpdate_ProjectUpdateBatchID_FundSourceAllocationID] UNIQUE ([ProjectUpdateBatchID], [FundSourceAllocationID])
)
GO