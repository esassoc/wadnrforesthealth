CREATE TABLE [dbo].[WashingtonLegislativeDistrict](
    [WashingtonLegislativeDistrictID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WashingtonLegislativeDistrict_WashingtonLegislativeDistrictID] PRIMARY KEY,
    [WashingtonLegislativeDistrictLocation] [geometry] NOT NULL,
    [WashingtonLegislativeDistrictNumber] [int] NOT NULL,
    [WashingtonLegislativeDistrictName] [varchar](200) NOT NULL
)
GO
CREATE SPATIAL INDEX [SPATIAL_WashingtonLegislativeDistrict_WashingtonLegislativeDistrictLocation] ON [dbo].[WashingtonLegislativeDistrict]
(
    [WashingtonLegislativeDistrictLocation]
)USING GEOMETRY_AUTO_GRID 
WITH (BOUNDING_BOX =(-125, 45, -116, 50), CELLS_PER_OBJECT = 8) 
GO