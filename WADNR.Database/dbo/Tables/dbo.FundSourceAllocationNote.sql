CREATE TABLE [dbo].[FundSourceAllocationNote](
    [FundSourceAllocationNoteID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceAllocationNote_FundSourceAllocationNoteID] PRIMARY KEY,
    [FundSourceAllocationID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationNote_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY REFERENCES [dbo].[FundSourceAllocation]([FundSourceAllocationID]),
    [FundSourceAllocationNoteText] [varchar](max) NULL,
    [CreatedByPersonID] [int] NOT NULL CONSTRAINT [FK_FundSourceAllocationNote_Person_CreatedByPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [CreatedDate] [datetime] NOT NULL,
    [LastUpdatedByPersonID] [int] NULL CONSTRAINT [FK_FundSourceAllocationNote_Person_LastUpdatedByPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [LastUpdatedDate] [datetime] NULL
)
GO