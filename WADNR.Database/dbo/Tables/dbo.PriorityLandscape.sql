CREATE TABLE [dbo].[PriorityLandscape](
	[PriorityLandscapeID] [int] NOT NULL CONSTRAINT [PK_PriorityLandscape_PriorityLandscapeID] PRIMARY KEY,
	[PriorityLandscapeName] [varchar](100) NOT NULL CONSTRAINT [AK_PriorityLandscape_PriorityLandscapeName] UNIQUE,
	[PriorityLandscapeLocation] [geometry] NULL,
	[PriorityLandscapeDescription] [dbo].[html] NULL,
	[PlanYear] [int] NULL,
	[PriorityLandscapeAboveMapText] [varchar](2000) NULL,
	[PriorityLandscapeCategoryID] [int] NULL CONSTRAINT [FK_PriorityLandscape_PriorityLandscapeCategory_PriorityLandscapeCategoryID] FOREIGN KEY REFERENCES [dbo].[PriorityLandscapeCategory]([PriorityLandscapeCategoryID]),
	[PriorityLandscapeExternalResources] [dbo].[html] NULL
)
GO
CREATE SPATIAL INDEX [SPATIAL_PriorityLandscape_PriorityLandscapeLocation] ON [dbo].[PriorityLandscape]([PriorityLandscapeLocation]) USING GEOMETRY_AUTO_GRID WITH (BOUNDING_BOX =(-125, 45, -117, 50), CELLS_PER_OBJECT = 8)
GO