SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundSourceAllocationProgramIndexProjectCode](
	[FundSourceAllocationProgramIndexProjectCodeID] [int] IDENTITY(1,1) NOT NULL,
	[FundSourceAllocationID] [int] NOT NULL,
	[ProgramIndexID] [int] NOT NULL,
	[ProjectCodeID] [int] NULL,
 CONSTRAINT [PK_FundSourceAllocationProgramIndexProjectCode_FundSourceAllocationProgramIndexProjectCodeID] PRIMARY KEY CLUSTERED 
(
	[FundSourceAllocationProgramIndexProjectCodeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_FundSourceAllocationProgramIndexProjectCode_FundSourceAllocationID_ProgramIndexID_ProjectCodeID] UNIQUE NONCLUSTERED 
(
	[FundSourceAllocationID] ASC,
	[ProgramIndexID] ASC,
	[ProjectCodeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[FundSourceAllocationProgramIndexProjectCode]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationProgramIndexProjectCode_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY([FundSourceAllocationID])
REFERENCES [dbo].[FundSourceAllocation] ([FundSourceAllocationID])
GO
ALTER TABLE [dbo].[FundSourceAllocationProgramIndexProjectCode] CHECK CONSTRAINT [FK_FundSourceAllocationProgramIndexProjectCode_FundSourceAllocation_FundSourceAllocationID]
GO
ALTER TABLE [dbo].[FundSourceAllocationProgramIndexProjectCode]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationProgramIndexProjectCode_ProgramIndex_ProgramIndexID] FOREIGN KEY([ProgramIndexID])
REFERENCES [dbo].[ProgramIndex] ([ProgramIndexID])
GO
ALTER TABLE [dbo].[FundSourceAllocationProgramIndexProjectCode] CHECK CONSTRAINT [FK_FundSourceAllocationProgramIndexProjectCode_ProgramIndex_ProgramIndexID]
GO
ALTER TABLE [dbo].[FundSourceAllocationProgramIndexProjectCode]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationProgramIndexProjectCode_ProjectCode_ProjectCodeID] FOREIGN KEY([ProjectCodeID])
REFERENCES [dbo].[ProjectCode] ([ProjectCodeID])
GO
ALTER TABLE [dbo].[FundSourceAllocationProgramIndexProjectCode] CHECK CONSTRAINT [FK_FundSourceAllocationProgramIndexProjectCode_ProjectCode_ProjectCodeID]