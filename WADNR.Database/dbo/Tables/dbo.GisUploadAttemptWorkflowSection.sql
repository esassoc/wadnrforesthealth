CREATE TABLE [dbo].[GisUploadAttemptWorkflowSection](
    [GisUploadAttemptWorkflowSectionID] [int] NOT NULL CONSTRAINT [PK_GisUploadAttemptWorkflowSection_GisUploadAttemptWorkflowSectionID] PRIMARY KEY,
    [GisUploadAttemptWorkflowSectionName] [varchar](50) NOT NULL,
    [GisUploadAttemptWorkflowSectionDisplayName] [varchar](50) NOT NULL,
    [SortOrder] [int] NOT NULL,
    [HasCompletionStatus] [bit] NOT NULL,
    [GisUploadAttemptWorkflowSectionGroupingID] [int] NOT NULL CONSTRAINT [FK_GisUploadAttemptWorkflowSection_GisUploadAttemptWorkflowSectionGrouping_GisUploadAttemptWorkflowSectionGroupingID] FOREIGN KEY REFERENCES [dbo].[GisUploadAttemptWorkflowSectionGrouping]([GisUploadAttemptWorkflowSectionGroupingID]),
    CONSTRAINT [AK_GisUploadAttemptWorkflowSection_GisUploadAttemptWorkflowSectionDisplayName] UNIQUE ([GisUploadAttemptWorkflowSectionDisplayName]),
    CONSTRAINT [AK_GisUploadAttemptWorkflowSection_GisUploadAttemptWorkflowSectionName] UNIQUE ([GisUploadAttemptWorkflowSectionName])
)
GO