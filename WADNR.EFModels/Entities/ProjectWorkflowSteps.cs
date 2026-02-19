using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using WADNR.Common.GeoSpatial;
using WADNR.EFModels.Workflows;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

/// <summary>
/// Static helper methods for the ProjectCreate workflow steps.
/// </summary>
public static class ProjectWorkflowSteps
{
    #region Basics Step

    public static async Task<ProjectBasicsStep?> GetBasicsStepAsync(WADNRDbContext dbContext, int projectID)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectID == projectID)
            .Select(p => new ProjectBasicsStep
            {
                ProjectID = p.ProjectID,
                ProjectName = p.ProjectName,
                ProjectDescription = p.ProjectDescription,
                ProjectTypeID = p.ProjectTypeID,
                ProjectStageID = p.ProjectStageID,
                PlannedDate = p.PlannedDate,
                CompletionDate = p.CompletionDate,
                ExpirationDate = p.ExpirationDate,
                FocusAreaID = p.FocusAreaID,
                LeadImplementerOrganizationID = p.ProjectOrganizations
                    .Where(po => po.RelationshipType.IsPrimaryContact)
                    .Select(po => (int?)po.OrganizationID)
                    .FirstOrDefault(),
                PercentageMatch = p.PercentageMatch,
                ProgramIDs = p.ProjectPrograms.Select(pp => pp.ProgramID).ToList()
            })
            .SingleOrDefaultAsync();
    }

    public static async Task<ProjectBasicsStep> CreateProjectFromBasicsStepAsync(WADNRDbContext dbContext, ProjectBasicsStepRequest request, int callingPersonID)
    {
        // Generate FHT project number
        var fhtProjectNumber = await GenerateFhtProjectNumberAsync(dbContext);

        var project = new Project
        {
            ProjectName = request.ProjectName,
            ProjectDescription = request.ProjectDescription,
            ProjectTypeID = request.ProjectTypeID,
            ProjectStageID = request.ProjectStageID,
            PlannedDate = request.PlannedDate,
            CompletionDate = request.CompletionDate,
            ExpirationDate = request.ExpirationDate,
            FocusAreaID = request.FocusAreaID,
            PercentageMatch = request.PercentageMatch,
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Draft,
            ProjectLocationSimpleTypeID = 1, // Default to "None" until location is set
            ProposingPersonID = callingPersonID,
            ProposingDate = DateTime.UtcNow,
            IsFeatured = false,
            FhtProjectNumber = fhtProjectNumber
        };

        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync();

        // Add Lead Implementer organization if provided
        if (request.LeadImplementerOrganizationID.HasValue)
        {
            var leadImplementerRelationshipType = await dbContext.RelationshipTypes
                .FirstOrDefaultAsync(rt => rt.IsPrimaryContact);
            if (leadImplementerRelationshipType != null)
            {
                dbContext.ProjectOrganizations.Add(new ProjectOrganization
                {
                    ProjectID = project.ProjectID,
                    OrganizationID = request.LeadImplementerOrganizationID.Value,
                    RelationshipTypeID = leadImplementerRelationshipType.RelationshipTypeID
                });
            }
        }

        // Add programs
        foreach (var programID in request.ProgramIDs)
        {
            dbContext.ProjectPrograms.Add(new ProjectProgram
            {
                ProjectID = project.ProjectID,
                ProgramID = programID
            });
        }

        await dbContext.SaveChangesAsync();

        return (await GetBasicsStepAsync(dbContext, project.ProjectID))!;
    }

    public static async Task<ProjectBasicsStep?> SaveBasicsStepAsync(WADNRDbContext dbContext, int projectID, ProjectBasicsStepRequest request, int callingPersonID)
    {
        var project = await dbContext.Projects
            .Include(p => p.ProjectPrograms)
            .Include(p => p.ProjectOrganizations)
                .ThenInclude(po => po.RelationshipType)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        project.ProjectName = request.ProjectName;
        project.ProjectDescription = request.ProjectDescription;
        project.ProjectTypeID = request.ProjectTypeID;
        project.ProjectStageID = request.ProjectStageID;
        project.PlannedDate = request.PlannedDate;
        project.CompletionDate = request.CompletionDate;
        project.ExpirationDate = request.ExpirationDate;
        project.FocusAreaID = request.FocusAreaID;
        project.PercentageMatch = request.PercentageMatch;

        // Sync Lead Implementer organization
        var leadImplementerRelationshipType = await dbContext.RelationshipTypes
            .FirstOrDefaultAsync(rt => rt.IsPrimaryContact);
        if (leadImplementerRelationshipType != null)
        {
            var existingLeadImplementer = project.ProjectOrganizations
                .FirstOrDefault(po => po.RelationshipType.IsPrimaryContact);

            if (request.LeadImplementerOrganizationID.HasValue)
            {
                if (existingLeadImplementer != null)
                {
                    // Update existing
                    existingLeadImplementer.OrganizationID = request.LeadImplementerOrganizationID.Value;
                }
                else
                {
                    // Add new
                    dbContext.ProjectOrganizations.Add(new ProjectOrganization
                    {
                        ProjectID = projectID,
                        OrganizationID = request.LeadImplementerOrganizationID.Value,
                        RelationshipTypeID = leadImplementerRelationshipType.RelationshipTypeID
                    });
                }
            }
            else if (existingLeadImplementer != null)
            {
                // Remove existing if request doesn't have one
                dbContext.ProjectOrganizations.Remove(existingLeadImplementer);
            }
        }

        // Sync programs
        var existingProgramIDs = project.ProjectPrograms.Select(pp => pp.ProgramID).ToHashSet();
        var requestedProgramIDs = request.ProgramIDs.ToHashSet();

        // Remove programs not in request
        var toRemove = project.ProjectPrograms.Where(pp => !requestedProgramIDs.Contains(pp.ProgramID)).ToList();
        dbContext.ProjectPrograms.RemoveRange(toRemove);

        // Add new programs
        foreach (var programID in requestedProgramIDs.Where(id => !existingProgramIDs.Contains(id)))
        {
            dbContext.ProjectPrograms.Add(new ProjectProgram
            {
                ProjectID = projectID,
                ProgramID = programID
            });
        }

        await dbContext.SaveChangesAsync();

        return await GetBasicsStepAsync(dbContext, projectID);
    }

    private static async Task<string> GenerateFhtProjectNumberAsync(WADNRDbContext dbContext)
    {
        // Get the max FHT number and increment
        var maxNumber = await dbContext.Projects
            .Where(p => p.FhtProjectNumber.StartsWith("FHT-"))
            .Select(p => p.FhtProjectNumber)
            .ToListAsync();

        var maxNumeric = maxNumber
            .Select(n =>
            {
                var parts = n.Split('-');
                return parts.Length > 1 && int.TryParse(parts[1], out var num) ? num : 0;
            })
            .DefaultIfEmpty(0)
            .Max();

        return $"FHT-{maxNumeric + 1:D5}";
    }

    #endregion

    #region Location Simple Step

    public static async Task<LocationSimpleStep?> GetLocationSimpleStepAsync(WADNRDbContext dbContext, int projectID)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectID == projectID)
            .Select(p => new LocationSimpleStep
            {
                ProjectID = p.ProjectID,
                Latitude = p.ProjectLocationPoint != null ? p.ProjectLocationPoint.Coordinate.Y : null,
                Longitude = p.ProjectLocationPoint != null ? p.ProjectLocationPoint.Coordinate.X : null,
                ProjectLocationSimpleTypeID = p.ProjectLocationSimpleTypeID,
                ProjectLocationNotes = p.ProjectLocationNotes
            })
            .SingleOrDefaultAsync();
    }

    public static async Task<LocationSimpleStep?> SaveLocationSimpleStepAsync(WADNRDbContext dbContext, int projectID, LocationSimpleStepRequest request)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID);
        if (project == null) return null;

        // Create point geometry
        var point = new Point(request.Longitude, request.Latitude) { SRID = 4326 };
        project.ProjectLocationPoint = point;
        project.ProjectLocationSimpleTypeID = request.ProjectLocationSimpleTypeID;
        project.ProjectLocationNotes = request.ProjectLocationNotes;

        await dbContext.SaveChangesAsync();

        // Auto-populate geographic regions based on new location
        await AutoAssignGeographicRegionsAsync(dbContext, projectID);

        return await GetLocationSimpleStepAsync(dbContext, projectID);
    }

    #endregion

    #region Location Detailed Step

    public static async Task<LocationDetailedStep?> GetLocationDetailedStepAsync(WADNRDbContext dbContext, int projectID)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Include(p => p.ProjectLocations)
                .ThenInclude(pl => pl.Treatments)
            .Include(p => p.ProjectLocationStagings)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        return new LocationDetailedStep
        {
            ProjectID = projectID,
            Locations = project.ProjectLocations.Select(pl => new ProjectLocationItem
            {
                ProjectLocationID = pl.ProjectLocationID,
                ProjectID = pl.ProjectID,
                ProjectLocationTypeID = pl.ProjectLocationTypeID,
                ProjectLocationTypeName = ProjectLocationType.AllLookupDictionary.TryGetValue(pl.ProjectLocationTypeID, out var locType)
                    ? locType.ProjectLocationTypeDisplayName
                    : null,
                ProjectLocationNotes = pl.ProjectLocationNotes,
                ProjectLocationName = pl.ProjectLocationName,
                GeoJson = pl.ProjectLocationGeometry?.AsText(),
                AreaInAcres = pl.ProjectLocationGeometry != null ? pl.ProjectLocationGeometry.Area * 247.105 : null, // Convert sq degrees to acres (approximate)
                HasTreatments = pl.Treatments.Any(),
                IsFromArcGis = pl.ArcGisObjectID.HasValue
            }).ToList(),
            StagedLocations = project.ProjectLocationStagings.Select(pls => new ProjectLocationStagingItem
            {
                ProjectLocationStagingID = pls.ProjectLocationStagingID,
                ProjectID = pls.ProjectID,
                ProjectLocationTypeID = 0, // Staging doesn't have type
                ProjectLocationTypeName = pls.FeatureClassName,
                ProjectLocationNotes = null,
                ProjectLocationName = pls.SelectedProperty,
                GeoJson = pls.GeoJson,
                AreaInAcres = null, // Area calculated from GeoJson on client
                ToBeDeleted = !pls.ShouldImport
            }).ToList()
        };
    }

    public static async Task<LocationDetailedStep?> SaveLocationDetailedStepAsync(WADNRDbContext dbContext, int projectID, LocationDetailedStepRequest request)
    {
        var project = await dbContext.Projects
            .Include(p => p.ProjectLocations)
                .ThenInclude(pl => pl.Treatments)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        // Get existing location IDs
        var existingLocationIDs = project.ProjectLocations.Select(pl => pl.ProjectLocationID).ToHashSet();
        var requestLocationIDs = request.Locations.Where(l => l.ProjectLocationID.HasValue).Select(l => l.ProjectLocationID!.Value).ToHashSet();

        // Remove locations not in request — but guard against deleting locations with treatments
        var toRemove = project.ProjectLocations.Where(pl => !requestLocationIDs.Contains(pl.ProjectLocationID)).ToList();
        var locationsWithTreatments = toRemove.Where(pl => pl.Treatments.Any()).ToList();
        if (locationsWithTreatments.Count > 0)
        {
            var names = string.Join(", ", locationsWithTreatments.Select(pl => $"'{pl.ProjectLocationName}'"));
            throw new InvalidOperationException($"Cannot delete project location(s) {names} because they have associated Treatments. Remove the Treatments first.");
        }

        // Validate that locations with treatments keep their type as Treatment Area
        foreach (var locRequest in request.Locations.Where(l => l.ProjectLocationID.HasValue))
        {
            var existing = project.ProjectLocations.FirstOrDefault(pl => pl.ProjectLocationID == locRequest.ProjectLocationID!.Value);
            if (existing != null && existing.Treatments.Any() && locRequest.ProjectLocationTypeID != existing.ProjectLocationTypeID)
            {
                throw new InvalidOperationException($"Cannot change the location type of '{existing.ProjectLocationName}' because it has associated Treatments.");
            }
        }

        dbContext.ProjectLocations.RemoveRange(toRemove);

        // Update existing and add new
        foreach (var locRequest in request.Locations)
        {
            if (locRequest.ProjectLocationID.HasValue && existingLocationIDs.Contains(locRequest.ProjectLocationID.Value))
            {
                // Update existing
                var existing = project.ProjectLocations.First(pl => pl.ProjectLocationID == locRequest.ProjectLocationID.Value);
                existing.ProjectLocationTypeID = locRequest.ProjectLocationTypeID;
                existing.ProjectLocationNotes = locRequest.ProjectLocationNotes;
                existing.ProjectLocationName = locRequest.ProjectLocationName;
                // Parse GeoJSON to geometry if needed
                if (!string.IsNullOrEmpty(locRequest.GeoJson))
                {
                    var reader = new NetTopologySuite.IO.WKTReader();
                    existing.ProjectLocationGeometry = reader.Read(locRequest.GeoJson).MakeValid();
                    existing.ProjectLocationGeometry.SRID = 4326;
                }
            }
            else
            {
                // Add new
                var newLocation = new ProjectLocation
                {
                    ProjectID = projectID,
                    ProjectLocationTypeID = locRequest.ProjectLocationTypeID,
                    ProjectLocationNotes = locRequest.ProjectLocationNotes,
                    ProjectLocationName = locRequest.ProjectLocationName
                };

                if (!string.IsNullOrEmpty(locRequest.GeoJson))
                {
                    var reader = new NetTopologySuite.IO.WKTReader();
                    newLocation.ProjectLocationGeometry = reader.Read(locRequest.GeoJson).MakeValid();
                    newLocation.ProjectLocationGeometry.SRID = 4326;
                }

                dbContext.ProjectLocations.Add(newLocation);
            }
        }

        await dbContext.SaveChangesAsync();

        // Auto-populate geographic regions based on updated locations
        await AutoAssignGeographicRegionsAsync(dbContext, projectID);

        return await GetLocationDetailedStepAsync(dbContext, projectID);
    }

    public static async Task<LocationDetailedStep?> ApproveGdbImportAsync(WADNRDbContext dbContext, int projectID, int personID, GdbApproveRequest request)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID);
        if (project == null) return null;

        var stagingRows = await dbContext.ProjectLocationStagings
            .Where(s => s.ProjectID == projectID && s.PersonID == personID)
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

                dbContext.ProjectLocations.Add(new ProjectLocation
                {
                    ProjectID = projectID,
                    ProjectLocationTypeID = (int)ProjectLocationTypeEnum.ProjectArea,
                    ProjectLocationName = locationName,
                    ProjectLocationGeometry = geometry,
                    ImportedFromGisUpload = true
                });

                locationIndex++;
            }
        }

        // Clean up staging rows
        dbContext.ProjectLocationStagings.RemoveRange(stagingRows);
        await dbContext.SaveChangesAsync();

        // Auto-populate geographic regions based on updated locations
        await AutoAssignGeographicRegionsAsync(dbContext, projectID);

        return await GetLocationDetailedStepAsync(dbContext, projectID);
    }

    #endregion

    #region Geographic Assignment Steps

    public static async Task<GeographicAssignmentStep?> GetPriorityLandscapesStepAsync(WADNRDbContext dbContext, int projectID)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Include(p => p.ProjectPriorityLandscapes)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        var allLandscapes = await dbContext.PriorityLandscapes
            .AsNoTracking()
            .OrderBy(pl => pl.PriorityLandscapeName)
            .Select(pl => new GeographicLookupItem
            {
                ID = pl.PriorityLandscapeID,
                DisplayName = pl.PriorityLandscapeName
            })
            .ToListAsync();

        return new GeographicAssignmentStep
        {
            ProjectID = projectID,
            SelectedIDs = project.ProjectPriorityLandscapes.Select(ppl => ppl.PriorityLandscapeID).ToList(),
            NoSelectionExplanation = project.NoPriorityLandscapesExplanation,
            AvailableOptions = allLandscapes
        };
    }

    public static async Task<GeographicAssignmentStep?> SavePriorityLandscapesStepAsync(WADNRDbContext dbContext, int projectID, GeographicOverrideRequest request)
    {
        var project = await dbContext.Projects
            .Include(p => p.ProjectPriorityLandscapes)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        // Sync priority landscapes
        var existingIDs = project.ProjectPriorityLandscapes.Select(ppl => ppl.PriorityLandscapeID).ToHashSet();
        var requestedIDs = request.SelectedIDs.ToHashSet();

        // Remove not in request
        var toRemove = project.ProjectPriorityLandscapes.Where(ppl => !requestedIDs.Contains(ppl.PriorityLandscapeID)).ToList();
        dbContext.ProjectPriorityLandscapes.RemoveRange(toRemove);

        // Add new
        foreach (var id in requestedIDs.Where(id => !existingIDs.Contains(id)))
        {
            dbContext.ProjectPriorityLandscapes.Add(new ProjectPriorityLandscape
            {
                ProjectID = projectID,
                PriorityLandscapeID = id
            });
        }

        project.NoPriorityLandscapesExplanation = request.NoSelectionExplanation;

        await dbContext.SaveChangesAsync();

        return await GetPriorityLandscapesStepAsync(dbContext, projectID);
    }

    public static async Task<GeographicAssignmentStep?> GetDnrUplandRegionsStepAsync(WADNRDbContext dbContext, int projectID)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Include(p => p.ProjectRegions)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        var allRegions = await dbContext.DNRUplandRegions
            .AsNoTracking()
            .OrderBy(r => r.DNRUplandRegionName)
            .Select(r => new GeographicLookupItem
            {
                ID = r.DNRUplandRegionID,
                DisplayName = r.DNRUplandRegionName
            })
            .ToListAsync();

        return new GeographicAssignmentStep
        {
            ProjectID = projectID,
            SelectedIDs = project.ProjectRegions.Select(pr => pr.DNRUplandRegionID).ToList(),
            NoSelectionExplanation = project.NoRegionsExplanation,
            AvailableOptions = allRegions
        };
    }

    public static async Task<GeographicAssignmentStep?> SaveDnrUplandRegionsStepAsync(WADNRDbContext dbContext, int projectID, GeographicOverrideRequest request)
    {
        var project = await dbContext.Projects
            .Include(p => p.ProjectRegions)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        // Sync regions
        var existingIDs = project.ProjectRegions.Select(pr => pr.DNRUplandRegionID).ToHashSet();
        var requestedIDs = request.SelectedIDs.ToHashSet();

        // Remove not in request
        var toRemove = project.ProjectRegions.Where(pr => !requestedIDs.Contains(pr.DNRUplandRegionID)).ToList();
        dbContext.ProjectRegions.RemoveRange(toRemove);

        // Add new
        foreach (var id in requestedIDs.Where(id => !existingIDs.Contains(id)))
        {
            dbContext.ProjectRegions.Add(new ProjectRegion
            {
                ProjectID = projectID,
                DNRUplandRegionID = id
            });
        }

        project.NoRegionsExplanation = request.NoSelectionExplanation;

        await dbContext.SaveChangesAsync();

        return await GetDnrUplandRegionsStepAsync(dbContext, projectID);
    }

    public static async Task<GeographicAssignmentStep?> GetCountiesStepAsync(WADNRDbContext dbContext, int projectID)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Include(p => p.ProjectCounties)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        var allCounties = await dbContext.Counties
            .AsNoTracking()
            .OrderBy(c => c.CountyName)
            .Select(c => new GeographicLookupItem
            {
                ID = c.CountyID,
                DisplayName = c.CountyName
            })
            .ToListAsync();

        return new GeographicAssignmentStep
        {
            ProjectID = projectID,
            SelectedIDs = project.ProjectCounties.Select(pc => pc.CountyID).ToList(),
            NoSelectionExplanation = project.NoCountiesExplanation,
            AvailableOptions = allCounties
        };
    }

    public static async Task<GeographicAssignmentStep?> SaveCountiesStepAsync(WADNRDbContext dbContext, int projectID, GeographicOverrideRequest request)
    {
        var project = await dbContext.Projects
            .Include(p => p.ProjectCounties)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        // Sync counties
        var existingIDs = project.ProjectCounties.Select(pc => pc.CountyID).ToHashSet();
        var requestedIDs = request.SelectedIDs.ToHashSet();

        // Remove not in request
        var toRemove = project.ProjectCounties.Where(pc => !requestedIDs.Contains(pc.CountyID)).ToList();
        dbContext.ProjectCounties.RemoveRange(toRemove);

        // Add new
        foreach (var id in requestedIDs.Where(id => !existingIDs.Contains(id)))
        {
            dbContext.ProjectCounties.Add(new ProjectCounty
            {
                ProjectID = projectID,
                CountyID = id
            });
        }

        project.NoCountiesExplanation = request.NoSelectionExplanation;

        await dbContext.SaveChangesAsync();

        return await GetCountiesStepAsync(dbContext, projectID);
    }

    /// <summary>
    /// Auto-assigns geographic regions (priority landscapes, DNR upland regions, counties)
    /// based on project location intersections.
    /// </summary>
    private static async Task AutoAssignGeographicRegionsAsync(WADNRDbContext dbContext, int projectID)
    {
        var project = await dbContext.Projects
            .Include(p => p.ProjectLocations)
            .Include(p => p.ProjectPriorityLandscapes)
            .Include(p => p.ProjectRegions)
            .Include(p => p.ProjectCounties)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return;

        // Build combined geometry from simple and detailed locations
        var geometries = new List<Geometry>();
        if (project.ProjectLocationPoint != null)
        {
            geometries.Add(project.ProjectLocationPoint);
        }
        foreach (var loc in project.ProjectLocations)
        {
            if (loc.ProjectLocationGeometry != null)
            {
                geometries.Add(loc.ProjectLocationGeometry);
            }
        }

        if (geometries.Count == 0)
        {
            // No locations to intersect - set explanations
            project.NoPriorityLandscapesExplanation ??= "Neither the simple location nor the detailed location on this project intersects with any Priority Landscape.";
            project.NoRegionsExplanation ??= "Neither the simple location nor the detailed location on this project intersects with any DNR Upland Region.";
            project.NoCountiesExplanation ??= "Neither the simple location nor the detailed location on this project intersects with any County.";
            await dbContext.SaveChangesAsync();
            return;
        }

        // Create union of all geometries for intersection queries
        var combinedGeometry = geometries.Count == 1 ? geometries[0] : new GeometryCollection(geometries.ToArray()).Union();

        // Find intersecting priority landscapes
        var priorityLandscapeIDs = await dbContext.PriorityLandscapes
            .AsNoTracking()
            .Where(pl => pl.PriorityLandscapeLocation.Intersects(combinedGeometry))
            .Select(pl => pl.PriorityLandscapeID)
            .ToListAsync();

        // Clear and repopulate priority landscapes
        dbContext.ProjectPriorityLandscapes.RemoveRange(project.ProjectPriorityLandscapes);
        foreach (var id in priorityLandscapeIDs)
        {
            dbContext.ProjectPriorityLandscapes.Add(new ProjectPriorityLandscape
            {
                ProjectID = projectID,
                PriorityLandscapeID = id
            });
        }
        project.NoPriorityLandscapesExplanation = priorityLandscapeIDs.Count == 0
            ? "Neither the simple location nor the detailed location on this project intersects with any Priority Landscape."
            : null;

        // Find intersecting DNR upland regions
        var regionIDs = await dbContext.DNRUplandRegions
            .AsNoTracking()
            .Where(r => r.DNRUplandRegionLocation.Intersects(combinedGeometry))
            .Select(r => r.DNRUplandRegionID)
            .ToListAsync();

        // Clear and repopulate regions
        dbContext.ProjectRegions.RemoveRange(project.ProjectRegions);
        foreach (var id in regionIDs)
        {
            dbContext.ProjectRegions.Add(new ProjectRegion
            {
                ProjectID = projectID,
                DNRUplandRegionID = id
            });
        }
        project.NoRegionsExplanation = regionIDs.Count == 0
            ? "Neither the simple location nor the detailed location on this project intersects with any DNR Upland Region."
            : null;

        // Find intersecting counties
        var countyIDs = await dbContext.Counties
            .AsNoTracking()
            .Where(c => c.CountyFeature.Intersects(combinedGeometry))
            .Select(c => c.CountyID)
            .ToListAsync();

        // Clear and repopulate counties
        dbContext.ProjectCounties.RemoveRange(project.ProjectCounties);
        foreach (var id in countyIDs)
        {
            dbContext.ProjectCounties.Add(new ProjectCounty
            {
                ProjectID = projectID,
                CountyID = id
            });
        }
        project.NoCountiesExplanation = countyIDs.Count == 0
            ? "Neither the simple location nor the detailed location on this project intersects with any County."
            : null;

        await dbContext.SaveChangesAsync();
    }

    #endregion

    #region Organizations Step

    public static async Task<ProjectOrganizationsStep?> GetOrganizationsStepAsync(WADNRDbContext dbContext, int projectID)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectID == projectID)
            .Select(p => new ProjectOrganizationsStep
            {
                ProjectID = p.ProjectID,
                Organizations = p.ProjectOrganizations.Select(po => new ProjectOrganizationStepItem
                {
                    ProjectOrganizationID = po.ProjectOrganizationID,
                    OrganizationID = po.OrganizationID,
                    OrganizationName = po.Organization.DisplayName,
                    RelationshipTypeID = po.RelationshipTypeID,
                    RelationshipTypeName = po.RelationshipType.RelationshipTypeName,
                    IsPrimaryContact = po.RelationshipType.IsPrimaryContact
                }).ToList()
            })
            .SingleOrDefaultAsync();
    }

    public static async Task<ProjectOrganizationsStep?> SaveOrganizationsStepAsync(WADNRDbContext dbContext, int projectID, ProjectOrganizationsStepRequest request)
    {
        var project = await dbContext.Projects
            .Include(p => p.ProjectOrganizations)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        // Sync organizations
        var existingIDs = project.ProjectOrganizations.Select(po => po.ProjectOrganizationID).ToHashSet();
        var requestIDs = request.Organizations.Where(o => o.ProjectOrganizationID.HasValue).Select(o => o.ProjectOrganizationID!.Value).ToHashSet();

        // Remove not in request
        var toRemove = project.ProjectOrganizations.Where(po => !requestIDs.Contains(po.ProjectOrganizationID)).ToList();
        dbContext.ProjectOrganizations.RemoveRange(toRemove);

        // Update existing and add new
        foreach (var orgRequest in request.Organizations)
        {
            if (orgRequest.ProjectOrganizationID.HasValue && existingIDs.Contains(orgRequest.ProjectOrganizationID.Value))
            {
                // Update existing
                var existing = project.ProjectOrganizations.First(po => po.ProjectOrganizationID == orgRequest.ProjectOrganizationID.Value);
                existing.OrganizationID = orgRequest.OrganizationID;
                existing.RelationshipTypeID = orgRequest.RelationshipTypeID;
            }
            else
            {
                // Add new
                dbContext.ProjectOrganizations.Add(new ProjectOrganization
                {
                    ProjectID = projectID,
                    OrganizationID = orgRequest.OrganizationID,
                    RelationshipTypeID = orgRequest.RelationshipTypeID
                });
            }
        }

        await dbContext.SaveChangesAsync();

        return await GetOrganizationsStepAsync(dbContext, projectID);
    }

    #endregion

    #region Contacts Step

    public static async Task<ProjectContactsStep?> GetContactsStepAsync(WADNRDbContext dbContext, int projectID)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectID == projectID)
            .Select(p => new ProjectContactsStep
            {
                ProjectID = p.ProjectID,
                Contacts = p.ProjectPeople.Select(pp => new ProjectContactStepItem
                {
                    ProjectPersonID = pp.ProjectPersonID,
                    PersonID = pp.PersonID,
                    PersonFullName = pp.Person.FirstName + " " + pp.Person.LastName,
                    ProjectPersonRelationshipTypeID = pp.ProjectPersonRelationshipTypeID,
                    RelationshipTypeName = pp.ProjectPersonRelationshipType.ProjectPersonRelationshipTypeName
                }).ToList()
            })
            .SingleOrDefaultAsync();
    }

    public static async Task<ProjectContactsStep?> SaveContactsStepAsync(WADNRDbContext dbContext, int projectID, ProjectContactsStepRequest request)
    {
        var project = await dbContext.Projects
            .Include(p => p.ProjectPeople)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        // Sync contacts
        var existingIDs = project.ProjectPeople.Select(pp => pp.ProjectPersonID).ToHashSet();
        var requestIDs = request.Contacts.Where(c => c.ProjectPersonID.HasValue).Select(c => c.ProjectPersonID!.Value).ToHashSet();

        // Remove not in request
        var toRemove = project.ProjectPeople.Where(pp => !requestIDs.Contains(pp.ProjectPersonID)).ToList();
        dbContext.ProjectPeople.RemoveRange(toRemove);

        // Update existing and add new
        foreach (var contactRequest in request.Contacts)
        {
            if (contactRequest.ProjectPersonID.HasValue && existingIDs.Contains(contactRequest.ProjectPersonID.Value))
            {
                // Update existing
                var existing = project.ProjectPeople.First(pp => pp.ProjectPersonID == contactRequest.ProjectPersonID.Value);
                existing.PersonID = contactRequest.PersonID;
                existing.ProjectPersonRelationshipTypeID = contactRequest.ProjectPersonRelationshipTypeID;
            }
            else
            {
                // Add new
                dbContext.ProjectPeople.Add(new ProjectPerson
                {
                    ProjectID = projectID,
                    PersonID = contactRequest.PersonID,
                    ProjectPersonRelationshipTypeID = contactRequest.ProjectPersonRelationshipTypeID
                });
            }
        }

        await dbContext.SaveChangesAsync();

        return await GetContactsStepAsync(dbContext, projectID);
    }

    #endregion

    #region Expected Funding Step

    public static async Task<ExpectedFundingStep?> GetExpectedFundingStepAsync(WADNRDbContext dbContext, int projectID)
    {
        var project = await dbContext.Projects
            .AsNoTracking()
            .Include(p => p.ProjectFundingSources)
            .Include(p => p.ProjectFundSourceAllocationRequests)
            .ThenInclude(ar => ar.FundSourceAllocation)
            .ThenInclude(fsa => fsa.FundSource)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        var dto = new ExpectedFundingStep
        {
            ProjectID = projectID,
            EstimatedTotalCost = project.EstimatedTotalCost,
            ProjectFundingSourceNotes = project.ProjectFundingSourceNotes,
            SelectedFundingSourceIDs = project.ProjectFundingSources.Select(pfs => pfs.FundingSourceID).ToList(),
            AllocationRequests = project.ProjectFundSourceAllocationRequests.Select(ar => new FundSourceAllocationRequestStepItem
            {
                ProjectFundSourceAllocationRequestID = ar.ProjectFundSourceAllocationRequestID,
                FundSourceAllocationID = ar.FundSourceAllocationID,
                FundSourceAllocationName = ar.FundSourceAllocation.FundSourceAllocationName,
                FundSourceName = ar.FundSourceAllocation.FundSource.FundSourceName,
                TotalAmount = ar.TotalAmount
            }).ToList()
        };

        return dto;
    }

    public static async Task<ExpectedFundingStep?> SaveExpectedFundingStepAsync(WADNRDbContext dbContext, int projectID, ExpectedFundingStepRequest request)
    {
        var project = await dbContext.Projects
            .Include(p => p.ProjectFundingSources)
            .Include(p => p.ProjectFundSourceAllocationRequests)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        project.EstimatedTotalCost = request.EstimatedTotalCost;
        project.ProjectFundingSourceNotes = request.ProjectFundingSourceNotes;

        // Sync funding sources (checkboxes)
        var existingFundingSourceIDs = project.ProjectFundingSources.Select(pfs => pfs.FundingSourceID).ToHashSet();
        var requestedFundingSourceIDs = request.FundingSourceIDs.ToHashSet();

        var toRemoveFundingSources = project.ProjectFundingSources.Where(pfs => !requestedFundingSourceIDs.Contains(pfs.FundingSourceID)).ToList();
        dbContext.ProjectFundingSources.RemoveRange(toRemoveFundingSources);

        foreach (var fundingSourceID in requestedFundingSourceIDs.Where(id => !existingFundingSourceIDs.Contains(id)))
        {
            dbContext.ProjectFundingSources.Add(new ProjectFundingSource
            {
                ProjectID = projectID,
                FundingSourceID = fundingSourceID
            });
        }

        // Sync allocation requests
        var existingArIDs = project.ProjectFundSourceAllocationRequests.Select(ar => ar.ProjectFundSourceAllocationRequestID).ToHashSet();
        var requestArIDs = request.AllocationRequests.Where(ar => ar.ProjectFundSourceAllocationRequestID.HasValue).Select(ar => ar.ProjectFundSourceAllocationRequestID!.Value).ToHashSet();

        var toRemoveAr = project.ProjectFundSourceAllocationRequests.Where(ar => !requestArIDs.Contains(ar.ProjectFundSourceAllocationRequestID)).ToList();
        dbContext.ProjectFundSourceAllocationRequests.RemoveRange(toRemoveAr);

        foreach (var arRequest in request.AllocationRequests)
        {
            if (arRequest.ProjectFundSourceAllocationRequestID.HasValue && existingArIDs.Contains(arRequest.ProjectFundSourceAllocationRequestID.Value))
            {
                var existing = project.ProjectFundSourceAllocationRequests.First(ar => ar.ProjectFundSourceAllocationRequestID == arRequest.ProjectFundSourceAllocationRequestID.Value);
                existing.FundSourceAllocationID = arRequest.FundSourceAllocationID;
                existing.TotalAmount = arRequest.TotalAmount;
                existing.UpdateDate = DateTime.Now;
            }
            else
            {
                dbContext.ProjectFundSourceAllocationRequests.Add(new ProjectFundSourceAllocationRequest
                {
                    ProjectID = projectID,
                    FundSourceAllocationID = arRequest.FundSourceAllocationID,
                    TotalAmount = arRequest.TotalAmount,
                    CreateDate = DateTime.Now
                });
            }
        }

        await dbContext.SaveChangesAsync();

        return await GetExpectedFundingStepAsync(dbContext, projectID);
    }

    #endregion

    #region Classifications Step

    public static async Task<ProjectClassificationsStep?> GetClassificationsStepAsync(WADNRDbContext dbContext, int projectID)
    {
        return await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectID == projectID)
            .Select(p => new ProjectClassificationsStep
            {
                ProjectID = p.ProjectID,
                Classifications = p.ProjectClassifications.Select(pc => new ProjectClassificationStepItem
                {
                    ProjectClassificationID = pc.ProjectClassificationID,
                    ClassificationID = pc.ClassificationID,
                    ClassificationName = pc.Classification.DisplayName,
                    ClassificationSystemID = pc.Classification.ClassificationSystemID,
                    ClassificationSystemName = pc.Classification.ClassificationSystem.ClassificationSystemName,
                    ProjectClassificationNotes = pc.ProjectClassificationNotes
                }).ToList()
            })
            .SingleOrDefaultAsync();
    }

    public static async Task<ProjectClassificationsStep?> SaveClassificationsStepAsync(WADNRDbContext dbContext, int projectID, ProjectClassificationsStepRequest request)
    {
        var project = await dbContext.Projects
            .Include(p => p.ProjectClassifications)
            .FirstOrDefaultAsync(p => p.ProjectID == projectID);

        if (project == null) return null;

        // Sync classifications
        var existingIDs = project.ProjectClassifications.Select(pc => pc.ProjectClassificationID).ToHashSet();
        var requestIDs = request.Classifications.Where(c => c.ProjectClassificationID.HasValue).Select(c => c.ProjectClassificationID!.Value).ToHashSet();

        var toRemove = project.ProjectClassifications.Where(pc => !requestIDs.Contains(pc.ProjectClassificationID)).ToList();
        dbContext.ProjectClassifications.RemoveRange(toRemove);

        foreach (var classRequest in request.Classifications)
        {
            if (classRequest.ProjectClassificationID.HasValue && existingIDs.Contains(classRequest.ProjectClassificationID.Value))
            {
                var existing = project.ProjectClassifications.First(pc => pc.ProjectClassificationID == classRequest.ProjectClassificationID.Value);
                existing.ClassificationID = classRequest.ClassificationID;
                existing.ProjectClassificationNotes = classRequest.ProjectClassificationNotes;
            }
            else
            {
                dbContext.ProjectClassifications.Add(new ProjectClassification
                {
                    ProjectID = projectID,
                    ClassificationID = classRequest.ClassificationID,
                    ProjectClassificationNotes = classRequest.ProjectClassificationNotes
                });
            }
        }

        await dbContext.SaveChangesAsync();

        return await GetClassificationsStepAsync(dbContext, projectID);
    }

    #endregion

    #region State Transitions

    public static async Task<WorkflowStateTransitionResponse> SubmitForApprovalAsync(WADNRDbContext dbContext, int projectID, int callingPersonID, string? comment)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID);
        if (project == null)
        {
            return new WorkflowStateTransitionResponse
            {
                ProjectID = projectID,
                Success = false,
                ErrorMessage = "Project not found."
            };
        }

        // Check current status allows submission
        if (project.ProjectApprovalStatusID != (int)ProjectApprovalStatusEnum.Draft &&
            project.ProjectApprovalStatusID != (int)ProjectApprovalStatusEnum.Returned)
        {
            return new WorkflowStateTransitionResponse
            {
                ProjectID = projectID,
                Success = false,
                ErrorMessage = "Project can only be submitted when in Draft or Returned status."
            };
        }

        // Check all required steps are complete
        var canSubmit = await ProjectCreateWorkflowProgress.CanSubmitAsync(dbContext, projectID);
        if (!canSubmit)
        {
            return new WorkflowStateTransitionResponse
            {
                ProjectID = projectID,
                Success = false,
                ErrorMessage = "Not all required steps are complete."
            };
        }

        project.ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.PendingApproval;
        project.SubmissionDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return new WorkflowStateTransitionResponse
        {
            ProjectID = projectID,
            NewProjectApprovalStatusID = project.ProjectApprovalStatusID,
            NewProjectApprovalStatusName = ProjectApprovalStatus.PendingApproval.ProjectApprovalStatusDisplayName,
            TransitionDate = project.SubmissionDate!.Value,
            Success = true
        };
    }

    public static async Task<WorkflowStateTransitionResponse> ApproveAsync(WADNRDbContext dbContext, int projectID, int callingPersonID, string? comment)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID);
        if (project == null)
        {
            return new WorkflowStateTransitionResponse
            {
                ProjectID = projectID,
                Success = false,
                ErrorMessage = "Project not found."
            };
        }

        if (project.ProjectApprovalStatusID != (int)ProjectApprovalStatusEnum.PendingApproval)
        {
            return new WorkflowStateTransitionResponse
            {
                ProjectID = projectID,
                Success = false,
                ErrorMessage = "Project can only be approved when in Pending Approval status."
            };
        }

        project.ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved;
        project.ApprovalDate = DateTime.UtcNow;
        project.ReviewedByPersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return new WorkflowStateTransitionResponse
        {
            ProjectID = projectID,
            NewProjectApprovalStatusID = project.ProjectApprovalStatusID,
            NewProjectApprovalStatusName = ProjectApprovalStatus.Approved.ProjectApprovalStatusDisplayName,
            TransitionDate = project.ApprovalDate!.Value,
            Success = true
        };
    }

    public static async Task<WorkflowStateTransitionResponse> ReturnAsync(WADNRDbContext dbContext, int projectID, int callingPersonID, string? comment)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID);
        if (project == null)
        {
            return new WorkflowStateTransitionResponse
            {
                ProjectID = projectID,
                Success = false,
                ErrorMessage = "Project not found."
            };
        }

        if (project.ProjectApprovalStatusID != (int)ProjectApprovalStatusEnum.PendingApproval)
        {
            return new WorkflowStateTransitionResponse
            {
                ProjectID = projectID,
                Success = false,
                ErrorMessage = "Project can only be returned when in Pending Approval status."
            };
        }

        project.ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Returned;
        project.ReviewedByPersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return new WorkflowStateTransitionResponse
        {
            ProjectID = projectID,
            NewProjectApprovalStatusID = project.ProjectApprovalStatusID,
            NewProjectApprovalStatusName = ProjectApprovalStatus.Returned.ProjectApprovalStatusDisplayName,
            TransitionDate = DateTime.UtcNow,
            Success = true
        };
    }

    public static async Task<WorkflowStateTransitionResponse> RejectAsync(WADNRDbContext dbContext, int projectID, int callingPersonID, string? comment)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID);
        if (project == null)
        {
            return new WorkflowStateTransitionResponse
            {
                ProjectID = projectID,
                Success = false,
                ErrorMessage = "Project not found."
            };
        }

        if (project.ProjectApprovalStatusID != (int)ProjectApprovalStatusEnum.PendingApproval)
        {
            return new WorkflowStateTransitionResponse
            {
                ProjectID = projectID,
                Success = false,
                ErrorMessage = "Project can only be rejected when in Pending Approval status."
            };
        }

        project.ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Rejected;
        project.ReviewedByPersonID = callingPersonID;

        await dbContext.SaveChangesAsync();

        return new WorkflowStateTransitionResponse
        {
            ProjectID = projectID,
            NewProjectApprovalStatusID = project.ProjectApprovalStatusID,
            NewProjectApprovalStatusName = ProjectApprovalStatus.Rejected.ProjectApprovalStatusDisplayName,
            TransitionDate = DateTime.UtcNow,
            Success = true
        };
    }

    public static async Task<WorkflowStateTransitionResponse> WithdrawAsync(WADNRDbContext dbContext, int projectID, int callingPersonID, string? comment)
    {
        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.ProjectID == projectID);
        if (project == null)
        {
            return new WorkflowStateTransitionResponse
            {
                ProjectID = projectID,
                Success = false,
                ErrorMessage = "Project not found."
            };
        }

        if (project.ProjectApprovalStatusID != (int)ProjectApprovalStatusEnum.PendingApproval)
        {
            return new WorkflowStateTransitionResponse
            {
                ProjectID = projectID,
                Success = false,
                ErrorMessage = "Project can only be withdrawn when in Pending Approval status."
            };
        }

        project.ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Draft;
        project.SubmissionDate = null;

        await dbContext.SaveChangesAsync();

        return new WorkflowStateTransitionResponse
        {
            ProjectID = projectID,
            NewProjectApprovalStatusID = project.ProjectApprovalStatusID,
            NewProjectApprovalStatusName = ProjectApprovalStatus.Draft.ProjectApprovalStatusDisplayName,
            TransitionDate = DateTime.UtcNow,
            Success = true
        };
    }

    #endregion
}
