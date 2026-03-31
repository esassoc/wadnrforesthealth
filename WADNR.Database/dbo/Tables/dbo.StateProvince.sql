CREATE TABLE [dbo].[StateProvince](
	[StateProvinceID] [int] NOT NULL CONSTRAINT [PK_StateProvince_StateProvinceID] PRIMARY KEY,
	[StateProvinceAbbreviation] [varchar](2) NOT NULL CONSTRAINT [AK_StateProvince_StateProvinceAbbreviation] UNIQUE,
	[StateProvinceName] [varchar](50) NOT NULL CONSTRAINT [AK_StateProvince_StateProvinceName] UNIQUE,
	[IsBpaRelevant] [bit] NOT NULL,
	[SouthWestLatitude] [decimal](5, 2) NULL,
	[SouthWestLongitude] [decimal](5, 2) NULL,
	[NorthEastLatitude] [decimal](5, 2) NULL,
	[NorthEastLongitude] [decimal](5, 2) NULL,
	[MapObjectID] [int] NULL,
	[StateProvinceFeature] [geometry] NULL,
	[CountryID] [int] NOT NULL CONSTRAINT [FK_StateProvince_Country_CountryID] FOREIGN KEY REFERENCES [dbo].[Country]([CountryID])
)
GO
CREATE SPATIAL INDEX [SPATIAL_StateProvince_StateProvinceFeature] ON [dbo].[StateProvince]
(
	[StateProvinceFeature]
)USING  GEOMETRY_AUTO_GRID 
WITH (BOUNDING_BOX =(-142, 33, -102, 79), 
CELLS_PER_OBJECT = 8)
GO