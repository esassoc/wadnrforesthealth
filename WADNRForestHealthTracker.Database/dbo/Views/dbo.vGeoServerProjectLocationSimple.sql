if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerProjectLocationSimple'))
	drop view dbo.vGeoServerProjectLocationSimple
go

create view dbo.vGeoServerProjectLocationSimple
as
select
	p.ProjectID,
	p.ProjectID as PrimaryKey,
	p.ProjectName,
	p.ProjectLocationPoint	
from
	dbo.Project as p
where p.ProjectLocationPoint is not null