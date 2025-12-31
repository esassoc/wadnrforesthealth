CREATE TABLE [dbo].[ProjectCreateSection](
    [ProjectCreateSectionID] [int] NOT NULL CONSTRAINT [PK_ProjectCreateSection_ProjectCreateSectionID] PRIMARY KEY,
    [ProjectCreateSectionName] [varchar](50) NOT NULL CONSTRAINT [AK_ProjectCreateSection_ProjectCreateSectionName] UNIQUE,
    [ProjectCreateSectionDisplayName] [varchar](50) NOT NULL CONSTRAINT [AK_ProjectCreateSection_ProjectCreateSectionDisplayName] UNIQUE,
    [SortOrder] [int] NOT NULL,
    [HasCompletionStatus] [bit] NOT NULL,
    [ProjectWorkflowSectionGroupingID] [int] NOT NULL CONSTRAINT [FK_ProjectCreateSection_ProjectWorkflowSectionGrouping_ProjectWorkflowSectionGroupingID] FOREIGN KEY REFERENCES [dbo].[ProjectWorkflowSectionGrouping]([ProjectWorkflowSectionGroupingID]),
    [IsSectionRequired] [bit] NOT NULL
)
GO