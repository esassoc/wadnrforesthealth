CREATE TABLE [dbo].[ProjectUpdateSection](
    [ProjectUpdateSectionID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectUpdateSection_ProjectUpdateSectionID] PRIMARY KEY,
    [ProjectUpdateSectionName] [varchar](50) NOT NULL CONSTRAINT [AK_ProjectUpdateSection_ProjectUpdateSectionName] UNIQUE,
    [ProjectUpdateSectionDisplayName] [varchar](50) NOT NULL CONSTRAINT [AK_ProjectUpdateSection_ProjectUpdateSectionDisplayName] UNIQUE,
    [SortOrder] [int] NOT NULL,
    [HasCompletionStatus] [bit] NOT NULL,
    [ProjectWorkflowSectionGroupingID] [int] NOT NULL CONSTRAINT [FK_ProjectUpdateSection_ProjectWorkflowSectionGrouping_ProjectWorkflowSectionGroupingID] FOREIGN KEY REFERENCES [dbo].[ProjectWorkflowSectionGrouping]([ProjectWorkflowSectionGroupingID])
)
GO