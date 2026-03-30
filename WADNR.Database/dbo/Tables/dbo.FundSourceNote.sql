CREATE TABLE [dbo].[FundSourceNote](
    [FundSourceNoteID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceNote_FundSourceNoteID] PRIMARY KEY,
    [FundSourceID] [int] NOT NULL CONSTRAINT [FK_FundSourceNote_FundSource_FundSourceID] FOREIGN KEY REFERENCES [dbo].[FundSource]([FundSourceID]),
    [FundSourceNoteText] [varchar](max) NULL,
    [CreatedByPersonID] [int] NOT NULL CONSTRAINT [FK_FundSourceNote_Person_CreatedByPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [CreatedDate] [datetime] NOT NULL,
    [LastUpdatedByPersonID] [int] NULL CONSTRAINT [FK_FundSourceNote_Person_LastUpdatedByPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [LastUpdatedDate] [datetime] NULL
)
GO