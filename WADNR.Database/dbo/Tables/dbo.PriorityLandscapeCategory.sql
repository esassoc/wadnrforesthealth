CREATE TABLE [dbo].[PriorityLandscapeCategory](
    [PriorityLandscapeCategoryID] [int] NOT NULL CONSTRAINT [PK_PriorityLandscapeCategory_PriorityLandscapeCategoryID] PRIMARY KEY,
    [PriorityLandscapeCategoryName] [varchar](100) NOT NULL,
    [PriorityLandscapeCategoryDisplayName] [varchar](100) NOT NULL,
    [PriorityLandscapeCategoryMapLayerColor] [varchar](20) NOT NULL
)
GO
