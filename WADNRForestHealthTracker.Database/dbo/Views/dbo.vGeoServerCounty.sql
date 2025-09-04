

create view dbo.vGeoServerCounty
as
select
    c.CountyID,
	c.CountyID as PrimaryKey,
	c.CountyName,
	c.CountyFeature,
	c.CountyFeature as Ogr_Geometry
   
from
	dbo.County as c
GO
