CREATE TABLE [dbo].[ProjectFundSourceAllocationRequest](
    [ProjectFundSourceAllocationRequestID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectFundSourceAllocationRequest_ProjectFundSourceAllocationRequestID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectFundSourceAllocationRequest_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [FundSourceAllocationID] [int] NOT NULL CONSTRAINT [FK_ProjectFundSourceAllocationRequest_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY REFERENCES [dbo].[FundSourceAllocation]([FundSourceAllocationID]),
    [TotalAmount] [money] NULL,
    [MatchAmount] [money] NULL,
    [PayAmount] [money] NULL,
    [CreateDate] [datetime] NOT NULL,
    [UpdateDate] [datetime] NULL,
    [ImportedFromTabularData] [bit] NOT NULL,
    CONSTRAINT [AK_ProjectFundSourceAllocationRequest_ProjectID_FundSourceAllocationID] UNIQUE ([ProjectID], [FundSourceAllocationID])
)
GO