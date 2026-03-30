CREATE TABLE [dbo].[FundSourceAllocationProgramIndexProjectCode](
    [FundSourceAllocationProgramIndexProjectCodeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceAllocationProgramIndexProjectCode_FundSourceAllocationProgramIndexProjectCodeID] PRIMARY KEY,
    [FundSourceAllocationID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationProgramIndexProjectCode_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY REFERENCES [dbo].[FundSourceAllocation]([FundSourceAllocationID]),
    [ProgramIndexID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationProgramIndexProjectCode_ProgramIndex_ProgramIndexID] FOREIGN KEY REFERENCES [dbo].[ProgramIndex]([ProgramIndexID]),
    [ProjectCodeID] [int] NULL CONSTRAINT [FK_FundSourceAllocationProgramIndexProjectCode_ProjectCode_ProjectCodeID] FOREIGN KEY REFERENCES [dbo].[ProjectCode]([ProjectCodeID]),
    CONSTRAINT [AK_FundSourceAllocationProgramIndexProjectCode_FundSourceAllocationID_ProgramIndexID_ProjectCodeID] UNIQUE ([FundSourceAllocationID], [ProgramIndexID], [ProjectCodeID])
)
GO