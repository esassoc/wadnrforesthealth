if exists (select * from dbo.sysobjects where id = object_id('dbo.vGeoServerProjectLocationDetailed'))
	drop view dbo.vGeoServerProjectLocationDetailed
go

create view dbo.vGeoServerProjectLocationDetailed
as
select
	p.ProjectID,
	pl.ProjectLocationID as PrimaryKey,
	p.ProjectName,
	plt.ProjectLocationTypeDisplayName,
	pl.ProjectLocationGeometry,
    po1.OrganizationIDList
from
	dbo.Project as p 
	join dbo.ProjectLocation as pl on p.ProjectID = pl.ProjectID
	join dbo.ProjectLocationType as plt on pl.ProjectLocationTypeID = plt.ProjectLocationTypeID
    left join (
                select po.ProjectID, '|' + STRING_AGG(po.OrganizationID,'|') within group(order by OrganizationID) + '|' as OrganizationIDList
                from (select distinct ProjectID, OrganizationID from dbo.ProjectOrganization) po
                group by po.ProjectID
    ) po1 on p.ProjectID = po1.ProjectID
where pl.ProjectLocationGeometry is not null


/*


select
	p.ProjectID,
	pl.ProjectLocationID as PrimaryKey,
	p.ProjectName,
	plt.ProjectLocationTypeDisplayName,
	pl.ProjectLocationGeometry,
    po1.OrganzationIDList
from
	dbo.Project as p 
	join dbo.ProjectLocation as pl on p.ProjectID = pl.ProjectID
	join dbo.ProjectLocationType as plt on pl.ProjectLocationTypeID = plt.ProjectLocationTypeID
    left join (
                select po.ProjectID, '|' + STRING_AGG(po.OrganizationID,'|') within group(order by OrganizationID) + '|' as OrganzationIDList
                from (select distinct ProjectID, OrganizationID from dbo.ProjectOrganization) po
                group by po.ProjectID
    ) po1 on p.ProjectID = po1.ProjectID
where pl.ProjectLocationGeometry is not null



select
	p.ProjectID,
	pl.ProjectLocationID as PrimaryKey,
	p.ProjectName,
	plt.ProjectLocationTypeDisplayName,
	pl.ProjectLocationGeometry,
	orgTable.OrganizationIdList AS OrganizationIdList
from
	dbo.Project as p 
	join dbo.ProjectLocation as pl on p.ProjectID = pl.ProjectID
	join dbo.ProjectLocationType as plt on pl.ProjectLocationTypeID = plt.ProjectLocationTypeID
	left join (select ProjectID, '|' + STRING_AGG(CONVERT(NVARCHAR(max), OrganizationID), '|') + '|' as OrganizationIdList from dbo.ProjectOrganization group by ProjectID) as orgTable on p.ProjectID = orgTable.ProjectID
where pl.ProjectLocationGeometry is not null

select
	p.ProjectID,
	pl.ProjectLocationID as PrimaryKey,
	p.ProjectName,
	plt.ProjectLocationTypeDisplayName,
	pl.ProjectLocationGeometry,
    po1.OrganzationIDList
from
	dbo.Project as p 
	join dbo.ProjectLocation as pl on p.ProjectID = pl.ProjectID
	join dbo.ProjectLocationType as plt on pl.ProjectLocationTypeID = plt.ProjectLocationTypeID
    left join (
                select po.ProjectID, '|' + STRING_AGG(po.OrganizationID,'|') + '|' as OrganzationIDList
                from (select distinct ProjectID, OrganizationID from dbo.ProjectOrganization) po
                group by po.ProjectID
    ) po1 on p.ProjectID = po1.ProjectID
where pl.ProjectLocationGeometry is not null

*/