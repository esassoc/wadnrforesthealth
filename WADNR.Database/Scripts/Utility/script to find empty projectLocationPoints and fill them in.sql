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
where p.ProjectLocationPoint is null

select * from #tmpA


select p.*, prog.ProgramID, prog.ProgramName, prog.ProgramShortName, prog.OrganizationID, prog.ProgramNotes from dbo.Project as p join dbo.ProjectProgram as pp on pp.ProjectID = p.ProjectID join dbo.Program as prog on pp.ProgramID = prog.ProgramID where p.ProjectID in (select ProjectID from #tmpA)


update dbo.Project
set ProjectLocationPoint = BetterPoint
from #tmpA x
join dbo.Project p on x.ProjectID = p.ProjectID
where p.ProjectLocationPoint is null


/*


select * from dbo.Project as p where p.ProjectLocationPoint is null


select * from dbo.Project as p join dbo.ProjectProgram as pp on p.ProjectID = pp.ProjectID join dbo.Program as prog on pp.ProgramID = prog.ProgramID where p.ProjectLocationPoint is null

*/