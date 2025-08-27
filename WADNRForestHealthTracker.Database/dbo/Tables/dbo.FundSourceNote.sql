SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundSourceNote](
	[FundSourceNoteID] [int] IDENTITY(1,1) NOT NULL,
	[FundSourceID] [int] NOT NULL,
	[FundSourceNoteText] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CreatedByPersonID] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[LastUpdatedByPersonID] [int] NULL,
	[LastUpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_FundSourceNote_FundSourceNoteID] PRIMARY KEY CLUSTERED 
(
	[FundSourceNoteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[FundSourceNote]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceNote_FundSource_FundSourceID] FOREIGN KEY([FundSourceID])
REFERENCES [dbo].[FundSource] ([FundSourceID])
GO
ALTER TABLE [dbo].[FundSourceNote] CHECK CONSTRAINT [FK_FundSourceNote_FundSource_FundSourceID]
GO
ALTER TABLE [dbo].[FundSourceNote]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceNote_Person_CreatedByPersonID_PersonID] FOREIGN KEY([CreatedByPersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[FundSourceNote] CHECK CONSTRAINT [FK_FundSourceNote_Person_CreatedByPersonID_PersonID]
GO
ALTER TABLE [dbo].[FundSourceNote]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceNote_Person_LastUpdatedByPersonID_PersonID] FOREIGN KEY([LastUpdatedByPersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[FundSourceNote] CHECK CONSTRAINT [FK_FundSourceNote_Person_LastUpdatedByPersonID_PersonID]