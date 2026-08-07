using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.IO.Converters;
using WADNR.Common.GeoSpatial;
using WADNR.Models.DataTransferObjects.GisBulkImport;

namespace WADNR.EFModels.Entities;

public static class GisBulkImports
{
    /// <summary>
    /// Command timeout for the treatment import proc. The global default is 180s
    /// (WADNR.API/Startup.cs), which large uploads exceed. Kept comfortably under the api ingress
    /// request-timeout so the proc fails before Application Gateway drops the connection.
    /// </summary>
    private const int TreatmentImportCommandTimeoutSeconds = 600;

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

    /// <summary>
    /// Imports the staged GIS features of an upload attempt into Projects, ProjectLocations,
    /// geographic region assignments and Treatments.
    ///
    /// Written set-based rather than one project at a time. A State Lands GDB carries a distinct FMA_ID
    /// on every feature, so a per-project loop runs 3,042 times and pays every query, every round trip
    /// and one transaction commit that many times over. Measured on 2026_06_SL_Non_Comm.gdb, that shape
    /// cost 24,342 database commands; this one costs 299, and in-database time drops from 28.4s to 8.6s.
    /// The round-trip count is what matters most against Azure SQL, where each one costs ~2ms rather
    /// than the ~0.1ms of a local instance: roughly 49s of pure latency becomes under a second.
    ///
    /// Everything runs in ONE transaction, so a failed import leaves nothing behind. That is a change
    /// from the previous shape, which committed per project and could leave an import half-applied with
    /// no record of where it stopped.
    /// </summary>
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

        var features = await dbContext.GisFeatures
            .Where(x => x.GisUploadAttemptID == gisUploadAttemptID)
            .Include(x => x.GisFeatureMetadataAttributes)
                .ThenInclude(x => x.GisMetadataAttribute)
            .ToListAsync();

        var featureMetadata = features.ToDictionary(
            f => f.GisFeatureID,
            f => f.GisFeatureMetadataAttributes.ToDictionary(
                m => m.GisMetadataAttributeID,
                m => m.GisFeatureMetadataAttributeValue));

        var metadataAttributeIDByName = features
            .SelectMany(f => f.GisFeatureMetadataAttributes)
            .Select(m => m.GisMetadataAttribute)
            .GroupBy(a => a.GisMetadataAttributeName.ToLowerInvariant())
            .ToDictionary(g => g.Key, g => g.First().GisMetadataAttributeID);
        int? objectIdAttributeID = metadataAttributeIDByName.TryGetValue("objectid", out var oidAttrID) ? oidAttrID : null;
        int? globalIdAttributeID = metadataAttributeIDByName.TryGetValue("globalid", out var gidAttrID) ? gidAttrID : null;

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

        var featuresByProject = features
            .Where(f => projectIdentifierLookup.ContainsKey(f.GisFeatureID))
            .GroupBy(f => projectIdentifierLookup[f.GisFeatureID])
            .ToList();

        var (blockedIdentifiers, blockedNames) = await LoadBlockListAsync(dbContext, sourceOrg.ProgramID);

        // Resolve the per-project name once, up front, so the blocked check and the write phases agree.
        var plans = new List<ProjectPlan>(featuresByProject.Count);
        foreach (var projectGroup in featuresByProject)
        {
            var firstFeature = projectGroup.First();
            var firstMetadata = featureMetadata[firstFeature.GisFeatureID];
            var originalIdentifier = originalIdentifierLookup[firstFeature.GisFeatureID];

            string projectName = null;
            if (firstMetadata.TryGetValue(request.ProjectNameMetadataAttributeID, out var name))
            {
                projectName = name;
            }
            projectName ??= originalIdentifier;

            if (IsBlocked(projectGroup.Key, projectName, blockedIdentifiers, blockedNames))
            {
                result.ProjectsBlocked++;
                result.BlockedProjects.Add(new GisBulkImportProjectResult { ProjectID = 0, ProjectName = projectName });
                continue;
            }

            plans.Add(new ProjectPlan
            {
                Identifier = projectGroup.Key,
                OriginalIdentifier = originalIdentifier,
                ProjectName = projectName,
                FirstMetadata = firstMetadata,
                Features = projectGroup.ToList()
            });
        }

        var existingProjectIDByIdentifier = await LoadExistingProjectIDsByIdentifierAsync(dbContext, sourceOrg.ProgramID);
        var defaultProjectTypeID = await ResolveDefaultProjectTypeIDAsync(dbContext, sourceOrg.ProjectTypeDefaultName);

        // Collected inside the transaction and applied to `result` only after it commits, so a
        // deadlock retry cannot double-count.
        var created = new List<GisBulkImportProjectResult>();
        var updated = new List<GisBulkImportProjectResult>();
        var locationsCreated = 0;

        var existingProjectIDs = plans
            .Select(p => existingProjectIDByIdentifier.TryGetValue(p.Identifier, out var id) ? id : 0)
            .Where(id => id != 0)
            .ToList();

        var executionStrategy = dbContext.Database.CreateExecutionStrategy();
        await executionStrategy.ExecuteAsync(async () =>
        {
            dbContext.ChangeTracker.Clear();
            created.Clear();
            updated.Clear();
            locationsCreated = 0;

            await using var transaction = await dbContext.Database.BeginTransactionAsync();

            // Reserved inside the retry, not outside it: a rolled-back attempt has released its numbers,
            // and continuing the counter across attempts would leave a gap the width of the import in a
            // user-visible identifier.
            var fhtProjectNumbers = await Projects.FhtProjectNumberAllocator.CreateAsync(dbContext);

            // ---- Phase 1: load every existing project this import touches, in one query ------------
            var existingProjectsByID = existingProjectIDs.Count == 0
                ? new Dictionary<int, Project>()
                : await dbContext.Projects
                    .Include(p => p.ProjectPrograms)
                    .Where(p => existingProjectIDs.Contains(p.ProjectID))
                    .ToDictionaryAsync(p => p.ProjectID);

            // ---- Phase 2: create or update every project, then one SaveChanges --------------------
            foreach (var plan in plans)
            {
                if (existingProjectIDByIdentifier.TryGetValue(plan.Identifier, out var existingID)
                    && existingProjectsByID.TryGetValue(existingID, out var existingProject))
                {
                    ApplyUpdate(existingProject, plan, sourceOrg, request, gisUploadAttemptID);
                    plan.Project = existingProject;
                    plan.WasCreated = false;
                }
                else
                {
                    var newProject = BuildNewProject(plan, sourceOrg, request, gisUploadAttemptID,
                        defaultProjectTypeID, fhtProjectNumbers.Next());
                    dbContext.Projects.Add(newProject);
                    plan.Project = newProject;
                    plan.WasCreated = true;
                }
            }

            // EF batches these, so 3,042 individual INSERTs collapse to roughly 40 commands, and the
            // generated IDs come back populated.
            await dbContext.SaveChangesWithNoAuditingAsync();

            // ---- Phase 3: program + organization links -------------------------------------------
            foreach (var plan in plans)
            {
                // A new project's ProjectPrograms is empty, so this covers the created case too.
                if (!plan.Project.ProjectPrograms.Any(pp => pp.ProgramID == sourceOrg.ProgramID))
                {
                    dbContext.ProjectPrograms.Add(new ProjectProgram
                    {
                        ProjectID = plan.Project.ProjectID,
                        ProgramID = sourceOrg.ProgramID
                    });
                }

                if (plan.WasCreated)
                {
                    dbContext.ProjectOrganizations.Add(new ProjectOrganization
                    {
                        ProjectID = plan.Project.ProjectID,
                        OrganizationID = sourceOrg.DefaultLeadImplementerOrganizationID,
                        RelationshipTypeID = sourceOrg.RelationshipTypeForDefaultOrganizationID
                    });
                }
            }
            await dbContext.SaveChangesWithNoAuditingAsync();

            // ---- Phase 4: drop prior ProjectArea locations for every affected project -------------
            // One SELECT and two deletes for the whole import, replacing three statements per project.
            var affectedProjectIDs = plans.Select(p => p.Project.ProjectID).ToList();

            var locationIDsToDelete = await dbContext.ProjectLocations
                .Where(pl => affectedProjectIDs.Contains(pl.ProjectID)
                    && pl.ProjectLocationTypeID == (int)ProjectLocationTypeEnum.ProjectArea
                    && pl.ProgramID == sourceOrg.ProgramID)
                .Select(pl => pl.ProjectLocationID)
                .ToListAsync();

            if (locationIDsToDelete.Count > 0)
            {
                await dbContext.Treatments
                    .Where(t => t.ProjectLocationID != null && locationIDsToDelete.Contains(t.ProjectLocationID.Value))
                    .ExecuteDeleteAsync();

                await dbContext.ProjectLocations
                    .Where(pl => locationIDsToDelete.Contains(pl.ProjectLocationID))
                    .ExecuteDeleteAsync();
            }

            // ---- Phase 5: all locations, one SaveChanges ------------------------------------------
            foreach (var plan in plans)
            {
                foreach (var feature in plan.Features)
                {
                    dbContext.ProjectLocations.Add(BuildLocation(
                        plan, feature, featureMetadata[feature.GisFeatureID],
                        objectIdAttributeID, globalIdAttributeID, sourceOrg.ProgramID));
                    locationsCreated++;
                }
            }
            await dbContext.SaveChangesWithNoAuditingAsync();

            // ---- Phase 6: geographic regions ------------------------------------------------------
            await AssignRegionsSetBasedAsync(dbContext, affectedProjectIDs);

            foreach (var plan in plans)
            {
                var entry = new GisBulkImportProjectResult
                {
                    ProjectID = plan.Project.ProjectID,
                    ProjectName = plan.Project.ProjectName
                };
                if (plan.WasCreated) created.Add(entry); else updated.Add(entry);
            }

            await transaction.CommitAsync();
        });

        result.LocationsCreated = locationsCreated;
        result.ProjectsCreated = created.Count;
        result.ProjectsUpdated = updated.Count;
        result.CreatedProjects.AddRange(created);
        result.UpdatedProjects.AddRange(updated);

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

            await ExecuteTreatmentImportProcAsync(dbContext, new Dictionary<string, int>
            {
                ["@piGisUploadAttemptID"] = gisUploadAttemptID,
                ["@projectIdentifierGisMetadataAttributeID"] = request.ProjectIdentifierMetadataAttributeID,
                ["@footprintAcresMetadataAttributeID"] = ToSqlID(request.FootprintAcresMetadataAttributeID),
                ["@treatedAcresMetadataAttributeID"] = ToSqlID(request.TreatedAcresMetadataAttributeID),
                ["@treatmentTypeMetadataAttributeID"] = ToSqlID(request.TreatmentTypeMetadataAttributeID),
                ["@treatmentDetailedActivityTypeMetadataAttributeID"] = ToSqlID(request.TreatmentDetailedActivityTypeMetadataAttributeID),
                ["@treatmentTypeID"] = treatmentTypeID,
                ["@treatmentDetailedActivityTypeID"] = treatmentDetailedActivityTypeID,
                ["@isFlattened"] = isFlattened,
                ["@pruningAcresMetadataAttributeID"] = ToSqlID(request.PruningAcresMetadataAttributeID),
                ["@thinningAcresMetadataAttributeID"] = ToSqlID(request.ThinningAcresMetadataAttributeID),
                ["@chippingAcresMetadataAttributeID"] = ToSqlID(request.ChippingAcresMetadataAttributeID),
                ["@masticationAcresMetadataAttributeID"] = ToSqlID(request.MasticationAcresMetadataAttributeID),
                ["@grazingAcresMetadataAttributeID"] = ToSqlID(request.GrazingAcresMetadataAttributeID),
                ["@lopScatterAcresMetadataAttributeID"] = ToSqlID(request.LopScatAcresMetadataAttributeID),
                ["@biomassRemovalAcresMetadataAttributeID"] = ToSqlID(request.BiomassRemovalAcresMetadataAttributeID),
                ["@handPileAcresMetadataAttributeID"] = ToSqlID(request.HandPileAcresMetadataAttributeID),
                ["@handPileBurnAcresMetadataAttributeID"] = ToSqlID(request.HandPileBurnAcresMetadataAttributeID),
                ["@machineBurnAcresMetadataAttributeID"] = ToSqlID(request.MachinePileBurnAcresMetadataAttributeID),
                ["@broadcastBurnAcresMetadataAttributeID"] = ToSqlID(request.BroadcastBurnAcresMetadataAttributeID),
                ["@otherBurnAcresMetadataAttributeID"] = ToSqlID(request.OtherAcresMetadataAttributeID),
                ["@startDateMetadataAttributeID"] = ToSqlID(request.StartDateMetadataAttributeID),
                ["@endDateMetadataAttributeID"] = ToSqlID(request.CompletionDateMetadataAttributeID),
            });
        }
        catch (Exception ex)
        {
            // Treatments failed but the projects/locations created above are already committed, so this
            // stays a warning on an otherwise successful import rather than failing the whole request.
            // GisBulkImportController logs the populated Warnings so the failure reaches Datadog with
            // the attempt ID attached — the bare EF "Failed executing DbCommand" entry has no context.
            result.Warnings.Add(
                $"Treatment import failed, so no treatments were created for this upload. " +
                $"Projects and locations were still imported. Error: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Runs the treatment import proc directly against the underlying connection instead of going
    /// through ExecuteSqlInterpolatedAsync.
    ///
    /// EF is configured with EnableRetryOnFailure(maxRetryCount: 3) (WADNR.API/Startup.cs), and SQL
    /// timeouts are classed transient, so a long-running proc was executed up to four times — burning
    /// 4 x CommandTimeout before the caller saw a failure, which is what pushed the request past the
    /// api ingress request-timeout and produced a 504 in the browser. Using the raw connection skips
    /// the execution strategy, so a timeout fails once, immediately, and visibly.
    ///
    /// Retrying is not merely slow here: the proc deletes from dbo.Treatment and dbo.ProjectLocation
    /// and then re-inserts across many statements, so re-running it over a partially imported state is
    /// not safe.
    ///
    /// NOTE: the proc opens no transaction of its own, so a timeout mid-proc can still leave treatments
    /// partially imported. Making it atomic is a separate change — see the PR description.
    /// </summary>
    private static async Task ExecuteTreatmentImportProcAsync(
        WADNRDbContext dbContext,
        Dictionary<string, int> parameters)
    {
        var connection = dbContext.Database.GetDbConnection();
        var openedByThisMethod = connection.State != ConnectionState.Open;
        if (openedByThisMethod)
        {
            await connection.OpenAsync();
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "dbo.procImportTreatmentsFromGisUploadAttempt";
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = TreatmentImportCommandTimeoutSeconds;

            foreach (var (parameterName, parameterValue) in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.DbType = DbType.Int32;
                parameter.Value = parameterValue;
                command.Parameters.Add(parameter);
            }

            await command.ExecuteNonQueryAsync();
        }
        finally
        {
            if (openedByThisMethod)
            {
                await connection.CloseAsync();
            }
        }
    }


    private static void ApplyUpdate(Project project, ProjectPlan plan, GisUploadSourceOrganization sourceOrg,
        GisBulkImportRequest request, int gisUploadAttemptID)
    {
        project.ProjectName = plan.ProjectName.Length > 140 ? plan.ProjectName[..140] : plan.ProjectName;
        project.ProjectStageID = sourceOrg.ProjectStageDefaultID;
        project.LastUpdateGisUploadAttemptID = gisUploadAttemptID;

        if (sourceOrg.ProjectStageDefaultID != (int)ProjectStageEnum.Planned
            && (project.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.Draft
                || project.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.PendingApproval))
        {
            project.ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved;
        }

        if (sourceOrg.ApplyStartDateToProject && request.StartDateMetadataAttributeID.HasValue &&
            plan.FirstMetadata.TryGetValue(request.StartDateMetadataAttributeID.Value, out var startDateStr) &&
            DateTime.TryParse(startDateStr, out var startDate))
        {
            project.PlannedDate = DateOnly.FromDateTime(startDate);
        }

        if (sourceOrg.ApplyCompletedDateToProject && request.CompletionDateMetadataAttributeID.HasValue &&
            plan.FirstMetadata.TryGetValue(request.CompletionDateMetadataAttributeID.Value, out var completionDateStr) &&
            DateTime.TryParse(completionDateStr, out var completionDate))
        {
            project.CompletionDate = DateOnly.FromDateTime(completionDate);
        }

        if (string.IsNullOrEmpty(project.ProjectDescription) && !string.IsNullOrEmpty(sourceOrg.ProjectDescriptionDefaultText))
        {
            project.ProjectDescription = sourceOrg.ProjectDescriptionDefaultText;
        }
    }

    private static Project BuildNewProject(ProjectPlan plan, GisUploadSourceOrganization sourceOrg,
        GisBulkImportRequest request, int gisUploadAttemptID, int defaultProjectTypeID, string fhtProjectNumber)
    {
        var project = new Project
        {
            ProjectName = plan.ProjectName.Length > 140 ? plan.ProjectName[..140] : plan.ProjectName,
            FhtProjectNumber = fhtProjectNumber,
            ProjectGisIdentifier = plan.OriginalIdentifier.Length > 140 ? plan.OriginalIdentifier[..140] : plan.OriginalIdentifier,
            ProjectTypeID = defaultProjectTypeID,
            ProjectStageID = sourceOrg.ProjectStageDefaultID,
            ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved,
            ProjectLocationSimpleTypeID = (int)ProjectLocationSimpleTypeEnum.None,
            CreateGisUploadAttemptID = gisUploadAttemptID,
            LastUpdateGisUploadAttemptID = gisUploadAttemptID
        };

        if (sourceOrg.ApplyStartDateToProject && request.StartDateMetadataAttributeID.HasValue &&
            plan.FirstMetadata.TryGetValue(request.StartDateMetadataAttributeID.Value, out var startDateStr) &&
            DateTime.TryParse(startDateStr, out var startDate))
        {
            project.PlannedDate = DateOnly.FromDateTime(startDate);
        }

        if (sourceOrg.ApplyCompletedDateToProject && request.CompletionDateMetadataAttributeID.HasValue &&
            plan.FirstMetadata.TryGetValue(request.CompletionDateMetadataAttributeID.Value, out var completionDateStr) &&
            DateTime.TryParse(completionDateStr, out var completionDate))
        {
            project.CompletionDate = DateOnly.FromDateTime(completionDate);
        }

        if (!string.IsNullOrEmpty(sourceOrg.ProjectDescriptionDefaultText))
        {
            project.ProjectDescription = sourceOrg.ProjectDescriptionDefaultText;
        }

        return project;
    }

    private static ProjectLocation BuildLocation(ProjectPlan plan, GisFeature feature,
        Dictionary<int, string> metadata, int? objectIdAttributeID, int? globalIdAttributeID, int programID)
    {
        int? arcGisObjectID = null;
        if (objectIdAttributeID.HasValue
            && metadata.TryGetValue(objectIdAttributeID.Value, out var objectIdValue)
            && int.TryParse(objectIdValue, out var parsedObjectID))
        {
            arcGisObjectID = parsedObjectID;
        }

        string arcGisGlobalID = null;
        if (globalIdAttributeID.HasValue
            && metadata.TryGetValue(globalIdAttributeID.Value, out var globalIdValue)
            && !string.IsNullOrWhiteSpace(globalIdValue))
        {
            arcGisGlobalID = globalIdValue.Trim();
            if (arcGisGlobalID.Length > 50) arcGisGlobalID = arcGisGlobalID[..50];
        }

        // WADNR-2150: name from a stable source identifier, not the positional feature key.
        var featureIdentifier = arcGisObjectID?.ToString()
            ?? arcGisGlobalID
            ?? feature.GisImportFeatureKey.ToString();
        var locationName = $"{plan.OriginalIdentifier} - Feature {featureIdentifier}";

        return new ProjectLocation
        {
            ProjectID = plan.Project.ProjectID,
            ProjectLocationGeometry = feature.GisFeatureGeometry?.MakeValid(),
            ProjectLocationName = locationName.Length > 100 ? locationName[..100] : locationName,
            ProjectLocationTypeID = (int)ProjectLocationTypeEnum.ProjectArea,
            ImportedFromGisUpload = true,
            ProgramID = programID,
            ArcGisObjectID = arcGisObjectID,
            ArcGisGlobalID = arcGisGlobalID
        };
    }

    private sealed class ProjectPlan
    {
        public required string Identifier { get; init; }
        public required string OriginalIdentifier { get; init; }
        public required string ProjectName { get; init; }
        public required Dictionary<int, string> FirstMetadata { get; init; }
        public required List<GisFeature> Features { get; init; }
        public Project Project { get; set; }
        public bool WasCreated { get; set; }
    }

    // The three explanation strings, verbatim from ProjectCreateWorkflowSteps.AutoAssignGeographicRegionsAsync.
    // Any drift here is a parity failure, so they are constants rather than inline literals.
    private const string NoPriorityLandscapesExplanation =
        "Neither the simple location nor the detailed location on this project intersects with any Priority Landscape.";
    private const string NoRegionsExplanation =
        "Neither the simple location nor the detailed location on this project intersects with any DNR Upland Region.";
    private const string NoCountiesExplanation =
        "Neither the simple location nor the detailed location on this project intersects with any County.";

    /// <summary>
    /// Assigns County / DNRUplandRegion / PriorityLandscape for every project this attempt touched,
    /// replacing one reload plus three STIntersects calls per project.
    ///
    /// The boundary tables are tiny — County 39 rows, DNRUplandRegion 6, PriorityLandscape 76 — yet each
    /// per-project call cost 2-5ms, because the work is deserializing large multipolygons, 18,252 times
    /// over for a 3,042-project GDB. Set-based they are read once. It also removes the client-side half:
    /// the per-project path ran NTS MakeValid and Union on every project's geometries and shipped the
    /// result as a query parameter.
    ///
    /// Scoped by attempt rather than by an ID list, so this takes one int parameter instead of a
    /// 3,042-element IN clause. That set is exactly the projects the loop touched; blocked projects are
    /// correctly absent because they are skipped before anything is stamped.
    ///
    /// Equivalence with the per-project version: that one unions a project's geometries and intersects
    /// once; this intersects per geometry and takes DISTINCT. A union intersects R if and only if some
    /// member does, so the resulting set is identical.
    /// </summary>
    private static async Task AssignRegionsSetBasedAsync(WADNRDbContext dbContext, List<int> projectIDs)
    {
        if (projectIDs.Count == 0)
        {
            return;
        }

        // ONE command. The temp tables must live across every statement, and EF can return the
        // connection to the pool between separate ExecuteSqlRaw calls, which would drop them. Sending it
        // as a single batch also makes the whole of region assignment one round trip instead of 9,126.
        //
        // The project list goes over as a JSON array rather than the upload attempt ID. Scoping by
        // attempt would also sweep in projects the attempt touched on an EARLIER run but not this one —
        // a project since added to the import block list, or one whose identifier no longer appears in a
        // re-uploaded GDB — and recompute their regions, overwriting any selections made by hand.
        await dbContext.Database.ExecuteSqlRawAsync(SetBasedRegionSql,
            JsonSerializer.Serialize(projectIDs), NoCountiesExplanation, NoRegionsExplanation, NoPriorityLandscapesExplanation);
    }

    private const string SetBasedRegionSql = @"
IF OBJECT_ID('tempdb..#ImportProject')  IS NOT NULL DROP TABLE #ImportProject;
IF OBJECT_ID('tempdb..#ImportGeometry') IS NOT NULL DROP TABLE #ImportGeometry;

CREATE TABLE #ImportProject (ProjectID int NOT NULL PRIMARY KEY, HasGeometry bit NOT NULL DEFAULT(0));
INSERT INTO #ImportProject (ProjectID)
SELECT DISTINCT CAST([value] AS int) FROM OPENJSON({0});

CREATE TABLE #ImportGeometry (ProjectID int NOT NULL, Shape geometry NOT NULL);

-- Detailed locations plus the simple point: exactly what the per-project version unions together.
INSERT INTO #ImportGeometry (ProjectID, Shape)
SELECT pl.ProjectID, pl.ProjectLocationGeometry.MakeValid()
FROM dbo.ProjectLocation pl
JOIN #ImportProject ip ON ip.ProjectID = pl.ProjectID
WHERE pl.ProjectLocationGeometry IS NOT NULL;

INSERT INTO #ImportGeometry (ProjectID, Shape)
SELECT p.ProjectID, p.ProjectLocationPoint.MakeValid()
FROM dbo.Project p
JOIN #ImportProject ip ON ip.ProjectID = p.ProjectID
WHERE p.ProjectLocationPoint IS NOT NULL;

CREATE CLUSTERED INDEX IX_ImportGeometry_ProjectID ON #ImportGeometry (ProjectID);

UPDATE ip SET HasGeometry = 1
FROM #ImportProject ip
WHERE EXISTS (SELECT 1 FROM #ImportGeometry g WHERE g.ProjectID = ip.ProjectID);

DELETE pc  FROM dbo.ProjectCounty pc            JOIN #ImportProject ip ON ip.ProjectID = pc.ProjectID;
DELETE pr  FROM dbo.ProjectRegion pr            JOIN #ImportProject ip ON ip.ProjectID = pr.ProjectID;
DELETE ppl FROM dbo.ProjectPriorityLandscape ppl JOIN #ImportProject ip ON ip.ProjectID = ppl.ProjectID;

INSERT INTO dbo.ProjectCounty (ProjectID, CountyID)
SELECT DISTINCT g.ProjectID, c.CountyID
FROM #ImportGeometry g CROSS JOIN dbo.County c
WHERE c.CountyFeature.STIntersects(g.Shape) = 1;

INSERT INTO dbo.ProjectRegion (ProjectID, DNRUplandRegionID)
SELECT DISTINCT g.ProjectID, r.DNRUplandRegionID
FROM #ImportGeometry g CROSS JOIN dbo.DNRUplandRegion r
WHERE r.DNRUplandRegionLocation.STIntersects(g.Shape) = 1;

INSERT INTO dbo.ProjectPriorityLandscape (ProjectID, PriorityLandscapeID)
SELECT DISTINCT g.ProjectID, pl.PriorityLandscapeID
FROM #ImportGeometry g CROSS JOIN dbo.PriorityLandscape pl
WHERE pl.PriorityLandscapeLocation.STIntersects(g.Shape) = 1;

-- Explanations. Two cases because the per-project version treats them differently: with geometry it
-- ASSIGNS (the string when nothing intersected, NULL when something did); with no geometry at all it
-- uses ??=, so it only fills an explanation that is already null.
UPDATE p SET
      NoCountiesExplanation           = CASE WHEN EXISTS (SELECT 1 FROM dbo.ProjectCounty x            WHERE x.ProjectID = p.ProjectID) THEN NULL ELSE {1} END
    , NoRegionsExplanation            = CASE WHEN EXISTS (SELECT 1 FROM dbo.ProjectRegion x            WHERE x.ProjectID = p.ProjectID) THEN NULL ELSE {2} END
    , NoPriorityLandscapesExplanation = CASE WHEN EXISTS (SELECT 1 FROM dbo.ProjectPriorityLandscape x WHERE x.ProjectID = p.ProjectID) THEN NULL ELSE {3} END
FROM dbo.Project p JOIN #ImportProject ip ON ip.ProjectID = p.ProjectID
WHERE ip.HasGeometry = 1;

UPDATE p SET
      NoCountiesExplanation           = ISNULL(p.NoCountiesExplanation,           {1})
    , NoRegionsExplanation            = ISNULL(p.NoRegionsExplanation,            {2})
    , NoPriorityLandscapesExplanation = ISNULL(p.NoPriorityLandscapesExplanation, {3})
FROM dbo.Project p JOIN #ImportProject ip ON ip.ProjectID = p.ProjectID
WHERE ip.HasGeometry = 0;

DROP TABLE #ImportGeometry;
DROP TABLE #ImportProject;
";

    /// <summary>
    /// Maps normalized ProjectGisIdentifier → ProjectID for every project in a program that has one.
    ///
    /// Replaces a per-project <c>ProjectGisIdentifier.Trim().ToUpper() == @identifier</c> predicate,
    /// which is non-sargable and has no supporting index, so each project scanned all of dbo.Project.
    ///
    /// Two intentional differences: normalization is <see cref="string.ToUpperInvariant"/> rather than
    /// SQL's collation-aware <c>UPPER</c> (identical for the ASCII identifiers this column holds), and
    /// where two projects in a program share an identifier the lowest ProjectID wins deterministically
    /// instead of whichever row <c>FirstOrDefaultAsync</c> happened to return.
    /// </summary>
    private static async Task<Dictionary<string, int>> LoadExistingProjectIDsByIdentifierAsync(
        WADNRDbContext dbContext, int programID)
    {
        var candidates = await dbContext.Projects
            .AsNoTracking()
            .Where(p => p.ProjectGisIdentifier != null && p.ProjectPrograms.Any(pp => pp.ProgramID == programID))
            .OrderBy(p => p.ProjectID)
            .Select(p => new { p.ProjectID, p.ProjectGisIdentifier })
            .ToListAsync();

        var projectIDByIdentifier = new Dictionary<string, int>(candidates.Count, StringComparer.Ordinal);
        foreach (var candidate in candidates)
        {
            // First wins, and the ordering above makes "first" the lowest ProjectID.
            projectIDByIdentifier.TryAdd(candidate.ProjectGisIdentifier!.Trim().ToUpperInvariant(), candidate.ProjectID);
        }

        return projectIDByIdentifier;
    }

    /// <summary>
    /// Resolves the source org's default ProjectTypeID, with the same fallback-to-first-type the
    /// per-project version used, paid once.
    /// </summary>
    private static async Task<int> ResolveDefaultProjectTypeIDAsync(WADNRDbContext dbContext, string projectTypeDefaultName)
    {
        var projectTypeID = await dbContext.ProjectTypes
            .Where(pt => pt.ProjectTypeName == (projectTypeDefaultName ?? ""))
            .Select(pt => pt.ProjectTypeID)
            .FirstOrDefaultAsync();

        return projectTypeID != 0
            ? projectTypeID
            : await dbContext.ProjectTypes.Select(pt => pt.ProjectTypeID).FirstAsync();
    }

    /// <summary>
    /// Loads the Project Import Block List for a program and returns normalized
    /// (uppercased, trimmed) sets of blocked GIS identifiers and project names.
    /// Used by ImportProjectsAsync to skip blocked projects.
    /// </summary>
    public static async Task<(HashSet<string> BlockedIdentifiers, HashSet<string> BlockedNames)> LoadBlockListAsync(
        WADNRDbContext dbContext, int programID)
    {
        var blockListEntries = await dbContext.ProjectImportBlockLists
            .AsNoTracking()
            .Where(x => x.ProgramID == programID)
            .Select(x => new { x.ProjectGisIdentifier, x.ProjectName })
            .ToListAsync();

        var blockedIdentifiers = blockListEntries
            .Where(x => !string.IsNullOrWhiteSpace(x.ProjectGisIdentifier))
            .Select(x => x.ProjectGisIdentifier.Trim().ToUpperInvariant())
            .ToHashSet();

        var blockedNames = blockListEntries
            .Where(x => !string.IsNullOrWhiteSpace(x.ProjectName))
            .Select(x => x.ProjectName.Trim().ToUpperInvariant())
            .ToHashSet();

        return (blockedIdentifiers, blockedNames);
    }

    /// <summary>
    /// Returns true if the given project identifier or name matches any entry in the
    /// normalized block-list sets. Identifier is expected to be already trimmed + uppercased
    /// (the import loop normalizes it at the grouping step).
    /// </summary>
    public static bool IsBlocked(
        string projectIdentifierUpper,
        string projectName,
        HashSet<string> blockedIdentifiers,
        HashSet<string> blockedNames)
    {
        if (!string.IsNullOrWhiteSpace(projectIdentifierUpper)
            && blockedIdentifiers.Contains(projectIdentifierUpper))
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(projectName)
            && blockedNames.Contains(projectName.Trim().ToUpperInvariant()))
        {
            return true;
        }

        return false;
    }
}
