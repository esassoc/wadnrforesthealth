using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using WADNR.Common.JsonConverters;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Prepared;
using NetTopologySuite.IO.Converters;

namespace WADNR.Common.GeoSpatial;

public static class GeoJsonSerializer
{
    public static JsonSerializerOptions DefaultSerializerOptions = CreateGeoJSONSerializerOptions();

    // Compact (non-indented) options for machine-consumed exports (e.g. ogr2ogr GDB conversion).
    // Indented output roughly doubles the byte size of coordinate-heavy geometries — wasted bytes
    // when the consumer is GDAL, and a meaningful contributor to memory pressure on large exports.
    public static JsonSerializerOptions CompactSerializerOptions = CreateCompactGeoJSONSerializerOptions();

    private static JsonSerializerOptions CreateCompactGeoJSONSerializerOptions()
    {
        var options = CreateGeoJSONSerializerOptions();
        options.WriteIndented = false;
        return options;
    }

    // Flush the Utf8JsonWriter to the underlying stream once this many bytes are pending. Keeps the
    // in-memory buffer bounded to roughly one feature's worth of bytes during a large export.
    private const int StreamingFlushThresholdBytes = 64 * 1024;

    /// <summary>
    /// Serializes a FeatureCollection straight to a stream using compact (non-indented) options,
    /// one feature at a time.
    /// </summary>
    /// <remarks>
    /// NTS's STJ converters serialize an entire FeatureCollection in a single synchronous Write call,
    /// so <see cref="JsonSerializer.SerializeAsync(Stream, object, JsonSerializerOptions, CancellationToken)"/>
    /// never reaches an await point to flush mid-document — the whole GeoJSON document ends up buffered
    /// in one contiguous pooled byte buffer, which is what caused OutOfMemoryException on large multi-layer
    /// GDB exports (the failure occurred inside StjGeometryConverter.Write while growing that buffer).
    /// Writing the FeatureCollection envelope by hand and serializing each feature individually lets us
    /// flush to the stream between features, bounding memory to a single feature rather than the whole
    /// collection.
    /// </remarks>
    public static async Task SerializeFeatureCollectionToStreamAsync(FeatureCollection featureCollection, Stream stream)
    {
        await using var writer = new Utf8JsonWriter(stream);
        writer.WriteStartObject();
        writer.WriteString("type", "FeatureCollection");
        writer.WriteStartArray("features");

        foreach (var feature in featureCollection)
        {
            JsonSerializer.Serialize(writer, feature, CompactSerializerOptions);
            if (writer.BytesPending >= StreamingFlushThresholdBytes)
            {
                await writer.FlushAsync();
                await stream.FlushAsync();
            }
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
        await writer.FlushAsync();
        await stream.FlushAsync();
    }

    /// <summary>
    /// Returns the explicit OGR geometry-type token (e.g. "MULTIPOLYGON") to hand ogr2ogr's
    /// <c>-nlt</c> option for a layer built from this collection.
    /// </summary>
    /// <remarks>
    /// <c>-nlt PROMOTE_TO_MULTI</c> is NOT sufficient for a GeoJSON layer that mixes single and multi
    /// variants of the same shape (e.g. Polygon + MultiPolygon in one FeatureCollection): OGR reports
    /// such a layer's geometry type as <c>wkbUnknown</c>, and PROMOTE_TO_MULTI leaves wkbUnknown
    /// unchanged. OpenFileGDB's CreateLayer accepts wkbUnknown when *creating* a new datasource but
    /// rejects it when *adding a layer* (update mode) and, on large/complex inputs, can leave a
    /// half-created table behind — both of which broke the multi-layer GDB export. Passing a concrete
    /// multi type avoids wkbUnknown entirely. Falls back to "PROMOTE_TO_MULTI" only when the collection
    /// is empty or genuinely mixes dimensions (points + polygons), which can't share one GDB layer anyway.
    /// </remarks>
    public static string GetOgrMultiGeometryTypeToken(FeatureCollection featureCollection)
    {
        var hasPoint = false;
        var hasLine = false;
        var hasPolygon = false;
        var hasOther = false;

        foreach (var feature in featureCollection)
        {
            var geometry = feature.Geometry;
            if (geometry == null)
            {
                continue;
            }

            switch (geometry.OgcGeometryType)
            {
                case OgcGeometryType.Point:
                case OgcGeometryType.MultiPoint:
                    hasPoint = true;
                    break;
                case OgcGeometryType.LineString:
                case OgcGeometryType.MultiLineString:
                    hasLine = true;
                    break;
                case OgcGeometryType.Polygon:
                case OgcGeometryType.MultiPolygon:
                    hasPolygon = true;
                    break;
                default:
                    hasOther = true;
                    break;
            }
        }

        var distinctDimensions = (hasPoint ? 1 : 0) + (hasLine ? 1 : 0) + (hasPolygon ? 1 : 0);
        if (hasOther || distinctDimensions != 1)
        {
            return "PROMOTE_TO_MULTI";
        }

        if (hasPolygon)
        {
            return "MULTIPOLYGON";
        }

        return hasLine ? "MULTILINESTRING" : "MULTIPOINT";
    }

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, DefaultSerializerOptions);
    }

    public static async Task<T?> DeserializeAsync<T>(Stream stream)
    {
        return await JsonSerializer.DeserializeAsync<T>(stream, DefaultSerializerOptions);
    }

    public static string Serialize(object value)
    {
        return JsonSerializer.Serialize(value, DefaultSerializerOptions);
    }

    public static void RemoveAllProperties(IFeature feature)
    {
        // Just replace the AttributesTable with a new one instead of deleting all properties
        // because the STJ-serialized Feature has a read-only AttributesTable. 
        feature.Attributes = new AttributesTable();
    }

    public static Envelope GetExtentForFeatureCollection(FeatureCollection featureCollection, int? optionalBuffer)
    {
        var maxX = featureCollection.Max(x => x.Geometry.EnvelopeInternal.MaxX);
        var minX = featureCollection.Min(x => x.Geometry.EnvelopeInternal.MinX);
        var maxY = featureCollection.Max(x => x.Geometry.EnvelopeInternal.MaxY);
        var minY = featureCollection.Min(x => x.Geometry.EnvelopeInternal.MinY);
        var wkt = $"POLYGON(({minX} {minY}, {minX} {maxY}, {maxX} {maxY}, {maxX} {minY}, {minX} {minY}))";

        var envelope = new Envelope(minX, maxX, minY, maxY);
        if (optionalBuffer.HasValue)
        {
            envelope.ExpandBy(optionalBuffer.Value);
        }
        return envelope;
    }

    public static string GetGeoJsonStringFromGeoJsonByteArray(byte[] fileContentsByteArray)
    {
        return Encoding.UTF8.GetString(fileContentsByteArray);
    }

    public static async Task<T?> DeserializeFromFileAsync<T>(string pathToGeoJsonFile, JsonSerializerOptions jsonSerializerOptions)
    {
        await using var openStream = File.OpenRead(pathToGeoJsonFile);
        var deserializeAsync = await JsonSerializer.DeserializeAsync<T>(openStream, jsonSerializerOptions);
        await openStream.DisposeAsync();
        return deserializeAsync;
    }

    public static T? DeserializeFromFile<T>(string pathToGeoJsonFile, JsonSerializerOptions jsonSerializerOptions)
    {
        using var openStream = File.OpenRead(pathToGeoJsonFile);
        return JsonSerializer.Deserialize<T>(openStream, jsonSerializerOptions);
    }

    public static async Task<FeatureCollection> GetFeatureCollectionFromGeoJsonString(string geojson, JsonSerializerOptions jsonSerializerOptions)
    {
        await using var ms = new MemoryStream(Encoding.UTF8.GetBytes(geojson));
        return await GetFeatureCollectionFromGeoJsonStream(ms, jsonSerializerOptions);
    }

    public static async Task<FeatureCollection> GetFeatureCollectionFromGeoJsonByteArray(byte[] fileContentsByteArray, JsonSerializerOptions jsonSerializerOptions)
    {
        await using var memoryStream = new MemoryStream(fileContentsByteArray);
        return await GetFeatureCollectionFromGeoJsonStream(memoryStream, jsonSerializerOptions);
    }

    public static async Task<FeatureCollection> GetFeatureCollectionFromGeoJsonStream(Stream stream, JsonSerializerOptions jsonSerializerOptions)
    {
        return (await JsonSerializer.DeserializeAsync<FeatureCollection>(stream, jsonSerializerOptions))!;
    }

    public static async Task<List<IFeature>> GetFeatureCollectionFromGeoJsonByteArray(byte[] fileContentsByteArray, IPreparedGeometry boundingBox, JsonSerializerOptions jsonSerializerOptions)
    {
        var featureCollection = await GetFeatureCollectionFromGeoJsonByteArray(fileContentsByteArray, jsonSerializerOptions);
        return featureCollection.Where(x => boundingBox.Intersects(x.Geometry)).ToList();
    }

    public static JsonSerializerOptions CreateGeoJSONSerializerOptions()
    {
        return CreateGeoJSONSerializerOptions(6, 10);
    }

    public static JsonSerializerOptions CreateGeoJSONSerializerOptions(int numberOfSignificantDigits)
    {
        return CreateGeoJSONSerializerOptions(6, numberOfSignificantDigits);
    }

    public static JsonSerializerOptions CreateGeoJSONSerializerOptions(int coordinatePrecision, int numberOfSignificantDigits)
    {
        var jsonSerializerOptions = CreateDefaultJSONSerializerOptions(numberOfSignificantDigits);
        //var scale = Math.Pow(10, coordinatePrecision);
        //var geometryFactory = new GeometryFactory(new PrecisionModel(scale));
        jsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory(false));
        return jsonSerializerOptions;
    }

    public static JsonSerializerOptions CreateDefaultJSONSerializerOptions(int numberOfSignificantDigits)
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            WriteIndented = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = null,
        };
        jsonSerializerOptions.Converters.Add(new DoubleConverter(numberOfSignificantDigits));
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        return jsonSerializerOptions;
    }

    public static async Task SerializeToStream<T>(T objectToSerialize, JsonSerializerOptions jsonSerializerOptions, MemoryStream stream)
    {
        await JsonSerializer.SerializeAsync(stream, objectToSerialize, jsonSerializerOptions);
    }

    public static async Task SerializeToFileAsync<T>(T objectToSerialize, string fileOutput, JsonSerializerOptions jsonSerializerOptions)
    {
        await using var createStream = File.Create(fileOutput);
        await JsonSerializer.SerializeAsync(createStream, objectToSerialize, jsonSerializerOptions);
        await createStream.DisposeAsync();
    }

    public static void SerializeToFile<T>(T objectToSerialize, string fileOutput, JsonSerializerOptions jsonSerializerOptions)
    {
        using var createStream = File.Create(fileOutput);
        JsonSerializer.Serialize(createStream, objectToSerialize, jsonSerializerOptions);
    }

    public static void SerializeAsFeatureCollectionToFile(IEnumerable<IHasGeometry> features, string fileOutput, JsonSerializerOptions jsonSerializerOptions)
    {
        SerializeToFile(features.ToFeatureCollection(), fileOutput, jsonSerializerOptions);
    }

    public static async Task SerializeAsFeatureCollectionToFileAsync(IEnumerable<IHasGeometry> features, string fileOutput, JsonSerializerOptions jsonSerializerOptions)
    {
        await SerializeToFileAsync(features.ToFeatureCollection(), fileOutput, jsonSerializerOptions);
    }

    public static FeatureCollection ToFeatureCollection(this IEnumerable<IHasGeometry> features)
    {
        var featureCollection = new FeatureCollection();
        foreach (var feature in features)
        {
            featureCollection.Add(feature.ToGeoJsonFeature());
        }

        return featureCollection;
    }

    public static async Task SerializeAsGeoJsonToStream(FeatureCollection featureCollection, JsonSerializerOptions jsonSerializerOptions, MemoryStream stream)
    {
        await SerializeToStream<FeatureCollection>(featureCollection, jsonSerializerOptions, stream);
    }

    public static byte[] WriteFeaturesToByteArray(IEnumerable<IFeature> features, JsonSerializerOptions jsonSerializerOptions)
    {
        var featureCollection = new FeatureCollection();
        foreach (var feature in features)
        {
            featureCollection.Add(feature);
        }

        return SerializeToByteArray(featureCollection, jsonSerializerOptions);
    }

    public static byte[] SerializeToByteArray<T>(T objectToSerialize, JsonSerializerOptions jsonSerializerOptions)
    {
        return JsonSerializer.SerializeToUtf8Bytes(objectToSerialize, jsonSerializerOptions);
    }

    public static T DeserializeFromFeature<T>(IFeature feature, JsonSerializerOptions geoJSONSerializerOptions) where T : IHasGeometry
    {
        ((IPartiallyDeserializedAttributesTable)feature.Attributes).TryDeserializeJsonObject<T>(geoJSONSerializerOptions, out var deserialized);
        deserialized.Geometry = feature.Geometry;
        return deserialized;
    }

    public static T DeserializeFromFeatureWithCCWCheck<T>(IFeature feature, JsonSerializerOptions geoJSONSerializerOptions, int srid) where T : IHasGeometry
    {
        ((IPartiallyDeserializedAttributesTable)feature.Attributes).TryDeserializeJsonObject<T>(geoJSONSerializerOptions, out var deserialized);
        var geometry = feature.Geometry.MakeValid();
        if (geometry.GeometryType.ToUpper() == "POLYGON")
        {
            var polygon = (Polygon)geometry;
            if (!polygon.Shell.IsCCW)
            {
                geometry = geometry.Reverse();
            }
        }
        else if (geometry.GeometryType.ToUpper() == "MULTIPOLYGON")
        {
            if (geometry.NumGeometries == 1)
            {
                var geometryPart = (Polygon)geometry.GetGeometryN(0);
                if (!geometryPart.Shell.IsCCW)
                {
                    geometry = geometryPart.Reverse();
                }
            }
            else
            {
                for (var i = 0; i < geometry.NumGeometries; i++)
                {
                    var geometryPart = (Polygon)geometry.GetGeometryN(i);
                    if (!geometryPart.Shell.IsCCW)
                    {
                        // if any is not counter-clockwise, just reverse the whole geometry and stop processing the rest
                        geometry = geometry.Reverse();
                        break;
                    }
                }
            }
        }
        deserialized.Geometry = geometry;
        deserialized.Geometry.SRID = srid;
        return deserialized;
    }

    public static T? DeserializeFromFeatureWithNoGeometry<T>(IFeature feature, JsonSerializerOptions geoJSONSerializerOptions)
    {
        ((IPartiallyDeserializedAttributesTable)feature.Attributes).TryDeserializeJsonObject<T>(geoJSONSerializerOptions, out var deserialized);
        return deserialized;
    }

    public static async Task<List<T>> DeserializeFromFeatureCollection<T>(byte[] byteArray, JsonSerializerOptions geoJSONSerializerOptions) where T : IHasGeometry
    {
        var featureCollection = await GetFeatureCollectionFromGeoJsonByteArray(byteArray, geoJSONSerializerOptions);
        return DeserializeFromFeatureCollection<T>(featureCollection, geoJSONSerializerOptions);
    }

    public static async Task<List<T>> DeserializeFromFeatureCollectionWithCCWCheck<T>(byte[] byteArray, JsonSerializerOptions geoJSONSerializerOptions, int srid) where T : IHasGeometry
    {
        var featureCollection = await GetFeatureCollectionFromGeoJsonByteArray(byteArray, geoJSONSerializerOptions);
        return DeserializeFromFeatureCollectionWithCCWCheck<T>(featureCollection, geoJSONSerializerOptions, srid);
    }

    public static List<T> DeserializeFromFeatureCollection<T>(FeatureCollection featureCollection, JsonSerializerOptions geoJSONSerializerOptions) where T : IHasGeometry
    {
        return featureCollection.AsParallel().Select(x => DeserializeFromFeature<T>(x, geoJSONSerializerOptions)).ToList();
    }

    public static List<T> DeserializeFromFeatureCollectionWithCCWCheck<T>(FeatureCollection featureCollection, JsonSerializerOptions geoJSONSerializerOptions, int srid) where T : IHasGeometry
    {
        return featureCollection.AsParallel().Select(x => DeserializeFromFeatureWithCCWCheck<T>(x, geoJSONSerializerOptions, srid)).ToList();
    }

    public static List<T> DeserializeFromFeatureCollection<T>(FeatureCollection featureCollection) where T : IHasGeometry
    {
        return featureCollection.AsParallel().Select(x => DeserializeFromFeature<T>(x, DefaultSerializerOptions)).ToList();
    }

    public static async Task<List<T?>> DeserializeFromFeatureCollectionWithNoGeometry<T>(byte[] byteArray, JsonSerializerOptions geoJSONSerializerOptions)
    {
        var featureCollection = await GetFeatureCollectionFromGeoJsonByteArray(byteArray, geoJSONSerializerOptions);
        return DeserializeFromFeatureCollectionWithNoGeometry<T>(featureCollection, geoJSONSerializerOptions);
    }

    public static List<T?> DeserializeFromFeatureCollectionWithNoGeometry<T>(FeatureCollection featureCollection, JsonSerializerOptions geoJSONSerializerOptions)
    {
        return featureCollection.AsParallel().Select(x => DeserializeFromFeatureWithNoGeometry<T>(x, geoJSONSerializerOptions)).ToList();
    }

    public static Feature ToGeoJsonFeature<T>(this T featureClass) where T : IHasGeometry
    {
        var dictionary = ToKeyValuePairList(featureClass);
        var attributesTable = new AttributesTable(dictionary);
        return new Feature(featureClass.Geometry, attributesTable);
    }

    public static Dictionary<string, object?> ToKeyValuePairList<T>(T obj) where T : notnull
    {
        return obj.GetType().GetProperties().Where(x => !x.IsDefined(typeof(JsonIgnoreAttribute), false)).ToDictionary(p => p.Name, p => p.GetValue(obj));
    }
}