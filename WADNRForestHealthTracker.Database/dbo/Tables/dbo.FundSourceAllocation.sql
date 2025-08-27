SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundSourceAllocation](
	[FundSourceAllocationID] [int] IDENTITY(1,1) NOT NULL,
	[FundSourceAllocationName] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[AllocationAmount] [money] NULL,
	[FederalFundCodeID] [int] NULL,
	[OrganizationID] [int] NULL,
	[DNRUplandRegionID] [int] NULL,
	[DivisionID] [int] NULL,
	[FundSourceManagerID] [int] NULL,
	[FundSourceAllocationPriorityID] [int] NULL,
	[HasFundFSPs] [bit] NULL,
	[FundSourceAllocationSourceID] [int] NULL,
	[LikelyToUse] [bit] NULL,
	[FundSourceID] [int] NOT NULL,
 CONSTRAINT [PK_FundSourceAllocation_FundSourceAllocationID] PRIMARY KEY CLUSTERED 
(
	[FundSourceAllocationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[FundSourceAllocation]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocation_Division_DivisionID] FOREIGN KEY([DivisionID])
REFERENCES [dbo].[Division] ([DivisionID])
GO
ALTER TABLE [dbo].[FundSourceAllocation] CHECK CONSTRAINT [FK_FundSourceAllocation_Division_DivisionID]
GO
ALTER TABLE [dbo].[FundSourceAllocation]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocation_DNRUplandRegion_DNRUplandRegionID] FOREIGN KEY([DNRUplandRegionID])
REFERENCES [dbo].[DNRUplandRegion] ([DNRUplandRegionID])
GO
ALTER TABLE [dbo].[FundSourceAllocation] CHECK CONSTRAINT [FK_FundSourceAllocation_DNRUplandRegion_DNRUplandRegionID]
GO
ALTER TABLE [dbo].[FundSourceAllocation]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocation_FederalFundCode_FederalFundCodeID] FOREIGN KEY([FederalFundCodeID])
REFERENCES [dbo].[FederalFundCode] ([FederalFundCodeID])
GO
ALTER TABLE [dbo].[FundSourceAllocation] CHECK CONSTRAINT [FK_FundSourceAllocation_FederalFundCode_FederalFundCodeID]
GO
ALTER TABLE [dbo].[FundSourceAllocation]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocation_FundSource_FundSourceID] FOREIGN KEY([FundSourceID])
REFERENCES [dbo].[FundSource] ([FundSourceID])
GO
ALTER TABLE [dbo].[FundSourceAllocation] CHECK CONSTRAINT [FK_FundSourceAllocation_FundSource_FundSourceID]
GO
ALTER TABLE [dbo].[FundSourceAllocation]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocation_FundSourceAllocationPriority_FundSourceAllocationPriorityID] FOREIGN KEY([FundSourceAllocationPriorityID])
REFERENCES [dbo].[FundSourceAllocationPriority] ([FundSourceAllocationPriorityID])
GO
ALTER TABLE [dbo].[FundSourceAllocation] CHECK CONSTRAINT [FK_FundSourceAllocation_FundSourceAllocationPriority_FundSourceAllocationPriorityID]
GO
ALTER TABLE [dbo].[FundSourceAllocation]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocation_FundSourceAllocationSource_FundSourceAllocationSourceID] FOREIGN KEY([FundSourceAllocationSourceID])
REFERENCES [dbo].[FundSourceAllocationSource] ([FundSourceAllocationSourceID])
GO
ALTER TABLE [dbo].[FundSourceAllocation] CHECK CONSTRAINT [FK_FundSourceAllocation_FundSourceAllocationSource_FundSourceAllocationSourceID]
GO
ALTER TABLE [dbo].[FundSourceAllocation]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocation_Organization_OrganizationID] FOREIGN KEY([OrganizationID])
REFERENCES [dbo].[Organization] ([OrganizationID])
GO
ALTER TABLE [dbo].[FundSourceAllocation] CHECK CONSTRAINT [FK_FundSourceAllocation_Organization_OrganizationID]
GO
ALTER TABLE [dbo].[FundSourceAllocation]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocation_Person_FundSourceManagerID_PersonID] FOREIGN KEY([FundSourceManagerID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[FundSourceAllocation] CHECK CONSTRAINT [FK_FundSourceAllocation_Person_FundSourceManagerID_PersonID]