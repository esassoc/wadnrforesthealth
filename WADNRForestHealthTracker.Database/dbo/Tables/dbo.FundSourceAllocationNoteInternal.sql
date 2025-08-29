CREATE TABLE [dbo].[FundSourceAllocationNoteInternal](
	[FundSourceAllocationNoteInternalID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceAllocationNoteInternal_FundSourceAllocationNoteInternalID] PRIMARY KEY,
	[FundSourceAllocationID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationNoteInternal_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY REFERENCES [dbo].[FundSourceAllocation]([FundSourceAllocationID]),
	[FundSourceAllocationNoteInternalText] [varchar](max) NULL,
	[CreatedByPersonID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationNoteInternal_Person_CreatedByPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
	[CreatedDate] [datetime] NOT NULL,
	[LastUpdatedByPersonID] [int] NULL CONSTRAINT [FK_FundSourceAllocationNoteInternal_Person_LastUpdatedByPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
	[LastUpdatedDate] [datetime] NULL
)
GO