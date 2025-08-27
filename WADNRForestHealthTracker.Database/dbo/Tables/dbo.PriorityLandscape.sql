SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PriorityLandscape](
	[PriorityLandscapeID] [int] NOT NULL,
	[PriorityLandscapeName] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[PriorityLandscapeLocation] [geometry] NULL,
	[PriorityLandscapeDescription] [dbo].[html] NULL,
	[PlanYear] [int] NULL,
	[PriorityLandscapeAboveMapText] [varchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PriorityLandscapeCategoryID] [int] NULL,
	[PriorityLandscapeExternalResources] [dbo].[html] NULL,
 CONSTRAINT [PK_PriorityLandscape_PriorityLandscapeID] PRIMARY KEY CLUSTERED 
(
	[PriorityLandscapeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [AK_PriorityLandscape_PriorityLandscapeName] UNIQUE NONCLUSTERED 
(
	[PriorityLandscapeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[PriorityLandscape]  WITH CHECK ADD  CONSTRAINT [FK_PriorityLandscape_PriorityLandscapeCategory_PriorityLandscapeCategoryID] FOREIGN KEY([PriorityLandscapeCategoryID])
REFERENCES [dbo].[PriorityLandscapeCategory] ([PriorityLandscapeCategoryID])
GO
ALTER TABLE [dbo].[PriorityLandscape] CHECK CONSTRAINT [FK_PriorityLandscape_PriorityLandscapeCategory_PriorityLandscapeCategoryID]
GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF

GO
CREATE SPATIAL INDEX [SPATIAL_PriorityLandscape_PriorityLandscapeLocation] ON [dbo].[PriorityLandscape]
(
	[PriorityLandscapeLocation]
)USING  GEOMETRY_AUTO_GRID 
WITH (BOUNDING_BOX =(-125, 45, -117, 50), 
CELLS_PER_OBJECT = 8, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]