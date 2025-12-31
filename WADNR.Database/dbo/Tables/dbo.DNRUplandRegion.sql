CREATE TABLE [dbo].[DNRUplandRegion](
    [DNRUplandRegionID] [int] NOT NULL CONSTRAINT PK_DNRUplandRegion_DNRUplandRegionID PRIMARY KEY,
    [DNRUplandRegionAbbrev] [varchar](10) NULL,
    [DNRUplandRegionName] [varchar](100) NOT NULL CONSTRAINT AK_DNRUplandRegion_DNRUplandRegionName UNIQUE,
    [DNRUplandRegionLocation] [geometry] NULL,
    [RegionAddress] [varchar](255) NULL,
    [RegionCity] [varchar](30) NULL,
    [RegionState] [varchar](30) NULL,
    [RegionZip] [varchar](10) NULL,
    [RegionPhone] [varchar](30) NULL,
    [RegionEmail] [varchar](255) NULL,
    [DNRUplandRegionCoordinatorID] [int] NULL CONSTRAINT FK_DNRUplandRegion_Person_DNRUplandRegionCoordinatorID_PersonID FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [RegionContent] [dbo].[html] NULL
)
GO
--CREATE SPATIAL INDEX SPATIAL_DNRUplandRegion_DNRUplandRegionLocation ON [dbo].[DNRUplandRegion]([DNRUplandRegionLocation]) USING GEOMETRY_AUTO_GRID WITH (BOUNDING_BOX =(-125, 45, -116, 50), CELLS_PER_OBJECT = 8);
--GO