if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerCounty'))
	drop view dbo.vGeoServerCounty
go

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
