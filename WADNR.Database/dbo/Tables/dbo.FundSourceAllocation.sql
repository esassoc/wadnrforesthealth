CREATE TABLE [dbo].[FundSourceAllocation](
    [FundSourceAllocationID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceAllocation_FundSourceAllocationID] PRIMARY KEY,
    [FundSourceAllocationName] [varchar](100) NULL,
    [StartDate] [datetime] NULL,
    [EndDate] [datetime] NULL,
    [AllocationAmount] [money] NULL,
    [FederalFundCodeID] [int] NULL CONSTRAINT [FK_FundSourceAllocation_FederalFundCode_FederalFundCodeID] FOREIGN KEY REFERENCES [dbo].[FederalFundCode]([FederalFundCodeID]),
    [OrganizationID] [int] NULL CONSTRAINT [FK_FundSourceAllocation_Organization_OrganizationID] FOREIGN KEY REFERENCES [dbo].[Organization]([OrganizationID]),
    [DNRUplandRegionID] [int] NULL CONSTRAINT [FK_FundSourceAllocation_DNRUplandRegion_DNRUplandRegionID] FOREIGN KEY REFERENCES [dbo].[DNRUplandRegion]([DNRUplandRegionID]),
    [DivisionID] [int] NULL CONSTRAINT [FK_FundSourceAllocation_Division_DivisionID] FOREIGN KEY REFERENCES [dbo].[Division]([DivisionID]),
    [FundSourceManagerID] [int] NULL CONSTRAINT [FK_FundSourceAllocation_Person_FundSourceManagerID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [FundSourceAllocationPriorityID] [int] NULL CONSTRAINT [FK_FundSourceAllocation_FundSourceAllocationPriority_FundSourceAllocationPriorityID] FOREIGN KEY REFERENCES [dbo].[FundSourceAllocationPriority]([FundSourceAllocationPriorityID]),
    [HasFundFSPs] [bit] NULL,
    [FundSourceAllocationSourceID] [int] NULL CONSTRAINT [FK_FundSourceAllocation_FundSourceAllocationSource_FundSourceAllocationSourceID] FOREIGN KEY REFERENCES [dbo].[FundSourceAllocationSource]([FundSourceAllocationSourceID]),
    [LikelyToUse] [bit] NULL,
    [FundSourceID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocation_FundSource_FundSourceID] FOREIGN KEY REFERENCES [dbo].[FundSource]([FundSourceID])
)
GO