
create view dbo.vGeoServerDNRUplandRegion
as
select
	r.DNRUplandRegionID,
	r.DNRUplandRegionID as PrimaryKey,
	r.DNRUplandRegionName,
	r.DNRUplandRegionLocation,
	r.DNRUplandRegionLocation as Ogr_Geometry
	
from
	dbo.DNRUplandRegion as r
