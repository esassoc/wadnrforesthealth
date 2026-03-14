using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.IO.Converters;
using WADNR.Common.GeoSpatial;
using WADNR.Models.DataTransferObjects.GisBulkImport;

namespace WADNR.EFModels.Entities;

public static class GisBulkImports
{
    public static async Task<List<GisUploadSourceOrganizationSummary>> ListSourceOrganizationsAsync(WADNRDbContext dbContext)
    {
        return await dbContext.GisUploadSourceOrganizations
            .AsNoTracking()
            .OrderBy(x => x.GisUploadSourceOrganizationName)
            .Select(GisUploadSourceOrganizationProjections.AsSummary)
            .ToListAsync();
    }

    public static async Task<GisUploadAttemptDetail> CreateAttemptAsync(WADNRDbContext dbContext, int gisUploadSourceOrganizationID, int personID)
    {
        var attempt = new GisUploadAttempt
        {
            GisUploadSourceOrganizationID = gisUploadSourceOrganizationID,
            GisUploadAttemptCreatePersonID = personID,
            GisUploadAttemptCreateDate = DateTime.UtcNow
        };

        dbContext.GisUploadAttempts.Add(attempt);
        await dbContext.SaveChangesWithNoAuditingAsync();

        return await GetAttemptDetailAsync(dbContext, attempt.GisUploadAttemptID);
    }

    public static async Task<GisUploadAttemptDetail?> GetAttemptDetailAsync(WADNRDbContext dbContext, int gisUploadAttemptID)
    {
        return await dbContext.GisUploadAttempts
            .AsNoTracking()
            .Where(x => x.GisUploadAttemptID == gisUploadAttemptID)
            .Select(x => new GisUploadAttemptDetail
            {
                GisUploadAttemptID = x.GisUploadAttemptID,
                GisUploadSourceOrganizationID = x.GisUploadSourceOrganizationID,
                GisUploadSourceOrganizationName = x.GisUploadSourceOrganization.GisUploadSourceOrganizationName,
                GisUploadAttemptCreateDate = x.GisUploadAttemptCreateDate,
                CreatedByPersonName = x.GisUploadAttemptCreatePerson.FirstName + " " + x.GisUploadAttemptCreatePerson.LastName,
                FileUploadSuccessful = x.FileUploadSuccessful,
                FeaturesSaved = x.FeaturesSaved,
                AttributesSaved = x.AttributesSaved,
                AreaCalculationComplete = x.AreaCalculationComplete,
                ImportedToGeoJson = x.ImportedToGeoJson,
                FeatureCount = x.GisFeatures.Count
            })
            .SingleOrDefaultAsync();
    }

    public static async Task UploadAndProcessFileAsync(WADNRDbContext dbContext, int gisUploadAttemptID, string geoJson)
    {
        var attempt = await dbContext.GisUploadAttempts
            .FirstAsync(x => x.GisUploadAttemptID == gisUploadAttemptID);

        // Clear any existing data from a previous upload on this attempt (bulk SQL deletes)
        await dbContext.GisFeatureMetadataAttributes
            .Where(x => dbContext.GisFeatures
                .Where(f => f.GisUploadAttemptID == gisUploadAttemptID)
                .Select(f => f.GisFeatureID)
                .Contains(x.GisFeatureID))
            .ExecuteDeleteAsync();

        await dbContext.GisFeatures
            .Where(x => x.GisUploadAttemptID == gisUploadAttemptID)
            .ExecuteDeleteAsync();

        await dbContext.GisUploadAttemptGisMetadataAttributes
            .Where(x => x.GisUploadAttemptID == gisUploadAttemptID)
            .ExecuteDeleteAsync();

        var jsonOptions = new JsonSerializerOptions();
        jsonOptions.Converters.Add(new GeoJsonConverterFactory());

        var featureCollection = JsonSerializer.Deserialize<NetTopologySuite.Features.FeatureCollection>(geoJson, jsonOptions);
        if (featureCollection == null || featureCollection.Count == 0)
        {
            attempt.FileUploadSuccessful = false;
            await dbContext.SaveChangesWithNoAuditingAsync();
            return;
        }

        attempt.FileUploadSuccessful = true;

        // Collect all unique attribute names across features
        var allAttributeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var feature in featureCollection)
        {
            if (feature.Attributes != null)
            {
                foreach (var name in feature.Attributes.GetNames())
                {
                    allAttributeNames.Add(name);
                }
            }
        }

        // Create or find GisMetadataAttribute records for each column
        var existingAttributes = await dbContext.GisMetadataAttributes.ToListAsync();
        var attributeDictionary = existingAttributes.ToDictionary(x => x.GisMetadataAttributeName.ToLowerInvariant(), StringComparer.OrdinalIgnoreCase);

        foreach (var attrName in allAttributeNames)
        {
            if (!attributeDictionary.ContainsKey(attrName.ToLowerInvariant()))
            {
                var newAttr = new GisMetadataAttribute
                {
                    GisMetadataAttributeName = attrName.ToLowerInvariant()
                };
                dbContext.GisMetadataAttributes.Add(newAttr);
                attributeDictionary[attrName.ToLowerInvariant()] = newAttr;
            }
        }
        await dbContext.SaveChangesWithNoAuditingAsync();

        // Create GisUploadAttemptGisMetadataAttribute records (column headers for this upload)
        var sortOrder = 0;
        foreach (var attrName in allAttributeNames.OrderBy(x => x))
        {
            var metadataAttr = attributeDictionary[attrName.ToLowerInvariant()];
            dbContext.GisUploadAttemptGisMetadataAttributes.Add(new GisUploadAttemptGisMetadataAttribute
            {
                GisUploadAttemptID = gisUploadAttemptID,
                GisMetadataAttributeID = metadataAttr.GisMetadataAttributeID,
                SortOrder = sortOrder++
            });
        }

        // Phase 1: Create all GisFeature entities and save once (bulk)
        var featureKey = 0;
        var featureList = new List<(GisFeature gisFeature, IFeature sourceFeature)>();

        foreach (var feature in featureCollection)
        {
            if (feature.Geometry == null) continue;

            feature.Geometry.SRID = 4326;

            var gisFeature = new GisFeature
            {
                GisUploadAttemptID = gisUploadAttemptID,
                GisFeatureGeometry = feature.Geometry,
                GisImportFeatureKey = featureKey++,
                IsValid = feature.Geometry.IsValid
            };

            // Calculate area if polygon/multipolygon — reproject to EPSG:2927 (WA South, US survey feet) then convert to acres
            if (feature.Geometry is NetTopologySuite.Geometries.Polygon || feature.Geometry is NetTopologySuite.Geometries.MultiPolygon)
            {
                var projected = feature.Geometry.ProjectTo2927();
                var areaInSqFt = projected.Area;
                gisFeature.CalculatedArea = (decimal)(areaInSqFt / 43560.0); // sq ft → acres
            }

            dbContext.GisFeatures.Add(gisFeature);
            featureList.Add((gisFeature, feature));
        }

        await dbContext.SaveChangesWithNoAuditingAsync(); // Single save — all GisFeatureIDs now populated

        // Phase 2: Create all GisFeatureMetadataAttribute records (bulk)
        foreach (var (gisFeature, sourceFeature) in featureList)
        {
            if (sourceFeature.Attributes == null) continue;

            foreach (var attrName in sourceFeature.Attributes.GetNames())
            {
                var metadataAttr = attributeDictionary[attrName.ToLowerInvariant()];
                var value = sourceFeature.Attributes[attrName];
                dbContext.GisFeatureMetadataAttributes.Add(new GisFeatureMetadataAttribute
                {
                    GisFeatureID = gisFeature.GisFeatureID,
                    GisMetadataAttributeID = metadataAttr.GisMetadataAttributeID,
                    GisFeatureMetadataAttributeValue = value?.ToString()
                });
            }
        }

        attempt.FeaturesSaved = true;
        attempt.AttributesSaved = true;
        attempt.AreaCalculationComplete = true;
        attempt.ImportedToGeoJson = true;
        await dbContext.SaveChangesWithNoAuditingAsync();
    }

    public static async Task<List<GisFeatureGridRow>> GetFeaturesAsGridRowAsync(WADNRDbContext dbContext, int gisUploadAttemptID)
    {
        var features = await dbContext.GisFeatures
            .AsNoTracking()
            .Where(x => x.GisUploadAttemptID == gisUploadAttemptID)
            .OrderBy(x => x.GisImportFeatureKey)
            .Select(x => new
            {
                x.GisFeatureID,
                x.GisImportFeatureKey,
                x.IsValid,
                x.CalculatedArea,
                Metadata = x.GisFeatureMetadataAttributes.Select(m => new
                {
                    m.GisMetadataAttribute.GisMetadataAttributeName,
                    m.GisFeatureMetadataAttributeValue
                }).ToList()
            })
            .ToListAsync();

        return features.Select(f => new GisFeatureGridRow
        {
            GisFeatureID = f.GisFeatureID,
            GisImportFeatureKey = f.GisImportFeatureKey,
            IsValid = f.IsValid,
            CalculatedArea = f.CalculatedArea,
            MetadataValues = f.Metadata.ToDictionary(
                m => m.GisMetadataAttributeName,
                m => m.GisFeatureMetadataAttributeValue)
        }).ToList();
    }

    public static async Task<FeatureCollection> GetFeaturesAsFeatureCollectionAsync(WADNRDbContext dbContext, int gisUploadAttemptID)
    {
        var features = await dbContext.GisFeatures
            .AsNoTracking()
            .Where(x => x.GisUploadAttemptID == gisUploadAttemptID)
            .OrderBy(x => x.GisImportFeatureKey)
            .Select(x => new
            {
                x.GisFeatureID,
                x.IsValid,
                x.CalculatedArea,
                x.GisFeatureGeometry
            })
            .ToListAsync();

        var featureCollection = new FeatureCollection();
        foreach (var f in features)
        {
            if (f.GisFeatureGeometry == null) continue;

            var attributes = new AttributesTable
            {
                { "GisFeatureID", f.GisFeatureID },
                { "IsValid", f.IsValid },
                { "CalculatedArea", f.CalculatedArea }
            };
            featureCollection.Add(new Feature(f.GisFeatureGeometry, attributes));
        }

        return featureCollection;
    }

    public static async Task<List<GisMetadataAttributeItem>> GetMetadataAttributesAsync(WADNRDbContext dbContext, int gisUploadAttemptID)
    {
        return await dbContext.GisUploadAttemptGisMetadataAttributes
            .AsNoTracking()
            .Where(x => x.GisUploadAttemptID == gisUploadAttemptID)
            .OrderBy(x => x.SortOrder)
            .Select(x => new GisMetadataAttributeItem
            {
                GisMetadataAttributeID = x.GisMetadataAttributeID,
                GisMetadataAttributeName = x.GisMetadataAttribute.GisMetadataAttributeName,
                SortOrder = x.SortOrder
            })
            .ToListAsync();
    }

    public static async Task<GisMetadataMappingDefaults> GetDefaultMappingsAsync(WADNRDbContext dbContext, int gisUploadAttemptID)
    {
        var attempt = await dbContext.GisUploadAttempts
            .AsNoTracking()
            .Include(x => x.GisUploadSourceOrganization)
                .ThenInclude(x => x.GisDefaultMappings)
            .FirstAsync(x => x.GisUploadAttemptID == gisUploadAttemptID);

        // Get metadata attributes for this attempt
        var attemptAttributes = await dbContext.GisUploadAttemptGisMetadataAttributes
            .AsNoTracking()
            .Where(x => x.GisUploadAttemptID == gisUploadAttemptID)
            .Include(x => x.GisMetadataAttribute)
            .ToListAsync();

        var attrLookup = attemptAttributes.ToDictionary(
            x => x.GisMetadataAttribute.GisMetadataAttributeName.ToLowerInvariant(),
            x => x.GisMetadataAttributeID,
            StringComparer.OrdinalIgnoreCase);

        var defaults = new GisMetadataMappingDefaults();

        foreach (var mapping in attempt.GisUploadSourceOrganization.GisDefaultMappings)
        {
            var columnName = mapping.GisDefaultMappingColumnName.ToLowerInvariant();
            if (!attrLookup.TryGetValue(columnName, out var attrID))
            {
                continue;
            }

            // Map FieldDefinitionID to the appropriate property on the defaults DTO
            // This uses the FieldDefinition table IDs from the legacy system
            MapFieldDefinitionToDefault(defaults, mapping.FieldDefinitionID, attrID);
        }

        defaults.ImportIsFlattened = attempt.GisUploadSourceOrganization.ImportIsFlattened == true;

        return defaults;
    }

    private static void MapFieldDefinitionToDefault(GisMetadataMappingDefaults defaults, int fieldDefinitionID, int metadataAttributeID)
    {
        if (fieldDefinitionID == FieldDefinition.ProjectIdentifier.FieldDefinitionID)
            defaults.ProjectIdentifierMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.ProjectName.FieldDefinitionID)
            defaults.ProjectNameMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.TreatmentType.FieldDefinitionID)
            defaults.TreatmentTypeMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.CompletionDate.FieldDefinitionID)
            defaults.CompletionDateMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.PlannedDate.FieldDefinitionID)
            defaults.StartDateMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.ProjectStage.FieldDefinitionID)
            defaults.ProjectStageMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.LeadImplementerOrganization.FieldDefinitionID)
            defaults.LeadImplementerMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.FootprintAcres.FieldDefinitionID)
            defaults.FootprintAcresMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.Landowner.FieldDefinitionID)
            defaults.PrivateLandownerMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.TreatmentDetailedActivityType.FieldDefinitionID)
            defaults.TreatmentDetailedActivityTypeMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.TreatedAcres.FieldDefinitionID)
            defaults.TreatedAcresMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.PruningAcres.FieldDefinitionID)
            defaults.PruningAcresMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.ThinningAcres.FieldDefinitionID)
            defaults.ThinningAcresMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.ChippingAcres.FieldDefinitionID)
            defaults.ChippingAcresMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.MasticationAcres.FieldDefinitionID)
            defaults.MasticationAcresMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.GrazingAcres.FieldDefinitionID)
            defaults.GrazingAcresMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.LopAndScatterAcres.FieldDefinitionID)
            defaults.LopScatAcresMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.BiomassRemovalAcres.FieldDefinitionID)
            defaults.BiomassRemovalAcresMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.HandPileAcres.FieldDefinitionID)
            defaults.HandPileAcresMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.HandPileBurnAcres.FieldDefinitionID)
            defaults.HandPileBurnAcresMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.MachinePileBurnAcres.FieldDefinitionID)
            defaults.MachinePileBurnAcresMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.BroadcastBurnAcres.FieldDefinitionID)
            defaults.BroadcastBurnAcresMetadataAttributeID = metadataAttributeID;
        else if (fieldDefinitionID == FieldDefinition.OtherTreatmentAcres.FieldDefinitionID)
            defaults.OtherAcresMetadataAttributeID = metadataAttributeID;
    }

    public static async Task<GisBulkImportResult> ImportProjectsAsync(WADNRDbContext dbContext, int gisUploadAttemptID, GisBulkImportRequest request)
    {
        var result = new GisBulkImportResult();

        var attempt = await dbContext.GisUploadAttempts
            .Include(x => x.GisUploadSourceOrganization)
                .ThenInclude(x => x.GisCrossWalkDefaults)
            .Include(x => x.GisUploadSourceOrganization)
                .ThenInclude(x => x.GisExcludeIncludeColumns)
                    .ThenInclude(x => x.GisExcludeIncludeColumnValues)
            .FirstAsync(x => x.GisUploadAttemptID == gisUploadAttemptID);

        var sourceOrg = attempt.GisUploadSourceOrganization;

        // Load features with metadata
        var features = await dbContext.GisFeatures
            .Where(x => x.GisUploadAttemptID == gisUploadAttemptID)
            .Include(x => x.GisFeatureMetadataAttributes)
                .ThenInclude(x => x.GisMetadataAttribute)
            .ToListAsync();

        // Build metadata value lookup per feature
        var featureMetadata = features.ToDictionary(
            f => f.GisFeatureID,
            f => f.GisFeatureMetadataAttributes.ToDictionary(
                m => m.GisMetadataAttributeID,
                m => m.GisFeatureMetadataAttributeValue));

        // Get project identifier values to group features by project
        // Normalize to uppercase for case-insensitive grouping; keep originals for display/storage
        var projectIdentifierLookup = new Dictionary<int, string>();
        var originalIdentifierLookup = new Dictionary<int, string>();
        foreach (var feature in features)
        {
            if (featureMetadata.TryGetValue(feature.GisFeatureID, out var metadata) &&
                metadata.TryGetValue(request.ProjectIdentifierMetadataAttributeID, out var identifier) &&
                !string.IsNullOrWhiteSpace(identifier))
            {
                projectIdentifierLookup[feature.GisFeatureID] = identifier.Trim().ToUpperInvariant();
                originalIdentifierLookup[feature.GisFeatureID] = identifier.Trim();
            }
        }

        // Group features by project identifier
        var featuresByProject = features
            .Where(f => projectIdentifierLookup.ContainsKey(f.GisFeatureID))
            .GroupBy(f => projectIdentifierLookup[f.GisFeatureID])
            .ToList();

        foreach (var projectGroup in featuresByProject)
        {
            var projectIdentifier = projectGroup.Key;
            var firstFeature = projectGroup.First();
            var firstMetadata = featureMetadata[firstFeature.GisFeatureID];

            // Get project name (use original mixed-case identifier as fallback, not the uppercased key)
            var originalIdentifier = originalIdentifierLookup[firstFeature.GisFeatureID];
            string projectName = null;
            if (firstMetadata.TryGetValue(request.ProjectNameMetadataAttributeID, out var name))
            {
                projectName = name;
            }
            projectName ??= originalIdentifier;

            // Find existing project by GIS identifier within the same program (case-insensitive)
            var existingProject = await dbContext.Projects
                .Include(p => p.ProjectPrograms)
                .FirstOrDefaultAsync(p => p.ProjectGisIdentifier != null &&
                    p.ProjectGisIdentifier.Trim().ToUpper() == projectIdentifier &&
                    p.ProjectPrograms.Any(pp => pp.ProgramID == sourceOrg.ProgramID));

            if (existingProject != null)
            {
                // Update fields from GIS data (matching legacy behavior)
                existingProject.ProjectName = projectName.Length > 140 ? projectName[..140] : projectName;
                existingProject.ProjectStageID = sourceOrg.ProjectStageDefaultID;
                existingProject.LastUpdateGisUploadAttemptID = gisUploadAttemptID;

                // Auto-approve if stage is not Planned and project is Draft/PendingApproval
                if (sourceOrg.ProjectStageDefaultID != (int)ProjectStageEnum.Planned
                    && (existingProject.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.Draft
                        || existingProject.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.PendingApproval))
                {
                    existingProject.ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved;
                }

                // Update dates if configured
                if (sourceOrg.ApplyStartDateToProject && request.StartDateMetadataAttributeID.HasValue &&
                    firstMetadata.TryGetValue(request.StartDateMetadataAttributeID.Value, out var existingStartDateStr) &&
                    DateTime.TryParse(existingStartDateStr, out var existingStartDate))
                {
                    existingProject.PlannedDate = DateOnly.FromDateTime(existingStartDate);
                }

                if (sourceOrg.ApplyCompletedDateToProject && request.CompletionDateMetadataAttributeID.HasValue &&
                    firstMetadata.TryGetValue(request.CompletionDateMetadataAttributeID.Value, out var existingCompletionDateStr) &&
                    DateTime.TryParse(existingCompletionDateStr, out var existingCompletionDate))
                {
                    existingProject.CompletionDate = DateOnly.FromDateTime(existingCompletionDate);
                }

                // Set description only if empty
                if (string.IsNullOrEmpty(existingProject.ProjectDescription) && !string.IsNullOrEmpty(sourceOrg.ProjectDescriptionDefaultText))
                {
                    existingProject.ProjectDescription = sourceOrg.ProjectDescriptionDefaultText;
                }

                // Ensure program link exists
                if (!existingProject.ProjectPrograms.Any(pp => pp.ProgramID == sourceOrg.ProgramID))
                {
                    dbContext.ProjectPrograms.Add(new ProjectProgram
                    {
                        ProjectID = existingProject.ProjectID,
                        ProgramID = sourceOrg.ProgramID
                    });
                }

                result.ProjectsUpdated++;
                result.UpdatedProjects.Add(new GisBulkImportProjectResult
                {
                    ProjectID = existingProject.ProjectID,
                    ProjectName = existingProject.ProjectName
                });
            }
            else
            {
                // Look up ProjectTypeID from the source org's default name
                var projectTypeID = await dbContext.ProjectTypes
                    .Where(pt => pt.ProjectTypeName == (sourceOrg.ProjectTypeDefaultName ?? ""))
                    .Select(pt => pt.ProjectTypeID)
                    .FirstOrDefaultAsync();
                if (projectTypeID == 0)
                {
                    projectTypeID = await dbContext.ProjectTypes.Select(pt => pt.ProjectTypeID).FirstAsync();
                }

                var newProject = new Project
                {
                    ProjectName = projectName.Length > 140 ? projectName[..140] : projectName,
                    FhtProjectNumber = await Projects.GenerateFhtProjectNumberAsync(dbContext),
                    ProjectGisIdentifier = originalIdentifier.Length > 140 ? originalIdentifier[..140] : originalIdentifier,
                    ProjectTypeID = projectTypeID,
                    ProjectStageID = sourceOrg.ProjectStageDefaultID,
                    ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved,
                    ProjectLocationSimpleTypeID = (int)ProjectLocationSimpleTypeEnum.None,
                    CreateGisUploadAttemptID = gisUploadAttemptID,
                    LastUpdateGisUploadAttemptID = gisUploadAttemptID
                };

                // Set dates if configured
                if (sourceOrg.ApplyStartDateToProject && request.StartDateMetadataAttributeID.HasValue &&
                    firstMetadata.TryGetValue(request.StartDateMetadataAttributeID.Value, out var startDateStr) &&
                    DateTime.TryParse(startDateStr, out var startDate))
                {
                    newProject.PlannedDate = DateOnly.FromDateTime(startDate);
                }

                if (sourceOrg.ApplyCompletedDateToProject && request.CompletionDateMetadataAttributeID.HasValue &&
                    firstMetadata.TryGetValue(request.CompletionDateMetadataAttributeID.Value, out var completionDateStr) &&
                    DateTime.TryParse(completionDateStr, out var completionDate))
                {
                    newProject.CompletionDate = DateOnly.FromDateTime(completionDate);
                }

                // Set project description
                if (!string.IsNullOrEmpty(sourceOrg.ProjectDescriptionDefaultText))
                {
                    newProject.ProjectDescription = sourceOrg.ProjectDescriptionDefaultText;
                }

                dbContext.Projects.Add(newProject);
                await dbContext.SaveChangesWithNoAuditingAsync();

                // Link project to program
                dbContext.ProjectPrograms.Add(new ProjectProgram
                {
                    ProjectID = newProject.ProjectID,
                    ProgramID = sourceOrg.ProgramID
                });

                // Create default organization relationship
                dbContext.ProjectOrganizations.Add(new ProjectOrganization
                {
                    ProjectID = newProject.ProjectID,
                    OrganizationID = sourceOrg.DefaultLeadImplementerOrganizationID,
                    RelationshipTypeID = sourceOrg.RelationshipTypeForDefaultOrganizationID
                });

                existingProject = newProject;
                result.ProjectsCreated++;
                result.CreatedProjects.Add(new GisBulkImportProjectResult
                {
                    ProjectID = newProject.ProjectID,
                    ProjectName = newProject.ProjectName
                });
            }

            // Remove prior ProjectArea locations for this project+program before re-creating (matching legacy DeleteFull behavior)
            var locationIDsToDelete = await dbContext.ProjectLocations
                .Where(pl => pl.ProjectID == existingProject.ProjectID &&
                    pl.ProjectLocationTypeID == (int)ProjectLocationTypeEnum.ProjectArea &&
                    pl.ProgramID == sourceOrg.ProgramID)
                .Select(pl => pl.ProjectLocationID)
                .ToListAsync();

            if (locationIDsToDelete.Count > 0)
            {
                // Delete child Treatments first, then the locations
                await dbContext.Treatments
                    .Where(t => t.ProjectLocationID != null && locationIDsToDelete.Contains(t.ProjectLocationID.Value))
                    .ExecuteDeleteAsync();

                await dbContext.ProjectLocations
                    .Where(pl => locationIDsToDelete.Contains(pl.ProjectLocationID))
                    .ExecuteDeleteAsync();
            }

            // Create project locations from feature geometries
            foreach (var feature in projectGroup)
            {
                var locationName = $"{originalIdentifier} - Feature {feature.GisImportFeatureKey}";

                dbContext.ProjectLocations.Add(new ProjectLocation
                {
                    ProjectID = existingProject.ProjectID,
                    ProjectLocationGeometry = feature.GisFeatureGeometry,
                    ProjectLocationName = locationName.Length > 100 ? locationName[..100] : locationName,
                    ProjectLocationTypeID = (int)ProjectLocationTypeEnum.ProjectArea,
                    ImportedFromGisUpload = true,
                    ProgramID = sourceOrg.ProgramID
                });
                result.LocationsCreated++;
            }

            await dbContext.SaveChangesWithNoAuditingAsync();
        }

        // Call stored proc for treatment imports
        try
        {
            // Resolve default TreatmentTypeID from source org name (fallback to Other)
            var treatmentTypeID = TreatmentType.Other.TreatmentTypeID;
            if (!string.IsNullOrEmpty(sourceOrg.TreatmentTypeDefaultName))
            {
                var treatmentType = TreatmentType.All.SingleOrDefault(x =>
                    x.TreatmentTypeDisplayName.Equals(sourceOrg.TreatmentTypeDefaultName, StringComparison.InvariantCultureIgnoreCase));
                if (treatmentType != null)
                {
                    treatmentTypeID = treatmentType.TreatmentTypeID;
                }
            }

            var treatmentDetailedActivityTypeID = TreatmentDetailedActivityType.Other.TreatmentDetailedActivityTypeID;
            var isFlattened = sourceOrg.ImportIsFlattened == true ? 1 : 0;

            // Null metadata attribute IDs use -1 sentinel for the proc
            int ToSqlID(int? id) => id ?? -1;

            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $@"EXEC dbo.procImportTreatmentsFromGisUploadAttempt
                    @piGisUploadAttemptID = {gisUploadAttemptID},
                    @projectIdentifierGisMetadataAttributeID = {request.ProjectIdentifierMetadataAttributeID},
                    @footprintAcresMetadataAttributeID = {ToSqlID(request.FootprintAcresMetadataAttributeID)},
                    @treatedAcresMetadataAttributeID = {ToSqlID(request.TreatedAcresMetadataAttributeID)},
                    @treatmentTypeMetadataAttributeID = {ToSqlID(request.TreatmentTypeMetadataAttributeID)},
                    @treatmentDetailedActivityTypeMetadataAttributeID = {ToSqlID(request.TreatmentDetailedActivityTypeMetadataAttributeID)},
                    @treatmentTypeID = {treatmentTypeID},
                    @treatmentDetailedActivityTypeID = {treatmentDetailedActivityTypeID},
                    @isFlattened = {isFlattened},
                    @pruningAcresMetadataAttributeID = {ToSqlID(request.PruningAcresMetadataAttributeID)},
                    @thinningAcresMetadataAttributeID = {ToSqlID(request.ThinningAcresMetadataAttributeID)},
                    @chippingAcresMetadataAttributeID = {ToSqlID(request.ChippingAcresMetadataAttributeID)},
                    @masticationAcresMetadataAttributeID = {ToSqlID(request.MasticationAcresMetadataAttributeID)},
                    @grazingAcresMetadataAttributeID = {ToSqlID(request.GrazingAcresMetadataAttributeID)},
                    @lopScatterAcresMetadataAttributeID = {ToSqlID(request.LopScatAcresMetadataAttributeID)},
                    @biomassRemovalAcresMetadataAttributeID = {ToSqlID(request.BiomassRemovalAcresMetadataAttributeID)},
                    @handPileAcresMetadataAttributeID = {ToSqlID(request.HandPileAcresMetadataAttributeID)},
                    @handPileBurnAcresMetadataAttributeID = {ToSqlID(request.HandPileBurnAcresMetadataAttributeID)},
                    @machineBurnAcresMetadataAttributeID = {ToSqlID(request.MachinePileBurnAcresMetadataAttributeID)},
                    @broadcastBurnAcresMetadataAttributeID = {ToSqlID(request.BroadcastBurnAcresMetadataAttributeID)},
                    @otherBurnAcresMetadataAttributeID = {ToSqlID(request.OtherAcresMetadataAttributeID)},
                    @startDateMetadataAttributeID = {ToSqlID(request.StartDateMetadataAttributeID)},
                    @endDateMetadataAttributeID = {ToSqlID(request.CompletionDateMetadataAttributeID)}");
        }
        catch (Exception ex)
        {
            result.Warnings.Add($"Treatment import stored procedure encountered an error: {ex.Message}");
        }

        return result;
    }
}
