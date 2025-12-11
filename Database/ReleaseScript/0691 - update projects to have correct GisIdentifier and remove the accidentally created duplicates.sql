

/*
select 
	*
from
	dbo.Project as p
	join dbo.ProjectProgram as pp on p.ProjectID = pp.ProjectID
where
	pp.ProgramID = 3
	and p.CreateGisUploadAttemptID is not null
	and p.ProjectGisIdentifier is null

*/

--- TK - Need to paste this proc here because there are changes needed when it is called at the end of this release script
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pBulkDeleteProjects'))
    DROP PROCEDURE dbo.pBulkDeleteProjects
GO

CREATE PROCEDURE dbo.pBulkDeleteProjects(@ProjectIDList dbo.IDList readonly)
AS
begin


/*

	1/12/24 TK - If possible please keep tables in alpha order to match unit tests for easy updating and checking
	
	unit tests in questions:
	FlagChangesToForeignKeysReferencingProjectTableForDevelopersToUpdateBulkDeleteProjectsStoredProcedure
	FlagChangesToForeignKeysReferencingProjectUpdateBatchTableForDevelopersToUpdateBulkDeleteProjectsStoredProcedure

*/

--remove references to ProjectUpdateBatch First:
delete from dbo.ProjectCountyUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectDocumentUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
--delete from dbo.ProjectExemptReportingYearUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectExternalLinkUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectFundingSourceUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
--delete from dbo.ProjectFundSourceAllocationExpenditureUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectFundSourceAllocationRequestUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectImageUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
-- 1/12/24 TK - TreatmentUpdate has a reference to ProjectLocationUpdate
delete from dbo.TreatmentUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectLocationStagingUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectLocationUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectNoteUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectOrganizationUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectPersonUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectPriorityLandscapeUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectRegionUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectUpdateHistory where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectUpdateProgram where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))


--remove references to Project second:
delete from dbo.AgreementProject where ProjectID in (select ID from @ProjectIDList)
delete from dbo.InteractionEventProject where ProjectID in (select ID from @ProjectIDList)
delete from dbo.Invoice where InvoicePaymentRequestID in (select InvoicePaymentRequestID from dbo.InvoicePaymentRequest where ProjectID in (select ID from @ProjectIDList))
delete from dbo.InvoicePaymentRequest where ProjectID in (select ID from @ProjectIDList)
delete from dbo.NotificationProject where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProgramNotificationSentProject where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectClassification where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectCounty where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectDocument where ProjectID in (select ID from @ProjectIDList)
--delete from dbo.ProjectExemptReportingYear where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectExternalLink where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectFundingSource where ProjectID in (select ID from @ProjectIDList)
--delete from dbo.ProjectFundSourceAllocationExpenditure where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectFundSourceAllocationRequest where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectImage where ProjectID in (select ID from @ProjectIDList)
-- 1/12/24 TK - this is just setting the ID to null because we want to preserve the rest of the block list data and not delete the whole record, just remove the connection to the deleted projects
update dbo.ProjectImportBlockList set ProjectID = null where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectInternalNote where ProjectID in (select ID from @ProjectIDList)
-- 1/12/24 TK - Treatment has a reference to ProjectLocation
delete from dbo.Treatment where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectLocation where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectLocationStaging where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectNote where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectOrganization where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectPerson where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectPriorityLandscape where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectProgram where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectRegion where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectTag where ProjectID in (select ID from @ProjectIDList)




-- LAST TWO!
delete from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList)
delete p from dbo.Project as p where p.ProjectID in (select ID from @ProjectIDList)

end
go





	if object_id('tempdb.dbo.#tmpProjectNames') is not null drop table #tmpProjectNames
    select p.ProjectName
    into #tmpProjectNames
    from
		dbo.Project as p
		join dbo.ProjectProgram as pp on p.ProjectID = pp.ProjectID
	where
		pp.ProgramID = 3
		and p.CreateGisUploadAttemptID is not null
		and p.ProjectGisIdentifier is null


		--select * from #tmpProjectNames


if object_id('tempdb.dbo.#tmpProjectsToDelete') is not null drop table #tmpProjectsToDelete
    select p.ProjectID
    into #tmpProjectsToDelete
    from
		dbo.Project as p
	where
		p.ProjectName in (select ProjectName from #tmpProjectNames)
		and p.ProjectGisIdentifier is not null


--select * from #tmpProjectsToDelete

update dbo.Project set ProjectGisIdentifier = ProjectName
where ProjectName in (select ProjectName from #tmpProjectNames)
		and ProjectGisIdentifier is null


		--select * from dbo.Project where ProjectName in (select ProjectName from #tmpProjectNames)

DECLARE @projectToDeleteIdList AS dbo.IDList

INSERT INTO @projectToDeleteIdList (ID)
select ProjectID from #tmpProjectsToDelete

	exec dbo.pBulkDeleteProjects @projectToDeleteIdList


/*
select *
    
    from
		dbo.Project as p
		join dbo.ProjectProgram as pp on p.ProjectID = pp.ProjectID
	where
		pp.ProgramID = 3
		--and p.CreateGisUploadAttemptID is not null
		and p.ProjectGisIdentifier is null

*/