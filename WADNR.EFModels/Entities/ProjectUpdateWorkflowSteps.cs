using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Text.RegularExpressions;
using WADNR.Common.GeoSpatial;
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
    public static async Task<ProjectUpdateBatchDetail?> StartBatchAsync(WADNRDbContext dbContext, int projectID, int callingPersonID)
    {
        // Verify project exists and is in Approved status
        // Load project scalar fields only — child collections are loaded separately below
        // to avoid a massive Cartesian product join (14 includes with geometry columns).
        var project = await dbContext.Projects
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        if (project.ProjectApprovalStatusID != (int)ProjectApprovalStatusEnum.Approved)
        {
            throw new InvalidOperationException("Project Updates can only be started for Approved projects.");
        }

        // Check if there's already an active batch (use latest batch, matching HasExistingUpdateBatch logic)
        var latestBatch = await dbContext.ProjectUpdateBatches
            .Where(b => b.ProjectID == projectID)
            .OrderByDescending(b => b.ProjectUpdateBatchID)
            .FirstOrDefaultAsync();

        if (latestBatch != null && latestBatch.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Approved)
        {
            throw new InvalidOperationException("An Update batch is already in progress for this project.");
        }

        // Execute stored procedure to create batch and copy all project data in a single database call
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"EXEC dbo.pStartProjectUpdateBatch @ProjectID={projectID}, @CallingPersonID={callingPersonID}");

        return await GetCurrentBatchAsync(dbContext, projectID);
    }

    /// <summary>
    /// Gets the current (non-approved) batch for a project.
    /// </summary>
    public static async Task<ProjectUpdateBatchDetail?> GetCurrentBatchAsync(WADNRDbContext dbContext, int projectID)
    {
        var result = await dbContext.ProjectUpdateBatches
            .AsNoTracking()
            .Where(b => b.ProjectID == projectID &&
                b.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Approved)
            .Select(ProjectUpdateBatchProjections.AsDetail)
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
        // Load batch scalars only — child collections loaded separately to avoid
        // Cartesian product joins (was 18 includes with geometry tables).
        var batch = await dbContext.ProjectUpdateBatches
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null) return false;

        // Can only delete in Created or Returned state
        if (batch.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Created &&
            batch.ProjectUpdateStateID != (int)ProjectUpdateStateEnum.Returned)
        {
            throw new InvalidOperationException("Update batch can only be deleted when in Created or Returned state.");
        }

        // Load each child collection separately (one simple query per table)
        var projectUpdates = await dbContext.ProjectUpdates
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var projectUpdatePrograms = await dbContext.ProjectUpdatePrograms
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var countyUpdates = await dbContext.ProjectCountyUpdates
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var regionUpdates = await dbContext.ProjectRegionUpdates
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var priorityLandscapeUpdates = await dbContext.ProjectPriorityLandscapeUpdates
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var locationUpdates = await dbContext.ProjectLocationUpdates
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var locationStagingUpdates = await dbContext.ProjectLocationStagingUpdates
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var treatmentUpdates = await dbContext.TreatmentUpdates
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var personUpdates = await dbContext.ProjectPersonUpdates
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var organizationUpdates = await dbContext.ProjectOrganizationUpdates
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var fundingSourceUpdates = await dbContext.ProjectFundingSourceUpdates
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var allocationRequestUpdates = await dbContext.ProjectFundSourceAllocationRequestUpdates
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        // Include FileResource only on images/docs for orphan cleanup
        var imageUpdates = await dbContext.ProjectImageUpdates
            .Include(x => x.FileResource)
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var externalLinkUpdates = await dbContext.ProjectExternalLinkUpdates
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var documentUpdates = await dbContext.ProjectDocumentUpdates
            .Include(x => x.FileResource)
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var noteUpdates = await dbContext.ProjectNoteUpdates
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();
        var updateHistories = await dbContext.ProjectUpdateHistories
            .Where(x => x.ProjectUpdateBatchID == projectUpdateBatchID).ToListAsync();

        // Clean up FileResources for newly-uploaded photos/documents (not shared with live project)
        var imageFileResourceIDs = imageUpdates
            .Where(piu => piu.FileResourceID.HasValue)
            .Select(piu => piu.FileResourceID!.Value)
            .Distinct().ToList();
        var sharedImageFileResourceIDs = new HashSet<int>(
            await dbContext.ProjectImages
                .Where(pi => imageFileResourceIDs.Contains(pi.FileResourceID))
                .Select(pi => pi.FileResourceID)
                .ToListAsync());
        var orphanedImageFiles = imageUpdates
            .Where(piu => piu.FileResource != null && !sharedImageFileResourceIDs.Contains(piu.FileResourceID!.Value))
            .Select(piu => piu.FileResource!)
            .ToList();

        var docFileResourceIDs = documentUpdates.Select(pdu => pdu.FileResourceID).Distinct().ToList();
        var sharedDocFileResourceIDs = new HashSet<int>(
            await dbContext.ProjectDocuments
                .Where(pd => docFileResourceIDs.Contains(pd.FileResourceID))
                .Select(pd => pd.FileResourceID)
                .ToListAsync());
        var orphanedDocFiles = documentUpdates
            .Where(pdu => pdu.FileResource != null && !sharedDocFileResourceIDs.Contains(pdu.FileResourceID))
            .Select(pdu => pdu.FileResource!)
            .ToList();

        // Delete all related data manually (no cascading deletes in DB)
        dbContext.ProjectUpdates.RemoveRange(projectUpdates);
        dbContext.ProjectUpdatePrograms.RemoveRange(projectUpdatePrograms);
        dbContext.ProjectCountyUpdates.RemoveRange(countyUpdates);
        dbContext.ProjectRegionUpdates.RemoveRange(regionUpdates);
        dbContext.ProjectPriorityLandscapeUpdates.RemoveRange(priorityLandscapeUpdates);
        dbContext.TreatmentUpdates.RemoveRange(treatmentUpdates); // before locations (FK: TreatmentUpdate → ProjectLocationUpdate)
        dbContext.ProjectLocationUpdates.RemoveRange(locationUpdates);
        dbContext.ProjectLocationStagingUpdates.RemoveRange(locationStagingUpdates);
        dbContext.ProjectPersonUpdates.RemoveRange(personUpdates);
        dbContext.ProjectOrganizationUpdates.RemoveRange(organizationUpdates);
        dbContext.ProjectFundingSourceUpdates.RemoveRange(fundingSourceUpdates);
        dbContext.ProjectFundSourceAllocationRequestUpdates.RemoveRange(allocationRequestUpdates);
        dbContext.ProjectImageUpdates.RemoveRange(imageUpdates);
        dbContext.ProjectExternalLinkUpdates.RemoveRange(externalLinkUpdates);
        dbContext.ProjectDocumentUpdates.RemoveRange(documentUpdates);
        dbContext.ProjectNoteUpdates.RemoveRange(noteUpdates);
        dbContext.ProjectUpdateHistories.RemoveRange(updateHistories);
        dbContext.FileResources.RemoveRange(orphanedImageFiles);
        dbContext.FileResources.RemoveRange(orphanedDocFiles);

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
                geometry = reader.Read(locRequest.GeoJson).MakeValid();
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
                if (!string.IsNullOrEmpty(approval.SelectedPropertyName)
                    && feature.Attributes != null
                    && feature.Attributes.Exists(approval.SelectedPropertyName))
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
                    ProjectImageTimingID = img.ProjectImageTimingID,
                    ExcludeFromFactSheet = img.ExcludeFromFactSheet,
                    SortOrder = sortOrder++,
                    FileResourceUrl = img.FileResource != null
                        ? $"/file-resources/{img.FileResource.FileResourceGUID}"
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

    public static async Task<ProjectImageUpdate> CreatePhotoUpdateAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        int fileResourceID,
        string caption,
        string credit,
        int? projectImageTimingID,
        bool excludeFromFactSheet)
    {
        var hasExistingPhotos = await dbContext.ProjectImageUpdates
            .AnyAsync(x => x.ProjectUpdateBatchID == projectUpdateBatchID);

        var photoUpdate = new ProjectImageUpdate
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            FileResourceID = fileResourceID,
            Caption = caption,
            Credit = credit,
            ProjectImageTimingID = projectImageTimingID,
            ExcludeFromFactSheet = excludeFromFactSheet,
            IsKeyPhoto = !hasExistingPhotos
        };

        dbContext.ProjectImageUpdates.Add(photoUpdate);
        await dbContext.SaveChangesAsync();
        return photoUpdate;
    }

    public static async Task UpdatePhotoUpdateAsync(
        WADNRDbContext dbContext,
        int projectImageUpdateID,
        ProjectImageUpsertRequest request)
    {
        var photoUpdate = await dbContext.ProjectImageUpdates
            .Include(x => x.ProjectUpdateBatch)
            .FirstOrDefaultAsync(x => x.ProjectImageUpdateID == projectImageUpdateID);

        if (photoUpdate == null)
            throw new InvalidOperationException($"ProjectImageUpdate with ID {projectImageUpdateID} not found.");

        VerifyBatchIsEditable(photoUpdate.ProjectUpdateBatch);

        photoUpdate.Caption = request.Caption;
        photoUpdate.Credit = request.Credit;
        photoUpdate.ProjectImageTimingID = request.ProjectImageTimingID;
        photoUpdate.ExcludeFromFactSheet = request.ExcludeFromFactSheet;

        await dbContext.SaveChangesAsync();
    }

    public static async Task<Guid> DeletePhotoUpdateAsync(WADNRDbContext dbContext, int projectImageUpdateID)
    {
        var photoUpdate = await dbContext.ProjectImageUpdates
            .Include(x => x.FileResource)
            .Include(x => x.ProjectUpdateBatch)
            .FirstOrDefaultAsync(x => x.ProjectImageUpdateID == projectImageUpdateID);

        if (photoUpdate == null)
            throw new InvalidOperationException($"ProjectImageUpdate with ID {projectImageUpdateID} not found.");

        VerifyBatchIsEditable(photoUpdate.ProjectUpdateBatch);

        var batchID = photoUpdate.ProjectUpdateBatchID;
        var wasKeyPhoto = photoUpdate.IsKeyPhoto;
        var fileResourceGuid = Guid.Empty;

        dbContext.ProjectImageUpdates.Remove(photoUpdate);

        // FileResource sharing strategy: Unlike the legacy MVC which duplicated FileResource rows
        // (with their inline FileResourceData) for each update batch, the modern workflow shares
        // the same FileResourceID between ProjectImage and ProjectImageUpdate. This avoids
        // duplicating blob storage entries. The tradeoff is that we must check for sharing before
        // deleting a FileResource — if the original ProjectImage still references it, we skip
        // the delete. See DeleteDocumentUpdateAsync for the same pattern.
        if (photoUpdate.FileResource != null)
        {
            var isShared = await dbContext.ProjectImages
                .AnyAsync(pi => pi.FileResourceID == photoUpdate.FileResourceID);

            if (!isShared)
            {
                fileResourceGuid = photoUpdate.FileResource.FileResourceGUID;
                dbContext.FileResources.Remove(photoUpdate.FileResource);
            }
        }

        await dbContext.SaveChangesAsync();

        if (wasKeyPhoto)
        {
            var nextPhoto = await dbContext.ProjectImageUpdates
                .Where(x => x.ProjectUpdateBatchID == batchID)
                .OrderBy(x => x.ProjectImageUpdateID)
                .FirstOrDefaultAsync();

            if (nextPhoto != null)
            {
                nextPhoto.IsKeyPhoto = true;
                await dbContext.SaveChangesAsync();
            }
        }

        return fileResourceGuid;
    }

    public static async Task SetKeyPhotoUpdateAsync(WADNRDbContext dbContext, int projectUpdateBatchID, int projectImageUpdateID)
    {
        var batch = await dbContext.ProjectUpdateBatches
            .Include(b => b.ProjectImageUpdates)
            .FirstOrDefaultAsync(b => b.ProjectUpdateBatchID == projectUpdateBatchID);

        if (batch == null)
            throw new InvalidOperationException($"ProjectUpdateBatch with ID {projectUpdateBatchID} not found.");

        VerifyBatchIsEditable(batch);

        foreach (var photo in batch.ProjectImageUpdates)
        {
            photo.IsKeyPhoto = photo.ProjectImageUpdateID == projectImageUpdateID;
        }

        await dbContext.SaveChangesAsync();
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
                ProjectDocumentTypeID = doc.ProjectDocumentTypeID,
                FileResourceUrl = doc.FileResource != null
                    ? $"/file-resources/{doc.FileResource.FileResourceGUID}"
                    : null
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

    // Individual document CRUD for update workflow

    public static async Task CreateDocumentUpdateAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        int fileResourceID,
        string displayName,
        string? description,
        int? projectDocumentTypeID)
    {
        var docUpdate = new ProjectDocumentUpdate
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            FileResourceID = fileResourceID,
            DisplayName = displayName,
            Description = description,
            ProjectDocumentTypeID = projectDocumentTypeID
        };

        dbContext.ProjectDocumentUpdates.Add(docUpdate);
        await dbContext.SaveChangesAsync();
    }

    public static async Task UpdateDocumentUpdateAsync(
        WADNRDbContext dbContext,
        int projectDocumentUpdateID,
        ProjectDocumentUpsertRequest request)
    {
        var docUpdate = await dbContext.ProjectDocumentUpdates
            .Include(x => x.ProjectUpdateBatch)
            .FirstOrDefaultAsync(x => x.ProjectDocumentUpdateID == projectDocumentUpdateID);

        if (docUpdate == null)
            throw new InvalidOperationException($"ProjectDocumentUpdate with ID {projectDocumentUpdateID} not found.");

        VerifyBatchIsEditable(docUpdate.ProjectUpdateBatch);

        docUpdate.DisplayName = request.DisplayName;
        docUpdate.Description = request.Description;
        docUpdate.ProjectDocumentTypeID = request.ProjectDocumentTypeID;

        await dbContext.SaveChangesAsync();
    }

    public static async Task<Guid> DeleteDocumentUpdateAsync(WADNRDbContext dbContext, int projectDocumentUpdateID)
    {
        var docUpdate = await dbContext.ProjectDocumentUpdates
            .Include(x => x.FileResource)
            .Include(x => x.ProjectUpdateBatch)
            .FirstOrDefaultAsync(x => x.ProjectDocumentUpdateID == projectDocumentUpdateID);

        if (docUpdate == null)
            throw new InvalidOperationException($"ProjectDocumentUpdate with ID {projectDocumentUpdateID} not found.");

        VerifyBatchIsEditable(docUpdate.ProjectUpdateBatch);

        var fileResourceGuid = Guid.Empty;

        dbContext.ProjectDocumentUpdates.Remove(docUpdate);

        // Only delete the FileResource if it's not referenced by a live ProjectDocument.
        // Documents and photos copied from the original project share the same FileResourceID
        // (unlike legacy MVC which duplicated FileResource rows with inline data).
        // See StartBatchAsync comment for why we use shared references + isShared checks.
        if (docUpdate.FileResource != null)
        {
            var isShared = await dbContext.ProjectDocuments
                .AnyAsync(d => d.FileResourceID == docUpdate.FileResourceID);

            if (!isShared)
            {
                fileResourceGuid = docUpdate.FileResource.FileResourceGUID;
                dbContext.FileResources.Remove(docUpdate.FileResource);
            }
        }

        await dbContext.SaveChangesAsync();
        return fileResourceGuid;
    }

    // Individual note CRUD for update workflow

    public static async Task CreateNoteUpdateAsync(
        WADNRDbContext dbContext,
        int projectUpdateBatchID,
        string note,
        int callingPersonID)
    {
        var noteUpdate = new ProjectNoteUpdate
        {
            ProjectUpdateBatchID = projectUpdateBatchID,
            Note = note,
            CreateDate = DateTime.UtcNow,
            CreatePersonID = callingPersonID
        };

        dbContext.ProjectNoteUpdates.Add(noteUpdate);
        await dbContext.SaveChangesAsync();
    }

    public static async Task UpdateNoteUpdateAsync(
        WADNRDbContext dbContext,
        int projectNoteUpdateID,
        string note)
    {
        var noteUpdate = await dbContext.ProjectNoteUpdates
            .Include(x => x.ProjectUpdateBatch)
            .FirstOrDefaultAsync(x => x.ProjectNoteUpdateID == projectNoteUpdateID);

        if (noteUpdate == null)
            throw new InvalidOperationException($"ProjectNoteUpdate with ID {projectNoteUpdateID} not found.");

        VerifyBatchIsEditable(noteUpdate.ProjectUpdateBatch);

        noteUpdate.Note = note;
        noteUpdate.UpdateDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public static async Task DeleteNoteUpdateAsync(WADNRDbContext dbContext, int projectNoteUpdateID)
    {
        var noteUpdate = await dbContext.ProjectNoteUpdates
            .Include(x => x.ProjectUpdateBatch)
            .FirstOrDefaultAsync(x => x.ProjectNoteUpdateID == projectNoteUpdateID);

        if (noteUpdate == null)
            throw new InvalidOperationException($"ProjectNoteUpdate with ID {projectNoteUpdateID} not found.");

        VerifyBatchIsEditable(noteUpdate.ProjectUpdateBatch);

        dbContext.ProjectNoteUpdates.Remove(noteUpdate);
        await dbContext.SaveChangesAsync();
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

        // Load project scalars only — each revert sub-method loads only the
        // specific collection(s) it needs, avoiding a 14-include Cartesian product.
        var project = await dbContext.Projects
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

        var projectPrograms = await dbContext.ProjectPrograms
            .Where(x => x.ProjectID == project.ProjectID).ToListAsync();
        foreach (var pp in projectPrograms)
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

        var projectOrganizations = await dbContext.ProjectOrganizations
            .Where(x => x.ProjectID == project.ProjectID).ToListAsync();
        foreach (var po in projectOrganizations)
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

        var projectPeople = await dbContext.ProjectPeople
            .Where(x => x.ProjectID == project.ProjectID).ToListAsync();
        foreach (var pp in projectPeople)
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

        var projectFundingSources = await dbContext.ProjectFundingSources
            .Where(x => x.ProjectID == project.ProjectID).ToListAsync();
        foreach (var pfs in projectFundingSources)
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

        var projectAllocations = await dbContext.ProjectFundSourceAllocationRequests
            .Where(x => x.ProjectID == project.ProjectID).ToListAsync();
        foreach (var ar in projectAllocations)
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

        var projectExternalLinks = await dbContext.ProjectExternalLinks
            .Where(x => x.ProjectID == project.ProjectID).ToListAsync();
        foreach (var link in projectExternalLinks)
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
            .Include(d => d.FileResource)
            .Where(d => d.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        // Same shared-FileResource cleanup as RevertPhotosStepAsync — see comment there.
        var docFileResourceIDs = existingDocs.Select(d => d.FileResourceID).Distinct().ToList();
        var sharedDocFileResourceIDs = new HashSet<int>(
            await dbContext.ProjectDocuments
                .Where(pd => docFileResourceIDs.Contains(pd.FileResourceID))
                .Select(pd => pd.FileResourceID)
                .ToListAsync());
        var orphanedDocFiles = existingDocs
            .Where(d => d.FileResource != null && !sharedDocFileResourceIDs.Contains(d.FileResourceID))
            .Select(d => d.FileResource!)
            .ToList();

        dbContext.ProjectDocumentUpdates.RemoveRange(existingDocs);
        dbContext.FileResources.RemoveRange(orphanedDocFiles);

        var projectDocuments = await dbContext.ProjectDocuments
            .Where(x => x.ProjectID == project.ProjectID).ToListAsync();
        foreach (var doc in projectDocuments)
        {
            dbContext.ProjectDocumentUpdates.Add(new ProjectDocumentUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                FileResourceID = doc.FileResourceID,
                DisplayName = doc.DisplayName,
                Description = doc.Description,
                ProjectDocumentTypeID = doc.ProjectDocumentTypeID
            });
        }

        var existingNotes = await dbContext.ProjectNoteUpdates
            .Where(n => n.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectNoteUpdates.RemoveRange(existingNotes);

        var projectNotes = await dbContext.ProjectNotes
            .Where(x => x.ProjectID == project.ProjectID).ToListAsync();
        foreach (var note in projectNotes)
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
        // Delete child TreatmentUpdates before removing ProjectLocationUpdates (FK constraint)
        var existingTreatments = await dbContext.TreatmentUpdates
            .Where(t => t.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.TreatmentUpdates.RemoveRange(existingTreatments);

        var existingLocs = await dbContext.ProjectLocationUpdates
            .Where(l => l.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.ProjectLocationUpdates.RemoveRange(existingLocs);

        // Flush deletes so new inserts get fresh IDs
        await dbContext.SaveChangesAsync();

        var projectLocations = await dbContext.ProjectLocations
            .Where(x => x.ProjectID == project.ProjectID).ToListAsync();
        foreach (var loc in projectLocations)
        {
            dbContext.ProjectLocationUpdates.Add(new ProjectLocationUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                ProjectLocationTypeID = loc.ProjectLocationTypeID,
                ProjectLocationUpdateGeometry = loc.ProjectLocationGeometry,
                ProjectLocationUpdateNotes = loc.ProjectLocationNotes,
                ProjectLocationUpdateName = loc.ProjectLocationName,
                ArcGisObjectID = loc.ArcGisObjectID,
                ArcGisGlobalID = loc.ArcGisGlobalID
            });
        }

        // Save new locations so they have IDs for treatment FK mapping
        await dbContext.SaveChangesAsync();

        // Re-create TreatmentUpdates mapped to the new ProjectLocationUpdates
        var newLocationUpdates = await dbContext.ProjectLocationUpdates
            .Where(plu => plu.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var projectTreatments = await dbContext.Treatments
            .Include(t => t.ProjectLocation)
            .Where(t => t.ProjectLocation.ProjectID == project.ProjectID).ToListAsync();
        foreach (var treatment in projectTreatments)
        {
            int? projectLocationUpdateID = null;
            if (treatment.ProjectLocation != null)
            {
                var matchingLocation = newLocationUpdates.FirstOrDefault(plu =>
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

        await dbContext.SaveChangesAsync();
        return true;
    }

    private static async Task<bool> RevertPhotosStepAsync(WADNRDbContext dbContext, ProjectUpdateBatch batch, Project project)
    {
        var existingPhotos = await dbContext.ProjectImageUpdates
            .Include(i => i.FileResource)
            .Where(i => i.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        // Clean up FileResources that were newly uploaded during this update batch.
        // Photos copied from the original project share their FileResourceID with ProjectImage,
        // so those must NOT be deleted. Only FileResources unique to this batch are orphaned.
        var photoFileResourceIDs = existingPhotos
            .Where(p => p.FileResourceID.HasValue)
            .Select(p => p.FileResourceID!.Value)
            .Distinct()
            .ToList();
        var sharedPhotoFileResourceIDs = new HashSet<int>(
            await dbContext.ProjectImages
                .Where(pi => photoFileResourceIDs.Contains(pi.FileResourceID))
                .Select(pi => pi.FileResourceID)
                .ToListAsync());
        var orphanedPhotoFiles = existingPhotos
            .Where(p => p.FileResource != null && !sharedPhotoFileResourceIDs.Contains(p.FileResourceID!.Value))
            .Select(p => p.FileResource!)
            .ToList();

        dbContext.ProjectImageUpdates.RemoveRange(existingPhotos);
        dbContext.FileResources.RemoveRange(orphanedPhotoFiles);

        var projectImages = await dbContext.ProjectImages
            .Where(x => x.ProjectID == project.ProjectID).ToListAsync();
        foreach (var img in projectImages)
        {
            dbContext.ProjectImageUpdates.Add(new ProjectImageUpdate
            {
                ProjectUpdateBatchID = batch.ProjectUpdateBatchID,
                FileResourceID = img.FileResourceID,
                ProjectImageID = img.ProjectImageID,
                Caption = img.Caption,
                Credit = img.Credit,
                IsKeyPhoto = img.IsKeyPhoto,
                ExcludeFromFactSheet = img.ExcludeFromFactSheet,
                ProjectImageTimingID = img.ProjectImageTimingID
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

        var projectPriorityLandscapes = await dbContext.ProjectPriorityLandscapes
            .Where(x => x.ProjectID == project.ProjectID).ToListAsync();
        foreach (var pl in projectPriorityLandscapes)
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

        var projectRegions = await dbContext.ProjectRegions
            .Where(x => x.ProjectID == project.ProjectID).ToListAsync();
        foreach (var pr in projectRegions)
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

        var projectCounties = await dbContext.ProjectCounties
            .Where(x => x.ProjectID == project.ProjectID).ToListAsync();
        foreach (var pc in projectCounties)
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
        var existingTreatments = await dbContext.TreatmentUpdates
            .Where(t => t.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();
        dbContext.TreatmentUpdates.RemoveRange(existingTreatments);

        // Get current location updates for FK mapping
        var locationUpdates = await dbContext.ProjectLocationUpdates
            .Where(plu => plu.ProjectUpdateBatchID == batch.ProjectUpdateBatchID)
            .ToListAsync();

        var projectTreatments = await dbContext.Treatments
            .Include(t => t.ProjectLocation)
            .Where(t => t.ProjectLocation.ProjectID == project.ProjectID).ToListAsync();
        foreach (var treatment in projectTreatments)
        {
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
        // Execute stored procedure to merge all update data back to project tables in a single transaction
        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"EXEC dbo.pCommitProjectUpdateToProject @ProjectUpdateBatchID={batch.ProjectUpdateBatchID}");
    }

    #endregion
}
