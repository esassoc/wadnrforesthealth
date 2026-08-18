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
            // Only areal geometry is usable: every downstream consumer treats a ProjectLocation as a
            // project *area* (acreage, treatment footprints, region/county intersection). Legacy
            // filtered to Polygon / MultiPolygon in IsUsableFeatureGeoJson; the rewrite only skipped
            // null geometry, so a point or line feature would have produced a zero-acre location.
            if (!IsUsableGeometry(feature.Geometry)) continue;

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

        // Resolve the metadata attribute IDs for the Esri system fields (stored lowercased)
        // so each created ProjectLocation can carry its source OBJECTID / GlobalID. Absent for
        // non-Esri sources, in which case ArcGisObjectID / ArcGisGlobalID stay null.
        var metadataAttributeIDByName = features
            .SelectMany(f => f.GisFeatureMetadataAttributes)
            .Select(m => m.GisMetadataAttribute)
            .GroupBy(a => a.GisMetadataAttributeName.ToLowerInvariant())
            .ToDictionary(g => g.Key, g => g.First().GisMetadataAttributeID);
        int? objectIdAttributeID = metadataAttributeIDByName.TryGetValue("objectid", out var oidAttrID) ? oidAttrID : null;
        int? globalIdAttributeID = metadataAttributeIDByName.TryGetValue("globalid", out var gidAttrID) ? gidAttrID : null;

        // Apply the source org's include/exclude column configuration before anything else looks at
        // the features, matching legacy's FilterListBasedOnIncludeExcludeCriteria. The rewrite loaded
        // this configuration and then never read it, so every program's whitelist/blacklist was inert
        // — DNR State Lands has a blacklist on technique_cd that was being ignored entirely.
        //
        // Scope note: this filters the in-memory feature list only, so excluded features are left out
        // of project and location creation but are still visible to
        // dbo.procImportTreatmentsFromGisUploadAttempt, which reads dbo.GisFeature scoped solely by
        // GisUploadAttemptID. An excluded feature can therefore still produce a Treatment. That is
        // deliberate: legacy behaved exactly the same way, and this change is scoped to restoring
        // legacy behaviour rather than extending it. Pushing the exclusion into the proc is a
        // separate change, and one worth making together with correcting the column-name mismatch
        // that currently stops this filter matching anything in production (the configuration names
        // technique_cd; uploads stage the 10-character truncated technique_).
        var featureCountBeforeFiltering = features.Count;
        features = ApplyExcludeIncludeFilters(features, sourceOrg, metadataAttributeIDByName, featureMetadata);
        var featuresExcluded = featureCountBeforeFiltering - features.Count;
        if (featuresExcluded > 0)
        {
            // Surfaced rather than dropped silently: an admin comparing the source GDB's feature
            // count against what landed needs to know the difference was deliberate.
            result.Warnings.Add(
                $"Excluded {featuresExcluded} of {featureCountBeforeFiltering} features per this program's " +
                $"GIS include/exclude column configuration.");
        }

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

        // Pre-load the Project Import Block List for this program. Match is case-insensitive
        // on either ProjectGisIdentifier or ProjectName, matching the normalization the
        // import loop already applies at line ~400.
        var (blockedIdentifiers, blockedNames) = await LoadBlockListAsync(dbContext, sourceOrg.ProgramID);

        // Block-list entries can also point straight at a ProjectID. Legacy honoured that; the
        // rewrite only matched on identifier/name, so an entry created by picking an existing
        // project did nothing.
        var blockedProjectIDs = await dbContext.ProjectImportBlockLists
            .AsNoTracking()
            .Where(x => x.ProgramID == sourceOrg.ProgramID && x.ProjectID != null)
            .Select(x => x.ProjectID!.Value)
            .ToHashSetAsync();

        // Private landowners imported from GIS metadata. Only pay for the Person list when the
        // Landowner column is actually mapped.
        //
        // Primary contact is deliberately not handled here. Legacy resolved it against
        // FieldDefinition.ProjectPrimaryContact (252), while the only GisDefaultMapping row that
        // exists is on FieldDefinition.PrimaryContact (275) — so it never imported on any legacy
        // path, and wiring it up now would be a new feature rather than restored parity.
        var importsPeople = request.PrivateLandownerMetadataAttributeID.HasValue;
        var personLookup = importsPeople ? await LoadPersonLookupAsync(dbContext) : null;

        // Legacy skipped a project outright when the program's default stage is Completed, the stage
        // isn't derived from the data, and the feature carries no completion date
        // (GisUploadSourceOrganization.RequiresCompletionDate). The rewrite imported it anyway.
        var requiresCompletionDate = sourceOrg.ProjectStageDefaultID == ProjectStage.Completed.ProjectStageID
            && !sourceOrg.DataDeriveProjectStage;
        var projectsSkippedForMissingCompletionDate = 0;

        // Default project type for newly-created projects. Resolved once — it doesn't vary per
        // project — and falling back to "Other" rather than an arbitrary row (WADNR-2287).
        var defaultProjectTypeID = await ResolveDefaultProjectTypeIDAsync(dbContext, sourceOrg);

        // Programs whose projects are candidates for matching an incoming GIS identifier. Normally
        // just this source org's program, but when the org belongs to a merge grouping (the USFS
        // sources) a project already created by a sibling program must be matched and updated
        // rather than duplicated. The program link and ProjectLocation.ProgramID below still use
        // sourceOrg.ProgramID.
        var matchProgramIDs = await ResolveMatchProgramIDsAsync(dbContext, sourceOrg);

        // Crosswalks configured on this source org, used to map raw GIS values onto FHT lookups.
        var projectStageCrossWalks = sourceOrg.GisCrossWalkDefaults
            .Where(x => x.FieldDefinitionID == FieldDefinition.ProjectStage.FieldDefinitionID)
            .ToList();
        var leadImplementerCrossWalks = sourceOrg.GisCrossWalkDefaults
            .Where(x => x.FieldDefinitionID == FieldDefinition.LeadImplementerOrganization.FieldDefinitionID)
            .ToList();

        // Only pay for the organization list when a LeadImplementer crosswalk is actually configured.
        var organizationIDByName = leadImplementerCrossWalks.Count > 0
            ? await LoadOrganizationIDsByNameAsync(dbContext)
            : new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

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

            // Skip projects that match the Project Import Block List for this program (creates AND updates).
            if (IsBlocked(projectIdentifier, projectName, blockedIdentifiers, blockedNames))
            {
                result.ProjectsBlocked++;
                result.BlockedProjects.Add(new GisBulkImportProjectResult
                {
                    ProjectID = 0,
                    ProjectName = projectName
                });
                continue;
            }

            // Project stage: the source org's configured default unless it is set to derive the
            // stage from the GIS data, in which case the raw value is run through the ProjectStage
            // crosswalk (WADNR-2287 — this was dropped in the rewrite and the default always won).
            var projectStageSourceValue = FirstNonEmptyMetadataValue(
                projectGroup, featureMetadata, request.ProjectStageMetadataAttributeID);

            // Dates are resolved across every feature in the project (earliest start, latest
            // completion) and understand ArcGIS Online's epoch-millisecond encoding — see
            // ResolveDateFromFeatures. The rewrite parsed only the first feature and only with
            // DateTime.TryParse, so nightly AGOL imports resolved no dates at all.
            var featureStartDate = ResolveDateFromFeatures(
                projectGroup, featureMetadata, request.StartDateMetadataAttributeID, useEarliest: true);
            var featureCompletionDate = ResolveDateFromFeatures(
                projectGroup, featureMetadata, request.CompletionDateMetadataAttributeID, useEarliest: false);
            var hasCompletionDate = featureCompletionDate.HasValue;
            var projectStageID = DeriveProjectStageID(
                sourceOrg, projectStageCrossWalks, projectStageSourceValue, hasCompletionDate);

            // A program whose projects are Completed by definition can't import one with no
            // completion date — legacy dropped these rather than creating a bad record.
            if (requiresCompletionDate && !hasCompletionDate)
            {
                projectsSkippedForMissingCompletionDate++;
                continue;
            }

            // Landowner values carried on the features, gathered across the whole project group the
            // way legacy did (distinct, non-empty, in feature order).
            var landownerValues = importsPeople
                ? DistinctMetadataValues(projectGroup, featureMetadata, request.PrivateLandownerMetadataAttributeID)
                : new List<string>();

            // Lead implementer: crosswalk the raw value onto an Organization, falling back to the
            // source org's configured default when unmapped or unrecognized.
            var leadImplementerSourceValue = FirstNonEmptyMetadataValue(
                projectGroup, featureMetadata, request.LeadImplementerMetadataAttributeID);
            var leadImplementerOrganizationID = ResolveLeadImplementerOrganizationID(
                sourceOrg, leadImplementerCrossWalks, organizationIDByName, leadImplementerSourceValue);

            // Per-iteration outcomes — applied to `result` only after the transaction commits,
            // so deadlock-triggered retries don't double-count.
            var wasCreated = false;
            var wasUpdated = false;
            var wasBlockedByProjectID = false;
            var locationsCreatedThisIteration = 0;
            // People created inside this iteration's transaction. Held back from the shared
            // personLookup until the transaction commits — see ApplyProjectLandownersAsync.
            var peopleCreatedThisIteration = new List<(int PersonID, string FirstName, string LastName, DateTime CreateDate)>();
            GisBulkImportProjectResult resultEntry = null;

            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                // Reset tracker + outcomes on each retry attempt so we re-run cleanly.
                dbContext.ChangeTracker.Clear();
                wasCreated = false;
                wasUpdated = false;
                wasBlockedByProjectID = false;
                locationsCreatedThisIteration = 0;
                peopleCreatedThisIteration.Clear();
                resultEntry = null;

                await using var transaction = await dbContext.Database.BeginTransactionAsync();

                // Find existing project by GIS identifier within the matching program(s) (case-insensitive)
                var existingProject = await dbContext.Projects
                    .Include(p => p.ProjectPrograms)
                    .FirstOrDefaultAsync(p => p.ProjectGisIdentifier != null &&
                        p.ProjectGisIdentifier.Trim().ToUpper() == projectIdentifier &&
                        p.ProjectPrograms.Any(pp => matchProgramIDs.Contains(pp.ProgramID)));

                if (existingProject != null && blockedProjectIDs.Contains(existingProject.ProjectID))
                {
                    // Block-list entry pointing at this exact project — skip create and update alike.
                    wasBlockedByProjectID = true;
                    resultEntry = new GisBulkImportProjectResult
                    {
                        ProjectID = existingProject.ProjectID,
                        ProjectName = existingProject.ProjectName
                    };
                    await transaction.RollbackAsync();
                    return;
                }

                if (existingProject != null)
                {
                    // Update fields from GIS data (matching legacy behavior)
                    existingProject.ProjectName = projectName.Length > 140 ? projectName[..140] : projectName;
                    existingProject.ProjectStageID = projectStageID;
                    existingProject.LastUpdateGisUploadAttemptID = gisUploadAttemptID;

                    // Auto-approve if stage is not Planned and project is Draft/PendingApproval
                    if (projectStageID != (int)ProjectStageEnum.Planned
                        && (existingProject.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.Draft
                            || existingProject.ProjectApprovalStatusID == (int)ProjectApprovalStatusEnum.PendingApproval))
                    {
                        existingProject.ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved;
                    }

                    // Update dates if configured. Widened by any treatments this project carries for
                    // *other* programs, so a shared project keeps a span covering all of them - legacy
                    // did this in CalculateStartDate / CalculateCompletionDate.
                    var (updateStartDate, updateCompletionDate) = await WidenDatesFromOtherProgramTreatmentsAsync(
                        dbContext, sourceOrg, existingProject.ProjectID, featureStartDate, featureCompletionDate);

                    if (sourceOrg.ApplyStartDateToProject && updateStartDate.HasValue)
                    {
                        existingProject.PlannedDate = DateOnly.FromDateTime(updateStartDate.Value);
                    }

                    if (sourceOrg.ApplyCompletedDateToProject && updateCompletionDate.HasValue)
                    {
                        existingProject.CompletionDate = DateOnly.FromDateTime(updateCompletionDate.Value);
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

                    wasUpdated = true;
                    resultEntry = new GisBulkImportProjectResult
                    {
                        ProjectID = existingProject.ProjectID,
                        ProjectName = existingProject.ProjectName
                    };
                }
                else
                {
                    var newProject = new Project
                    {
                        ProjectName = projectName.Length > 140 ? projectName[..140] : projectName,
                        FhtProjectNumber = await Projects.GenerateFhtProjectNumberAsync(dbContext),
                        ProjectGisIdentifier = originalIdentifier.Length > 140 ? originalIdentifier[..140] : originalIdentifier,
                        ProjectTypeID = defaultProjectTypeID,
                        ProjectStageID = projectStageID,
                        ProjectApprovalStatusID = (int)ProjectApprovalStatusEnum.Approved,
                        ProjectLocationSimpleTypeID = (int)ProjectLocationSimpleTypeEnum.None,
                        CreateGisUploadAttemptID = gisUploadAttemptID,
                        LastUpdateGisUploadAttemptID = gisUploadAttemptID
                    };

                    // Set dates if configured. A brand-new project has no treatments yet, so there is
                    // nothing to widen against.
                    if (sourceOrg.ApplyStartDateToProject && featureStartDate.HasValue)
                    {
                        newProject.PlannedDate = DateOnly.FromDateTime(featureStartDate.Value);
                    }

                    if (sourceOrg.ApplyCompletedDateToProject && featureCompletionDate.HasValue)
                    {
                        newProject.CompletionDate = DateOnly.FromDateTime(featureCompletionDate.Value);
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

                    // Create the lead implementer relationship — the crosswalked organization when
                    // the GIS data maps to one, otherwise the source org's configured default.
                    dbContext.ProjectOrganizations.Add(new ProjectOrganization
                    {
                        ProjectID = newProject.ProjectID,
                        OrganizationID = leadImplementerOrganizationID,
                        RelationshipTypeID = sourceOrg.RelationshipTypeForDefaultOrganizationID
                    });

                    existingProject = newProject;
                    wasCreated = true;
                    resultEntry = new GisBulkImportProjectResult
                    {
                        ProjectID = newProject.ProjectID,
                        ProjectName = newProject.ProjectName
                    };
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
                    // Carry the source Esri identifiers through so downstream consumers (e.g. the
                    // GDB export's ProjectLocations layer) can join back to the source service.
                    var metadata = featureMetadata[feature.GisFeatureID];
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

                    // Derive the location name from a stable source identifier rather than the
                    // positional GisImportFeatureKey (a per-attempt running counter). The Esri
                    // OBJECTID / GlobalID are stable across re-imports, so re-running an import
                    // produces the same names (delete-then-recreate stays clean) and two
                    // unrelated features can no longer land on the same name. Falls back to the
                    // feature key only for non-Esri sources that carry neither identifier.
                    var featureIdentifier = arcGisObjectID?.ToString()
                        ?? arcGisGlobalID
                        ?? feature.GisImportFeatureKey.ToString();
                    var locationName = $"{originalIdentifier} - Feature {featureIdentifier}";

                    dbContext.ProjectLocations.Add(new ProjectLocation
                    {
                        ProjectID = existingProject.ProjectID,
                        // GIS source features can be topologically invalid (self-intersections, etc.).
                        // Normalize on the way in, matching the interactive project workflow
                        // (ProjectCreateWorkflowSteps.cs), so downstream spatial operations
                        // (AutoAssignGeographicRegionsAsync's STIntersects, GeoServer, GDB export)
                        // don't hit SQL Server error 24144 on an invalid instance.
                        ProjectLocationGeometry = feature.GisFeatureGeometry?.MakeValid(),
                        ProjectLocationName = locationName.Length > 100 ? locationName[..100] : locationName,
                        ProjectLocationTypeID = (int)ProjectLocationTypeEnum.ProjectArea,
                        ImportedFromGisUpload = true,
                        ProgramID = sourceOrg.ProgramID,
                        ArcGisObjectID = arcGisObjectID,
                        ArcGisGlobalID = arcGisGlobalID
                    });
                    locationsCreatedThisIteration++;
                }

                await dbContext.SaveChangesWithNoAuditingAsync();

                // Private landowners and primary contact from the GIS metadata. Ports legacy's
                // MakeProjectPeopleAndSave, which the rewrite dropped entirely even though the
                // Landowner column mapping is still configured (and still posted by the UI).
                if (importsPeople)
                {
                    await ApplyProjectLandownersAsync(
                        dbContext, personLookup, existingProject.ProjectID, landownerValues, peopleCreatedThisIteration);
                }

                // Populate County / DNR Upland Region / Priority Landscape from the just-saved
                // location geometries, mirroring the interactive project workflow. Runs inside the
                // same transaction (atomic with the locations) and saves without auditing, matching
                // this pipeline's convention. Idempotent, so execution-strategy retries are safe.
                await ProjectCreateWorkflowSteps.AutoAssignGeographicRegionsAsync(
                    dbContext, existingProject.ProjectID, useNoAuditingSave: true);

                await transaction.CommitAsync();
            });

            // Apply outcomes after the transaction commits (retries won't double-count)
            personLookup?.Merge(peopleCreatedThisIteration);

            if (wasBlockedByProjectID)
            {
                result.ProjectsBlocked++;
                result.BlockedProjects.Add(resultEntry);
                continue;
            }

            result.LocationsCreated += locationsCreatedThisIteration;
            if (wasCreated)
            {
                result.ProjectsCreated++;
                result.CreatedProjects.Add(resultEntry);
            }
            else if (wasUpdated)
            {
                result.ProjectsUpdated++;
                result.UpdatedProjects.Add(resultEntry);
            }
        }

        if (projectsSkippedForMissingCompletionDate > 0)
        {
            result.Warnings.Add(
                $"Skipped {projectsSkippedForMissingCompletionDate} project(s) with no completion date, because " +
                $"this program's default project stage is Completed and it does not derive the stage from the data.");
        }

        // Call the treatment import proc. Sources configured to import detailed locations *instead
        // of* treatments (Forest Stewardship, ProgramID 5) skip it entirely, matching legacy.
        var treatmentsImported = true;
        if (!sourceOrg.ImportAsDetailedLocationInsteadOfTreatments)
        {
            try
            {
                await ImportTreatmentsAsync(dbContext, gisUploadAttemptID, request, sourceOrg);
            }
            catch (Exception ex)
            {
                // Treatments failed but the projects/locations created above are already committed, so this
                // stays a warning on an otherwise successful import rather than failing the whole request.
                // GisBulkImportController logs the populated Warnings so the failure reaches Datadog with
                // the attempt ID attached — the bare EF "Failed executing DbCommand" entry has no context.
                treatmentsImported = false;
                result.Warnings.Add(
                    $"Treatment import failed, so no treatments were created for this upload. " +
                    $"Projects and locations were still imported. Error: {ex.Message}");
            }
        }

        // Legacy re-derived each touched project's type from its treatments *after* the treatment
        // import (UpdateProjectTypesIfNeeded). The rewrite dropped this, so the source org's
        // AdjustProjectTypeBasedOnTreatmentTypes flag became inert while still being editable in the
        // Program admin UI — WADNR-2287. Skipped when the treatment import failed, so we never derive
        // a type from a partially imported treatment set.
        if (treatmentsImported)
        {
            await ApplyProjectTypeFromTreatmentTypesAsync(dbContext, sourceOrg, gisUploadAttemptID);
        }

        if (sourceOrg.ImportAsDetailedLocationInsteadOfTreatments)
        {
            // The proc is what normally sets Project.ProjectLocationPoint / ProjectLocationSimpleTypeID
            // (its final UPDATE, joining Project -> Treatment -> ProjectLocation). Gating the proc off
            // above would otherwise leave these sources with no simple location at all, so derive the
            // centroid here the way legacy's MakeProjectLocationsAndSave did for exactly this flag.
            await ApplySimpleLocationFromProjectAreasAsync(dbContext, gisUploadAttemptID);
        }

        // Assign the DNR Service Forestry Regional Coordinator to newly-created Landowner Assistance
        // projects, from the DNR Upland Region they landed in. Ports legacy's AddProjectCoordinators,
        // which ran right after the regions were calculated. Runs last because it depends on the
        // regions AutoAssignGeographicRegionsAsync assigned inside the loop.
        await ApplyRegionalCoordinatorsAsync(dbContext, gisUploadAttemptID);

        // Clear this attempt's staged GIS features once everything above succeeded, so the staging
        // tables don't grow without bound. Legacy called dbo.pClearGisImportTables, which does an
        // unfiltered DELETE across every attempt's features plus an index rebuild and a full-scan
        // statistics update — unsafe here, because it would destroy a concurrently running AGOL
        // import's staged features and block the request while it ran. Scoped to this attempt, and
        // skipped whenever anything went wrong so the staged data is still there to diagnose.
        // Gated on an actual failure, NOT on Warnings being empty: Warnings also carries purely
        // informational outcomes (features excluded by configuration, projects skipped for a missing
        // completion date), and treating those as failures would permanently suppress the cleanup
        // this block exists to perform.
        if (treatmentsImported)
        {
            await ClearStagedFeaturesAsync(dbContext, gisUploadAttemptID);
        }

        return result;
    }

    /// <summary>
    /// Builds the parameter set for dbo.procImportTreatmentsFromGisUploadAttempt and runs it.
    /// </summary>
    private static async Task ImportTreatmentsAsync(
        WADNRDbContext dbContext,
        int gisUploadAttemptID,
        GisBulkImportRequest request,
        GisUploadSourceOrganization sourceOrg)
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

    /// <summary>
    /// Whether a staged GeoJSON feature's geometry is usable as a project area. Ports legacy's
    /// IsUsableFeatureGeoJson / ListOfValidGeoJsonTypes, which accepted Polygon and MultiPolygon only.
    /// </summary>
    private static bool IsUsableGeometry(NetTopologySuite.Geometries.Geometry geometry) =>
        geometry is NetTopologySuite.Geometries.Polygon or NetTopologySuite.Geometries.MultiPolygon;

    /// <summary>
    /// Applies the source organization's include/exclude column configuration to the feature set.
    /// Ports legacy's FilterListBasedOnIncludeExcludeCriteria: each configured column either
    /// whitelists (keep only features whose value matches) or blacklists (drop those features).
    /// Columns compose — each one narrows the set produced by the previous.
    ///
    /// Matching is case-insensitive and trimmed on both the column name and the values. Legacy used
    /// ordinal comparisons, but the modern upload path lowercases every metadata attribute name
    /// (UploadAndProcessFileAsync), so an ordinal match on the configured column name would silently
    /// never fire for any mixed-case configuration.
    /// </summary>
    private static List<GisFeature> ApplyExcludeIncludeFilters(
        List<GisFeature> features,
        GisUploadSourceOrganization sourceOrg,
        Dictionary<string, int> metadataAttributeIDByName,
        Dictionary<int, Dictionary<int, string>> featureMetadata)
    {
        var excludeIncludeColumns = sourceOrg.GisExcludeIncludeColumns.ToList();
        if (excludeIncludeColumns.Count == 0)
        {
            return features;
        }

        var filtered = features;
        foreach (var column in excludeIncludeColumns)
        {
            var columnName = column.GisDefaultMappingColumnName?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(columnName)
                || !metadataAttributeIDByName.TryGetValue(columnName, out var metadataAttributeID))
            {
                // Configured against a column this upload doesn't carry — nothing to filter on.
                continue;
            }

            var filterValues = column.GisExcludeIncludeColumnValues
                .Select(x => x.GisExcludeIncludeColumnValueForFiltering?.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

            if (filterValues.Count == 0)
            {
                continue;
            }

            bool MatchesFilterValue(GisFeature feature) =>
                featureMetadata.TryGetValue(feature.GisFeatureID, out var metadata)
                && metadata.TryGetValue(metadataAttributeID, out var value)
                && value != null
                && filterValues.Contains(value.Trim());

            filtered = column.IsWhitelist
                ? filtered.Where(MatchesFilterValue).ToList()
                : filtered.Where(x => !MatchesFilterValue(x)).ToList();
        }

        return filtered;
    }

    /// <summary>
    /// Distinct non-empty values of a metadata attribute across a project's features, in feature
    /// order — the shape legacy's landowner / primary-contact dictionaries produced.
    /// </summary>
    private static List<string> DistinctMetadataValues(
        IEnumerable<GisFeature> projectFeatures,
        Dictionary<int, Dictionary<int, string>> featureMetadata,
        int? metadataAttributeID)
    {
        if (!metadataAttributeID.HasValue)
        {
            return new List<string>();
        }

        var values = new List<string>();
        var seen = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        foreach (var feature in projectFeatures)
        {
            if (featureMetadata.TryGetValue(feature.GisFeatureID, out var metadata)
                && metadata.TryGetValue(metadataAttributeID.Value, out var value)
                && !string.IsNullOrWhiteSpace(value)
                && seen.Add(value.Trim()))
            {
                values.Add(value.Trim());
            }
        }

        return values;
    }

    /// <summary>
    /// Minimal in-memory Person index used to match GIS landowner names and contact emails against
    /// existing people, mirroring legacy's "load all People once" approach without dragging the
    /// whole entity graph in.
    /// </summary>
    private sealed class PersonLookup
    {
        public List<(int PersonID, string FirstName, string LastName, DateTime CreateDate)> People { get; init; } = new();

        public int? FindByName(string firstName, string lastName) => People
            .Where(x => string.Equals(x.FirstName, firstName, StringComparison.InvariantCultureIgnoreCase)
                && (string.Equals(x.LastName, lastName, StringComparison.InvariantCultureIgnoreCase)
                    || (string.IsNullOrEmpty(x.LastName) && string.IsNullOrEmpty(lastName))))
            .OrderBy(x => x.CreateDate)
            .Select(x => (int?)x.PersonID)
            .FirstOrDefault();

        public void Add(int personID, string firstName, string lastName, DateTime createDate) =>
            People.Add((personID, firstName, lastName, createDate));

        /// <summary>Merges people created by a transaction that has now committed.</summary>
        public void Merge(IEnumerable<(int PersonID, string FirstName, string LastName, DateTime CreateDate)> created)
        {
            foreach (var person in created)
            {
                People.Add(person);
            }
        }
    }

    private static string Truncate(string value, int maxLength) =>
        value != null && value.Length > maxLength ? value[..maxLength] : value;

    private static async Task<PersonLookup> LoadPersonLookupAsync(WADNRDbContext dbContext)
    {
        var people = await dbContext.People
            .AsNoTracking()
            .Select(x => new { x.PersonID, x.FirstName, x.LastName, x.CreateDate })
            .ToListAsync();

        return new PersonLookup
        {
            People = people
                .Select(x => (x.PersonID, x.FirstName, x.LastName, x.CreateDate))
                .ToList()
        };
    }

    /// <summary>
    /// Creates the ProjectPerson rows for a project's private landowners from the GIS metadata,
    /// creating Person records for landowners we've never seen. Ports legacy's
    /// GenerateProjectPersonListForPrivateLandowners, which the rewrite dropped entirely even though
    /// the Landowner column mapping is still configured on DNR LOA NE and still posted by the UI.
    ///
    /// Matching legacy, existing landowner rows are replaced only when the import actually carries
    /// landowner values — an upload with no landowner column never clears ones entered by hand.
    /// </summary>
    private static async Task ApplyProjectLandownersAsync(
        WADNRDbContext dbContext,
        PersonLookup personLookup,
        int projectID,
        List<string> landownerValues,
        List<(int PersonID, string FirstName, string LastName, DateTime CreateDate)> createdPeople)
    {
        if (landownerValues.Count == 0)
        {
            return;
        }

        // Tracked removal rather than ExecuteDeleteAsync: this is at most a handful of rows per
        // project, and it keeps the change inside the surrounding transaction's change tracker.
        var existingLandownerRows = await dbContext.ProjectPeople
            .Where(x => x.ProjectID == projectID
                && x.ProjectPersonRelationshipTypeID == ProjectPersonRelationshipType.PrivateLandowner.ProjectPersonRelationshipTypeID)
            .ToListAsync();
        dbContext.ProjectPeople.RemoveRange(existingLandownerRows);

        foreach (var landowner in landownerValues)
        {
            var (firstName, lastName) = SplitLandownerName(landowner);
            if (string.IsNullOrWhiteSpace(firstName))
            {
                continue;
            }

            // Person.FirstName / LastName are varchar(100). GIS landowner values are unbounded text
            // and routinely carry long trust names, so truncate the way every other string
            // assignment in this pipeline does rather than letting a 2628 abort the whole import.
            firstName = Truncate(firstName, PersonNameMaxLength);
            lastName = Truncate(lastName, PersonNameMaxLength);

            var personID = personLookup.FindByName(firstName, lastName);
            if (personID == null)
            {
                var person = new Person
                {
                    FirstName = firstName,
                    LastName = lastName,
                    CreateDate = DateTime.UtcNow,
                    IsActive = true,
                    IsUser = false,
                    ReceiveSupportEmails = false,
                    CreatedAsPartOfBulkImport = true
                };
                dbContext.People.Add(person);
                await dbContext.SaveChangesWithNoAuditingAsync();

                dbContext.PersonRoles.Add(new PersonRole
                {
                    PersonID = person.PersonID,
                    RoleID = Role.Unassigned.RoleID
                });

                // Buffered rather than written straight into personLookup: this runs inside the
                // per-project transaction, and an execution-strategy retry rolls the Person insert
                // back. Publishing the ID to the shared index before the commit would leave the
                // retry resolving a PersonID that no longer exists and violating the ProjectPerson
                // foreign key. The caller merges the buffer only after the transaction commits.
                createdPeople.Add((person.PersonID, firstName, lastName, person.CreateDate));
                personID = person.PersonID;
            }

            dbContext.ProjectPeople.Add(new ProjectPerson
            {
                ProjectID = projectID,
                PersonID = personID.Value,
                ProjectPersonRelationshipTypeID = ProjectPersonRelationshipType.PrivateLandowner.ProjectPersonRelationshipTypeID,
                CreatedAsPartOfBulkImport = true
            });
        }

        await dbContext.SaveChangesWithNoAuditingAsync();
    }

    /// <summary>
    /// Splits a GIS landowner value into first/last name. Ports legacy's ExtractFirstName /
    /// ExtractLastName: "Last, First" splits on the comma; anything else becomes the first name whole.
    /// </summary>
    private static (string FirstName, string LastName) SplitLandownerName(string landowner)
    {
        var parts = landowner.Split(',');
        return parts.Length == 2
            ? (parts[1].Trim(), parts[0].Trim())
            : (landowner.Trim(), null);
    }

    /// <summary>
    /// Assigns the DNR Service Forestry Regional Coordinator to projects this attempt created that
    /// are in the Landowner Assistance program and landed in a DNR Upland Region that has one.
    /// Ports legacy's AddProjectCoordinators.
    /// </summary>
    private static async Task ApplyRegionalCoordinatorsAsync(WADNRDbContext dbContext, int gisUploadAttemptID)
    {
        var coordinatorPairs = await dbContext.ProjectRegions
            .AsNoTracking()
            .Where(pr => pr.Project.CreateGisUploadAttemptID == gisUploadAttemptID
                && pr.Project.ProjectPrograms.Any(pp => pp.ProgramID == Program.LandownerAssistanceProgramID)
                && pr.DNRUplandRegion.DNRUplandRegionCoordinatorID != null)
            .Select(pr => new { pr.ProjectID, PersonID = pr.DNRUplandRegion.DNRUplandRegionCoordinatorID!.Value })
            .Distinct()
            .ToListAsync();

        if (coordinatorPairs.Count == 0)
        {
            return;
        }

        var relationshipTypeID = ProjectPersonRelationshipType.ServiceForestryRegionalCoordinator.ProjectPersonRelationshipTypeID;
        var projectIDs = coordinatorPairs.Select(x => x.ProjectID).Distinct().ToList();

        var existingPairs = (await dbContext.ProjectPeople
                .AsNoTracking()
                .Where(x => projectIDs.Contains(x.ProjectID) && x.ProjectPersonRelationshipTypeID == relationshipTypeID)
                .Select(x => new { x.ProjectID, x.PersonID })
                .ToListAsync())
            .Select(x => (x.ProjectID, x.PersonID))
            .ToHashSet();

        var added = false;
        foreach (var pair in coordinatorPairs)
        {
            if (!existingPairs.Add((pair.ProjectID, pair.PersonID)))
            {
                continue;
            }

            dbContext.ProjectPeople.Add(new ProjectPerson
            {
                ProjectID = pair.ProjectID,
                PersonID = pair.PersonID,
                ProjectPersonRelationshipTypeID = relationshipTypeID,
                CreatedAsPartOfBulkImport = true
            });
            added = true;
        }

        if (added)
        {
            await dbContext.SaveChangesWithNoAuditingAsync();
        }
    }

    /// <summary>
    /// Sets Project.ProjectLocationPoint to the centroid of the project's imported project areas and
    /// marks the simple location type as a point on the map.
    ///
    /// Only needed for sources that skip the treatment proc: the proc's final statement normally does
    /// this, so gating it off would otherwise leave those projects with no simple location. Legacy
    /// computed the same centroid inline for exactly these sources.
    /// </summary>
    private static async Task ApplySimpleLocationFromProjectAreasAsync(WADNRDbContext dbContext, int gisUploadAttemptID)
    {
        var projects = await dbContext.Projects
            .Where(p => p.LastUpdateGisUploadAttemptID == gisUploadAttemptID)
            .ToListAsync();

        if (projects.Count == 0)
        {
            return;
        }

        var projectIDs = projects.Select(p => p.ProjectID).ToList();
        var geometriesByProjectID = (await dbContext.ProjectLocations
                .AsNoTracking()
                .Where(pl => projectIDs.Contains(pl.ProjectID)
                    && pl.ProjectLocationTypeID == (int)ProjectLocationTypeEnum.ProjectArea
                    && pl.ProjectLocationGeometry != null)
                .Select(pl => new { pl.ProjectID, pl.ProjectLocationGeometry })
                .ToListAsync())
            .GroupBy(x => x.ProjectID)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ProjectLocationGeometry!).ToList());

        var anyChanged = false;
        foreach (var project in projects)
        {
            if (!geometriesByProjectID.TryGetValue(project.ProjectID, out var geometries) || geometries.Count == 0)
            {
                continue;
            }

            // Union then centroid, matching the proc's geometry::UnionAggregate(...).STCentroid().
            var combined = geometries[0];
            for (var i = 1; i < geometries.Count; i++)
            {
                combined = combined.Union(geometries[i]);
            }

            var centroid = combined?.Centroid;
            if (centroid == null || centroid.IsEmpty)
            {
                continue;
            }

            // Only fill an unset simple location. This runs for every project the attempt touched,
            // not just newly-created ones, so overwriting unconditionally would replace a point a
            // steward positioned by hand with the computed centroid on every nightly run.
            if (project.ProjectLocationSimpleTypeID != (int)ProjectLocationSimpleTypeEnum.None
                && project.ProjectLocationPoint != null)
            {
                continue;
            }

            project.ProjectLocationPoint = centroid;
            project.ProjectLocationSimpleTypeID = (int)ProjectLocationSimpleTypeEnum.PointOnMap;
            anyChanged = true;
        }

        if (anyChanged)
        {
            await dbContext.SaveChangesWithNoAuditingAsync();
        }
    }

    /// <summary>
    /// Deletes this attempt's staged GisFeature / GisFeatureMetadataAttribute rows once the import
    /// has fully succeeded. See the call site for why this is scoped to one attempt rather than
    /// calling legacy's dbo.pClearGisImportTables.
    /// </summary>
    private static async Task ClearStagedFeaturesAsync(WADNRDbContext dbContext, int gisUploadAttemptID)
    {
        var featureIDsQuery = dbContext.GisFeatures
            .Where(f => f.GisUploadAttemptID == gisUploadAttemptID)
            .Select(f => f.GisFeatureID);

        // Bulk delete is the whole point here — an upload can stage hundreds of thousands of
        // metadata rows — but ExecuteDelete is relational-only, so fall back to tracked removal on
        // non-relational providers (the in-memory tests) to keep behaviour identical either way.
        if (dbContext.Database.IsRelational())
        {
            await dbContext.GisFeatureMetadataAttributes
                .Where(x => featureIDsQuery.Contains(x.GisFeatureID))
                .ExecuteDeleteAsync();

            await dbContext.GisFeatures
                .Where(x => x.GisUploadAttemptID == gisUploadAttemptID)
                .ExecuteDeleteAsync();
            return;
        }

        var featureIDs = await featureIDsQuery.ToListAsync();
        dbContext.GisFeatureMetadataAttributes.RemoveRange(
            await dbContext.GisFeatureMetadataAttributes.Where(x => featureIDs.Contains(x.GisFeatureID)).ToListAsync());
        dbContext.GisFeatures.RemoveRange(
            await dbContext.GisFeatures.Where(x => x.GisUploadAttemptID == gisUploadAttemptID).ToListAsync());
        await dbContext.SaveChangesWithNoAuditingAsync();
    }

    /// <summary>Person.FirstName / LastName are varchar(100); GIS landowner values are unbounded text.</summary>
    private const int PersonNameMaxLength = 100;

    /// <summary>1990-01-01 and 2100-01-01 as epoch milliseconds — the plausible range for a project date.</summary>
    private const long EarliestPlausibleEpochMilliseconds = 631152000000L;
    private const long LatestPlausibleEpochMilliseconds = 4102444800000L;

    private const string CommercialProjectTypeName = "Commercial vegetation treatment";
    private const string NonCommercialProjectTypeName = "Non-commercial vegetation treatment";
    private const string PrescribedFireProjectTypeName = "Prescribed fire treatment";
    private const string OtherProjectTypeName = "Other";

    /// <summary>
    /// Resolves the ProjectTypeID assigned to projects this import creates: the source
    /// organization's configured ProjectTypeDefaultName, falling back to "Other".
    ///
    /// WADNR-2287: this previously fell back to <c>ProjectTypes.First()</c> — with no ORDER BY that
    /// is whatever row SQL Server returns first, i.e. the lowest ProjectTypeID, which in production
    /// is "Research and Monitoring". Every project created by a source org with no configured
    /// default (DNR State Lands) landed on that arbitrary type. Legacy fell back to "Other".
    ///
    /// Throws rather than picking an arbitrary row if "Other" is missing — a wrong project type on
    /// public-facing data is worse than a failed import.
    /// </summary>
    private static async Task<int> ResolveDefaultProjectTypeIDAsync(
        WADNRDbContext dbContext, GisUploadSourceOrganization sourceOrg)
    {
        var projectTypes = await LoadProjectTypeIDsByNameAsync(dbContext);

        var configuredName = sourceOrg.ProjectTypeDefaultName?.Trim();
        if (!string.IsNullOrEmpty(configuredName) && projectTypes.TryGetValue(configuredName, out var configuredID))
        {
            return configuredID;
        }

        if (projectTypes.TryGetValue(OtherProjectTypeName, out var otherID))
        {
            return otherID;
        }

        throw new InvalidOperationException(
            $"GIS bulk import cannot assign a project type: source organization " +
            $"'{sourceOrg.GisUploadSourceOrganizationName}' has no usable ProjectTypeDefaultName " +
            $"('{sourceOrg.ProjectTypeDefaultName}') and no ProjectType named '{OtherProjectTypeName}' exists to fall back to.");
    }

    /// <summary>
    /// ProjectTypeID by trimmed name, case-insensitive. ProjectType is user-managed data (not a
    /// seeded lookup), so IDs differ between environments — always resolve by name.
    /// </summary>
    private static async Task<Dictionary<string, int>> LoadProjectTypeIDsByNameAsync(WADNRDbContext dbContext)
    {
        var projectTypes = await dbContext.ProjectTypes
            .AsNoTracking()
            .Select(pt => new { pt.ProjectTypeID, pt.ProjectTypeName })
            .ToListAsync();

        return projectTypes
            .Where(pt => !string.IsNullOrWhiteSpace(pt.ProjectTypeName))
            .GroupBy(pt => pt.ProjectTypeName.Trim(), StringComparer.InvariantCultureIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().ProjectTypeID, StringComparer.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// OrganizationID by trimmed name, case-insensitive — used by the LeadImplementer crosswalk,
    /// whose mapped values are organization names rather than IDs.
    /// </summary>
    private static async Task<Dictionary<string, int>> LoadOrganizationIDsByNameAsync(WADNRDbContext dbContext)
    {
        var organizations = await dbContext.Organizations
            .AsNoTracking()
            .Select(o => new { o.OrganizationID, o.OrganizationName })
            .ToListAsync();

        return organizations
            .Where(o => !string.IsNullOrWhiteSpace(o.OrganizationName))
            .GroupBy(o => o.OrganizationName.Trim(), StringComparer.InvariantCultureIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().OrganizationID, StringComparer.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// The programs whose projects an incoming GIS identifier may match. Normally just the source
    /// organization's own program; when the org belongs to a GisUploadProgramMergeGrouping (the USFS
    /// sources) every program in the grouping participates, so a project already created by a
    /// sibling program is updated rather than duplicated.
    /// </summary>
    private static async Task<List<int>> ResolveMatchProgramIDsAsync(
        WADNRDbContext dbContext, GisUploadSourceOrganization sourceOrg)
    {
        if (!sourceOrg.GisUploadProgramMergeGroupingID.HasValue)
        {
            return new List<int> { sourceOrg.ProgramID };
        }

        var programIDs = await dbContext.GisUploadSourceOrganizations
            .AsNoTracking()
            .Where(x => x.GisUploadProgramMergeGroupingID == sourceOrg.GisUploadProgramMergeGroupingID.Value)
            .Select(x => x.ProgramID)
            .Distinct()
            .ToListAsync();

        if (!programIDs.Contains(sourceOrg.ProgramID))
        {
            programIDs.Add(sourceOrg.ProgramID);
        }

        return programIDs;
    }

    /// <summary>
    /// First non-empty value of the given metadata attribute across a project's features.
    /// Returns null when the attribute isn't mapped or no feature carries a value.
    /// </summary>
    private static string FirstNonEmptyMetadataValue(
        IEnumerable<GisFeature> projectFeatures,
        Dictionary<int, Dictionary<int, string>> featureMetadata,
        int? metadataAttributeID)
    {
        if (!metadataAttributeID.HasValue)
        {
            return null;
        }

        foreach (var feature in projectFeatures)
        {
            if (featureMetadata.TryGetValue(feature.GisFeatureID, out var metadata)
                && metadata.TryGetValue(metadataAttributeID.Value, out var value)
                && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    /// <summary>
    /// Resolves a date for a project from a metadata column across all of its features: the earliest
    /// value for a start date, the latest for a completion date. Ports legacy CalculateStartDate /
    /// CalculateCompletionDate.
    ///
    /// The epoch fallback is the important part. A GDB upload carries dates as text that
    /// DateTime.TryParse understands, but the ArcGIS Online endpoints the nightly jobs read return
    /// them as epoch milliseconds, which land here as bare digit strings. Legacy handled both; the
    /// rewrite parsed only with DateTime.TryParse and only on the first feature, so the nightly LOA
    /// and USFS imports resolved no dates at all.
    /// </summary>
    private static DateTime? ResolveDateFromFeatures(
        IEnumerable<GisFeature> projectFeatures,
        Dictionary<int, Dictionary<int, string>> featureMetadata,
        int? metadataAttributeID,
        bool useEarliest)
    {
        if (!metadataAttributeID.HasValue)
        {
            return null;
        }

        var rawValues = new List<string>();
        foreach (var feature in projectFeatures)
        {
            if (featureMetadata.TryGetValue(feature.GisFeatureID, out var metadata)
                && metadata.TryGetValue(metadataAttributeID.Value, out var value)
                && !string.IsNullOrWhiteSpace(value))
            {
                rawValues.Add(value.Trim());
            }
        }

        var distinctValues = rawValues.Distinct(StringComparer.InvariantCultureIgnoreCase).ToList();

        var parsedDates = distinctValues
            .Where(x => DateTime.TryParse(x, out _))
            .Select(DateTime.Parse)
            .ToList();

        if (parsedDates.Count == 0)
        {
            // Bounded deliberately. long.TryParse accepts any digit string, so an unbounded epoch
            // read would turn a yyyyMMdd value like "20240115" into 1970-01-01, and a numeric ID or
            // tick count would make DateTimeOffset.FromUnixTimeMilliseconds throw and fail the whole
            // import. Only values landing in a plausible project date range are accepted as epochs.
            parsedDates = distinctValues
                .Select(x => long.TryParse(x, out var ms) ? (long?)ms : null)
                .Where(ms => ms.HasValue
                    && ms.Value >= EarliestPlausibleEpochMilliseconds
                    && ms.Value <= LatestPlausibleEpochMilliseconds)
                .Select(ms => DateTimeOffset.FromUnixTimeMilliseconds(ms!.Value).UtcDateTime)
                .ToList();
        }

        if (parsedDates.Count == 0)
        {
            return null;
        }

        return useEarliest ? parsedDates.Min() : parsedDates.Max();
    }

    /// <summary>
    /// Widens a project start / completion date to cover treatments it carries for programs other
    /// than the one being imported, so a project shared across programs keeps a span covering all of
    /// them rather than being clipped to this import own features. Ports the second half of legacy
    /// CalculateStartDate / CalculateCompletionDate.
    /// </summary>
    private static async Task<(DateTime? StartDate, DateTime? CompletionDate)> WidenDatesFromOtherProgramTreatmentsAsync(
        WADNRDbContext dbContext,
        GisUploadSourceOrganization sourceOrg,
        int projectID,
        DateTime? startDate,
        DateTime? completionDate)
    {
        if (!sourceOrg.ApplyStartDateToProject && !sourceOrg.ApplyCompletedDateToProject)
        {
            return (startDate, completionDate);
        }

        var otherProgramDates = await dbContext.Treatments
            .AsNoTracking()
            .Where(t => t.ProjectID == projectID && t.ProgramID != null && t.ProgramID != sourceOrg.ProgramID)
            .Select(t => new { t.TreatmentStartDate, t.TreatmentEndDate })
            .ToListAsync();

        if (otherProgramDates.Count == 0)
        {
            return (startDate, completionDate);
        }

        var otherStartDates = otherProgramDates
            .Where(x => x.TreatmentStartDate != null)
            .Select(x => x.TreatmentStartDate!.Value.ToDateTime(TimeOnly.MinValue))
            .ToList();
        if (otherStartDates.Count > 0)
        {
            var earliestOtherStart = otherStartDates.Min();
            if (!startDate.HasValue || earliestOtherStart < startDate.Value)
            {
                startDate = earliestOtherStart;
            }
        }

        var otherEndDates = otherProgramDates
            .Where(x => x.TreatmentEndDate != null)
            .Select(x => x.TreatmentEndDate!.Value.ToDateTime(TimeOnly.MinValue))
            .ToList();
        if (otherEndDates.Count > 0)
        {
            var latestOtherEnd = otherEndDates.Max();
            if (!completionDate.HasValue || latestOtherEnd > completionDate.Value)
            {
                completionDate = latestOtherEnd;
            }
        }

        return (startDate, completionDate);
    }

    /// <summary>
    /// Project stage for a project in this import. Ports legacy's CalculateProjectStageIfNeeded:
    /// the source org's configured default unless DataDeriveProjectStage is set, in which case the
    /// raw GIS value is run through the ProjectStage crosswalk. WADNR-2287 — the rewrite dropped
    /// this entirely and always used the configured default.
    ///
    /// Legacy used <c>Single(...)</c> on the crosswalk lookup, which threw and failed the whole
    /// import when a source value had no crosswalk row. This falls back instead.
    /// </summary>
    private static int DeriveProjectStageID(
        GisUploadSourceOrganization sourceOrg,
        List<GisCrossWalkDefault> projectStageCrossWalks,
        string projectStageSourceValue,
        bool hasCompletionDate)
    {
        var projectStageID = sourceOrg.ProjectStageDefaultID;

        if (!sourceOrg.DataDeriveProjectStage)
        {
            return projectStageID;
        }

        // Nothing has been completed without a completion date.
        if (!hasCompletionDate)
        {
            projectStageID = ProjectStage.Planned.ProjectStageID;
        }

        if (string.IsNullOrWhiteSpace(projectStageSourceValue))
        {
            return projectStageID;
        }

        var mappedStageName = projectStageCrossWalks
            .FirstOrDefault(x => string.Equals(
                x.GisCrossWalkSourceValue?.Trim(), projectStageSourceValue.Trim(), StringComparison.InvariantCultureIgnoreCase))
            ?.GisCrossWalkMappedValue;

        if (string.IsNullOrWhiteSpace(mappedStageName))
        {
            // Unmapped source value — keep what we have rather than throwing (legacy bug).
            return projectStageID;
        }

        var mappedStage = ProjectStage.All.SingleOrDefault(x => string.Equals(
            x.ProjectStageName, mappedStageName.Trim(), StringComparison.InvariantCultureIgnoreCase));

        // A mapped value that doesn't name a real stage means the source considered it done.
        return mappedStage?.ProjectStageID ?? ProjectStage.Completed.ProjectStageID;
    }

    /// <summary>
    /// Lead implementer organization for a newly-created project. Ports the mapping half of legacy's
    /// UpdateProjectOrganizationRecords: crosswalk the raw GIS value onto an Organization name, then
    /// onto an OrganizationID, falling back to the source org's configured default whenever the
    /// value is absent, unmapped, or names an organization that doesn't exist.
    ///
    /// Legacy's bulk delete of existing ProjectOrganizations is deliberately not ported — this only
    /// runs on the create path, matching the modern pipeline.
    /// </summary>
    private static int ResolveLeadImplementerOrganizationID(
        GisUploadSourceOrganization sourceOrg,
        List<GisCrossWalkDefault> leadImplementerCrossWalks,
        Dictionary<string, int> organizationIDByName,
        string leadImplementerSourceValue)
    {
        if (leadImplementerCrossWalks.Count == 0 || string.IsNullOrWhiteSpace(leadImplementerSourceValue))
        {
            return sourceOrg.DefaultLeadImplementerOrganizationID;
        }

        var mappedOrganizationName = leadImplementerCrossWalks
            .FirstOrDefault(x => string.Equals(
                x.GisCrossWalkSourceValue?.Trim(), leadImplementerSourceValue.Trim(), StringComparison.InvariantCultureIgnoreCase))
            ?.GisCrossWalkMappedValue;

        if (string.IsNullOrWhiteSpace(mappedOrganizationName))
        {
            return sourceOrg.DefaultLeadImplementerOrganizationID;
        }

        return organizationIDByName.TryGetValue(mappedOrganizationName.Trim(), out var organizationID)
            ? organizationID
            : sourceOrg.DefaultLeadImplementerOrganizationID;
    }

    /// <summary>
    /// Re-derives each touched project's type from its treatments when the source organization has
    /// AdjustProjectTypeBasedOnTreatmentTypes set. Ports legacy's UpdateProjectTypesIfNeeded, which
    /// the rewrite dropped — WADNR-2287.
    ///
    /// A project is only reassigned when its treatments share exactly one distinct TreatmentTypeID;
    /// a mixed or empty treatment set leaves the type alone. When the source is flattened, only
    /// treatments with treated acres count, matching legacy.
    ///
    /// Scoped by LastUpdateGisUploadAttemptID, which the create path sets alongside
    /// CreateGisUploadAttemptID, so this covers projects created *and* updated by this attempt.
    /// </summary>
    private static async Task ApplyProjectTypeFromTreatmentTypesAsync(
        WADNRDbContext dbContext, GisUploadSourceOrganization sourceOrg, int gisUploadAttemptID)
    {
        if (!sourceOrg.AdjustProjectTypeBasedOnTreatmentTypes)
        {
            return;
        }

        var projectTypeIDsByName = await LoadProjectTypeIDsByNameAsync(dbContext);
        var projectTypeIDByTreatmentTypeID = new Dictionary<int, int>();
        void MapTreatmentType(TreatmentType treatmentType, string projectTypeName)
        {
            if (projectTypeIDsByName.TryGetValue(projectTypeName, out var projectTypeID))
            {
                projectTypeIDByTreatmentTypeID[treatmentType.TreatmentTypeID] = projectTypeID;
            }
        }

        MapTreatmentType(TreatmentType.Commercial, CommercialProjectTypeName);
        MapTreatmentType(TreatmentType.NonCommercial, NonCommercialProjectTypeName);
        MapTreatmentType(TreatmentType.PrescribedFire, PrescribedFireProjectTypeName);

        if (projectTypeIDByTreatmentTypeID.Count == 0)
        {
            return;
        }

        var projects = await dbContext.Projects
            .Where(p => p.LastUpdateGisUploadAttemptID == gisUploadAttemptID)
            .ToListAsync();

        if (projects.Count == 0)
        {
            return;
        }

        var projectIDs = projects.Select(p => p.ProjectID).ToList();
        var treatmentsQuery = dbContext.Treatments
            .AsNoTracking()
            .Where(t => projectIDs.Contains(t.ProjectID));

        if (sourceOrg.ImportIsFlattened == true)
        {
            treatmentsQuery = treatmentsQuery.Where(t => t.TreatmentTreatedAcres > 0);
        }

        var treatmentTypeIDsByProjectID = (await treatmentsQuery
                .Select(t => new { t.ProjectID, t.TreatmentTypeID })
                .Distinct()
                .ToListAsync())
            .GroupBy(t => t.ProjectID)
            .ToDictionary(g => g.Key, g => g.Select(t => t.TreatmentTypeID).ToList());

        var anyChanged = false;
        foreach (var project in projects)
        {
            if (!treatmentTypeIDsByProjectID.TryGetValue(project.ProjectID, out var treatmentTypeIDs)
                || treatmentTypeIDs.Count != 1)
            {
                continue;
            }

            if (!projectTypeIDByTreatmentTypeID.TryGetValue(treatmentTypeIDs[0], out var derivedProjectTypeID)
                || project.ProjectTypeID == derivedProjectTypeID)
            {
                continue;
            }

            project.ProjectTypeID = derivedProjectTypeID;
            anyChanged = true;
        }

        if (anyChanged)
        {
            await dbContext.SaveChangesWithNoAuditingAsync();
        }
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
