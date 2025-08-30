CREATE TABLE [dbo].[ProjectLocation](
    [ProjectLocationID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectLocation_ProjectLocationID] PRIMARY KEY,
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectLocation_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [ProjectLocationGeometry] [geometry] NOT NULL,
    [ProjectLocationNotes] [varchar](255) NULL,
    [ProjectLocationTypeID] [int] NOT NULL CONSTRAINT [FK_ProjectLocation_ProjectLocationType_ProjectLocationTypeID] FOREIGN KEY REFERENCES [dbo].[ProjectLocationType]([ProjectLocationTypeID]),
    [ProjectLocationName] [varchar](100) NOT NULL,
    [ArcGisObjectID] [int] NULL,
    [ArcGisGlobalID] [varchar](50) NULL,
    [ProgramID] [int] NULL CONSTRAINT [FK_ProjectLocation_Program_ProgramID] FOREIGN KEY REFERENCES [dbo].[Program]([ProgramID]),
    [ImportedFromGisUpload] [bit] NULL,
    [TemporaryTreatmentCacheID] [int] NULL,
    CONSTRAINT [AK_ProjectLocation_ProjectID_ProjectLocationName] UNIQUE ([ProjectID], [ProjectLocationName])
)
GO

CREATE SPATIAL INDEX [SPATIAL_ProjectLocation_ProjectLocationGeometry] ON [dbo].[ProjectLocation]
(
	[ProjectLocationGeometry]
)USING  GEOMETRY_AUTO_GRID 
WITH (BOUNDING_BOX =(-125, 45, -117, 50), 
CELLS_PER_OBJECT = 8, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]