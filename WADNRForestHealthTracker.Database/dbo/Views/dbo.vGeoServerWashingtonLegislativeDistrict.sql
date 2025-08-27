if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerWashingtonLegislativeDistrict'))
	drop view dbo.vGeoServerWashingtonLegislativeDistrict
go

create view dbo.vGeoServerWashingtonLegislativeDistrict
as
select
    ld.WashingtonLegislativeDistrictID,
	ld.WashingtonLegislativeDistrictID as PrimaryKey,
	ld.WashingtonLegislativeDistrictName,
    ld.WashingtonLegislativeDistrictLocation,
	ld.WashingtonLegislativeDistrictLocation as Ogr_Geometry
	
from
	dbo.WashingtonLegislativeDistrict as ld
GO
