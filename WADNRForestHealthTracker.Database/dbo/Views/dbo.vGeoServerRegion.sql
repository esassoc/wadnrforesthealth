if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerDnrUplandRegion'))
	drop view dbo.vGeoServerDnrUplandRegion
go

create view dbo.vGeoServerDNRUplandRegion
as
select
	r.DNRUplandRegionID,
	r.DNRUplandRegionID as PrimaryKey,
	r.DNRUplandRegionName,
	r.DNRUplandRegionLocation,
	r.DNRUplandRegionLocation as Ogr_Geometry
	
from
	dbo.DnrUplandRegion as r
