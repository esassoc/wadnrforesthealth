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
            .Select(x => new GisUploadSourceOrganizationSummary
            {
                GisUploadSourceOrganizationID = x.GisUploadSourceOrganizationID,
                GisUploadSourceOrganizationName = x.GisUploadSourceOrganizationName,
                ProgramName = x.Program.ProgramName,
                ProgramDisplayName = x.Program.ProgramName
                    + (x.Program.ProgramShortName != null ? " (" + x.Program.ProgramShortName + ")" : "")
                    + (!x.Program.ProgramIsActive ? " (Inactive)" : ""),
                ProgramID = x.ProgramID
            })
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

        // Clear any existing data from a previous upload on this attempt
        var existingFeatures = await dbContext.GisFeatures
            .Where(x => x.GisUploadAttemptID == gisUploadAttemptID)
            .ToListAsync();
        if (existingFeatures.Any())
        {
            var featureIDs = existingFeatures.Select(f => f.GisFeatureID).ToList();
            var existingFeatureMetadata = await dbContext.GisFeatureMetadataAttributes
                .Where(x => featureIDs.Contains(x.GisFeatureID))
                .ToListAsync();
            dbContext.GisFeatureMetadataAttributes.RemoveRange(existingFeatureMetadata);
            dbContext.GisFeatures.RemoveRange(existingFeatures);
        }

        var existingAttemptAttrs = await dbContext.GisUploadAttemptGisMetadataAttributes
            .Where(x => x.GisUploadAttemptID == gisUploadAttemptID)
            .ToListAsync();
        if (existingAttemptAttrs.Any())
        {
            dbContext.GisUploadAttemptGisMetadataAttributes.RemoveRange(existingAttemptAttrs);
        }

        await dbContext.SaveChangesWithNoAuditingAsync();

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

        // Save features and their metadata values
        var featureKey = 0;
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
            await dbContext.SaveChangesWithNoAuditingAsync(); // Need ID for metadata attributes

            if (feature.Attributes != null)
            {
                foreach (var attrName in feature.Attributes.GetNames())
                {
                    var metadataAttr = attributeDictionary[attrName.ToLowerInvariant()];
                    var value = feature.Attributes[attrName];
                    dbContext.GisFeatureMetadataAttributes.Add(new GisFeatureMetadataAttribute
                    {
                        GisFeatureID = gisFeature.GisFeatureID,
                        GisMetadataAttributeID = metadataAttr.GisMetadataAttributeID,
                        GisFeatureMetadataAttributeValue = value?.ToString()
                    });
                }
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
        // FieldDefinition IDs map to specific import fields
        // These values correspond to the FieldDefinition table in the database
        switch (fieldDefinitionID)
        {
            case 44: // FhtProjectNumber / Project Identifier
                defaults.ProjectIdentifierMetadataAttributeID = metadataAttributeID;
                break;
            case 22: // ProjectName
                defaults.ProjectNameMetadataAttributeID = metadataAttributeID;
                break;
            case 305: // TreatmentType
                defaults.TreatmentTypeMetadataAttributeID = metadataAttributeID;
                break;
            case 56: // CompletionDate
                defaults.CompletionDateMetadataAttributeID = metadataAttributeID;
                break;
            case 97: // ProjectInitiationDate / StartDate
                defaults.StartDateMetadataAttributeID = metadataAttributeID;
                break;
            case 24: // ProjectStage
                defaults.ProjectStageMetadataAttributeID = metadataAttributeID;
                break;
            case 296: // LeadImplementer
                defaults.LeadImplementerMetadataAttributeID = metadataAttributeID;
                break;
            case 304: // FootprintAcres
                defaults.FootprintAcresMetadataAttributeID = metadataAttributeID;
                break;
            case 298: // PrivateLandowner
                defaults.PrivateLandownerMetadataAttributeID = metadataAttributeID;
                break;
            case 306: // TreatmentDetailedActivityType
                defaults.TreatmentDetailedActivityTypeMetadataAttributeID = metadataAttributeID;
                break;
            case 307: // TreatedAcres
                defaults.TreatedAcresMetadataAttributeID = metadataAttributeID;
                break;
            case 308: defaults.PruningAcresMetadataAttributeID = metadataAttributeID; break;
            case 309: defaults.ThinningAcresMetadataAttributeID = metadataAttributeID; break;
            case 310: defaults.ChippingAcresMetadataAttributeID = metadataAttributeID; break;
            case 311: defaults.MasticationAcresMetadataAttributeID = metadataAttributeID; break;
            case 312: defaults.GrazingAcresMetadataAttributeID = metadataAttributeID; break;
            case 313: defaults.LopScatAcresMetadataAttributeID = metadataAttributeID; break;
            case 314: defaults.BiomassRemovalAcresMetadataAttributeID = metadataAttributeID; break;
            case 315: defaults.HandPileAcresMetadataAttributeID = metadataAttributeID; break;
            case 316: defaults.HandPileBurnAcresMetadataAttributeID = metadataAttributeID; break;
            case 317: defaults.MachinePileBurnAcresMetadataAttributeID = metadataAttributeID; break;
            case 318: defaults.BroadcastBurnAcresMetadataAttributeID = metadataAttributeID; break;
            case 319: defaults.OtherAcresMetadataAttributeID = metadataAttributeID; break;
        }
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
        var projectIdentifierLookup = new Dictionary<int, string>();
        foreach (var feature in features)
        {
            if (featureMetadata.TryGetValue(feature.GisFeatureID, out var metadata) &&
                metadata.TryGetValue(request.ProjectIdentifierMetadataAttributeID, out var identifier) &&
                !string.IsNullOrWhiteSpace(identifier))
            {
                projectIdentifierLookup[feature.GisFeatureID] = identifier;
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

            // Get project name
            string projectName = null;
            if (firstMetadata.TryGetValue(request.ProjectNameMetadataAttributeID, out var name))
            {
                projectName = name;
            }
            projectName ??= projectIdentifier;

            // Find existing project by FHT number within the same program
            var existingProject = await dbContext.Projects
                .FirstOrDefaultAsync(p => p.FhtProjectNumber == projectIdentifier &&
                    p.ProjectPrograms.Any(pp => pp.ProgramID == sourceOrg.ProgramID));

            if (existingProject != null)
            {
                existingProject.LastUpdateGisUploadAttemptID = gisUploadAttemptID;
                result.ProjectsUpdated++;
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
                    FhtProjectNumber = projectIdentifier.Length > 20 ? projectIdentifier[..20] : projectIdentifier,
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
                    newProject.PlannedDate = startDate;
                }

                if (sourceOrg.ApplyCompletedDateToProject && request.CompletionDateMetadataAttributeID.HasValue &&
                    firstMetadata.TryGetValue(request.CompletionDateMetadataAttributeID.Value, out var completionDateStr) &&
                    DateTime.TryParse(completionDateStr, out var completionDate))
                {
                    newProject.CompletionDate = completionDate;
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
            }

            // Create project locations from feature geometries
            foreach (var feature in projectGroup)
            {
                var locationName = $"{projectIdentifier} - Feature {feature.GisImportFeatureKey}";

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

        // Call stored proc for treatment imports if applicable
        try
        {
            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC dbo.procImportTreatmentsFromGisUploadAttempt @GisUploadAttemptID = {gisUploadAttemptID}");
        }
        catch
        {
            result.Warnings.Add("Treatment import stored procedure encountered an error. Treatments may not have been fully imported.");
        }

        return result;
    }
}
