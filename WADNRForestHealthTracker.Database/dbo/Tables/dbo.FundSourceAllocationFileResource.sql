SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundSourceAllocationFileResource](
	[FundSourceAllocationFileResourceID] [int] IDENTITY(1,1) NOT NULL,
	[FundSourceAllocationID] [int] NOT NULL,
	[FileResourceID] [int] NOT NULL,
	[DisplayName] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_FundSourceAllocationFileResource_FundSourceAllocationFileResourceID] PRIMARY KEY CLUSTERED 
(
	[FundSourceAllocationFileResourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_FundSourceAllocationFileResource_FundSourceAllocationID_FileResourceID] UNIQUE NONCLUSTERED 
(
	[FundSourceAllocationID] ASC,
	[FileResourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[FundSourceAllocationFileResource]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationFileResource_FileResource_FileResourceID] FOREIGN KEY([FileResourceID])
REFERENCES [dbo].[FileResource] ([FileResourceID])
GO
ALTER TABLE [dbo].[FundSourceAllocationFileResource] CHECK CONSTRAINT [FK_FundSourceAllocationFileResource_FileResource_FileResourceID]
GO
ALTER TABLE [dbo].[FundSourceAllocationFileResource]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceAllocationFileResource_FundSourceAllocation_FundSourceAllocationID] FOREIGN KEY([FundSourceAllocationID])
REFERENCES [dbo].[FundSourceAllocation] ([FundSourceAllocationID])
GO
ALTER TABLE [dbo].[FundSourceAllocationFileResource] CHECK CONSTRAINT [FK_FundSourceAllocationFileResource_FundSourceAllocation_FundSourceAllocationID]