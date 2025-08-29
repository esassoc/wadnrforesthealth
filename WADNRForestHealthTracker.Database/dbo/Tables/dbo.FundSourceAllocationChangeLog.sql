CREATE TABLE [dbo].[FundSourceAllocationChangeLog](
    [FundSourceAllocationChangeLogID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceAllocationChangeLog_FundSourceAllocationChangeLogID] PRIMARY KEY,
    [FundSourceAllocationID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationChangeLog_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY REFERENCES [dbo].[FundSourceAllocation]([FundSourceAllocationID]),
    [FundSourceAllocationAmountOldValue] [money] NULL,
    [FundSourceAllocationAmountNewValue] [money] NULL,
    [FundSourceAllocationAmountNote] [varchar](2000) NULL,
    [ChangePersonID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationChangeLog_Person_ChangePersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [ChangeDate] [datetime] NOT NULL
)
GO