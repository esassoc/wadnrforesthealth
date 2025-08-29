CREATE TABLE [dbo].[FundSourceNoteInternal](
	[FundSourceNoteInternalID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceNoteInternal_FundSourceNoteInternalID] PRIMARY KEY,
	[FundSourceID] [int] NOT NULL CONSTRAINT [FK_FundSourceNoteInternal_FundSource_FundSourceID] FOREIGN KEY REFERENCES [dbo].[FundSource]([FundSourceID]),
	[FundSourceNoteText] [varchar](max) NULL,
	[CreatedByPersonID] [int] NOT NULL CONSTRAINT [FK_FundSourceNoteInternal_Person_CreatedByPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
	[CreatedDate] [datetime] NOT NULL,
	[LastUpdatedByPersonID] [int] NULL CONSTRAINT [FK_FundSourceNoteInternal_Person_LastUpdatedByPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
	[LastUpdatedDate] [datetime] NULL
)
GO