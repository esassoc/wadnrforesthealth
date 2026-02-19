CREATE PROCEDURE dbo.pCommitProjectUpdateToProject
    @ProjectUpdateBatchID int
AS
BEGIN
    SET NOCOUNT ON;

    -- Resolve ProjectID from the batch
    DECLARE @ProjectID int;
    SELECT @ProjectID = ProjectID FROM dbo.ProjectUpdateBatch WHERE ProjectUpdateBatchID = @ProjectUpdateBatchID;

    IF @ProjectID IS NULL
        RETURN;

    BEGIN TRANSACTION;

    BEGIN TRY
        -- 1. Update Project scalar fields from ProjectUpdate
        UPDATE p SET
            p.ProjectDescription = pu.ProjectDescription,
            p.ProjectStageID = pu.ProjectStageID,
            p.PlannedDate = pu.PlannedDate,
            p.CompletionDate = pu.CompletionDate,
            p.ExpirationDate = pu.ExpirationDate,
            p.EstimatedTotalCost = pu.EstimatedTotalCost,
            p.FocusAreaID = pu.FocusAreaID,
            p.PercentageMatch = pu.PercentageMatch,
            p.ProjectLocationPoint = pu.ProjectLocationPoint,
            p.ProjectLocationSimpleTypeID = pu.ProjectLocationSimpleTypeID,
            p.ProjectLocationNotes = pu.ProjectLocationNotes,
            p.ProjectFundingSourceNotes = pu.ProjectFundingSourceNotes,
            p.NoPriorityLandscapesExplanation = b.NoPriorityLandscapesExplanation,
            p.NoRegionsExplanation = b.NoRegionsExplanation,
            p.NoCountiesExplanation = b.NoCountiesExplanation
        FROM dbo.Project p
        INNER JOIN dbo.ProjectUpdate pu ON pu.ProjectUpdateBatchID = @ProjectUpdateBatchID
        INNER JOIN dbo.ProjectUpdateBatch b ON b.ProjectUpdateBatchID = @ProjectUpdateBatchID
        WHERE p.ProjectID = @ProjectID;

        -- 2. Compute orphaned FileResource IDs (images/docs removed during update)
        --    Must compute BEFORE deleting the project-side rows.
        DECLARE @OrphanedFileResourceIDs TABLE (FileResourceID int);

        -- Orphaned image file resources
        INSERT INTO @OrphanedFileResourceIDs (FileResourceID)
        SELECT pi.FileResourceID
        FROM dbo.ProjectImage pi
        WHERE pi.ProjectID = @ProjectID
            AND pi.FileResourceID NOT IN (
                SELECT piu.FileResourceID FROM dbo.ProjectImageUpdate piu
                WHERE piu.ProjectUpdateBatchID = @ProjectUpdateBatchID AND piu.FileResourceID IS NOT NULL
            );

        -- Orphaned document file resources
        INSERT INTO @OrphanedFileResourceIDs (FileResourceID)
        SELECT pd.FileResourceID
        FROM dbo.ProjectDocument pd
        WHERE pd.ProjectID = @ProjectID
            AND pd.FileResourceID NOT IN (
                SELECT pdu.FileResourceID FROM dbo.ProjectDocumentUpdate pdu
                WHERE pdu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            );

        -- 3. Clear ProjectImageUpdate.ProjectImageID FK for images about to be deleted
        --    (Legacy MVC batches populated this FK; modern workflow does not use it.)
        UPDATE piu SET piu.ProjectImageID = NULL
        FROM dbo.ProjectImageUpdate piu
        WHERE piu.ProjectImageID IS NOT NULL
            AND piu.ProjectImageID IN (SELECT pi.ProjectImageID FROM dbo.ProjectImage pi WHERE pi.ProjectID = @ProjectID);

        -- 4. DELETE in FK-safe order: Treatments first (FK to ProjectLocation), then everything else
        DELETE FROM dbo.Treatment WHERE ProjectID = @ProjectID;
        DELETE FROM dbo.ProjectLocation WHERE ProjectID = @ProjectID;
        DELETE FROM dbo.ProjectProgram WHERE ProjectID = @ProjectID;
        DELETE FROM dbo.ProjectPriorityLandscape WHERE ProjectID = @ProjectID;
        DELETE FROM dbo.ProjectRegion WHERE ProjectID = @ProjectID;
        DELETE FROM dbo.ProjectCounty WHERE ProjectID = @ProjectID;
        DELETE FROM dbo.ProjectOrganization WHERE ProjectID = @ProjectID;
        DELETE FROM dbo.ProjectPerson WHERE ProjectID = @ProjectID;
        DELETE FROM dbo.ProjectFundingSource WHERE ProjectID = @ProjectID;
        DELETE FROM dbo.ProjectFundSourceAllocationRequest WHERE ProjectID = @ProjectID;
        DELETE FROM dbo.ProjectImage WHERE ProjectID = @ProjectID;
        DELETE FROM dbo.ProjectExternalLink WHERE ProjectID = @ProjectID;
        DELETE FROM dbo.ProjectDocument WHERE ProjectID = @ProjectID;
        DELETE FROM dbo.ProjectNote WHERE ProjectID = @ProjectID;

        -- 5. INSERT from update tables back to project tables

        -- Programs
        INSERT INTO dbo.ProjectProgram (ProjectID, ProgramID)
        SELECT @ProjectID, up.ProgramID
        FROM dbo.ProjectUpdateProgram up
        WHERE up.ProjectUpdateBatchID = @ProjectUpdateBatchID;

        -- Priority Landscapes
        INSERT INTO dbo.ProjectPriorityLandscape (ProjectID, PriorityLandscapeID)
        SELECT @ProjectID, upl.PriorityLandscapeID
        FROM dbo.ProjectPriorityLandscapeUpdate upl
        WHERE upl.ProjectUpdateBatchID = @ProjectUpdateBatchID;

        -- Regions
        INSERT INTO dbo.ProjectRegion (ProjectID, DNRUplandRegionID)
        SELECT @ProjectID, ur.DNRUplandRegionID
        FROM dbo.ProjectRegionUpdate ur
        WHERE ur.ProjectUpdateBatchID = @ProjectUpdateBatchID;

        -- Counties
        INSERT INTO dbo.ProjectCounty (ProjectID, CountyID)
        SELECT @ProjectID, uc.CountyID
        FROM dbo.ProjectCountyUpdate uc
        WHERE uc.ProjectUpdateBatchID = @ProjectUpdateBatchID;

        -- Locations
        INSERT INTO dbo.ProjectLocation (ProjectID, ProjectLocationTypeID, ProjectLocationGeometry,
            ProjectLocationNotes, ProjectLocationName, ArcGisObjectID, ArcGisGlobalID)
        SELECT @ProjectID, ul.ProjectLocationTypeID, ul.ProjectLocationUpdateGeometry,
            ul.ProjectLocationUpdateNotes, ul.ProjectLocationUpdateName, ul.ArcGisObjectID, ul.ArcGisGlobalID
        FROM dbo.ProjectLocationUpdate ul
        WHERE ul.ProjectUpdateBatchID = @ProjectUpdateBatchID;

        -- Organizations
        INSERT INTO dbo.ProjectOrganization (ProjectID, OrganizationID, RelationshipTypeID)
        SELECT @ProjectID, uo.OrganizationID, uo.RelationshipTypeID
        FROM dbo.ProjectOrganizationUpdate uo
        WHERE uo.ProjectUpdateBatchID = @ProjectUpdateBatchID;

        -- Contacts
        INSERT INTO dbo.ProjectPerson (ProjectID, PersonID, ProjectPersonRelationshipTypeID)
        SELECT @ProjectID, up.PersonID, up.ProjectPersonRelationshipTypeID
        FROM dbo.ProjectPersonUpdate up
        WHERE up.ProjectUpdateBatchID = @ProjectUpdateBatchID;

        -- Funding Sources
        INSERT INTO dbo.ProjectFundingSource (ProjectID, FundingSourceID)
        SELECT @ProjectID, ufs.FundingSourceID
        FROM dbo.ProjectFundingSourceUpdate ufs
        WHERE ufs.ProjectUpdateBatchID = @ProjectUpdateBatchID;

        -- Allocation Requests
        INSERT INTO dbo.ProjectFundSourceAllocationRequest (ProjectID, FundSourceAllocationID,
            TotalAmount, MatchAmount, PayAmount, CreateDate, UpdateDate, ImportedFromTabularData)
        SELECT @ProjectID, uar.FundSourceAllocationID,
            uar.TotalAmount, uar.MatchAmount, uar.PayAmount, uar.CreateDate, uar.UpdateDate, uar.ImportedFromTabularData
        FROM dbo.ProjectFundSourceAllocationRequestUpdate uar
        WHERE uar.ProjectUpdateBatchID = @ProjectUpdateBatchID;

        -- Images
        INSERT INTO dbo.ProjectImage (ProjectID, FileResourceID, Caption, Credit, IsKeyPhoto, ExcludeFromFactSheet, ProjectImageTimingID)
        SELECT @ProjectID, ui.FileResourceID, ui.Caption, ui.Credit, ui.IsKeyPhoto, ui.ExcludeFromFactSheet, ui.ProjectImageTimingID
        FROM dbo.ProjectImageUpdate ui
        WHERE ui.ProjectUpdateBatchID = @ProjectUpdateBatchID AND ui.FileResourceID IS NOT NULL;

        -- External Links
        INSERT INTO dbo.ProjectExternalLink (ProjectID, ExternalLinkLabel, ExternalLinkUrl)
        SELECT @ProjectID, uel.ExternalLinkLabel, uel.ExternalLinkUrl
        FROM dbo.ProjectExternalLinkUpdate uel
        WHERE uel.ProjectUpdateBatchID = @ProjectUpdateBatchID;

        -- Documents
        INSERT INTO dbo.ProjectDocument (ProjectID, FileResourceID, DisplayName, Description, ProjectDocumentTypeID)
        SELECT @ProjectID, ud.FileResourceID, ud.DisplayName, ud.Description, ud.ProjectDocumentTypeID
        FROM dbo.ProjectDocumentUpdate ud
        WHERE ud.ProjectUpdateBatchID = @ProjectUpdateBatchID;

        -- Notes
        INSERT INTO dbo.ProjectNote (ProjectID, Note, CreatePersonID, CreateDate, UpdatePersonID, UpdateDate)
        SELECT @ProjectID, un.Note, un.CreatePersonID, un.CreateDate, un.UpdatePersonID, un.UpdateDate
        FROM dbo.ProjectNoteUpdate un
        WHERE un.ProjectUpdateBatchID = @ProjectUpdateBatchID;

        -- 6. Treatments (match to new project locations by name + geometry)
        --    Treatments with a location update
        INSERT INTO dbo.Treatment (ProjectID, ProjectLocationID, TreatmentTypeID, TreatmentDetailedActivityTypeID,
            TreatmentCodeID, TreatmentStartDate, TreatmentEndDate, TreatmentFootprintAcres, TreatmentTreatedAcres,
            TreatmentNotes, CostPerAcre, TreatmentTypeImportedText, TreatmentDetailedActivityTypeImportedText,
            ProgramID, ImportedFromGis, CreateGisUploadAttemptID, UpdateGisUploadAttemptID)
        SELECT @ProjectID, pl.ProjectLocationID, tu.TreatmentTypeID, tu.TreatmentDetailedActivityTypeID,
            tu.TreatmentCodeID, tu.TreatmentStartDate, tu.TreatmentEndDate, tu.TreatmentFootprintAcres, tu.TreatmentTreatedAcres,
            tu.TreatmentNotes, tu.CostPerAcre, tu.TreatmentTypeImportedText, tu.TreatmentDetailedActivityTypeImportedText,
            tu.ProgramID, tu.ImportedFromGis, tu.CreateGisUploadAttemptID, tu.UpdateGisUploadAttemptID
        FROM dbo.TreatmentUpdate tu
        INNER JOIN dbo.ProjectLocationUpdate plu ON tu.ProjectLocationUpdateID = plu.ProjectLocationUpdateID
        LEFT JOIN dbo.ProjectLocation pl
            ON pl.ProjectID = @ProjectID
            AND pl.ProjectLocationName = plu.ProjectLocationUpdateName
            AND pl.ProjectLocationGeometry.MakeValid().STEquals(plu.ProjectLocationUpdateGeometry.MakeValid()) = 1
        WHERE tu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND tu.ProjectLocationUpdateID IS NOT NULL;

        --    Treatments with no location
        INSERT INTO dbo.Treatment (ProjectID, ProjectLocationID, TreatmentTypeID, TreatmentDetailedActivityTypeID,
            TreatmentCodeID, TreatmentStartDate, TreatmentEndDate, TreatmentFootprintAcres, TreatmentTreatedAcres,
            TreatmentNotes, CostPerAcre, TreatmentTypeImportedText, TreatmentDetailedActivityTypeImportedText,
            ProgramID, ImportedFromGis, CreateGisUploadAttemptID, UpdateGisUploadAttemptID)
        SELECT @ProjectID, NULL, tu.TreatmentTypeID, tu.TreatmentDetailedActivityTypeID,
            tu.TreatmentCodeID, tu.TreatmentStartDate, tu.TreatmentEndDate, tu.TreatmentFootprintAcres, tu.TreatmentTreatedAcres,
            tu.TreatmentNotes, tu.CostPerAcre, tu.TreatmentTypeImportedText, tu.TreatmentDetailedActivityTypeImportedText,
            tu.ProgramID, tu.ImportedFromGis, tu.CreateGisUploadAttemptID, tu.UpdateGisUploadAttemptID
        FROM dbo.TreatmentUpdate tu
        WHERE tu.ProjectUpdateBatchID = @ProjectUpdateBatchID
            AND tu.ProjectLocationUpdateID IS NULL;

        -- 7. Delete orphaned FileResources (images/docs removed during update)
        DELETE fr FROM dbo.FileResource fr
        INNER JOIN @OrphanedFileResourceIDs o ON fr.FileResourceID = o.FileResourceID;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
