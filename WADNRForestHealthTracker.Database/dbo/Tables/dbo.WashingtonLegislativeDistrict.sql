SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WashingtonLegislativeDistrict](
	[WashingtonLegislativeDistrictID] [int] IDENTITY(1,1) NOT NULL,
	[WashingtonLegislativeDistrictLocation] [geometry] NOT NULL,
	[WashingtonLegislativeDistrictNumber] [int] NOT NULL,
	[WashingtonLegislativeDistrictName] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_WashingtonLegislativeDistrict_WashingtonLegislativeDistrictID] PRIMARY KEY CLUSTERED 
(
	[WashingtonLegislativeDistrictID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ARITHABORT ON
SET CONCAT_NULL_YIELDS_NULL ON
SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
SET NUMERIC_ROUNDABORT OFF

GO
CREATE SPATIAL INDEX [SPATIAL_WashingtonLegislativeDistrict_WashingtonLegislativeDistrictLocation] ON [dbo].[WashingtonLegislativeDistrict]
(
	[WashingtonLegislativeDistrictLocation]
)USING  GEOMETRY_AUTO_GRID 
WITH (BOUNDING_BOX =(-125, 45, -116, 50), 
CELLS_PER_OBJECT = 8, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]