CREATE TABLE [dbo].[ProjectUpdateSection](
    [ProjectUpdateSectionID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectUpdateSection_ProjectUpdateSectionID] PRIMARY KEY,
    [ProjectUpdateID] [int] NOT NULL CONSTRAINT [FK_ProjectUpdateSection_ProjectUpdate_ProjectUpdateID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdate]([ProjectUpdateID]),
    [ProjectWorkflowSectionGroupingID] [int] NOT NULL CONSTRAINT [FK_ProjectUpdateSection_ProjectWorkflowSectionGrouping_ProjectWorkflowSectionGroupingID] FOREIGN KEY REFERENCES [dbo].[ProjectWorkflowSectionGrouping]([ProjectWorkflowSectionGroupingID]),
    [ProjectUpdateSectionContent] [dbo].[html] NULL
)
GO