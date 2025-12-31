CREATE TABLE [dbo].[FundSourceAllocationProgramManager](
	[FundSourceAllocationProgramManagerID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceAllocationProgramManager_FundSourceAllocationProgramManagerID] PRIMARY KEY,
	[FundSourceAllocationID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationProgramManager_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY REFERENCES [dbo].[FundSourceAllocation]([FundSourceAllocationID]),
	[PersonID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationProgramManager_Person_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID])
)
GO