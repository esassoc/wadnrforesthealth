using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace WADNRForestHealthTracker.Common.GeoSpatial;

public interface IHasGeometry
{
    [JsonIgnore]
    Geometry Geometry { get; set; }
}