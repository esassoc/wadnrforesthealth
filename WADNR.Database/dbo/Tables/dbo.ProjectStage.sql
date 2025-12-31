CREATE TABLE [dbo].[ProjectStage](
    [ProjectStageID] [int] NOT NULL CONSTRAINT [PK_ProjectStage_ProjectStageID] PRIMARY KEY,
    [ProjectStageName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectStage_ProjectStageName] UNIQUE,
    [ProjectStageDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectStage_ProjectStageDisplayName] UNIQUE,
    [SortOrder] [int] NOT NULL,
    [ProjectStageColor] [varchar](20) NOT NULL
)
GO
