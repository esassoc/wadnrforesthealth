using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.IO.Converters;
using WADNR.Common.JsonConverters;

namespace WADNR.API.Tests.Helpers;

/// <summary>
/// Mirrors the JsonSerializerOptions configured in Startup.cs.
/// </summary>
public static class TestJsonSerializerOptions
{
    public static JsonSerializerOptions Create()
    {
        var options = new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            WriteIndented = false,
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = null,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };
        options.Converters.Add(new GeoJsonConverterFactory(false));
        options.Converters.Add(new DoubleConverter(7));
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
