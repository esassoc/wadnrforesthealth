CREATE TABLE [dbo].[GisUploadAttemptWorkflowSectionGrouping](
	[GisUploadAttemptWorkflowSectionGroupingID] [int] NOT NULL CONSTRAINT [PK_GisUploadAttemptWorkflowSectionGrouping_GisUploadAttemptWorkflowSectionGroupingID] PRIMARY KEY,
	[GisUploadAttemptWorkflowSectionGroupingName] [varchar](50) NOT NULL,
	[GisUploadAttemptWorkflowSectionGroupingDisplayName] [varchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [AK_GisUploadAttemptWorkflowSectionGrouping_GisUploadAttemptWorkflowSectionGroupingDisplayName] UNIQUE ([GisUploadAttemptWorkflowSectionGroupingDisplayName]),
 CONSTRAINT [AK_GisUploadAttemptWorkflowSectionGrouping_GisUploadAttemptWorkflowSectionGroupingName] UNIQUE ([GisUploadAttemptWorkflowSectionGroupingName])
)
GO
