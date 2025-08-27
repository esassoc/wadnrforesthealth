SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundSourceAllocationPriority](
	[FundSourceAllocationPriorityID] [int] NOT NULL,
	[FundSourceAllocationPriorityNumber] [int] NOT NULL,
	[FundSourceAllocationPriorityColor] [varchar](8) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_FundSourceAllocationPriority_FundSourceAllocationPriorityID] PRIMARY KEY CLUSTERED 
(
	[FundSourceAllocationPriorityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
