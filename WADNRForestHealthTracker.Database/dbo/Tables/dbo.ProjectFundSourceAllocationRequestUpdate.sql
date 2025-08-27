SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectFundSourceAllocationRequestUpdate](
	[ProjectFundSourceAllocationRequestUpdateID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectUpdateBatchID] [int] NOT NULL,
	[FundSourceAllocationID] [int] NOT NULL,
	[TotalAmount] [money] NULL,
	[MatchAmount] [money] NULL,
	[PayAmount] [money] NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NULL,
	[ImportedFromTabularData] [bit] NOT NULL,
 CONSTRAINT [PK_ProjectFundSourceAllocationRequestUpdate_ProjectFundSourceAllocationRequestUpdateID] PRIMARY KEY CLUSTERED 
(
	[ProjectFundSourceAllocationRequestUpdateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_ProjectFundSourceAllocationRequestUpdate_ProjectUpdateBatchID_FundSourceAllocationID] UNIQUE NONCLUSTERED 
(
	[ProjectUpdateBatchID] ASC,
	[FundSourceAllocationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProjectFundSourceAllocationRequestUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectFundSourceAllocationRequestUpdate_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY([FundSourceAllocationID])
REFERENCES [dbo].[FundSourceAllocation] ([FundSourceAllocationID])
GO
ALTER TABLE [dbo].[ProjectFundSourceAllocationRequestUpdate] CHECK CONSTRAINT [FK_ProjectFundSourceAllocationRequestUpdate_FundSourceAllocation_FundSourceAllocationID]
GO
ALTER TABLE [dbo].[ProjectFundSourceAllocationRequestUpdate]  WITH CHECK ADD  CONSTRAINT [FK_ProjectFundSourceAllocationRequestUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY([ProjectUpdateBatchID])
REFERENCES [dbo].[ProjectUpdateBatch] ([ProjectUpdateBatchID])
GO
ALTER TABLE [dbo].[ProjectFundSourceAllocationRequestUpdate] CHECK CONSTRAINT [FK_ProjectFundSourceAllocationRequestUpdate_ProjectUpdateBatch_ProjectUpdateBatchID]