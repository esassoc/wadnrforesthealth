using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Text.RegularExpressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

/// <summary>
/// Static helper methods for the Project Update workflow steps.
/// This workflow handles versioned editing of approved projects.
/// </summary>
public static class ProjectUpdateWorkflowSteps
{
    #region Batch Management

    /// <summary>
    /// Starts a new Update batch by copying all data from the Project tables to the Update tables.
    /// </summary>
    public static async Task<ProjectUpdateBatchResponse?> StartBatchAsync(WADNRDbContext dbContext, int projectID, int callingPersonID)
    {
        // Verify project exists and is in Approved status
        var project = await dbContext.Projects
            .Include(p => p.ProjectPrograms)
            .Include(p => p.ProjectPriorityLandscapes)
            .Include(p => p.ProjectRegions)
            .Include(p => p.ProjectCounties)
            .Include(p => p.ProjectLocations)
            .Include(p => p.ProjectOrganizations)
            .Include(p => p.ProjectPeople)
            .Include(p => p.ProjectFundingSources)
            .Include(p => p.ProjectFundSourceAllocationRequests)
            .Include(p => p.ProjectImages)
            .Include(p => p.ProjectExternalLinks)
            .Include(p => p.ProjectDocuments)
            .Include(p => p.ProjectNotes)
            .Include(p => p.Treatments)
                .ThenInclude(t => t.ProjectLocation)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        if (project.ProjectApprovalStatusID != (int)ProjectApprovalStatusEnum.Approved)
        {
            throw new InvalidOperationException("Project Updates can only be started for Approved projects.");
        }

        // Check if there's already an active batch
        var existingBatch = await dbContext.ProjectUpdateBatches
            .FirstOrDefaultAsync(b => b.ProjectID == projectID &&
                b.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Approved);

        if (existingBatch != null)
        {
            throw new InvalidOperationException("An Update batch is already in progress for this project.");
        }

        // Create the batch
        var batch = new ProjectUpdateBatch
        {
            ProjectID = projectID,
            ProjectUpdateStateID = (int)ProjectUpdateStateEnum.Created,
            LastUpdateDate = DateTime.UtcNow,
            LastUpdatePersonID = callingPersonID,
            NoPriorityLandscapesExplanation = project.NoPriorityLandscapesExplanation,
            NoRegionsExplanation = project.NoRegionsExplanation,
            NoCountiesExplanation = project.NoCountiesExplanation
        };
        dbContext.ProjectUpdateBatches.Add(batch);
        await dbContext.SaveChangesAsync();

        // Copy project basics to ProjectUpdate
        var projectUpdate = new ProjectUpdate
        {
            ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
            ProjectStageID = project.ProjectStageID,
            ProjectDescription = project.ProjectDescription,
            CompletionDate = project.CompletionDate,
            EstimatedTotalCost = project.EstimatedTotalCost,
            ProjectLocationPoint = project.ProjectLocationPoint,
            ProjectLocationNotes = project.ProjectLocationNotes,
            PlannedDate = project.PlannedDate,
            ProjectLocationSimpleTypeID = project.ProjectLocationSimpleTypeID,
            FocusAreaID = project.FocusAreaID,
            ExpirationDate = project.ExpirationDate,
            ProjectFundingSourceNotes = project.ProjectFundingSourceNotes,
            PercentageMatch = project.PercentageMatch
        };
        dbContext.Set<ProjectUpdate>().Add(projectUpdate);

        // Copy programs
        foreach (var pp in project.ProjectPrograms)
        {
            dbContext.ProjectUpdatePrograms.Add(new ProjectUpdateProgram
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                ProgramID = pp.ProgramID
            });
        }

        // Copy priority landscapes
        foreach (var pl in project.ProjectPriorityLandscapes)
        {
            dbContext.ProjectPriorityLandscapeUpdates.Add(new ProjectPriorityLandscapeUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                PriorityLandscapeID = pl.PriorityLandscapeID
            });
        }

        // Copy regions
        foreach (var pr in project.ProjectRegions)
        {
            dbContext.ProjectRegionUpdates.Add(new ProjectRegionUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                DNRUplandRegionID = pr.DNRUplandRegionID
            });
        }

        // Copy counties
        foreach (var pc in project.ProjectCounties)
        {
            dbContext.ProjectCountyUpdates.Add(new ProjectCountyUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                CountyID = pc.CountyID
            });
        }

        // Copy detailed locations
        foreach (var loc in project.ProjectLocations)
        {
            dbContext.ProjectLocationUpdates.Add(new ProjectLocationUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                ProjectLocationTypeID = loc.ProjectLocationTypeID,
                ProjectLocationUpdateGeometry = loc.ProjectLocationGeometry,
                ProjectLocationUpdateNotes = loc.ProjectLocationNotes,
                ProjectLocationUpdateName = loc.ProjectLocationName
            });
        }

        // Flush to assign ProjectLocationUpdate IDs (needed for treatment FK mapping)
        await dbContext.SaveChangesAsync();

        // Reload the batch's location updates with their assigned IDs
        var locationUpdates = await dbContext.ProjectLocationUpdates
            .Where(plu => plu.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        // Copy treatments (must happen after locations are saved)
        foreach (var treatment in project.Treatments)
        {
            // Match to the corresponding ProjectLocationUpdate by geometry + name
            int? projectLocationUpdateID = null;
            if (treatment.ProjectLocation != null)
            {
                var matchingLocation = locationUpdates.FirstOrDefault(plu =>
                    plu.ProjectLocationUpdateName == treatment.ProjectLocation.ProjectLocationName &&
                    plu.ProjectLocationUpdateGeometry.EqualsTopologically(treatment.ProjectLocation.ProjectLocationGeometry));
                projectLocationUpdateID = matchingLocation?.ProjectLocationUpdateID;
            }

            dbContext.TreatmentUpdates.Add(new TreatmentUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                ProjectLocationUpdateID = projectLocationUpdateID,
                TreatmentTypeID = treatment.TreatmentTypeID,
                TreatmentDetailedActivityTypeID = treatment.TreatmentDetailedActivityTypeID,
                TreatmentCodeID = treatment.TreatmentCodeID,
                TreatmentStartDate = treatment.TreatmentStartDate,
                TreatmentEndDate = treatment.TreatmentEndDate,
                TreatmentFootprintAcres = treatment.TreatmentFootprintAcres,
                TreatmentTreatedAcres = treatment.TreatmentTreatedAcres,
                TreatmentNotes = treatment.TreatmentNotes,
                CostPerAcre = treatment.CostPerAcre,
                TreatmentTypeImportedText = treatment.TreatmentTypeImportedText,
                TreatmentDetailedActivityTypeImportedText = treatment.TreatmentDetailedActivityTypeImportedText,
                ProgramID = treatment.ProgramID,
                ImportedFromGis = treatment.ImportedFromGis,
                CreateGisUploadAttemptID = treatment.CreateGisUploadAttemptID,
                UpdateGisUploadAttemptID = treatment.UpdateGisUploadAttemptID
            });
        }

        // Copy organizations
        foreach (var po in project.ProjectOrganizations)
        {
            dbContext.ProjectOrganizationUpdates.Add(new ProjectOrganizationUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                OrganizationID = po.OrganizationID,
                RelationshipTypeID = po.RelationshipTypeID
            });
        }

        // Copy contacts
        foreach (var pp in project.ProjectPeople)
        {
            dbContext.ProjectPersonUpdates.Add(new ProjectPersonUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                PersonID = pp.PersonID,
                ProjectPersonRelationshipTypeID = pp.ProjectPersonRelationshipTypeID
            });
        }

        // Copy funding sources
        foreach (var pfs in project.ProjectFundingSources)
        {
            dbContext.ProjectFundingSourceUpdates.Add(new ProjectFundingSourceUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                FundingSourceID = pfs.FundingSourceID
            });
        }

        // Copy allocation requests
        foreach (var ar in project.ProjectFundSourceAllocationRequests)
        {
            dbContext.ProjectFundSourceAllocationRequestUpdates.Add(new ProjectFundSourceAllocationRequestUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                FundSourceAllocationID = ar.FundSourceAllocationID,
                TotalAmount = ar.TotalAmount,
                CreateDate = ar.CreateDate
            });
        }

        // Copy images
        foreach (var img in project.ProjectImages)
        {
            dbContext.ProjectImageUpdates.Add(new ProjectImageUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                FileResourceID = img.FileResourceID,
                Caption = img.Caption,
                Credit = img.Credit,
                IsKeyPhoto = img.IsKeyPhoto,
                ExcludeFromFactSheet = img.ExcludeFromFactSheet
            });
        }

        // Copy external links
        foreach (var link in project.ProjectExternalLinks)
        {
            dbContext.ProjectExternalLinkUpdates.Add(new ProjectExternalLinkUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                ExternalLinkLabel = link.ExternalLinkLabel,
                ExternalLinkUrl = link.ExternalLinkUrl
            });
        }

        // Copy documents
        foreach (var doc in project.ProjectDocuments)
        {
            dbContext.ProjectDocumentUpdates.Add(new ProjectDocumentUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                FileResourceID = doc.FileResourceID,
                DisplayName = doc.DisplayName,
                Description = doc.Description
            });
        }

        // Copy notes
        foreach (var note in project.ProjectNotes)
        {
            dbContext.ProjectNoteUpdates.Add(new ProjectNoteUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                Note = note.Note,
                CreateDate = note.CreateDate,
                CreatePersonID = note.CreatePersonID,
                UpdateDate = note.UpdateDate
            });
        }

        await dbContext.SaveChangesAsync();

        return await GetCurrentBatchAsync(dbContext, projectID);
    }

    /// <summary>
    /// Gets the current (non-approved) batch for a project.
    /// </summary>
    public static async Task<ProjectUpdateBatchResponse?> GetCurrentBatchAsync(WADNRDbContext dbContext, int projectID)
    {
        var result = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Where(b => b.ProjectID == projectID &&
                b.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Approved)
            .Select(b => new ProjectUpdateBatchResponse
            {
                ProjectUpdateBatchID = b.ProjectUpdateBatchID,
                ProjectID = b.ProjectID,
                ProjectName = b.Project.ProjectName,
                ProjectUpdateStateID = b.ProjectUpdateStateID,
                ProjectUpdateStateName = null, // Resolved client-side below
                LastUpdateDate = b.LastUpdateDate,
                LastUpdatedByPersonName = b.LastUpdatePerson.FirstName + " " + b.LastUpdatePerson.LastName
            })
            .FirstOrDefaultAsync();

        // Resolve lookup value client-side to avoid EF Core translation issues
        if (result != null && ProjectUpdateState.AllLookupDictionary.TryGetValue(result.ProjectUpdateStateID, out var state))
        {
            result.ProjectUpdateStateName = state.ProjectUpdateStateDisplayName;
        }

        return result;
    }

    /// <summary>
    /// Deletes an Update batch and all its related data.
    /// </summary>
    public static async Task<bool> DeleteBatchAsync(WADNRDbContext dbContext, int projectUpdateBatchID, int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.ProjectUpdates)
            .Include(b => b.ProjectUpdatePrograms)
            .Include(b => b.ProjectCountyUpdates)
            .Include(b => b.ProjectRegionUpdates)
            .Include(b => b.ProjectPriorityLandscapeUpdates)
            .Include(b => b.ProjectLocationUpdates)
            .Include(b => b.ProjectLocationStagingUpdates)
            .Include(b => b.TreatmentUpdates)
            .Include(b => b.ProjectPersonUpdates)
            .Include(b => b.ProjectOrganizationUpdates)
            .Include(b => b.ProjectFundingSourceUpdates)
            .Include(b => b.ProjectFundSourceAllocationRequestUpdates)
            .Include(b => b.ProjectImageUpdates)
            .Include(b => b.ProjectExternalLinkUpdates)
            .Include(b => b.ProjectDocumentUpdates)
            .Include(b => b.ProjectNoteUpdates)
            .Include(b => b.ProjectUpdateHistories)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return false;

        // Can only delete in Created or Returned state
        if (batch.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Created &&
            batch.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Returned)
        {
            throw new InvalidOperationException("Update batch can only be deleted when in Created or Returned state.");
        }

        // Delete all related data manually (no cascading deletes in DB)
        dbContext.ProjectUpdates.RemoveRange(batch.ProjectUpdates);
        dbContext.ProjectUpdatePrograms.RemoveRange(batch.ProjectUpdatePrograms);
        dbContext.ProjectCountyUpdates.RemoveRange(batch.ProjectCountyUpdates);
        dbContext.ProjectRegionUpdates.RemoveRange(batch.ProjectRegionUpdates);
        dbContext.ProjectPriorityLandscapeUpdates.RemoveRange(batch.ProjectPriorityLandscapeUpdates);
        dbContext.ProjectLocationUpdates.RemoveRange(batch.ProjectLocationUpdates);
        dbContext.ProjectLocationStagingUpdates.RemoveRange(batch.ProjectLocationStagingUpdates);
        dbContext.TreatmentUpdates.RemoveRange(batch.TreatmentUpdates);
        dbContext.ProjectPersonUpdates.RemoveRange(batch.ProjectPersonUpdates);
        dbContext.ProjectOrganizationUpdates.RemoveRange(batch.ProjectOrganizationUpdates);
        dbContext.ProjectFundingSourceUpdates.RemoveRange(batch.ProjectFundingSourceUpdates);
        dbContext.ProjectFundSourceAllocationRequestUpdates.RemoveRange(batch.ProjectFundSourceAllocationRequestUpdates);
        dbContext.ProjectImageUpdates.RemoveRange(batch.ProjectImageUpdates);
        dbContext.ProjectExternalLinkUpdates.RemoveRange(batch.ProjectExternalLinkUpdates);
        dbContext.ProjectDocumentUpdates.RemoveRange(batch.ProjectDocumentUpdates);
        dbContext.ProjectNoteUpdates.RemoveRange(batch.ProjectNoteUpdates);
        dbContext.ProjectUpdateHistories.RemoveRange(batch.ProjectUpdateHistories);

        // Now delete the batch itself
        dbContext.ProjectUpdateBatches.Remove(batch);
        await dbContext.SaveChangesAsync();

        return true;
    }

    #endregion

    #region Basics Step

    public static async Task<ProjectUpdateBasicsStep?> GetBasicsStepAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.Project).ThenInclude(p => p.ProjectType)
            .Include(b => b.Project).ThenInclude(p => p.ProjectPrograms)
                .ThenInclude(pp => pp.Program)
                .ThenInclude(prog => prog.GisUploadSourceOrganization)
                .ThenInclude(g => g.GisDefaultMappings)
            .Include(b => b.ProjectUpdates)
            .Include(b => b.ProjectUpdatePrograms)
            .Include(b => b.ProjectOrganizationUpdates)
                .ThenInclude(ou => ou.RelationshipType)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        var projectUpdate = batch.ProjectUpdates.FirstOrDefault();
        if (projectUpdate == null) return null;

        var programs = batch.Project?.ProjectPrograms?.Select(pp => pp.Program) ?? Enumerable.Empty<Program>();

        var dto = new ProjectUpdateBasicsStep
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            ProjectName = batch.Project?.ProjectName ?? string.Empty,
            ProjectDescription = projectUpdate.ProjectDescription,
            ProjectTypeID = batch.Project?.ProjectTypeID ?? 0,
            ProjectTypeName = batch.Project?.ProjectType?.ProjectTypeName ?? string.Empty,
            ProjectStageID = projectUpdate.ProjectStageID,
            PlannedDate = projectUpdate.PlannedDate,
            CompletionDate = projectUpdate.CompletionDate,
            ExpirationDate = projectUpdate.ExpirationDate,
            FocusAreaID = projectUpdate.FocusAreaID,
            LeadImplementerOrganizationID = batch.ProjectOrganizationUpdates
                .Where(ou => ou.RelationshipType.IsPrimaryContact)
                .Select(ou => (int?)ou.OrganizationID)
                .FirstOrDefault(),
            PercentageMatch = projectUpdate.PercentageMatch,
            ProgramIDs = batch.ProjectUpdatePrograms.Select(pp => pp.ProgramID).ToList(),
            IsProjectStageImported = CheckIfFieldIsImported(programs, (int)FieldDefinitionEnum.ProjectStage),
            IsPlannedDateImported = CheckIfFieldIsImported(programs, (int)FieldDefinitionEnum.PlannedDate),
            IsCompletionDateImported = CheckIfFieldIsImported(programs, (int)FieldDefinitionEnum.CompletionDate),
        };

        return dto;
    }

    public static async Task<ProjectUpdateBasicsStep?> SaveBasicsStepAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        ProjectUpdateBasicsStepRequest request,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.Project).ThenInclude(p => p.ProjectPrograms)
                .ThenInclude(pp => pp.Program)
                .ThenInclude(prog => prog.GisUploadSourceOrganization)
                .ThenInclude(g => g.GisDefaultMappings)
            .Include(b => b.ProjectUpdates)
            .Include(b => b.ProjectUpdatePrograms)
            .Include(b => b.ProjectOrganizationUpdates)
                .ThenInclude(ou => ou.RelationshipType)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        // Verify batch is editable
        VerifyBatchIsEditable(batch);

        var projectUpdate = batch.ProjectUpdates.FirstOrDefault();
        if (projectUpdate == null) return null;

        var programs = batch.Project?.ProjectPrograms?.Select(pp => pp.Program) ?? Enumerable.Empty<Program>();

        // Update the ProjectUpdate record — guard imported fields
        projectUpdate.ProjectDescription = request.ProjectDescription;
        if (!CheckIfFieldIsImported(programs, (int)FieldDefinitionEnum.ProjectStage))
            projectUpdate.ProjectStageID = request.ProjectStageID;
        if (!CheckIfFieldIsImported(programs, (int)FieldDefinitionEnum.PlannedDate))
            projectUpdate.PlannedDate = request.PlannedDate;
        if (!CheckIfFieldIsImported(programs, (int)FieldDefinitionEnum.CompletionDate))
            projectUpdate.CompletionDate = request.CompletionDate;
        projectUpdate.ExpirationDate = request.ExpirationDate;
        projectUpdate.FocusAreaID = request.FocusAreaID;
        projectUpdate.PercentageMatch = request.PercentageMatch;

        // Update batch tracking
        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        // Sync Lead Implementer organization
        var leadImplementerRelationshipType = await dbContext.RelationshipTypes
            .FirstOrDefaultAsync(rt => rt.IsPrimaryContact);
        if (leadImplementerRelationshipType != null)
        {
            var existingLeadImplementer = batch.ProjectOrganizationUpdates
                .FirstOrDefault(ou => ou.RelationshipType.IsPrimaryContact);

            if (request.LeadImplementerOrganizationID.HasValue)
            {
                if (existingLeadImplementer != null)
                {
                    existingLeadImplementer.OrganizationID = request.LeadImplementerOrganizationID.Value;
                }
                else
                {
                    dbContext.ProjectOrganizationUpdates.Add(new ProjectOrganizationUpdate
                    {
                        ProjectUpdateBatchID = projectUpdateBatchID,
                        OrganizationID = request.LeadImplementerOrganizationID.Value,
                        RelationshipTypeID = leadImplementerRelationshipType.RelationshipTypeID
                    });
                }
            }
            else if (existingLeadImplementer != null)
            {
                dbContext.ProjectOrganizationUpdates.Remove(existingLeadImplementer);
            }
        }

        // Sync programs
        var existingProgramIDs = batch.ProjectUpdatePrograms.Select(pp => pp.ProgramID).ToHashSet();
        var requestedProgramIDs = request.ProgramIDs.ToHashSet();

        var toRemove = batch.ProjectUpdatePrograms.Where(pp => !requestedProgramIDs.Contains(pp.ProgramID)).ToList();
        dbContext.ProjectUpdatePrograms.RemoveRange(toRemove);

        foreach (var programID in requestedProgramIDs.Where(id => !existingProgramIDs.Contains(id)))
        {
            dbContext.ProjectUpdatePrograms.Add(new ProjectUpdateProgram
            {
                ProjectUpdateBatchID = projectUpdateBatchID,
                ProgramID = programID
            });
        }

        await dbContext.SaveChangesAsync();

        return await GetBasicsStepAsync(dbContext, projectUpdateBatchID);
    }

    #endregion

    #region Location Simple Step

    public static async Task<ProjectUpdateLocationSimpleStep?> GetLocationSimpleStepAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.ProjectUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        var projectUpdate = batch.ProjectUpdates.FirstOrDefault();
        if (projectUpdate == null) return null;

        return new ProjectUpdateLocationSimpleStep
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            Latitude = projectUpdate.ProjectLocationPoint?.Coordinate.Y,
            Longitude = projectUpdate.ProjectLocationPoint?.Coordinate.X,
            ProjectLocationSimpleTypeID = projectUpdate.ProjectLocationSimpleTypeID,
            ProjectLocationNotes = projectUpdate.ProjectLocationNotes
        };
    }

    public static async Task<ProjectUpdateLocationSimpleStep?> SaveLocationSimpleStepAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        ProjectUpdateLocationSimpleStepRequest request,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.ProjectUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        VerifyBatchIsEditable(batch);

        var projectUpdate = batch.ProjectUpdates.FirstOrDefault();
        if (projectUpdate == null) return null;

        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            var point = new Point(request.Longitude.Value, request.Latitude.Value) { SRID = 4326 };
            projectUpdate.ProjectLocationPoint = point;
        }
        else
        {
            projectUpdate.ProjectLocationPoint = null;
        }
        projectUpdate.ProjectLocationSimpleTypeID = request.ProjectLocationSimpleTypeID;
        projectUpdate.ProjectLocationNotes = request.ProjectLocationNotes;

        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return await GetLocationSimpleStepAsync(dbContext, projectUpdateBatchID);
    }

    #endregion

    #region Location Detailed Step

    public static async Task<ProjectUpdateLocationDetailedStep?> GetLocationDetailedStepAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.ProjectLocationUpdates)
                .ThenInclude(pl => pl.TreatmentUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        return new ProjectUpdateLocationDetailedStep
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            Locations = batch.ProjectLocationUpdates.Select(pl => new ProjectLocationUpdateItem
            {
                ProjectLocationUpdateID = pl.ProjectLocationUpdateID,
                ProjectUpdateBatchID = pl.ProjectUpdateBatchID,
                ProjectLocationTypeID = pl.ProjectLocationTypeID,
                ProjectLocationTypeName = ProjectLocationType.AllLookupDictionary.TryGetValue(pl.ProjectLocationTypeID, out var locType)
                    ? locType.ProjectLocationTypeDisplayName
                    : string.Empty,
                ProjectLocationNotes = pl.ProjectLocationUpdateNotes,
                ProjectLocationName = pl.ProjectLocationUpdateName,
                GeoJson = pl.ProjectLocationUpdateGeometry?.AsText(),
                AreaInAcres = pl.ProjectLocationUpdateGeometry != null ? pl.ProjectLocationUpdateGeometry.Area * 247.105 : null,
                HasTreatments = pl.TreatmentUpdates.Any(),
                IsFromArcGis = pl.ArcGisObjectID.HasValue
            }).ToList()
        };
    }

    public static async Task<ProjectUpdateLocationDetailedStep?> SaveLocationDetailedStepAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        ProjectUpdateLocationDetailedStepRequest request,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.ProjectLocationUpdates)
                .ThenInclude(pl => pl.TreatmentUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        VerifyBatchIsEditable(batch);

        var existingIDs = batch.ProjectLocationUpdates.Select(pl => pl.ProjectLocationUpdateID).ToHashSet();
        var requestIDs = request.Locations.Where(l => l.ProjectLocationUpdateID.HasValue).Select(l => l.ProjectLocationUpdateID!.Value).ToHashSet();

        // Remove locations not in request — but guard against deleting locations with treatments
        var toRemove = batch.ProjectLocationUpdates.Where(pl => !requestIDs.Contains(pl.ProjectLocationUpdateID)).ToList();
        var locationsWithTreatments = toRemove.Where(pl => pl.TreatmentUpdates.Any()).ToList();
        if (locationsWithTreatments.Count > 0)
        {
            var names = string.Join(", ", locationsWithTreatments.Select(pl => $"'{pl.ProjectLocationUpdateName}'"));
            throw new InvalidOperationException($"Cannot delete project location(s) {names} because they have associated Treatments. Remove the Treatments first.");
        }

        // Validate that locations with treatments keep their type
        foreach (var locRequest in request.Locations.Where(l => l.ProjectLocationUpdateID.HasValue))
        {
            var existing = batch.ProjectLocationUpdates.FirstOrDefault(pl => pl.ProjectLocationUpdateID == locRequest.ProjectLocationUpdateID!.Value);
            if (existing != null && existing.TreatmentUpdates.Any() && locRequest.ProjectLocationTypeID != existing.ProjectLocationTypeID)
            {
                throw new InvalidOperationException($"Cannot change the location type of '{existing.ProjectLocationUpdateName}' because it has associated Treatments.");
            }
        }

        dbContext.ProjectLocationUpdates.RemoveRange(toRemove);

        // Update existing and add new
        foreach (var locRequest in request.Locations)
        {
            Geometry? geometry = null;
            if (!string.IsNullOrEmpty(locRequest.GeoJson))
            {
                var reader = new NetTopologySuite.IO.WKTReader();
                geometry = reader.Read(locRequest.GeoJson);
                geometry.SRID = 4326;
            }

            if (locRequest.ProjectLocationUpdateID.HasValue && existingIDs.Contains(locRequest.ProjectLocationUpdateID.Value))
            {
                var existing = batch.ProjectLocationUpdates.First(pl => pl.ProjectLocationUpdateID == locRequest.ProjectLocationUpdateID.Value);
                existing.ProjectLocationTypeID = locRequest.ProjectLocationTypeID;
                existing.ProjectLocationUpdateNotes = locRequest.ProjectLocationNotes;
                existing.ProjectLocationUpdateName = locRequest.ProjectLocationName ?? string.Empty;
                if (geometry != null)
                {
                    existing.ProjectLocationUpdateGeometry = geometry;
                }
            }
            else
            {
                var newLocation = new ProjectLocationUpdate
                {
                    ProjectUpdateBatchID = projectUpdateBatchID,
                    ProjectLocationTypeID = locRequest.ProjectLocationTypeID,
                    ProjectLocationUpdateNotes = locRequest.ProjectLocationNotes,
                    ProjectLocationUpdateName = locRequest.ProjectLocationName ?? string.Empty,
                    ProjectLocationUpdateGeometry = geometry ?? Point.Empty
                };
                dbContext.ProjectLocationUpdates.Add(newLocation);
            }
        }

        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return await GetLocationDetailedStepAsync(dbContext, projectUpdateBatchID);
    }

    public static async Task<ProjectUpdateLocationDetailedStep?> ApproveGdbImportAsync(WADNRDbContext dbContext, int projectUpdateBatchID, int personID, GdbApproveRequest request)
    {
        var batch = await dbContext.ProjectUpdateBatches.FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);
        if (batch == null) return null;

        var stagingRows = await dbContext.ProjectLocationStagingUpdates
            .Where(s => s.ProjectUpdateBatchID == projectUpdateBatchID && s.PersonID == personID)
            .ToListAsync();

        var layerLookup = request.Layers
            .Where(l => l.ShouldImport)
            .ToDictionary(l => l.FeatureClassName, StringComparer.OrdinalIgnoreCase);

        var jsonOptions = new System.Text.Json.JsonSerializerOptions();
        jsonOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory());

        foreach (var staging in stagingRows)
        {
            if (!layerLookup.TryGetValue(staging.FeatureClassName, out var approval))
            {
                continue;
            }

            var featureCollection = System.Text.Json.JsonSerializer.Deserialize<NetTopologySuite.Features.FeatureCollection>(staging.GeoJson, jsonOptions);
            if (featureCollection == null) continue;

            var locationIndex = 1;
            foreach (var feature in featureCollection)
            {
                var geometry = feature.Geometry;
                if (geometry == null) continue;
                geometry.SRID = 4326;

                // Use selected property as name, falling back to feature class + index
                string locationName = null;
                if (!string.IsNullOrEmpty(approval.SelectedPropertyName) && feature.Attributes != null)
                {
                    var propValue = feature.Attributes[approval.SelectedPropertyName];
                    if (propValue != null)
                    {
                        locationName = propValue.ToString();
                    }
                }

                if (string.IsNullOrEmpty(locationName))
                {
                    locationName = $"{staging.FeatureClassName} {locationIndex}";
                }

                // Truncate to DB max length
                if (locationName.Length > 100)
                {
                    locationName = locationName.Substring(0, 100);
                }

                dbContext.ProjectLocationUpdates.Add(new ProjectLocationUpdate
                {
                    ProjectUpdateBatchID = projectUpdateBatchID,
                    ProjectLocationTypeID = (int)ProjectLocationTypeEnum.ProjectArea,
                    ProjectLocationUpdateName = locationName,
                    ProjectLocationUpdateGeometry = geometry
                });

                locationIndex++;
            }
        }

        // Clean up staging rows
        dbContext.ProjectLocationStagingUpdates.RemoveRange(stagingRows);

        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = personID;

        await dbContext.SaveChangesAsync();

        return await GetLocationDetailedStepAsync(dbContext, projectUpdateBatchID);
    }

    #endregion

    #region Priority Landscapes Step

    public static async Task<ProjectUpdateGeographicStep?> GetPriorityLandscapesStepAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.ProjectPriorityLandscapeUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        var allLandscapes = await dbContext.PriorityLandscapes
            .AsNoTracking()
            .OrderBy(pl => pl.PriorityLandscapeName)
            .Select(pl => new GeographicLookupItem
            {
                ID = pl.PriorityLandscapeID,
                DisplayName = pl.PriorityLandscapeName
            })
            .ToListAsync();

        return new ProjectUpdateGeographicStep
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            SelectedIDs = batch.ProjectPriorityLandscapeUpdates.Select(ppl => ppl.PriorityLandscapeID).ToList(),
            NoSelectionExplanation = batch.NoPriorityLandscapesExplanation,
            AvailableOptions = allLandscapes
        };
    }

    public static async Task<ProjectUpdateGeographicStep?> SavePriorityLandscapesStepAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        ProjectUpdateGeographicStepRequest request,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.ProjectPriorityLandscapeUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        VerifyBatchIsEditable(batch);

        var existingIDs = batch.ProjectPriorityLandscapeUpdates.Select(ppl => ppl.PriorityLandscapeID).ToHashSet();
        var requestedIDs = request.SelectedIDs.ToHashSet();

        var toRemove = batch.ProjectPriorityLandscapeUpdates.Where(ppl => !requestedIDs.Contains(ppl.PriorityLandscapeID)).ToList();
        dbContext.ProjectPriorityLandscapeUpdates.RemoveRange(toRemove);

        foreach (var id in requestedIDs.Where(id => !existingIDs.Contains(id)))
        {
            dbContext.ProjectPriorityLandscapeUpdates.Add(new ProjectPriorityLandscapeUpdate
            {
                ProjectUpdateBatchID = projectUpdateBatchID,
                PriorityLandscapeID = id
            });
        }

        batch.NoPriorityLandscapesExplanation = request.NoSelectionExplanation;
        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return await GetPriorityLandscapesStepAsync(dbContext, projectUpdateBatchID);
    }

    #endregion

    #region DNR Upland Regions Step

    public static async Task<ProjectUpdateGeographicStep?> GetDnrUplandRegionsStepAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.ProjectRegionUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        var allRegions = await dbContext.DNRUplandRegions
            .AsNoTracking()
            .OrderBy(r => r.DNRUplandRegionName)
            .Select(r => new GeographicLookupItem
            {
                ID = r.DNRUplandRegionID,
                DisplayName = r.DNRUplandRegionName
            })
            .ToListAsync();

        return new ProjectUpdateGeographicStep
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            SelectedIDs = batch.ProjectRegionUpdates.Select(pr => pr.DNRUplandRegionID).ToList(),
            NoSelectionExplanation = batch.NoRegionsExplanation,
            AvailableOptions = allRegions
        };
    }

    public static async Task<ProjectUpdateGeographicStep?> SaveDnrUplandRegionsStepAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        ProjectUpdateGeographicStepRequest request,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.ProjectRegionUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        VerifyBatchIsEditable(batch);

        var existingIDs = batch.ProjectRegionUpdates.Select(pr => pr.DNRUplandRegionID).ToHashSet();
        var requestedIDs = request.SelectedIDs.ToHashSet();

        var toRemove = batch.ProjectRegionUpdates.Where(pr => !requestedIDs.Contains(pr.DNRUplandRegionID)).ToList();
        dbContext.ProjectRegionUpdates.RemoveRange(toRemove);

        foreach (var id in requestedIDs.Where(id => !existingIDs.Contains(id)))
        {
            dbContext.ProjectRegionUpdates.Add(new ProjectRegionUpdate
            {
                ProjectUpdateBatchID = projectUpdateBatchID,
                DNRUplandRegionID = id
            });
        }

        batch.NoRegionsExplanation = request.NoSelectionExplanation;
        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return await GetDnrUplandRegionsStepAsync(dbContext, projectUpdateBatchID);
    }

    #endregion

    #region Counties Step

    public static async Task<ProjectUpdateGeographicStep?> GetCountiesStepAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.ProjectCountyUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        var allCounties = await dbContext.Counties
            .AsNoTracking()
            .OrderBy(c => c.CountyName)
            .Select(c => new GeographicLookupItem
            {
                ID = c.CountyID,
                DisplayName = c.CountyName
            })
            .ToListAsync();

        return new ProjectUpdateGeographicStep
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            SelectedIDs = batch.ProjectCountyUpdates.Select(pc => pc.CountyID).ToList(),
            NoSelectionExplanation = batch.NoCountiesExplanation,
            AvailableOptions = allCounties
        };
    }

    public static async Task<ProjectUpdateGeographicStep?> SaveCountiesStepAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        ProjectUpdateGeographicStepRequest request,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.ProjectCountyUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        VerifyBatchIsEditable(batch);

        var existingIDs = batch.ProjectCountyUpdates.Select(pc => pc.CountyID).ToHashSet();
        var requestedIDs = request.SelectedIDs.ToHashSet();

        var toRemove = batch.ProjectCountyUpdates.Where(pc => !requestedIDs.Contains(pc.CountyID)).ToList();
        dbContext.ProjectCountyUpdates.RemoveRange(toRemove);

        foreach (var id in requestedIDs.Where(id => !existingIDs.Contains(id)))
        {
            dbContext.ProjectCountyUpdates.Add(new ProjectCountyUpdate
            {
                ProjectUpdateBatchID = projectUpdateBatchID,
                CountyID = id
            });
        }

        batch.NoCountiesExplanation = request.NoSelectionExplanation;
        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return await GetCountiesStepAsync(dbContext, projectUpdateBatchID);
    }

    #endregion

    #region Treatments Step

    public static async Task<ProjectUpdateTreatmentsStep?> GetTreatmentsStepAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.TreatmentUpdates)
                .ThenInclude(t => t.ProjectLocationUpdate)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        return new ProjectUpdateTreatmentsStep
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            Treatments = batch.TreatmentUpdates.Select(t => new TreatmentUpdateItem
            {
                TreatmentUpdateID = t.TreatmentUpdateID,
                ProjectUpdateBatchID = t.ProjectUpdateBatchID,
                ProjectLocationUpdateID = t.ProjectLocationUpdateID,
                ProjectLocationName = t.ProjectLocationUpdate?.ProjectLocationUpdateName,
                TreatmentTypeID = t.TreatmentTypeID,
                TreatmentTypeName = TreatmentType.AllLookupDictionary.TryGetValue(t.TreatmentTypeID, out var treatmentType)
                    ? treatmentType.TreatmentTypeDisplayName
                    : string.Empty,
                TreatmentDetailedActivityTypeID = t.TreatmentDetailedActivityTypeID,
                TreatmentDetailedActivityTypeName = TreatmentDetailedActivityType.AllLookupDictionary.TryGetValue(t.TreatmentDetailedActivityTypeID, out var activityType)
                    ? activityType.TreatmentDetailedActivityTypeDisplayName
                    : string.Empty,
                TreatmentCodeID = t.TreatmentCodeID,
                TreatmentCodeName = t.TreatmentCodeID.HasValue && TreatmentCode.AllLookupDictionary.TryGetValue(t.TreatmentCodeID.Value, out var treatmentCode)
                    ? treatmentCode.TreatmentCodeDisplayName
                    : null,
                TreatmentFootprintAcres = t.TreatmentFootprintAcres,
                TreatmentTreatedAcres = t.TreatmentTreatedAcres,
                TreatmentNotes = t.TreatmentNotes,
                TreatmentStartYear = t.TreatmentStartDate?.Year,
                TreatmentEndYear = t.TreatmentEndDate?.Year,
                CostPerAcre = t.CostPerAcre,
                TotalCost = t.CostPerAcre.HasValue && t.TreatmentTreatedAcres.HasValue
                    ? t.CostPerAcre.Value * t.TreatmentTreatedAcres.Value
                    : null,
                ImportedFromGis = t.ImportedFromGis ?? false
            }).ToList()
        };
    }

    public static async Task<ProjectUpdateTreatmentsStep?> SaveTreatmentsStepAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        ProjectUpdateTreatmentsStepRequest request,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.TreatmentUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        VerifyBatchIsEditable(batch);

        var existingIDs = batch.TreatmentUpdates.Select(t => t.TreatmentUpdateID).ToHashSet();
        var requestIDs = request.Treatments.Where(t => t.TreatmentUpdateID.HasValue).Select(t => t.TreatmentUpdateID!.Value).ToHashSet();

        var toRemove = batch.TreatmentUpdates.Where(t => !requestIDs.Contains(t.TreatmentUpdateID)).ToList();
        dbContext.TreatmentUpdates.RemoveRange(toRemove);

        foreach (var treatmentRequest in request.Treatments)
        {
            if (treatmentRequest.TreatmentUpdateID.HasValue && existingIDs.Contains(treatmentRequest.TreatmentUpdateID.Value))
            {
                var existing = batch.TreatmentUpdates.First(t => t.TreatmentUpdateID == treatmentRequest.TreatmentUpdateID.Value);
                existing.ProjectLocationUpdateID = treatmentRequest.ProjectLocationUpdateID;
                existing.TreatmentTypeID = treatmentRequest.TreatmentTypeID;
                existing.TreatmentDetailedActivityTypeID = treatmentRequest.TreatmentDetailedActivityTypeID;
                existing.TreatmentCodeID = treatmentRequest.TreatmentCodeID;
                existing.TreatmentFootprintAcres = treatmentRequest.TreatmentFootprintAcres ?? 0;
                existing.TreatmentTreatedAcres = treatmentRequest.TreatmentTreatedAcres;
                existing.TreatmentNotes = treatmentRequest.TreatmentNotes;
                existing.TreatmentStartDate = treatmentRequest.TreatmentStartYear.HasValue
                    ? new DateTime(treatmentRequest.TreatmentStartYear.Value, 1, 1)
                    : null;
                existing.TreatmentEndDate = treatmentRequest.TreatmentEndYear.HasValue
                    ? new DateTime(treatmentRequest.TreatmentEndYear.Value, 12, 31)
                    : null;
                existing.CostPerAcre = treatmentRequest.CostPerAcre;
            }
            else
            {
                dbContext.TreatmentUpdates.Add(new TreatmentUpdate
                {
                    ProjectUpdateBatchID = projectUpdateBatchID,
                    ProjectLocationUpdateID = treatmentRequest.ProjectLocationUpdateID,
                    TreatmentTypeID = treatmentRequest.TreatmentTypeID,
                    TreatmentDetailedActivityTypeID = treatmentRequest.TreatmentDetailedActivityTypeID,
                    TreatmentCodeID = treatmentRequest.TreatmentCodeID,
                    TreatmentFootprintAcres = treatmentRequest.TreatmentFootprintAcres ?? 0,
                    TreatmentTreatedAcres = treatmentRequest.TreatmentTreatedAcres,
                    TreatmentNotes = treatmentRequest.TreatmentNotes,
                    TreatmentStartDate = treatmentRequest.TreatmentStartYear.HasValue
                        ? new DateTime(treatmentRequest.TreatmentStartYear.Value, 1, 1)
                        : null,
                    TreatmentEndDate = treatmentRequest.TreatmentEndYear.HasValue
                        ? new DateTime(treatmentRequest.TreatmentEndYear.Value, 12, 31)
                        : null,
                    CostPerAcre = treatmentRequest.CostPerAcre
                });
            }
        }

        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return await GetTreatmentsStepAsync(dbContext, projectUpdateBatchID);
    }

    public static async Task<TreatmentUpdateDetail?> GetTreatmentUpdateByIDAsync(WADNRDbContext dbContext, int treatmentUpdateID)
    {
        var t = await dbContext.TreatmentUpdates
            .AsNoTracking()
            .Include(tu => tu.ProjectLocationUpdate)
            .Include(tu => tu.Program)
            .FirstOrDefaultAsync(tu => tu.TreatmentUpdateID == treatmentUpdateID);

        if (t == null) return null;

        return new TreatmentUpdateDetail
        {
            TreatmentUpdateID = t.TreatmentUpdateID,
            ProjectUpdateBatchID = t.ProjectUpdateBatchID,
            ProjectLocationUpdateID = t.ProjectLocationUpdateID,
            TreatmentAreaName = t.ProjectLocationUpdate?.ProjectLocationUpdateName,
            TreatmentTypeID = t.TreatmentTypeID,
            TreatmentTypeName = TreatmentType.AllLookupDictionary.TryGetValue(t.TreatmentTypeID, out var treatmentType)
                ? treatmentType.TreatmentTypeDisplayName : string.Empty,
            TreatmentDetailedActivityTypeID = t.TreatmentDetailedActivityTypeID,
            TreatmentDetailedActivityTypeName = TreatmentDetailedActivityType.AllLookupDictionary.TryGetValue(t.TreatmentDetailedActivityTypeID, out var activityType)
                ? activityType.TreatmentDetailedActivityTypeDisplayName : string.Empty,
            TreatmentCodeID = t.TreatmentCodeID,
            TreatmentCodeName = t.TreatmentCodeID.HasValue && TreatmentCode.AllLookupDictionary.TryGetValue(t.TreatmentCodeID.Value, out var code)
                ? code.TreatmentCodeDisplayName : null,
            TreatmentStartDate = t.TreatmentStartDate,
            TreatmentEndDate = t.TreatmentEndDate,
            TreatmentFootprintAcres = t.TreatmentFootprintAcres,
            TreatmentTreatedAcres = t.TreatmentTreatedAcres,
            CostPerAcre = t.CostPerAcre,
            TotalCost = t.CostPerAcre.HasValue && t.TreatmentTreatedAcres.HasValue
                ? t.CostPerAcre.Value * t.TreatmentTreatedAcres.Value : null,
            TreatmentNotes = t.TreatmentNotes,
            ProgramID = t.ProgramID,
            ProgramName = t.Program?.ProgramName,
            ImportedFromGis = t.ImportedFromGis ?? false
        };
    }

    public static async Task<TreatmentUpdateDetail?> CreateTreatmentUpdateAsync(
        WADNRDbContext dbContext, int projectID, TreatmentUpdateUpsertRequest request, int callingPersonID)
    {
        var batchResponse = await GetCurrentBatchAsync(dbContext, projectID);
        if (batchResponse == null) return null;

        var batch = await dbContext.ProjectUpdateBatches
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == batchResponse.ProjectUpdateBatchID);
        if (batch == null) return null;

        VerifyBatchIsEditable(batch);

        var entity = new TreatmentUpdate
        {
            ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
            ProjectLocationUpdateID = request.ProjectLocationUpdateID,
            TreatmentTypeID = request.TreatmentTypeID,
            TreatmentDetailedActivityTypeID = request.TreatmentDetailedActivityTypeID,
            TreatmentCodeID = request.TreatmentCodeID,
            TreatmentStartDate = request.TreatmentStartDate,
            TreatmentEndDate = request.TreatmentEndDate,
            TreatmentFootprintAcres = request.TreatmentFootprintAcres,
            TreatmentTreatedAcres = request.TreatmentTreatedAcres,
            CostPerAcre = request.CostPerAcre,
            TreatmentNotes = request.TreatmentNotes,
            ProgramID = request.ProgramID,
            ImportedFromGis = false
        };

        dbContext.TreatmentUpdates.Add(entity);
        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;
        await dbContext.SaveChangesAsync();

        return await GetTreatmentUpdateByIDAsync(dbContext, entity.TreatmentUpdateID);
    }

    public static async Task<TreatmentUpdateDetail?> UpdateTreatmentUpdateAsync(
        WADNRDbContext dbContext, int treatmentUpdateID, TreatmentUpdateUpsertRequest request, int callingPersonID)
    {
        var entity = await dbContext.TreatmentUpdates
            .Include(tu => tu.ProjectUpdateBatch)
            .FirstOrDefaultAsync(tu => tu.TreatmentUpdateID == treatmentUpdateID);

        if (entity == null) return null;

        VerifyBatchIsEditable(entity.ProjectUpdateBatch);

        entity.ProjectLocationUpdateID = request.ProjectLocationUpdateID;
        entity.TreatmentTypeID = request.TreatmentTypeID;
        entity.TreatmentDetailedActivityTypeID = request.TreatmentDetailedActivityTypeID;
        entity.TreatmentCodeID = request.TreatmentCodeID;
        entity.TreatmentStartDate = request.TreatmentStartDate;
        entity.TreatmentEndDate = request.TreatmentEndDate;
        entity.TreatmentFootprintAcres = request.TreatmentFootprintAcres;
        entity.TreatmentTreatedAcres = request.TreatmentTreatedAcres;
        entity.CostPerAcre = request.CostPerAcre;
        entity.TreatmentNotes = request.TreatmentNotes;
        entity.ProgramID = request.ProgramID;

        entity.ProjectUpdateBatch.LastUpdateDate = DateTime.UtcNow;
        entity.ProjectUpdateBatch.LastUpdatePersonID = callingPersonID;
        await dbContext.SaveChangesAsync();

        return await GetTreatmentUpdateByIDAsync(dbContext, entity.TreatmentUpdateID);
    }

    public static async Task<List<TreatmentAreaUpdateLookupItem>> ListTreatmentAreasForUpdateBatchAsync(
        WADNRDbContext dbContext, int projectID)
    {
        var batchResponse = await GetCurrentBatchAsync(dbContext, projectID);
        if (batchResponse == null) return new();

        return await dbContext.ProjectLocationUpdates
            .AsNoTracking()
            .Where(plu => plu.ProjectUpdateBatchID == batchResponse.ProjectUpdateBatchID
                && plu.ProjectLocationTypeID == (int)ProjectLocationTypeEnum.TreatmentArea)
            .OrderBy(plu => plu.ProjectLocationUpdateName)
            .Select(plu => new TreatmentAreaUpdateLookupItem
            {
                ProjectLocationUpdateID = plu.ProjectLocationUpdateID,
                ProjectLocationUpdateName = plu.ProjectLocationUpdateName
            })
            .ToListAsync();
    }

    #endregion

    #region Contacts Step

    public static async Task<ProjectUpdateContactsStep?> GetContactsStepAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.ProjectPersonUpdates)
                .ThenInclude(pp => pp.Person)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        return new ProjectUpdateContactsStep
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            Contacts = batch.ProjectPersonUpdates.Select(pp => new ProjectPersonUpdateItem
            {
                ProjectPersonUpdateID = pp.ProjectPersonUpdateID,
                PersonID = pp.PersonID,
                PersonFullName = pp.Person.FirstName + " " + pp.Person.LastName,
                ProjectPersonRelationshipTypeID = pp.ProjectPersonRelationshipTypeID,
                RelationshipTypeName = ProjectPersonRelationshipType.AllLookupDictionary.TryGetValue(pp.ProjectPersonRelationshipTypeID, out var relType)
                    ? relType.ProjectPersonRelationshipTypeDisplayName
                    : string.Empty
            }).ToList()
        };
    }

    public static async Task<ProjectUpdateContactsStep?> SaveContactsStepAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        ProjectUpdateContactsStepRequest request,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.ProjectPersonUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        VerifyBatchIsEditable(batch);

        var existingIDs = batch.ProjectPersonUpdates.Select(pp => pp.ProjectPersonUpdateID).ToHashSet();
        var requestIDs = request.Contacts.Where(c => c.ProjectPersonUpdateID.HasValue).Select(c => c.ProjectPersonUpdateID!.Value).ToHashSet();

        var toRemove = batch.ProjectPersonUpdates.Where(pp => !requestIDs.Contains(pp.ProjectPersonUpdateID)).ToList();
        dbContext.ProjectPersonUpdates.RemoveRange(toRemove);

        foreach (var contactRequest in request.Contacts)
        {
            if (contactRequest.ProjectPersonUpdateID.HasValue && existingIDs.Contains(contactRequest.ProjectPersonUpdateID.Value))
            {
                var existing = batch.ProjectPersonUpdates.First(pp => pp.ProjectPersonUpdateID == contactRequest.ProjectPersonUpdateID.Value);
                existing.PersonID = contactRequest.PersonID;
                existing.ProjectPersonRelationshipTypeID = contactRequest.ProjectPersonRelationshipTypeID;
            }
            else
            {
                dbContext.ProjectPersonUpdates.Add(new ProjectPersonUpdate
                {
                    ProjectUpdateBatchID = projectUpdateBatchID,
                    PersonID = contactRequest.PersonID,
                    ProjectPersonRelationshipTypeID = contactRequest.ProjectPersonRelationshipTypeID
                });
            }
        }

        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return await GetContactsStepAsync(dbContext, projectUpdateBatchID);
    }

    #endregion

    #region Organizations Step

    public static async Task<ProjectUpdateOrganizationsStep?> GetOrganizationsStepAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.ProjectOrganizationUpdates)
                .ThenInclude(po => po.Organization)
            .Include(b => b.ProjectOrganizationUpdates)
                .ThenInclude(po => po.RelationshipType)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        return new ProjectUpdateOrganizationsStep
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            Organizations = batch.ProjectOrganizationUpdates.Select(po => new ProjectOrganizationUpdateItem
            {
                ProjectOrganizationUpdateID = po.ProjectOrganizationUpdateID,
                OrganizationID = po.OrganizationID,
                OrganizationName = po.Organization.DisplayName,
                RelationshipTypeID = po.RelationshipTypeID,
                RelationshipTypeName = po.RelationshipType.RelationshipTypeName,
                IsPrimaryContact = po.RelationshipType.IsPrimaryContact
            }).ToList()
        };
    }

    public static async Task<ProjectUpdateOrganizationsStep?> SaveOrganizationsStepAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        ProjectUpdateOrganizationsStepRequest request,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.ProjectOrganizationUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        VerifyBatchIsEditable(batch);

        var existingIDs = batch.ProjectOrganizationUpdates.Select(po => po.ProjectOrganizationUpdateID).ToHashSet();
        var requestIDs = request.Organizations.Where(o => o.ProjectOrganizationUpdateID.HasValue).Select(o => o.ProjectOrganizationUpdateID!.Value).ToHashSet();

        var toRemove = batch.ProjectOrganizationUpdates.Where(po => !requestIDs.Contains(po.ProjectOrganizationUpdateID)).ToList();
        dbContext.ProjectOrganizationUpdates.RemoveRange(toRemove);

        foreach (var orgRequest in request.Organizations)
        {
            if (orgRequest.ProjectOrganizationUpdateID.HasValue && existingIDs.Contains(orgRequest.ProjectOrganizationUpdateID.Value))
            {
                var existing = batch.ProjectOrganizationUpdates.First(po => po.ProjectOrganizationUpdateID == orgRequest.ProjectOrganizationUpdateID.Value);
                existing.OrganizationID = orgRequest.OrganizationID;
                existing.RelationshipTypeID = orgRequest.RelationshipTypeID;
            }
            else
            {
                dbContext.ProjectOrganizationUpdates.Add(new ProjectOrganizationUpdate
                {
                    ProjectUpdateBatchID = projectUpdateBatchID,
                    OrganizationID = orgRequest.OrganizationID,
                    RelationshipTypeID = orgRequest.RelationshipTypeID
                });
            }
        }

        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return await GetOrganizationsStepAsync(dbContext, projectUpdateBatchID);
    }

    #endregion

    #region Expected Funding Step

    public static async Task<ProjectUpdateExpectedFundingStep?> GetExpectedFundingStepAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.ProjectUpdates)
            .Include(b => b.ProjectFundingSourceUpdates)
            .Include(b => b.ProjectFundSourceAllocationRequestUpdates)
                .ThenInclude(ar => ar.FundSourceAllocation)
                    .ThenInclude(fsa => fsa.FundSource)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        var projectUpdate = batch.ProjectUpdates.FirstOrDefault();

        return new ProjectUpdateExpectedFundingStep
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            EstimatedTotalCost = projectUpdate?.EstimatedTotalCost,
            ProjectFundingSourceNotes = projectUpdate?.ProjectFundingSourceNotes,
            SelectedFundingSourceIDs = batch.ProjectFundingSourceUpdates.Select(pfs => pfs.FundingSourceID).ToList(),
            AllocationRequests = batch.ProjectFundSourceAllocationRequestUpdates.Select(ar => new FundSourceAllocationRequestUpdateItem
            {
                ProjectFundSourceAllocationRequestUpdateID = ar.ProjectFundSourceAllocationRequestUpdateID,
                FundSourceAllocationID = ar.FundSourceAllocationID,
                FundSourceAllocationName = ar.FundSourceAllocation.FundSourceAllocationName,
                FundSourceName = ar.FundSourceAllocation.FundSource.FundSourceName,
                TotalAmount = ar.TotalAmount
            }).ToList()
        };
    }

    public static async Task<ProjectUpdateExpectedFundingStep?> SaveExpectedFundingStepAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        ProjectUpdateExpectedFundingStepRequest request,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.ProjectUpdates)
            .Include(b => b.ProjectFundingSourceUpdates)
            .Include(b => b.ProjectFundSourceAllocationRequestUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        VerifyBatchIsEditable(batch);

        var projectUpdate = batch.ProjectUpdates.FirstOrDefault();
        if (projectUpdate != null)
        {
            projectUpdate.EstimatedTotalCost = request.EstimatedTotalCost;
            projectUpdate.ProjectFundingSourceNotes = request.ProjectFundingSourceNotes;
        }

        // Sync funding sources
        var existingFundingSourceIDs = batch.ProjectFundingSourceUpdates.Select(pfs => pfs.FundingSourceID).ToHashSet();
        var requestedFundingSourceIDs = request.FundingSourceIDs.ToHashSet();

        var toRemoveFundingSources = batch.ProjectFundingSourceUpdates.Where(pfs => !requestedFundingSourceIDs.Contains(pfs.FundingSourceID)).ToList();
        dbContext.ProjectFundingSourceUpdates.RemoveRange(toRemoveFundingSources);

        foreach (var fundingSourceID in requestedFundingSourceIDs.Where(id => !existingFundingSourceIDs.Contains(id)))
        {
            dbContext.ProjectFundingSourceUpdates.Add(new ProjectFundingSourceUpdate
            {
                ProjectUpdateBatchID = projectUpdateBatchID,
                FundingSourceID = fundingSourceID
            });
        }

        // Sync allocation requests
        var existingArIDs = batch.ProjectFundSourceAllocationRequestUpdates.Select(ar => ar.ProjectFundSourceAllocationRequestUpdateID).ToHashSet();
        var requestArIDs = request.AllocationRequests.Where(ar => ar.ProjectFundSourceAllocationRequestUpdateID.HasValue).Select(ar => ar.ProjectFundSourceAllocationRequestUpdateID!.Value).ToHashSet();

        var toRemoveAr = batch.ProjectFundSourceAllocationRequestUpdates.Where(ar => !requestArIDs.Contains(ar.ProjectFundSourceAllocationRequestUpdateID)).ToList();
        dbContext.ProjectFundSourceAllocationRequestUpdates.RemoveRange(toRemoveAr);

        foreach (var arRequest in request.AllocationRequests)
        {
            if (arRequest.ProjectFundSourceAllocationRequestUpdateID.HasValue && existingArIDs.Contains(arRequest.ProjectFundSourceAllocationRequestUpdateID.Value))
            {
                var existing = batch.ProjectFundSourceAllocationRequestUpdates.First(ar => ar.ProjectFundSourceAllocationRequestUpdateID == arRequest.ProjectFundSourceAllocationRequestUpdateID.Value);
                existing.FundSourceAllocationID = arRequest.FundSourceAllocationID;
                existing.TotalAmount = arRequest.TotalAmount;
            }
            else
            {
                dbContext.ProjectFundSourceAllocationRequestUpdates.Add(new ProjectFundSourceAllocationRequestUpdate
                {
                    ProjectUpdateBatchID = projectUpdateBatchID,
                    FundSourceAllocationID = arRequest.FundSourceAllocationID,
                    TotalAmount = arRequest.TotalAmount,
                    CreateDate = DateTime.UtcNow
                });
            }
        }

        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return await GetExpectedFundingStepAsync(dbContext, projectUpdateBatchID);
    }

    #endregion

    #region Photos Step

    public static async Task<ProjectUpdatePhotosStep?> GetPhotosStepAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.ProjectImageUpdates)
                .ThenInclude(img => img.FileResource)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        var sortOrder = 0;
        return new ProjectUpdatePhotosStep
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            Photos = batch.ProjectImageUpdates
                .OrderBy(img => img.ProjectImageUpdateID)
                .Select(img => new ProjectImageUpdateItem
                {
                    ProjectImageUpdateID = img.ProjectImageUpdateID,
                    ProjectUpdateBatchID = img.ProjectUpdateBatchID,
                    FileResourceID = img.FileResourceID ?? 0,
                    Caption = img.Caption,
                    Credit = img.Credit,
                    IsKeyPhoto = img.IsKeyPhoto,
                    ExcludeFromFactSheet = img.ExcludeFromFactSheet,
                    SortOrder = sortOrder++,
                    FileResourceUrl = img.FileResource != null
                        ? $"/api/file-resources/{img.FileResourceID}"
                        : null
                }).ToList()
        };
    }

    public static async Task<ProjectUpdatePhotosStep?> SavePhotosStepAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        ProjectUpdatePhotosStepRequest request,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.ProjectImageUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        VerifyBatchIsEditable(batch);

        var existingIDs = batch.ProjectImageUpdates.Select(img => img.ProjectImageUpdateID).ToHashSet();
        var requestIDs = request.Photos.Where(p => p.ProjectImageUpdateID.HasValue).Select(p => p.ProjectImageUpdateID!.Value).ToHashSet();

        var toRemove = batch.ProjectImageUpdates.Where(img => !requestIDs.Contains(img.ProjectImageUpdateID)).ToList();
        dbContext.ProjectImageUpdates.RemoveRange(toRemove);

        foreach (var photoRequest in request.Photos)
        {
            if (photoRequest.ProjectImageUpdateID.HasValue && existingIDs.Contains(photoRequest.ProjectImageUpdateID.Value))
            {
                var existing = batch.ProjectImageUpdates.First(img => img.ProjectImageUpdateID == photoRequest.ProjectImageUpdateID.Value);
                existing.FileResourceID = photoRequest.FileResourceID;
                existing.Caption = photoRequest.Caption;
                existing.Credit = photoRequest.Credit;
                existing.IsKeyPhoto = photoRequest.IsKeyPhoto;
                existing.ExcludeFromFactSheet = photoRequest.ExcludeFromFactSheet;
            }
            else
            {
                dbContext.ProjectImageUpdates.Add(new ProjectImageUpdate
                {
                    ProjectUpdateBatchID = projectUpdateBatchID,
                    FileResourceID = photoRequest.FileResourceID,
                    Caption = photoRequest.Caption,
                    Credit = photoRequest.Credit,
                    IsKeyPhoto = photoRequest.IsKeyPhoto,
                    ExcludeFromFactSheet = photoRequest.ExcludeFromFactSheet
                });
            }
        }

        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return await GetPhotosStepAsync(dbContext, projectUpdateBatchID);
    }

    #endregion

    #region External Links Step

    public static async Task<ProjectUpdateExternalLinksStep?> GetExternalLinksStepAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.ProjectExternalLinkUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        return new ProjectUpdateExternalLinksStep
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            ExternalLinks = batch.ProjectExternalLinkUpdates.Select(link => new ProjectExternalLinkUpdateItem
            {
                ProjectExternalLinkUpdateID = link.ProjectExternalLinkUpdateID,
                ProjectUpdateBatchID = link.ProjectUpdateBatchID,
                ExternalLinkLabel = link.ExternalLinkLabel,
                ExternalLinkUrl = link.ExternalLinkUrl
            }).ToList()
        };
    }

    public static async Task<ProjectUpdateExternalLinksStep?> SaveExternalLinksStepAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        ProjectUpdateExternalLinksStepRequest request,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.ProjectExternalLinkUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        VerifyBatchIsEditable(batch);

        var existingIDs = batch.ProjectExternalLinkUpdates.Select(link => link.ProjectExternalLinkUpdateID).ToHashSet();
        var requestIDs = request.ExternalLinks.Where(l => l.ProjectExternalLinkUpdateID.HasValue).Select(l => l.ProjectExternalLinkUpdateID!.Value).ToHashSet();

        var toRemove = batch.ProjectExternalLinkUpdates.Where(link => !requestIDs.Contains(link.ProjectExternalLinkUpdateID)).ToList();
        dbContext.ProjectExternalLinkUpdates.RemoveRange(toRemove);

        foreach (var linkRequest in request.ExternalLinks)
        {
            if (linkRequest.ProjectExternalLinkUpdateID.HasValue && existingIDs.Contains(linkRequest.ProjectExternalLinkUpdateID.Value))
            {
                var existing = batch.ProjectExternalLinkUpdates.First(link => link.ProjectExternalLinkUpdateID == linkRequest.ProjectExternalLinkUpdateID.Value);
                existing.ExternalLinkLabel = linkRequest.ExternalLinkLabel;
                existing.ExternalLinkUrl = linkRequest.ExternalLinkUrl;
            }
            else
            {
                dbContext.ProjectExternalLinkUpdates.Add(new ProjectExternalLinkUpdate
                {
                    ProjectUpdateBatchID = projectUpdateBatchID,
                    ExternalLinkLabel = linkRequest.ExternalLinkLabel,
                    ExternalLinkUrl = linkRequest.ExternalLinkUrl
                });
            }
        }

        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return await GetExternalLinksStepAsync(dbContext, projectUpdateBatchID);
    }

    #endregion

    #region Documents & Notes Step

    public static async Task<ProjectUpdateDocumentsNotesStep?> GetDocumentsNotesStepAsync(WADNRDbContext dbContext, int projectUpdateBatchID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Include(b => b.ProjectDocumentUpdates)
                .ThenInclude(doc => doc.FileResource)
            .Include(b => b.ProjectNoteUpdates)
                .ThenInclude(note => note.CreatePerson)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        return new ProjectUpdateDocumentsNotesStep
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            Documents = batch.ProjectDocumentUpdates.Select(doc => new ProjectDocumentUpdateItem
            {
                ProjectDocumentUpdateID = doc.ProjectDocumentUpdateID,
                ProjectUpdateBatchID = doc.ProjectUpdateBatchID,
                FileResourceID = doc.FileResourceID,
                DocumentTitle = doc.DisplayName,
                DocumentDescription = doc.Description,
                FileResourceUrl = $"/api/file-resources/{doc.FileResourceID}"
            }).ToList(),
            Notes = batch.ProjectNoteUpdates.Select(note => new ProjectNoteUpdateItem
            {
                ProjectNoteUpdateID = note.ProjectNoteUpdateID,
                ProjectUpdateBatchID = note.ProjectUpdateBatchID,
                Note = note.Note,
                CreateDate = note.CreateDate,
                CreatedByPersonName = note.CreatePerson != null
                    ? note.CreatePerson.FirstName + " " + note.CreatePerson.LastName
                    : null,
                UpdateDate = note.UpdateDate
            }).ToList()
        };
    }

    public static async Task<ProjectUpdateDocumentsNotesStep?> SaveDocumentsNotesStepAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        ProjectUpdateDocumentsNotesStepRequest request,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.ProjectDocumentUpdates)
            .Include(b => b.ProjectNoteUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return null;

        VerifyBatchIsEditable(batch);

        // Sync documents
        var existingDocIDs = batch.ProjectDocumentUpdates.Select(doc => doc.ProjectDocumentUpdateID).ToHashSet();
        var requestDocIDs = request.Documents.Where(d => d.ProjectDocumentUpdateID.HasValue).Select(d => d.ProjectDocumentUpdateID!.Value).ToHashSet();

        var toRemoveDocs = batch.ProjectDocumentUpdates.Where(doc => !requestDocIDs.Contains(doc.ProjectDocumentUpdateID)).ToList();
        dbContext.ProjectDocumentUpdates.RemoveRange(toRemoveDocs);

        foreach (var docRequest in request.Documents)
        {
            if (docRequest.ProjectDocumentUpdateID.HasValue && existingDocIDs.Contains(docRequest.ProjectDocumentUpdateID.Value))
            {
                var existing = batch.ProjectDocumentUpdates.First(doc => doc.ProjectDocumentUpdateID == docRequest.ProjectDocumentUpdateID.Value);
                existing.FileResourceID = docRequest.FileResourceID;
                existing.DisplayName = docRequest.DocumentTitle;
                existing.Description = docRequest.DocumentDescription;
            }
            else
            {
                dbContext.ProjectDocumentUpdates.Add(new ProjectDocumentUpdate
                {
                    ProjectUpdateBatchID = projectUpdateBatchID,
                    FileResourceID = docRequest.FileResourceID,
                    DisplayName = docRequest.DocumentTitle,
                    Description = docRequest.DocumentDescription
                });
            }
        }

        // Sync notes
        var existingNoteIDs = batch.ProjectNoteUpdates.Select(note => note.ProjectNoteUpdateID).ToHashSet();
        var requestNoteIDs = request.Notes.Where(n => n.ProjectNoteUpdateID.HasValue).Select(n => n.ProjectNoteUpdateID!.Value).ToHashSet();

        var toRemoveNotes = batch.ProjectNoteUpdates.Where(note => !requestNoteIDs.Contains(note.ProjectNoteUpdateID)).ToList();
        dbContext.ProjectNoteUpdates.RemoveRange(toRemoveNotes);

        foreach (var noteRequest in request.Notes)
        {
            if (noteRequest.ProjectNoteUpdateID.HasValue && existingNoteIDs.Contains(noteRequest.ProjectNoteUpdateID.Value))
            {
                var existing = batch.ProjectNoteUpdates.First(note => note.ProjectNoteUpdateID == noteRequest.ProjectNoteUpdateID.Value);
                existing.Note = noteRequest.Note;
                existing.UpdateDate = DateTime.UtcNow;
            }
            else
            {
                dbContext.ProjectNoteUpdates.Add(new ProjectNoteUpdate
                {
                    ProjectUpdateBatchID = projectUpdateBatchID,
                    Note = noteRequest.Note,
                    CreateDate = DateTime.UtcNow,
                    CreatePersonID = callingPersonID
                });
            }
        }

        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return await GetDocumentsNotesStepAsync(dbContext, projectUpdateBatchID);
    }

    #endregion

    #region State Transitions

    public static async Task<WorkflowStateTransitionResponse> SubmitAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null)
        {
            return new WorkflowStateTransitionResponse
            {
                Success = false,
                ErrorMessage = "Update batch not found."
            };
        }

        if (batch.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Created &&
            batch.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Returned)
        {
            return new WorkflowStateTransitionResponse
            {
                ProjectID = batch.ProjectID,
                Success = false,
                ErrorMessage = "Update batch can only be submitted when in Created or Returned state."
            };
        }

        batch.ProjectUpdateStateID = (int)ProjectUpdateStateEnum.Submitted;
        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        // Record in history
        dbContext.ProjectUpdateHistories.Add(new ProjectUpdateHistory
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            ProjectUpdateStateID = (int)ProjectUpdateStateEnum.Submitted,
            TransitionDate = DateTime.UtcNow,
            UpdatePersonID = callingPersonID
        });

        await dbContext.SaveChangesAsync();

        return new WorkflowStateTransitionResponse
        {
            ProjectID = batch.ProjectID,
            NewProjectApprovalStatusID = batch.ProjectUpdateStateID,
            NewProjectApprovalStatusName = ProjectUpdateState.Submitted.ProjectUpdateStateDisplayName,
            TransitionDate = DateTime.UtcNow,
            Success = true
        };
    }

    public static async Task<WorkflowStateTransitionResponse> ApproveAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.Project)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null)
        {
            return new WorkflowStateTransitionResponse
            {
                Success = false,
                ErrorMessage = "Update batch not found."
            };
        }

        if (batch.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Submitted)
        {
            return new WorkflowStateTransitionResponse
            {
                ProjectID = batch.ProjectID,
                Success = false,
                ErrorMessage = "Update batch can only be approved when in Submitted state."
            };
        }

        // Generate diff logs BEFORE committing changes (so we can compare update to current)
        await ProjectUpdateDiffs.GenerateAndStoreDiffsAsync(dbContext, batch);

        // Commit all changes to the Project tables
        await CommitChangesToProjectAsync(dbContext, batch);

        batch.ProjectUpdateStateID = (int)ProjectUpdateStateEnum.Approved;
        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        // Record in history
        dbContext.ProjectUpdateHistories.Add(new ProjectUpdateHistory
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            ProjectUpdateStateID = (int)ProjectUpdateStateEnum.Approved,
            TransitionDate = DateTime.UtcNow,
            UpdatePersonID = callingPersonID
        });

        await dbContext.SaveChangesAsync();

        return new WorkflowStateTransitionResponse
        {
            ProjectID = batch.ProjectID,
            NewProjectApprovalStatusID = batch.ProjectUpdateStateID,
            NewProjectApprovalStatusName = ProjectUpdateState.Approved.ProjectUpdateStateDisplayName,
            TransitionDate = DateTime.UtcNow,
            Success = true
        };
    }

    public static async Task<WorkflowStateTransitionResponse> ReturnAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        int callingPersonID,
        ProjectUpdateReturnRequest? request)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null)
        {
            return new WorkflowStateTransitionResponse
            {
                Success = false,
                ErrorMessage = "Update batch not found."
            };
        }

        if (batch.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Submitted)
        {
            return new WorkflowStateTransitionResponse
            {
                ProjectID = batch.ProjectID,
                Success = false,
                ErrorMessage = "Update batch can only be returned when in Submitted state."
            };
        }

        batch.ProjectUpdateStateID = (int)ProjectUpdateStateEnum.Returned;
        batch.LastUpdateDate = DateTime.UtcNow;
        batch.LastUpdatePersonID = callingPersonID;

        // Store per-section reviewer comments
        batch.BasicsComment = request?.BasicsComment;
        batch.LocationSimpleComment = request?.LocationSimpleComment;
        batch.LocationDetailedComment = request?.LocationDetailedComment;
        batch.ExpectedFundingComment = request?.ExpectedFundingComment;
        batch.ContactsComment = request?.ContactsComment;
        batch.OrganizationsComment = request?.OrganizationsComment;

        // Record in history
        dbContext.ProjectUpdateHistories.Add(new ProjectUpdateHistory
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            ProjectUpdateStateID = (int)ProjectUpdateStateEnum.Returned,
            TransitionDate = DateTime.UtcNow,
            UpdatePersonID = callingPersonID
        });

        await dbContext.SaveChangesAsync();

        return new WorkflowStateTransitionResponse
        {
            ProjectID = batch.ProjectID,
            NewProjectApprovalStatusID = batch.ProjectUpdateStateID,
            NewProjectApprovalStatusName = ProjectUpdateState.Returned.ProjectUpdateStateDisplayName,
            TransitionDate = DateTime.UtcNow,
            Success = true
        };
    }

    #endregion

    #region Step Revert

    /// <summary>
    /// Reverts a specific step in the update workflow by re-copying data from the approved project.
    /// </summary>
    public static async Task<bool> RevertStepAsync(WADNRDbContext dbContext, int projectUpdateBatchID, string stepKey, int callingPersonID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.Project)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return false;

        VerifyBatchIsEditable(batch);

        var project = await dbContext.Projects
            .Include(p => p.ProjectPrograms)
            .Include(p => p.ProjectPriorityLandscapes)
            .Include(p => p.ProjectRegions)
            .Include(p => p.ProjectCounties)
            .Include(p => p.ProjectLocations)
            .Include(p => p.ProjectOrganizations)
            .Include(p => p.ProjectPeople)
            .Include(p => p.ProjectFundingSources)
            .Include(p => p.ProjectFundSourceAllocationRequests)
            .Include(p => p.ProjectImages)
            .Include(p => p.ProjectExternalLinks)
            .Include(p => p.ProjectDocuments)
            .Include(p => p.ProjectNotes)
            .FirstOrDefaultAsync(p => p.ProjectID == batch.ProjectID);

        if (project == null) return false;

        // Normalize PascalCase (e.g., "ExpectedFunding") to kebab-case ("expected-funding")
        var normalizedKey = Regex.Replace(stepKey, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();

        var success = normalizedKey switch
        {
            "basics" => await RevertBasicsStepAsync(dbContext, batch, project),
            "organizations" => await RevertOrganizationsStepAsync(dbContext, batch, project),
            "contacts" => await RevertContactsStepAsync(dbContext, batch, project),
            "expected-funding" => await RevertExpectedFundingStepAsync(dbContext, batch, project),
            "external-links" => await RevertExternalLinksStepAsync(dbContext, batch, project),
            "documents-notes" => await RevertDocumentsNotesStepAsync(dbContext, batch, project),
            "location-simple" => await RevertLocationSimpleStepAsync(dbContext, batch, project),
            "location-detailed" => await RevertLocationDetailedStepAsync(dbContext, batch, project),
            "photos" => await RevertPhotosStepAsync(dbContext, batch, project),
            "priority-landscapes" => await RevertPriorityLandscapesStepAsync(dbContext, batch, project),
            "dnr-upland-regions" => await RevertDnrUplandRegionsStepAsync(dbContext, batch, project),
            "counties" => await RevertCountiesStepAsync(dbContext, batch, project),
            "treatments" => await RevertTreatmentsStepAsync(dbContext, batch, project),
            _ => false
        };

        if (success)
        {
            batch.LastUpdateDate = DateTime.UtcNow;
            batch.LastUpdatePersonID = callingPersonID;
            await dbContext.SaveChangesAsync();
        }

        return success;
    }

    private static async Task<bool> RevertBasicsStepAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch, Project project)
    {
        var projectUpdate = await dbContext.ProjectUpdates
            .FirstOrDefaultAsync(pu => pu.ProjectUpdateBatchID == batch.ProjectUpdateBatchID);

        if (projectUpdate == null) return false;

        projectUpdate.ProjectStageID = project.ProjectStageID;
        projectUpdate.ProjectDescription = project.ProjectDescription;
        projectUpdate.PlannedDate = project.PlannedDate;
        projectUpdate.CompletionDate = project.CompletionDate;
        projectUpdate.ExpirationDate = project.ExpirationDate;
        projectUpdate.FocusAreaID = project.FocusAreaID;
        projectUpdate.PercentageMatch = project.PercentageMatch;

        // Also revert programs
        var existingPrograms = await dbContext.ProjectUpdatePrograms
            .Where(p => p.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectUpdatePrograms.RemoveRange(existingPrograms);

        foreach (var pp in project.ProjectPrograms)
        {
            dbContext.ProjectUpdatePrograms.Add(new ProjectUpdateProgram
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                ProgramID = pp.ProgramID
            });
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    private static async Task<bool> RevertOrganizationsStepAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch, Project project)
    {
        var existingOrgs = await dbContext.ProjectOrganizationUpdates
            .Where(o => o.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectOrganizationUpdates.RemoveRange(existingOrgs);

        foreach (var po in project.ProjectOrganizations)
        {
            dbContext.ProjectOrganizationUpdates.Add(new ProjectOrganizationUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                OrganizationID = po.OrganizationID,
                RelationshipTypeID = po.RelationshipTypeID
            });
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    private static async Task<bool> RevertContactsStepAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch, Project project)
    {
        var existingContacts = await dbContext.ProjectPersonUpdates
            .Where(c => c.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectPersonUpdates.RemoveRange(existingContacts);

        foreach (var pp in project.ProjectPeople)
        {
            dbContext.ProjectPersonUpdates.Add(new ProjectPersonUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                PersonID = pp.PersonID,
                ProjectPersonRelationshipTypeID = pp.ProjectPersonRelationshipTypeID
            });
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    private static async Task<bool> RevertExpectedFundingStepAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch, Project project)
    {
        var projectUpdate = await dbContext.ProjectUpdates
            .FirstOrDefaultAsync(pu => pu.ProjectUpdateBatchID == batch.ProjectUpdateBatchID);

        if (projectUpdate != null)
        {
            projectUpdate.EstimatedTotalCost = project.EstimatedTotalCost;
            projectUpdate.ProjectFundingSourceNotes = project.ProjectFundingSourceNotes;
        }

        var existingFunding = await dbContext.ProjectFundingSourceUpdates
            .Where(f => f.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectFundingSourceUpdates.RemoveRange(existingFunding);

        foreach (var pfs in project.ProjectFundingSources)
        {
            dbContext.ProjectFundingSourceUpdates.Add(new ProjectFundingSourceUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                FundingSourceID = pfs.FundingSourceID
            });
        }

        var existingAllocations = await dbContext.ProjectFundSourceAllocationRequestUpdates
            .Where(a => a.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectFundSourceAllocationRequestUpdates.RemoveRange(existingAllocations);

        foreach (var ar in project.ProjectFundSourceAllocationRequests)
        {
            dbContext.ProjectFundSourceAllocationRequestUpdates.Add(new ProjectFundSourceAllocationRequestUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                FundSourceAllocationID = ar.FundSourceAllocationID,
                TotalAmount = ar.TotalAmount,
                CreateDate = ar.CreateDate
            });
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    private static async Task<bool> RevertExternalLinksStepAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch, Project project)
    {
        var existingLinks = await dbContext.ProjectExternalLinkUpdates
            .Where(l => l.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectExternalLinkUpdates.RemoveRange(existingLinks);

        foreach (var link in project.ProjectExternalLinks)
        {
            dbContext.ProjectExternalLinkUpdates.Add(new ProjectExternalLinkUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                ExternalLinkLabel = link.ExternalLinkLabel,
                ExternalLinkUrl = link.ExternalLinkUrl
            });
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    private static async Task<bool> RevertDocumentsNotesStepAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch, Project project)
    {
        var existingDocs = await dbContext.ProjectDocumentUpdates
            .Where(d => d.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectDocumentUpdates.RemoveRange(existingDocs);

        foreach (var doc in project.ProjectDocuments)
        {
            dbContext.ProjectDocumentUpdates.Add(new ProjectDocumentUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                FileResourceID = doc.FileResourceID,
                DisplayName = doc.DisplayName,
                Description = doc.Description
            });
        }

        var existingNotes = await dbContext.ProjectNoteUpdates
            .Where(n => n.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectNoteUpdates.RemoveRange(existingNotes);

        foreach (var note in project.ProjectNotes)
        {
            dbContext.ProjectNoteUpdates.Add(new ProjectNoteUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                Note = note.Note,
                CreateDate = note.CreateDate,
                CreatePersonID = note.CreatePersonID,
                UpdateDate = note.UpdateDate
            });
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    private static async Task<bool> RevertLocationSimpleStepAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch, Project project)
    {
        var projectUpdate = await dbContext.ProjectUpdates
            .FirstOrDefaultAsync(pu => pu.ProjectUpdateBatchID == batch.ProjectUpdateBatchID);

        if (projectUpdate == null) return false;

        projectUpdate.ProjectLocationPoint = project.ProjectLocationPoint;
        projectUpdate.ProjectLocationSimpleTypeID = project.ProjectLocationSimpleTypeID;
        projectUpdate.ProjectLocationNotes = project.ProjectLocationNotes;

        await dbContext.SaveChangesAsync();
        return true;
    }

    private static async Task<bool> RevertLocationDetailedStepAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch, Project project)
    {
        var existingLocs = await dbContext.ProjectLocationUpdates
            .Where(l => l.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectLocationUpdates.RemoveRange(existingLocs);

        foreach (var loc in project.ProjectLocations)
        {
            dbContext.ProjectLocationUpdates.Add(new ProjectLocationUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                ProjectLocationTypeID = loc.ProjectLocationTypeID,
                ProjectLocationUpdateGeometry = loc.ProjectLocationGeometry,
                ProjectLocationUpdateNotes = loc.ProjectLocationNotes,
                ProjectLocationUpdateName = loc.ProjectLocationName
            });
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    private static async Task<bool> RevertPhotosStepAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch, Project project)
    {
        var existingPhotos = await dbContext.ProjectImageUpdates
            .Where(i => i.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectImageUpdates.RemoveRange(existingPhotos);

        foreach (var img in project.ProjectImages)
        {
            dbContext.ProjectImageUpdates.Add(new ProjectImageUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                FileResourceID = img.FileResourceID,
                Caption = img.Caption,
                Credit = img.Credit,
                IsKeyPhoto = img.IsKeyPhoto,
                ExcludeFromFactSheet = img.ExcludeFromFactSheet
            });
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    private static async Task<bool> RevertPriorityLandscapesStepAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch, Project project)
    {
        var existingPl = await dbContext.ProjectPriorityLandscapeUpdates
            .Where(pl => pl.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectPriorityLandscapeUpdates.RemoveRange(existingPl);

        foreach (var pl in project.ProjectPriorityLandscapes)
        {
            dbContext.ProjectPriorityLandscapeUpdates.Add(new ProjectPriorityLandscapeUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                PriorityLandscapeID = pl.PriorityLandscapeID
            });
        }

        batch.NoPriorityLandscapesExplanation = project.NoPriorityLandscapesExplanation;

        await dbContext.SaveChangesAsync();
        return true;
    }

    private static async Task<bool> RevertDnrUplandRegionsStepAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch, Project project)
    {
        var existingRegions = await dbContext.ProjectRegionUpdates
            .Where(r => r.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectRegionUpdates.RemoveRange(existingRegions);

        foreach (var pr in project.ProjectRegions)
        {
            dbContext.ProjectRegionUpdates.Add(new ProjectRegionUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                DNRUplandRegionID = pr.DNRUplandRegionID
            });
        }

        batch.NoRegionsExplanation = project.NoRegionsExplanation;

        await dbContext.SaveChangesAsync();
        return true;
    }

    private static async Task<bool> RevertCountiesStepAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch, Project project)
    {
        var existingCounties = await dbContext.ProjectCountyUpdates
            .Where(c => c.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectCountyUpdates.RemoveRange(existingCounties);

        foreach (var pc in project.ProjectCounties)
        {
            dbContext.ProjectCountyUpdates.Add(new ProjectCountyUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                CountyID = pc.CountyID
            });
        }

        batch.NoCountiesExplanation = project.NoCountiesExplanation;

        await dbContext.SaveChangesAsync();
        return true;
    }

    private static async Task<bool> RevertTreatmentsStepAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch, Project project)
    {
        // Treatments are tied to ProjectLocationUpdates, so we need the location update ID mapping
        // This is more complex - for now, just remove all treatments
        // The user can re-add them manually if needed, or we'd need to revert locations first
        var existingTreatments = await dbContext.TreatmentUpdates
            .Where(t => t.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.TreatmentUpdates.RemoveRange(existingTreatments);

        // We can't directly copy treatments without first reverting location detailed step
        // because treatments reference ProjectLocationUpdateIDs that may not exist
        // The recommended approach is to revert location-detailed first, then treatments

        await dbContext.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Private Helpers

    private static void VerifyBatchIsEditable(ProjectUpdateBatch batch)
    {
        if (batch.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Created &&
            batch.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Returned)
        {
            throw new InvalidOperationException("Update batch cannot be edited when in Submitted or Approved state.");
        }
    }

    private static bool CheckIfFieldIsImported(IEnumerable<Program> programs, int fieldDefinitionID)
    {
        foreach (var program in programs)
        {
            var gisSource = program.GisUploadSourceOrganization;
            if (gisSource == null) continue;

            if (fieldDefinitionID == (int)FieldDefinitionEnum.PlannedDate && !gisSource.ApplyStartDateToProject)
                continue;
            if (fieldDefinitionID == (int)FieldDefinitionEnum.CompletionDate && !gisSource.ApplyCompletedDateToProject)
                continue;

            if (gisSource.GisDefaultMappings.Any(m =>
                m.FieldDefinitionID == fieldDefinitionID && !string.IsNullOrEmpty(m.GisDefaultMappingColumnName)))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Commits all changes from the Update tables to the Project tables.
    /// </summary>
    private static async Task CommitChangesToProjectAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch)
    {
        var project = await dbContext.Projects
            .Include(p => p.ProjectPrograms)
            .Include(p => p.ProjectPriorityLandscapes)
            .Include(p => p.ProjectRegions)
            .Include(p => p.ProjectCounties)
            .Include(p => p.ProjectLocations)
            .Include(p => p.ProjectOrganizations)
            .Include(p => p.ProjectPeople)
            .Include(p => p.ProjectFundingSources)
            .Include(p => p.ProjectFundSourceAllocationRequests)
            .Include(p => p.ProjectImages)
            .Include(p => p.ProjectExternalLinks)
            .Include(p => p.ProjectDocuments)
            .Include(p => p.ProjectNotes)
            .FirstOrDefaultAsync(p => p.ProjectID == batch.ProjectID);

        if (project == null) return;

        // Load update data
        var batchWithData = await dbContext.ProjectUpdateBatches
            .Include(b => b.ProjectUpdates)
            .Include(b => b.ProjectUpdatePrograms)
            .Include(b => b.ProjectPriorityLandscapeUpdates)
            .Include(b => b.ProjectRegionUpdates)
            .Include(b => b.ProjectCountyUpdates)
            .Include(b => b.ProjectLocationUpdates)
            .Include(b => b.ProjectOrganizationUpdates)
            .Include(b => b.ProjectPersonUpdates)
            .Include(b => b.ProjectFundingSourceUpdates)
            .Include(b => b.ProjectFundSourceAllocationRequestUpdates)
            .Include(b => b.ProjectImageUpdates)
            .Include(b => b.ProjectExternalLinkUpdates)
            .Include(b => b.ProjectDocumentUpdates)
            .Include(b => b.ProjectNoteUpdates)
            .FirstAsync(b => b.ProjectUpdateBatchID == batch.ProjectUpdateBatchID);

        var projectUpdate = batchWithData.ProjectUpdates.FirstOrDefault();
        if (projectUpdate == null) return;

        // Commit basic fields
        project.ProjectDescription = projectUpdate.ProjectDescription;
        project.ProjectStageID = projectUpdate.ProjectStageID;
        project.PlannedDate = projectUpdate.PlannedDate;
        project.CompletionDate = projectUpdate.CompletionDate;
        project.ExpirationDate = projectUpdate.ExpirationDate;
        project.EstimatedTotalCost = projectUpdate.EstimatedTotalCost;
        project.FocusAreaID = projectUpdate.FocusAreaID;
        project.PercentageMatch = projectUpdate.PercentageMatch;
        project.ProjectLocationPoint = projectUpdate.ProjectLocationPoint;
        project.ProjectLocationSimpleTypeID = projectUpdate.ProjectLocationSimpleTypeID;
        project.ProjectLocationNotes = projectUpdate.ProjectLocationNotes;
        project.ProjectFundingSourceNotes = projectUpdate.ProjectFundingSourceNotes;
        project.NoPriorityLandscapesExplanation = batchWithData.NoPriorityLandscapesExplanation;
        project.NoRegionsExplanation = batchWithData.NoRegionsExplanation;
        project.NoCountiesExplanation = batchWithData.NoCountiesExplanation;

        // Sync Programs
        dbContext.ProjectPrograms.RemoveRange(project.ProjectPrograms);
        foreach (var pp in batchWithData.ProjectUpdatePrograms)
        {
            dbContext.ProjectPrograms.Add(new ProjectProgram
            {
                ProjectID = project.ProjectID,
                ProgramID = pp.ProgramID
            });
        }

        // Sync Priority Landscapes
        dbContext.ProjectPriorityLandscapes.RemoveRange(project.ProjectPriorityLandscapes);
        foreach (var pl in batchWithData.ProjectPriorityLandscapeUpdates)
        {
            dbContext.ProjectPriorityLandscapes.Add(new ProjectPriorityLandscape
            {
                ProjectID = project.ProjectID,
                PriorityLandscapeID = pl.PriorityLandscapeID
            });
        }

        // Sync Regions
        dbContext.ProjectRegions.RemoveRange(project.ProjectRegions);
        foreach (var pr in batchWithData.ProjectRegionUpdates)
        {
            dbContext.ProjectRegions.Add(new ProjectRegion
            {
                ProjectID = project.ProjectID,
                DNRUplandRegionID = pr.DNRUplandRegionID
            });
        }

        // Sync Counties
        dbContext.ProjectCounties.RemoveRange(project.ProjectCounties);
        foreach (var pc in batchWithData.ProjectCountyUpdates)
        {
            dbContext.ProjectCounties.Add(new ProjectCounty
            {
                ProjectID = project.ProjectID,
                CountyID = pc.CountyID
            });
        }

        // Sync Locations
        dbContext.ProjectLocations.RemoveRange(project.ProjectLocations);
        foreach (var loc in batchWithData.ProjectLocationUpdates)
        {
            dbContext.ProjectLocations.Add(new ProjectLocation
            {
                ProjectID = project.ProjectID,
                ProjectLocationTypeID = loc.ProjectLocationTypeID,
                ProjectLocationGeometry = loc.ProjectLocationUpdateGeometry,
                ProjectLocationNotes = loc.ProjectLocationUpdateNotes,
                ProjectLocationName = loc.ProjectLocationUpdateName
            });
        }

        // Sync Organizations
        dbContext.ProjectOrganizations.RemoveRange(project.ProjectOrganizations);
        foreach (var po in batchWithData.ProjectOrganizationUpdates)
        {
            dbContext.ProjectOrganizations.Add(new ProjectOrganization
            {
                ProjectID = project.ProjectID,
                OrganizationID = po.OrganizationID,
                RelationshipTypeID = po.RelationshipTypeID
            });
        }

        // Sync Contacts
        dbContext.ProjectPeople.RemoveRange(project.ProjectPeople);
        foreach (var pp in batchWithData.ProjectPersonUpdates)
        {
            dbContext.ProjectPeople.Add(new ProjectPerson
            {
                ProjectID = project.ProjectID,
                PersonID = pp.PersonID,
                ProjectPersonRelationshipTypeID = pp.ProjectPersonRelationshipTypeID
            });
        }

        // Sync Funding Sources
        dbContext.ProjectFundingSources.RemoveRange(project.ProjectFundingSources);
        foreach (var pfs in batchWithData.ProjectFundingSourceUpdates)
        {
            dbContext.ProjectFundingSources.Add(new ProjectFundingSource
            {
                ProjectID = project.ProjectID,
                FundingSourceID = pfs.FundingSourceID
            });
        }

        // Sync Allocation Requests
        dbContext.ProjectFundSourceAllocationRequests.RemoveRange(project.ProjectFundSourceAllocationRequests);
        foreach (var ar in batchWithData.ProjectFundSourceAllocationRequestUpdates)
        {
            dbContext.ProjectFundSourceAllocationRequests.Add(new ProjectFundSourceAllocationRequest
            {
                ProjectID = project.ProjectID,
                FundSourceAllocationID = ar.FundSourceAllocationID,
                TotalAmount = ar.TotalAmount,
                CreateDate = ar.CreateDate
            });
        }

        // Sync Images
        dbContext.ProjectImages.RemoveRange(project.ProjectImages);
        foreach (var img in batchWithData.ProjectImageUpdates)
        {
            if (img.FileResourceID.HasValue)
            {
                dbContext.ProjectImages.Add(new ProjectImage
                {
                    ProjectID = project.ProjectID,
                    FileResourceID = img.FileResourceID.Value,
                    Caption = img.Caption,
                    Credit = img.Credit,
                    IsKeyPhoto = img.IsKeyPhoto,
                    ExcludeFromFactSheet = img.ExcludeFromFactSheet
                });
            }
        }

        // Sync External Links
        dbContext.ProjectExternalLinks.RemoveRange(project.ProjectExternalLinks);
        foreach (var link in batchWithData.ProjectExternalLinkUpdates)
        {
            dbContext.ProjectExternalLinks.Add(new ProjectExternalLink
            {
                ProjectID = project.ProjectID,
                ExternalLinkLabel = link.ExternalLinkLabel,
                ExternalLinkUrl = link.ExternalLinkUrl
            });
        }

        // Sync Documents
        dbContext.ProjectDocuments.RemoveRange(project.ProjectDocuments);
        foreach (var doc in batchWithData.ProjectDocumentUpdates)
        {
            dbContext.ProjectDocuments.Add(new ProjectDocument
            {
                ProjectID = project.ProjectID,
                FileResourceID = doc.FileResourceID,
                DisplayName = doc.DisplayName,
                Description = doc.Description
            });
        }

        // Sync Notes
        dbContext.ProjectNotes.RemoveRange(project.ProjectNotes);
        foreach (var note in batchWithData.ProjectNoteUpdates)
        {
            dbContext.ProjectNotes.Add(new ProjectNote
            {
                ProjectID = project.ProjectID,
                Note = note.Note,
                CreateDate = note.CreateDate,
                CreatePersonID = note.CreatePersonID,
                UpdateDate = note.UpdateDate
            });
        }

        await dbContext.SaveChangesAsync();
    }

    #endregion
}
