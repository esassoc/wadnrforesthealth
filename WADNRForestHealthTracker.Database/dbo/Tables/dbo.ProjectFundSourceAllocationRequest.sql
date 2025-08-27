SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectFundSourceAllocationRequest](
	[ProjectFundSourceAllocationRequestID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[FundSourceAllocationID] [int] NOT NULL,
	[TotalAmount] [money] NULL,
	[MatchAmount] [money] NULL,
	[PayAmount] [money] NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NULL,
	[ImportedFromTabularData] [bit] NOT NULL,
 CONSTRAINT [PK_ProjectFundSourceAllocationRequest_ProjectFundSourceAllocationRequestID] PRIMARY KEY CLUSTERED 
(
	[ProjectFundSourceAllocationRequestID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_ProjectFundSourceAllocationRequest_ProjectID_FundSourceAllocationID] UNIQUE NONCLUSTERED 
(
	[ProjectID] ASC,
	[FundSourceAllocationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[ProjectFundSourceAllocationRequest]  WITH CHECK ADD  CONSTRAINT [FK_ProjectFundSourceAllocationRequest_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY([FundSourceAllocationID])
REFERENCES [dbo].[FundSourceAllocation] ([FundSourceAllocationID])
GO
ALTER TABLE [dbo].[ProjectFundSourceAllocationRequest] CHECK CONSTRAINT [FK_ProjectFundSourceAllocationRequest_FundSourceAllocation_FundSourceAllocationID]
GO
ALTER TABLE [dbo].[ProjectFundSourceAllocationRequest]  WITH CHECK ADD  CONSTRAINT [FK_ProjectFundSourceAllocationRequest_Project_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([ProjectID])
GO
ALTER TABLE [dbo].[ProjectFundSourceAllocationRequest] CHECK CONSTRAINT [FK_ProjectFundSourceAllocationRequest_Project_ProjectID]