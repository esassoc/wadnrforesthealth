CREATE TABLE [dbo].[ProjectWorkflowSectionGrouping](
    [ProjectWorkflowSectionGroupingID] [int] NOT NULL CONSTRAINT [PK_ProjectWorkflowSectionGrouping_ProjectWorkflowSectionGroupingID] PRIMARY KEY,
    [ProjectWorkflowSectionGroupingName] [varchar](50) NOT NULL CONSTRAINT [AK_ProjectWorkflowSectionGrouping_ProjectWorkflowSectionGroupingName] UNIQUE,
    [ProjectWorkflowSectionGroupingDisplayName] [varchar](50) NOT NULL CONSTRAINT [AK_ProjectWorkflowSectionGrouping_ProjectWorkflowSectionGroupingDisplayName] UNIQUE,
    [SortOrder] [int] NOT NULL
)
GO
