SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaxonomyBranch](
	[TaxonomyBranchID] [int] IDENTITY(1,1) NOT NULL,
	[TaxonomyTrunkID] [int] NOT NULL,
	[TaxonomyBranchName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[TaxonomyBranchDescription] [dbo].[html] NULL,
	[ThemeColor] [varchar](7) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TaxonomyBranchCode] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TaxonomyBranchSortOrder] [int] NULL,
 CONSTRAINT [PK_TaxonomyBranch_TaxonomyBranchID] PRIMARY KEY CLUSTERED 
(
	[TaxonomyBranchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[TaxonomyBranch]  WITH CHECK ADD  CONSTRAINT [FK_TaxonomyBranch_TaxonomyTrunk_TaxonomyTrunkID] FOREIGN KEY([TaxonomyTrunkID])
REFERENCES [dbo].[TaxonomyTrunk] ([TaxonomyTrunkID])
GO
ALTER TABLE [dbo].[TaxonomyBranch] CHECK CONSTRAINT [FK_TaxonomyBranch_TaxonomyTrunk_TaxonomyTrunkID]