CREATE TABLE [dbo].[County](
    [CountyName] [varchar](100) NOT NULL,
    [StateProvinceID] [int] NOT NULL CONSTRAINT FK_County_StateProvince_StateProvinceID FOREIGN KEY REFERENCES [dbo].[StateProvince]([StateProvinceID]),
    [CountyFeature] [geometry] NULL,
    [CountyID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT PK_County_CountyID PRIMARY KEY,
    CONSTRAINT AK_County_CountyName_StateProvinceID UNIQUE ([CountyName], [StateProvinceID])
)
GO
--CREATE SPATIAL INDEX SPATIAL_County_CountyFeature ON [dbo].[County]([CountyFeature]) USING GEOMETRY_AUTO_GRID WITH (BOUNDING_BOX =(-125, 45, -116, 50), CELLS_PER_OBJECT = 8);
--GO