SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundSourceAllocationSource](
	[FundSourceAllocationSourceID] [int] NOT NULL,
	[FundSourceAllocationSourceName] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[FundSourceAllocationSourceDisplayName] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_FundSourceAllocationSource_FundSourceAllocationSourceID] PRIMARY KEY CLUSTERED 
(
	[FundSourceAllocationSourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
