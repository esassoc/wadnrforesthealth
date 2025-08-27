if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerProjectTreatmentArea'))
	drop view dbo.vGeoServerProjectTreatmentArea
go

create view dbo.vGeoServerProjectTreatmentArea
as
select
	pa.TreatmentID,
	pa.TreatmentID as PrimaryKey,
	pa.ProjectID,
	p.ProjectName,
    p.FhtProjectNumber,
    pl.ProjectLocationGeometry as TreatmentFeature,
	pl.ProjectLocationGeometry as Ogr_Geometry
	
from
	dbo.Treatment as pa
    join dbo.Project p on pa.ProjectID = p.ProjectID
    join dbo.ProjectLocation pl on pa.ProjectLocationID = pl.ProjectLocationID
    where pl.ProjectLocationGeometry is not null



