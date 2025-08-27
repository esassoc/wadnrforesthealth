SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundSourceAllocationNoteInternal](
	[FundSourceAllocationNoteInternalID] [int] IDENTITY(1,1) NOT NULL,
	[FundSourceAllocationID] [int] NOT NULL,
	[FundSourceAllocationNoteInternalText] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CreatedByPersonID] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[LastUpdatedByPersonID] [int] NULL,
	[LastUpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_FundSourceAllocationNoteInternal_FundSourceAllocationNoteInternalID] PRIMARY KEY CLUSTERED 
(
	[FundSourceAllocationNoteInternalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[FundSourceAllocationNoteInternal]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationNoteInternal_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY([FundSourceAllocationID])
REFERENCES [dbo].[FundSourceAllocation] ([FundSourceAllocationID])
GO
ALTER TABLE [dbo].[FundSourceAllocationNoteInternal] CHECK CONSTRAINT [FK_FundSourceAllocationNoteInternal_FundSourceAllocation_FundSourceAllocationID]
GO
ALTER TABLE [dbo].[FundSourceAllocationNoteInternal]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationNoteInternal_Person_CreatedByPersonID_PersonID] FOREIGN KEY([CreatedByPersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[FundSourceAllocationNoteInternal] CHECK CONSTRAINT [FK_FundSourceAllocationNoteInternal_Person_CreatedByPersonID_PersonID]
GO
ALTER TABLE [dbo].[FundSourceAllocationNoteInternal]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationNoteInternal_Person_LastUpdatedByPersonID_PersonID] FOREIGN KEY([LastUpdatedByPersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[FundSourceAllocationNoteInternal] CHECK CONSTRAINT [FK_FundSourceAllocationNoteInternal_Person_LastUpdatedByPersonID_PersonID]