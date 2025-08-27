SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundSourceFileResource](
	[FundSourceFileResourceID] [int] IDENTITY(1,1) NOT NULL,
	[FundSourceID] [int] NOT NULL,
	[FileResourceID] [int] NOT NULL,
	[DisplayName] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Description] [varchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_FundSourceFileResource_FundSourceFileResourceID] PRIMARY KEY CLUSTERED 
(
	[FundSourceFileResourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_FundSourceFileResource_FundSourceID_FileResourceID] UNIQUE NONCLUSTERED 
(
	[FundSourceID] ASC,
	[FileResourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[FundSourceFileResource]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceFileResource_FileResource_FileResourceID] FOREIGN KEY([FileResourceID])
REFERENCES [dbo].[FileResource] ([FileResourceID])
GO
ALTER TABLE [dbo].[FundSourceFileResource] CHECK CONSTRAINT [FK_FundSourceFileResource_FileResource_FileResourceID]
GO
ALTER TABLE [dbo].[FundSourceFileResource]  WITH CHECK ADD  CONSTRAINT [FK_FundSourceFileResource_FundSource_FundSourceID] FOREIGN KEY([FundSourceID])
REFERENCES [dbo].[FundSource] ([FundSourceID])
GO
ALTER TABLE [dbo].[FundSourceFileResource] CHECK CONSTRAINT [FK_FundSourceFileResource_FundSource_FundSourceID]