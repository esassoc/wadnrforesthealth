CREATE PROCEDURE dbo.pStartProjectUpdateBatch
    @ProjectID int,
    @CallingPersonID int
AS
BEGIN
    SET NOCOUNT ON;

    -- Safety guard: project must exist and be Approved (ProjectApprovalStatusID = 3)
    IF NOT EXISTS (SELECT 1 FROM dbo.Project WHERE ProjectID = @ProjectID AND ProjectApprovalStatusID = 3)
        RETURN;

    -- Safety guard: no active (non-Approved) batch already exists
    IF EXISTS (
        SELECT 1 FROM dbo.ProjectUpdateBatch
        WHERE ProjectID = @ProjectID AND ProjectUpdateStateID <> 4 -- 4 = Approved
    )
        RETURN;

    -- 1. Create the batch
    DECLARE @BatchID int;
    DECLARE @Now datetime = GETUTCDATE();

    INSERT INTO dbo.ProjectUpdateBatch (ProjectID, ProjectUpdateStateID, LastUpdateDate, LastUpdatePersonID, IsPhotosUpdated,
        NoPriorityLandscapesExplanation, NoRegionsExplanation, NoCountiesExplanation)
    SELECT @ProjectID, 1, @Now, @CallingPersonID, 0,
        p.NoPriorityLandscapesExplanation, p.NoRegionsExplanation, p.NoCountiesExplanation
    FROM dbo.Project p
    WHERE p.ProjectID = @ProjectID;

    SET @BatchID = SCOPE_IDENTITY();

    -- 2. Copy project basics to ProjectUpdate
    INSERT INTO dbo.ProjectUpdate (ProjectUpdateBatchID, ProjectStageID, ProjectDescription, CompletionDate,
        EstimatedTotalCost, ProjectLocationPoint, ProjectLocationNotes, PlannedDate,
        ProjectLocationSimpleTypeID, FocusAreaID, ExpirationDate, ProjectFundingSourceNotes, PercentageMatch)
    SELECT @BatchID, p.ProjectStageID, p.ProjectDescription, p.CompletionDate,
        p.EstimatedTotalCost, p.ProjectLocationPoint, p.ProjectLocationNotes, p.PlannedDate,
        p.ProjectLocationSimpleTypeID, p.FocusAreaID, p.ExpirationDate, p.ProjectFundingSourceNotes, p.PercentageMatch
    FROM dbo.Project p
    WHERE p.ProjectID = @ProjectID;

    -- 3. Copy programs
    INSERT INTO dbo.ProjectUpdateProgram (ProjectUpdateBatchID, ProgramID)
    SELECT @BatchID, pp.ProgramID
    FROM dbo.ProjectProgram pp
    WHERE pp.ProjectID = @ProjectID;

    -- 4. Copy priority landscapes
    INSERT INTO dbo.ProjectPriorityLandscapeUpdate (ProjectUpdateBatchID, PriorityLandscapeID)
    SELECT @BatchID, pl.PriorityLandscapeID
    FROM dbo.ProjectPriorityLandscape pl
    WHERE pl.ProjectID = @ProjectID;

    -- 5. Copy regions
    INSERT INTO dbo.ProjectRegionUpdate (ProjectUpdateBatchID, DNRUplandRegionID)
    SELECT @BatchID, pr.DNRUplandRegionID
    FROM dbo.ProjectRegion pr
    WHERE pr.ProjectID = @ProjectID;

    -- 6. Copy counties
    INSERT INTO dbo.ProjectCountyUpdate (ProjectUpdateBatchID, CountyID)
    SELECT @BatchID, pc.CountyID
    FROM dbo.ProjectCounty pc
    WHERE pc.ProjectID = @ProjectID;

    -- 7. Copy detailed locations
    INSERT INTO dbo.ProjectLocationUpdate (ProjectUpdateBatchID, ProjectLocationTypeID,
        ProjectLocationUpdateGeometry, ProjectLocationUpdateNotes, ProjectLocationUpdateName,
        ArcGisObjectID, ArcGisGlobalID)
    SELECT @BatchID, loc.ProjectLocationTypeID,
        loc.ProjectLocationGeometry, loc.ProjectLocationNotes, loc.ProjectLocationName,
        loc.ArcGisObjectID, loc.ArcGisGlobalID
    FROM dbo.ProjectLocation loc
    WHERE loc.ProjectID = @ProjectID;

    -- 8. Copy treatments (match to location updates by name + geometry)
    INSERT INTO dbo.TreatmentUpdate (ProjectUpdateBatchID, ProjectLocationUpdateID,
        TreatmentTypeID, TreatmentDetailedActivityTypeID, TreatmentCodeID,
        TreatmentStartDate, TreatmentEndDate, TreatmentFootprintAcres, TreatmentTreatedAcres,
        TreatmentNotes, CostPerAcre, TreatmentTypeImportedText, TreatmentDetailedActivityTypeImportedText,
        ProgramID, ImportedFromGis, CreateGisUploadAttemptID, UpdateGisUploadAttemptID)
    SELECT @BatchID, plu.ProjectLocationUpdateID,
        t.TreatmentTypeID, t.TreatmentDetailedActivityTypeID, t.TreatmentCodeID,
        t.TreatmentStartDate, t.TreatmentEndDate, t.TreatmentFootprintAcres, t.TreatmentTreatedAcres,
        t.TreatmentNotes, t.CostPerAcre, t.TreatmentTypeImportedText, t.TreatmentDetailedActivityTypeImportedText,
        t.ProgramID, t.ImportedFromGis, t.CreateGisUploadAttemptID, t.UpdateGisUploadAttemptID
    FROM dbo.Treatment t
    INNER JOIN dbo.ProjectLocation pl ON t.ProjectLocationID = pl.ProjectLocationID
    LEFT JOIN dbo.ProjectLocationUpdate plu
        ON plu.ProjectUpdateBatchID = @BatchID
        AND plu.ProjectLocationUpdateName = pl.ProjectLocationName
        AND plu.ProjectLocationUpdateGeometry.MakeValid().STEquals(pl.ProjectLocationGeometry.MakeValid()) = 1
    WHERE pl.ProjectID = @ProjectID;

    -- Also copy treatments that have no ProjectLocation (ProjectLocationID IS NULL)
    INSERT INTO dbo.TreatmentUpdate (ProjectUpdateBatchID, ProjectLocationUpdateID,
        TreatmentTypeID, TreatmentDetailedActivityTypeID, TreatmentCodeID,
        TreatmentStartDate, TreatmentEndDate, TreatmentFootprintAcres, TreatmentTreatedAcres,
        TreatmentNotes, CostPerAcre, TreatmentTypeImportedText, TreatmentDetailedActivityTypeImportedText,
        ProgramID, ImportedFromGis, CreateGisUploadAttemptID, UpdateGisUploadAttemptID)
    SELECT @BatchID, NULL,
        t.TreatmentTypeID, t.TreatmentDetailedActivityTypeID, t.TreatmentCodeID,
        t.TreatmentStartDate, t.TreatmentEndDate, t.TreatmentFootprintAcres, t.TreatmentTreatedAcres,
        t.TreatmentNotes, t.CostPerAcre, t.TreatmentTypeImportedText, t.TreatmentDetailedActivityTypeImportedText,
        t.ProgramID, t.ImportedFromGis, t.CreateGisUploadAttemptID, t.UpdateGisUploadAttemptID
    FROM dbo.Treatment t
    WHERE t.ProjectID = @ProjectID AND t.ProjectLocationID IS NULL;

    -- 9. Copy organizations
    INSERT INTO dbo.ProjectOrganizationUpdate (ProjectUpdateBatchID, OrganizationID, RelationshipTypeID)
    SELECT @BatchID, po.OrganizationID, po.RelationshipTypeID
    FROM dbo.ProjectOrganization po
    WHERE po.ProjectID = @ProjectID;

    -- 10. Copy contacts
    INSERT INTO dbo.ProjectPersonUpdate (ProjectUpdateBatchID, PersonID, ProjectPersonRelationshipTypeID)
    SELECT @BatchID, pp.PersonID, pp.ProjectPersonRelationshipTypeID
    FROM dbo.ProjectPerson pp
    WHERE pp.ProjectID = @ProjectID;

    -- 11. Copy funding sources
    INSERT INTO dbo.ProjectFundingSourceUpdate (ProjectUpdateBatchID, FundingSourceID)
    SELECT @BatchID, pfs.FundingSourceID
    FROM dbo.ProjectFundingSource pfs
    WHERE pfs.ProjectID = @ProjectID;

    -- 12. Copy allocation requests
    INSERT INTO dbo.ProjectFundSourceAllocationRequestUpdate (ProjectUpdateBatchID, FundSourceAllocationID,
        TotalAmount, MatchAmount, PayAmount, CreateDate, UpdateDate, ImportedFromTabularData)
    SELECT @BatchID, ar.FundSourceAllocationID,
        ar.TotalAmount, ar.MatchAmount, ar.PayAmount, ar.CreateDate, ar.UpdateDate, ar.ImportedFromTabularData
    FROM dbo.ProjectFundSourceAllocationRequest ar
    WHERE ar.ProjectID = @ProjectID;

    -- 13. Copy images (share FileResourceID, don't duplicate blob)
    INSERT INTO dbo.ProjectImageUpdate (ProjectUpdateBatchID, FileResourceID, ProjectImageID,
        Caption, Credit, IsKeyPhoto, ExcludeFromFactSheet, ProjectImageTimingID)
    SELECT @BatchID, img.FileResourceID, img.ProjectImageID,
        img.Caption, img.Credit, img.IsKeyPhoto, img.ExcludeFromFactSheet, img.ProjectImageTimingID
    FROM dbo.ProjectImage img
    WHERE img.ProjectID = @ProjectID;

    -- 14. Copy external links
    INSERT INTO dbo.ProjectExternalLinkUpdate (ProjectUpdateBatchID, ExternalLinkLabel, ExternalLinkUrl)
    SELECT @BatchID, link.ExternalLinkLabel, link.ExternalLinkUrl
    FROM dbo.ProjectExternalLink link
    WHERE link.ProjectID = @ProjectID;

    -- 15. Copy documents (share FileResourceID, don't duplicate blob)
    INSERT INTO dbo.ProjectDocumentUpdate (ProjectUpdateBatchID, FileResourceID, DisplayName, Description, ProjectDocumentTypeID)
    SELECT @BatchID, doc.FileResourceID, doc.DisplayName, doc.Description, doc.ProjectDocumentTypeID
    FROM dbo.ProjectDocument doc
    WHERE doc.ProjectID = @ProjectID;

    -- 16. Copy notes
    INSERT INTO dbo.ProjectNoteUpdate (ProjectUpdateBatchID, Note, CreatePersonID, CreateDate, UpdatePersonID, UpdateDate)
    SELECT @BatchID, n.Note, n.CreatePersonID, n.CreateDate, n.UpdatePersonID, n.UpdateDate
    FROM dbo.ProjectNote n
    WHERE n.ProjectID = @ProjectID;

    -- 17. Record initial "Created" history entry
    INSERT INTO dbo.ProjectUpdateHistory (ProjectUpdateBatchID, ProjectUpdateStateID, TransitionDate, UpdatePersonID)
    VALUES (@BatchID, 1, @Now, @CallingPersonID); -- 1 = Created

    -- Return the new batch ID
    SELECT @BatchID AS ProjectUpdateBatchID;
END
GO
