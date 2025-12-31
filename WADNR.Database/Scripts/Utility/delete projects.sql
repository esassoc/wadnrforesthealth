DROP TABLE IF EXISTS #projectsToDelete 
--SELECT p.ProjectID INTO #projectsToDelete from dbo.Project as p join dbo.ProjectProgram as pp on p.ProjectID = pp.ProjectID where CompletionDate < '2017-01-01' and ProgramID = 1

--select * from #projectsToDelete
select ProjectID INTO #projectsToDelete from dbo.Project where ProjectID in (23156,26205,57236,57270,22350)

delete po from dbo.ProjectOrganization as po where po.ProjectID in (select * from #projectsToDelete)


delete t from dbo.Treatment as t where t.ProjectID in (select * from #projectsToDelete)

delete from dbo.TreatmentUpdate where ProgramID = 1

delete plu from dbo.ProjectLocationUpdate as plu where plu.ProjectLocationID in (select ProjectLocationID from dbo.ProjectLocation where ProjectID in (select * from #projectsToDelete))

delete from dbo.ProjectLocation where ProjectID in (select * from #projectsToDelete)

delete from dbo.ProjectRegion where ProjectID in (select * from #projectsToDelete)

delete from dbo.ProjectCounty where ProjectID in (select * from #projectsToDelete)

delete from dbo.ProjectPriorityLandscape where ProjectID in (select * from #projectsToDelete)

delete from dbo.ProjectProgram where ProjectID in (select * from #projectsToDelete)

delete from dbo.ProjectExternalLink where ProjectID in (select * from #projectsToDelete)

delete from dbo.ProjectImage where ProjectID in (select * from #projectsToDelete)

delete from dbo.ProjectClassification where ProjectID in (select * from #projectsToDelete)

delete from dbo.ProjectUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select * from #projectsToDelete))

delete from dbo.ProjectOrganizationUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select * from #projectsToDelete))

delete from dbo.ProjectUpdateHistory where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select * from #projectsToDelete))

delete from dbo.ProjectRegionUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select * from #projectsToDelete))

delete from dbo.ProjectUpdateProgram where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select * from #projectsToDelete))

delete from dbo.ProjectUpdateBatch where ProjectID in (select * from #projectsToDelete)

delete from dbo.ProjectPerson where ProjectID in (select * from #projectsToDelete)

delete p from dbo.Project as p where p.ProjectID in (select * from #projectsToDelete)