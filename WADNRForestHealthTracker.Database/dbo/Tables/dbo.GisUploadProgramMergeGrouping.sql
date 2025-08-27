SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GisUploadProgramMergeGrouping](
	[GisUploadProgramMergeGroupingID] [int] IDENTITY(1,1) NOT NULL,
	[GisUploadProgramMergeGroupingName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_GisUploadProgramMergeGrouping_GisUploadProgramMergeGroupingID] PRIMARY KEY CLUSTERED 
(
	[GisUploadProgramMergeGroupingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
