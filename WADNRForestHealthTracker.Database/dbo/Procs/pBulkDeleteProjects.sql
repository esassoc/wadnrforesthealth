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
delete from dbo.ProjectExemptReportingYearUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectExternalLinkUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectFundingSourceUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
delete from dbo.ProjectFundSourceAllocationExpenditureUpdate where ProjectUpdateBatchID in (select ProjectUpdateBatchID from dbo.ProjectUpdateBatch where ProjectID in (select ID from @ProjectIDList))
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
delete from dbo.ProjectExemptReportingYear where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectExternalLink where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectFundingSource where ProjectID in (select ID from @ProjectIDList)
delete from dbo.ProjectFundSourceAllocationExpenditure where ProjectID in (select ID from @ProjectIDList)
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

/*

exec dbo.pBulkDeleteProjects
ystem.Data.SqlClient.SqlException (0x80131904): The DELETE statement conflicted with the REFERENCE constraint "FK_ProjectLocationUpdate_ProjectUpdateBatch_ProjectUpdateBatchID". The conflict occurred in database "WADNRForestHealthDB", table "dbo.ProjectLocationUpdate", column 'ProjectUpdateBatchID'.
The DELETE statement conflicted with the REFERENCE constraint "FK_ProjectUpdateBatch_Project_ProjectID". The conflict occurred in database "WADNRForestHealthDB", table "dbo.ProjectUpdateBatch", column 'ProjectID'.
The statement has been terminated.
The statement has been terminated.

*/