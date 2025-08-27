SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaxonomyTrunk](
	[TaxonomyTrunkID] [int] IDENTITY(1,1) NOT NULL,
	[TaxonomyTrunkName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[TaxonomyTrunkDescription] [dbo].[html] NULL,
	[ThemeColor] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TaxonomyTrunkCode] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TaxonomyTrunkSortOrder] [int] NULL,
 CONSTRAINT [PK_TaxonomyTrunk_TaxonomyTrunkID] PRIMARY KEY CLUSTERED 
(
	[TaxonomyTrunkID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
