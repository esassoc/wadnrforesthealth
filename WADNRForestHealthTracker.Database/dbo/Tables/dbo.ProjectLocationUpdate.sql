CREATE TABLE [dbo].[ProjectLocationUpdate](
    [ProjectLocationUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectLocationUpdate_ProjectLocationUpdateID] PRIMARY KEY,
    [ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectLocationUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
    [ProjectLocationUpdateGeometry] [geometry] NOT NULL,
    [ProjectLocationUpdateNotes] [varchar](255) NULL,
    [ProjectLocationTypeID] [int] NOT NULL CONSTRAINT [FK_ProjectLocationUpdate_ProjectLocationType_ProjectLocationTypeID] FOREIGN KEY REFERENCES [dbo].[ProjectLocationType]([ProjectLocationTypeID]),
    [ProjectLocationUpdateName] [varchar](100) NOT NULL,
    [ArcGisObjectID] [int] NULL,
    [ArcGisGlobalID] [varchar](50) NULL,
    [ProjectLocationID] [int] NULL CONSTRAINT [FK_ProjectLocationUpdate_ProjectLocation_ProjectLocationID] FOREIGN KEY REFERENCES [dbo].[ProjectLocation]([ProjectLocationID]),
    CONSTRAINT [AK_ProjectLocationUpdate_ProjectUpdateBatchID_ProjectLocationUpdateName] UNIQUE ([ProjectUpdateBatchID], [ProjectLocationUpdateName])
)
GO
CREATE SPATIAL INDEX [SPATIAL_ProjectLocationUpdate_ProjectLocationUpdateGeometry] ON [dbo].[ProjectLocationUpdate]
(
	[ProjectLocationUpdateGeometry]
)USING  GEOMETRY_AUTO_GRID 
WITH (BOUNDING_BOX =(-124, 46, -117, 49), 
CELLS_PER_OBJECT = 8, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]