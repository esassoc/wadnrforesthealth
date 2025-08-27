SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GisUploadAttemptWorkflowSection](
	[GisUploadAttemptWorkflowSectionID] [int] NOT NULL,
	[GisUploadAttemptWorkflowSectionName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[GisUploadAttemptWorkflowSectionDisplayName] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[SortOrder] [int] NOT NULL,
	[HasCompletionStatus] [bit] NOT NULL,
	[GisUploadAttemptWorkflowSectionGroupingID] [int] NOT NULL,
 CONSTRAINT [PK_GisUploadAttemptWorkflowSection_GisUploadAttemptWorkflowSectionID] PRIMARY KEY CLUSTERED 
(
	[GisUploadAttemptWorkflowSectionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_GisUploadAttemptWorkflowSection_GisUploadAttemptWorkflowSectionDisplayName] UNIQUE NONCLUSTERED 
(
	[GisUploadAttemptWorkflowSectionDisplayName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_GisUploadAttemptWorkflowSection_GisUploadAttemptWorkflowSectionName] UNIQUE NONCLUSTERED 
(
	[GisUploadAttemptWorkflowSectionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[GisUploadAttemptWorkflowSection]  WITH CHECK ADD  CONSTRAINT [FK_GisUploadAttemptWorkflowSection_GisUploadAttemptWorkflowSectionGrouping_GisUploadAttemptWorkflowSectionGroupingID] FOREIGN KEY([GisUploadAttemptWorkflowSectionGroupingID])
REFERENCES [dbo].[GisUploadAttemptWorkflowSectionGrouping] ([GisUploadAttemptWorkflowSectionGroupingID])
GO
ALTER TABLE [dbo].[GisUploadAttemptWorkflowSection] CHECK CONSTRAINT [FK_GisUploadAttemptWorkflowSection_GisUploadAttemptWorkflowSectionGrouping_GisUploadAttemptWorkflowSectionGroupingID]