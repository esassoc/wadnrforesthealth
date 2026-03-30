
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