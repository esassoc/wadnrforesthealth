using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects.GisBulkImport;

namespace WADNR.API.Services;

public class GisDataImportService(
    IHttpClientFactory httpClientFactory,
    WADNRDbContext dbContext,
    ILogger<GisDataImportService> logger)
{
    /// <summary>
    /// Downloads features from an ArcGIS REST API using GET pagination (LOA pattern).
    /// Creates a GisUploadAttempt and imports projects using the GisBulkImports pipeline.
    /// </summary>
    public async Task DownloadAndImportFeaturesWithGetAsync(string arcOnlineUrl, int orgID, string accessToken)
    {
        var uploadSourceOrganization = await dbContext.GisUploadSourceOrganizations
            .Include(x => x.GisDefaultMappings)
            .SingleOrDefaultAsync(x => x.GisUploadSourceOrganizationID == orgID);

        if (uploadSourceOrganization == null)
        {
            throw new ApplicationException($"GisUploadSourceOrganization(ID:{orgID}) does not exist");
        }

        var mappedFieldNames = uploadSourceOrganization.GisDefaultMappings
            .Select(x => x.GisDefaultMappingColumnName).ToList();
        var outFields = string.Join(",", mappedFieldNames);
        var queryString = $"?f=json&outSr=4326&where=Approval_ID%20is%20not%20null&outFields={outFields}";
        var fullUrl = arcOnlineUrl + queryString;

        using var httpClient = httpClientFactory.CreateClient("GisApi");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Esri-Authorization", $"Bearer {accessToken}");

        // Get total record count
        var countUrl = fullUrl + "&returnCountOnly=true";
        logger.LogWarning("LOA count request URL: {Url}", countUrl);
        var countResponse = await httpClient.GetAsync(countUrl);
        countResponse.EnsureSuccessStatusCode();
        var countJson = await countResponse.Content.ReadAsStringAsync();
        using var countDoc = JsonDocument.Parse(countJson);
        var totalRecordCount = GetRequiredProperty(countDoc.RootElement, "count", "LOA count query").GetInt32();

        logger.LogInformation("DownloadAndImportFeaturesWithGet: Attempting to download {Count} from {Url}",
            totalRecordCount, arcOnlineUrl);

        // Paginated download
        var allFeatures = new List<JsonElement>();
        var resultOffset = 0;
        int batchCount;
        do
        {
            var batchUrl = fullUrl + $"&resultOffset={resultOffset}";
            logger.LogWarning("LOA feature request URL: {Url}", batchUrl);
            var response = await httpClient.GetAsync(batchUrl);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var features = GetRequiredProperty(doc.RootElement, "features", "LOA feature fetch").EnumerateArray().ToList();
            batchCount = features.Count;

            foreach (var feature in features)
            {
                allFeatures.Add(feature.Clone());
            }
            resultOffset += batchCount;
        } while (batchCount > 0 && allFeatures.Count < totalRecordCount);

        await ProcessAndImportFeaturesAsync(allFeatures, uploadSourceOrganization, orgID);
    }

    /// <summary>
    /// Downloads features from an ArcGIS REST API using POST with objectID batching and optional spatial filter (USFS pattern).
    /// </summary>
    public async Task DownloadAndImportFeaturesWithPostAsync(string arcOnlineUrl, int orgID,
        string? whereClause, int batchSize, bool useSpatialFilter)
    {
        var uploadSourceOrganization = await dbContext.GisUploadSourceOrganizations
            .Include(x => x.GisDefaultMappings)
            .Include(x => x.GisCrossWalkDefaults)
            .SingleOrDefaultAsync(x => x.GisUploadSourceOrganizationID == orgID);

        if (uploadSourceOrganization == null)
        {
            throw new ApplicationException($"GisUploadSourceOrganization(ID:{orgID}) does not exist");
        }

        var mappedFieldNames = uploadSourceOrganization.GisDefaultMappings
            .Select(x => x.GisDefaultMappingColumnName).ToList();
        var outFields = string.Join(",", mappedFieldNames);

        using var httpClient = httpClientFactory.CreateClient("GisApi");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "WADNR Forest Health Tracker");

        // Build WHERE clause with activity filter if provided
        var effectiveWhere = whereClause ?? "1=1";

        // Build form data for ID query
        var idQueryFormData = new List<KeyValuePair<string, string>>
        {
            new("where", effectiveWhere),
            new("f", "json"),
            new("outSR", "4326"),
            new("outFields", outFields),
            new("returnIdsOnly", "true")
        };

        if (useSpatialFilter)
        {
            idQueryFormData.Add(new("geometry", WaStateBoundaryPolygon.JsonPolygon));
            idQueryFormData.Add(new("geometryType", "esriGeometryPolygon"));
            idQueryFormData.Add(new("inSR", "4326"));
            idQueryFormData.Add(new("spatialRel", "esriSpatialRelIntersects"));
        }

        // Get object IDs
        var encodedItems = idQueryFormData.Select(i =>
            WebUtility.UrlEncode(i.Key) + "=" + WebUtility.UrlEncode(i.Value));
        var encodedContent = new StringContent(string.Join("&", encodedItems), null, "application/x-www-form-urlencoded");
        var idResponse = await httpClient.PostAsync(arcOnlineUrl, encodedContent);
        idResponse.EnsureSuccessStatusCode();
        var idJson = await idResponse.Content.ReadAsStringAsync();
        using var idDoc = JsonDocument.Parse(idJson);
        var objectIds = GetRequiredProperty(idDoc.RootElement, "objectIds", "POST ID query").EnumerateArray()
            .Select(x => x.GetInt32()).ToList();

        var totalRecordCount = objectIds.Count;
        logger.LogInformation("DownloadAndImportFeaturesWithPost: Attempting to download {Count} from {Url}",
            totalRecordCount, arcOnlineUrl);

        // Batch download features by objectID
        var baseFormData = new[]
        {
            new KeyValuePair<string, string>("f", "json"),
            new KeyValuePair<string, string>("outSR", "4326"),
            new KeyValuePair<string, string>("outFields", outFields),
            new KeyValuePair<string, string>("returnGeometry", "true")
        };

        var allFeatures = new List<JsonElement>();
        for (var i = 0; i < objectIds.Count; i += batchSize)
        {
            var batchIds = objectIds.Skip(i).Take(batchSize).ToList();
            var currentFormData = baseFormData.Append(
                new KeyValuePair<string, string>("where", $"OBJECTID in ({string.Join(",", batchIds)})"));
            var currentEncodedItems = currentFormData.Select(item =>
                WebUtility.UrlEncode(item.Key) + "=" + WebUtility.UrlEncode(item.Value));
            var currentEncodedContent = new StringContent(
                string.Join("&", currentEncodedItems), null, "application/x-www-form-urlencoded");

            var response = await httpClient.PostAsync(arcOnlineUrl, currentEncodedContent);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            foreach (var feature in GetRequiredProperty(doc.RootElement, "features", "POST feature fetch").EnumerateArray())
            {
                allFeatures.Add(feature.Clone());
            }
        }

        logger.LogInformation("Expected '{Expected}' features. Got '{Actual}' features.",
            totalRecordCount, allFeatures.Count);

        await ProcessAndImportFeaturesAsync(allFeatures, uploadSourceOrganization, orgID);
    }

    /// <summary>
    /// Converts Esri JSON features to GeoJSON, creates a GisUploadAttempt, and imports projects.
    /// </summary>
    private async Task ProcessAndImportFeaturesAsync(List<JsonElement> esriFeatures,
        GisUploadSourceOrganization sourceOrg, int orgID)
    {
        // Get system user (PersonID = 1 by convention)
        var systemPersonID = await dbContext.People
            .Where(x => x.IsActive)
            .OrderBy(x => x.PersonID)
            .Select(x => x.PersonID)
            .FirstAsync();

        // Create GisUploadAttempt
        var gisAttempt = new GisUploadAttempt
        {
            GisUploadSourceOrganizationID = orgID,
            GisUploadAttemptCreatePersonID = systemPersonID,
            GisUploadAttemptCreateDate = DateTime.UtcNow
        };
        dbContext.GisUploadAttempts.Add(gisAttempt);
        await dbContext.SaveChangesWithNoAuditingAsync();

        // Convert Esri JSON features to NTS GeoJSON FeatureCollection
        var featureCollection = ConvertEsriFeaturesToGeoJson(esriFeatures);

        if (featureCollection.Count == 0)
        {
            logger.LogWarning("No valid features to import for org {OrgID}", orgID);
            return;
        }

        // Serialize to GeoJSON string for the existing pipeline
        var jsonOptions = new JsonSerializerOptions();
        jsonOptions.Converters.Add(new GeoJsonConverterFactory());
        var geoJson = JsonSerializer.Serialize(featureCollection, jsonOptions);

        // Use existing GisBulkImports pipeline
        await GisBulkImports.UploadAndProcessFileAsync(dbContext, gisAttempt.GisUploadAttemptID, geoJson);

        // Get default mappings and create import request
        var defaults = await GisBulkImports.GetDefaultMappingsAsync(dbContext, gisAttempt.GisUploadAttemptID);
        var importRequest = new GisBulkImportRequest
        {
            ProjectIdentifierMetadataAttributeID = defaults.ProjectIdentifierMetadataAttributeID ?? 0,
            ProjectNameMetadataAttributeID = defaults.ProjectNameMetadataAttributeID ?? 0,
            TreatmentTypeMetadataAttributeID = defaults.TreatmentTypeMetadataAttributeID,
            CompletionDateMetadataAttributeID = defaults.CompletionDateMetadataAttributeID,
            StartDateMetadataAttributeID = defaults.StartDateMetadataAttributeID,
            ProjectStageMetadataAttributeID = defaults.ProjectStageMetadataAttributeID,
            LeadImplementerMetadataAttributeID = defaults.LeadImplementerMetadataAttributeID,
            FootprintAcresMetadataAttributeID = defaults.FootprintAcresMetadataAttributeID,
            PrivateLandownerMetadataAttributeID = defaults.PrivateLandownerMetadataAttributeID,
            TreatmentDetailedActivityTypeMetadataAttributeID = defaults.TreatmentDetailedActivityTypeMetadataAttributeID,
            TreatedAcresMetadataAttributeID = defaults.TreatedAcresMetadataAttributeID,
            PruningAcresMetadataAttributeID = defaults.PruningAcresMetadataAttributeID,
            ThinningAcresMetadataAttributeID = defaults.ThinningAcresMetadataAttributeID,
            ChippingAcresMetadataAttributeID = defaults.ChippingAcresMetadataAttributeID,
            MasticationAcresMetadataAttributeID = defaults.MasticationAcresMetadataAttributeID,
            GrazingAcresMetadataAttributeID = defaults.GrazingAcresMetadataAttributeID,
            LopScatAcresMetadataAttributeID = defaults.LopScatAcresMetadataAttributeID,
            BiomassRemovalAcresMetadataAttributeID = defaults.BiomassRemovalAcresMetadataAttributeID,
            HandPileAcresMetadataAttributeID = defaults.HandPileAcresMetadataAttributeID,
            HandPileBurnAcresMetadataAttributeID = defaults.HandPileBurnAcresMetadataAttributeID,
            MachinePileBurnAcresMetadataAttributeID = defaults.MachinePileBurnAcresMetadataAttributeID,
            BroadcastBurnAcresMetadataAttributeID = defaults.BroadcastBurnAcresMetadataAttributeID,
            OtherAcresMetadataAttributeID = defaults.OtherAcresMetadataAttributeID
        };

        var result = await GisBulkImports.ImportProjectsAsync(dbContext, gisAttempt.GisUploadAttemptID, importRequest);

        var message = new StringBuilder();
        message.AppendLine($"Successfully imported {result.ProjectsCreated} new projects.");
        message.AppendLine($"Successfully updated {result.ProjectsUpdated} existing projects.");
        message.AppendLine($"Created {result.LocationsCreated} project locations.");
        if (result.Warnings.Count > 0)
        {
            message.AppendLine($"Warnings: {string.Join("; ", result.Warnings)}");
        }
        logger.LogInformation("{Message}", message.ToString());
    }

    private static JsonElement GetRequiredProperty(JsonElement root, string propertyName, string context)
    {
        if (root.TryGetProperty("error", out var errorElement))
        {
            var code = errorElement.TryGetProperty("code", out var c) ? c.ToString() : "unknown";
            var message = errorElement.TryGetProperty("message", out var m) ? m.GetString() : "unknown";
            throw new ApplicationException(
                $"ArcGIS API error during {context}: code={code}, message={message}");
        }

        if (!root.TryGetProperty(propertyName, out var prop))
        {
            var responsePreview = root.ToString();
            throw new ApplicationException(
                $"ArcGIS API response during {context} missing expected property '{propertyName}'. " +
                $"Response: {responsePreview[..Math.Min(500, responsePreview.Length)]}");
        }

        return prop;
    }

    /// <summary>
    /// Converts Esri JSON features (with rings geometry) to an NTS FeatureCollection.
    /// </summary>
    private static FeatureCollection ConvertEsriFeaturesToGeoJson(List<JsonElement> esriFeatures)
    {
        var featureCollection = new FeatureCollection();
        var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);

        foreach (var esriFeature in esriFeatures)
        {
            if (!esriFeature.TryGetProperty("geometry", out var geometryElement))
                continue;
            if (!geometryElement.TryGetProperty("rings", out var ringsElement))
                continue;

            // Build attributes table from dictionary (AttributesTable indexer throws on new keys;
            // dictionary handles both new keys and duplicates via last-write-wins, matching legacy behavior)
            var attributesDict = new Dictionary<string, object>();
            if (esriFeature.TryGetProperty("attributes", out var attrsElement))
            {
                foreach (var prop in attrsElement.EnumerateObject())
                {
                    attributesDict[prop.Name] =
                        prop.Value.ValueKind == JsonValueKind.Null ? null : prop.Value.ToString();
                }
            }
            var attributesTable = new AttributesTable(attributesDict);

            // Convert Esri rings to NTS geometry
            var rings = new List<LinearRing>();
            foreach (var ring in ringsElement.EnumerateArray())
            {
                var coordinates = ring.EnumerateArray()
                    .Select(coord =>
                    {
                        var arr = coord.EnumerateArray().ToList();
                        return new Coordinate(arr[0].GetDouble(), arr[1].GetDouble());
                    })
                    .ToArray();

                if (coordinates.Length < 4) continue;

                // Ensure ring is closed
                if (!coordinates[0].Equals2D(coordinates[^1]))
                {
                    coordinates = coordinates.Append(coordinates[0]).ToArray();
                }

                rings.Add(geometryFactory.CreateLinearRing(coordinates));
            }

            if (rings.Count == 0) continue;

            Geometry geometry;
            if (rings.Count == 1)
            {
                geometry = geometryFactory.CreatePolygon(rings[0]);
            }
            else
            {
                // First ring is exterior, rest are holes
                geometry = geometryFactory.CreatePolygon(rings[0], rings.Skip(1).ToArray());
            }

            geometry.SRID = 4326;

            var feature = new Feature(geometry, attributesTable);
            featureCollection.Add(feature);
        }

        return featureCollection;
    }
}
