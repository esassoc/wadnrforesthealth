SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundSourceAllocationChangeLog](
	[FundSourceAllocationChangeLogID] [int] IDENTITY(1,1) NOT NULL,
	[FundSourceAllocationID] [int] NOT NULL,
	[FundSourceAllocationAmountOldValue] [money] NULL,
	[FundSourceAllocationAmountNewValue] [money] NULL,
	[FundSourceAllocationAmountNote] [nvarchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ChangePersonID] [int] NOT NULL,
	[ChangeDate] [datetime] NOT NULL,
 CONSTRAINT [PK_FundSourceAllocationChangeLog_FundSourceAllocationChangeLogID] PRIMARY KEY CLUSTERED 
(
	[FundSourceAllocationChangeLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[FundSourceAllocationChangeLog]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationChangeLog_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY([FundSourceAllocationID])
REFERENCES [dbo].[FundSourceAllocation] ([FundSourceAllocationID])
GO
ALTER TABLE [dbo].[FundSourceAllocationChangeLog] CHECK CONSTRAINT [FK_FundSourceAllocationChangeLog_FundSourceAllocation_FundSourceAllocationID]
GO
ALTER TABLE [dbo].[FundSourceAllocationChangeLog]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationChangeLog_Person_ChangePersonID_PersonID] FOREIGN KEY([ChangePersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[FundSourceAllocationChangeLog] CHECK CONSTRAINT [FK_FundSourceAllocationChangeLog_Person_ChangePersonID_PersonID]