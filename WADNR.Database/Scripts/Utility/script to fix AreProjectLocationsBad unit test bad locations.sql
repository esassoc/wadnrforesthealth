drop table if exists #tmpA
    select p.ProjectID,
p.ProjectName, 
p.ProjectLocationPoint.STDistance(l.ProjectLocationGeometry.STCentroid()) as Distance,
p.ProjectLocationPoint, l.ProjectLocationGeometry, l.ProjectLocationGeometry.STCentroid() as BetterPoint
into #tmpA
    from dbo.Project p
join(
    SELECT l.ProjectID, geometry::UnionAggregate(l.ProjectLocationGeometry) as ProjectLocationGeometry
from dbo.vGeoServerProjectLocationDetailed l
    group by l.ProjectID
    ) l on p.ProjectID = l.ProjectID
where p.ProjectLocationPoint.STIntersects(l.ProjectLocationGeometry) = 0
and p.ProjectLocationPoint.STDistance(l.ProjectLocationGeometry.STCentroid())
    > 0.001

update dbo.Project
set ProjectLocationPoint = BetterPoint
from #tmpA x
join dbo.Project p on x.ProjectID = p.ProjectID
where p.CreateGisUploadAttemptID is not null