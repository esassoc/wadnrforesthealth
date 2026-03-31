using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace WADNR.Common.GeoSpatial;

public static class GeometryHelper
{
    public const string POLYGON_EMPTY = "POLYGON EMPTY";

    public static Geometry MakeValid(this Geometry geometry)
    {
        return !geometry.IsValid ? NetTopologySuite.Geometries.Utilities.GeometryFixer.Fix(geometry) : geometry;
    }

    public static Geometry CreateLocationPoint4326FromLatLong(double latitude, double longitude)
    {
        return new Point(longitude, latitude) { SRID = 4326 };
    }

    public static Geometry CreateLocationPointFromLatLong(double latitude, double longitude, int coordinateSystemId)
    {
        return new Point(longitude, latitude) { SRID = coordinateSystemId };
    }

    /// <summary>
    /// Compute the geodesic destination point given a start lat/long (degrees), distance in meters and bearing in radians.
    /// Returns a point in EPSG:4326.
    /// Formula: https://www.movable-type.co.uk/scripts/latlong.html#destPoint
    /// </summary>
    public static Geometry CreateLocationPointFromLatLongWithOffset(double latitudeDegrees, double longitudeDegrees, double distanceMeters, double bearingRadians)
    {
        const double EarthRadius = 6371000.0; // meters (mean Earth radius)

        if (distanceMeters == 0)
        {
            return CreateLocationPoint4326FromLatLong(latitudeDegrees, longitudeDegrees);
        }

        var lat1 = ToRadians(latitudeDegrees);
        var lon1 = ToRadians(longitudeDegrees);
        var angularDistance = distanceMeters / EarthRadius;

        var lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(angularDistance) + Math.Cos(lat1) * Math.Sin(angularDistance) * Math.Cos(bearingRadians));
        var lon2 = lon1 + Math.Atan2(Math.Sin(bearingRadians) * Math.Sin(angularDistance) * Math.Cos(lat1), Math.Cos(angularDistance) - Math.Sin(lat1) * Math.Sin(lat2));

        var lat2Deg = ToDegrees(lat2);
        var lon2Deg = ToDegrees(lon2);

        return CreateLocationPoint4326FromLatLong(lat2Deg, lon2Deg);
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
    private static double ToDegrees(double radians) => radians * 180.0 / Math.PI;

    public static Geometry? UnionListGeometries(this IList<Geometry> inputGeometries)
    {
        if (inputGeometries.Count == 0)
        {
            return null;
        }

        Geometry union;
        // all geometries have to have the same SRS or the union isn't defined anyway, so just grab the first one
        var coordinateSystemId = inputGeometries.First().SRID;

        try
        {
            var reader = new NetTopologySuite.IO.WKBReader();

            var internalGeometries = inputGeometries.Select(x => x.MakeValid()).Select(x => reader.Read(x.AsBinary()))
                .ToList();

            union = NetTopologySuite.Operation.Union.CascadedPolygonUnion.Union(internalGeometries);
            union.SRID = coordinateSystemId;
            return union;
        }
        catch (TopologyException)
        {
            // fall back on the iterative union 

            union = inputGeometries.First();

            for (var i = 1; i < inputGeometries.Count; i++)
            {
                var temp = union.Union(inputGeometries[i]);
                union = temp;
            }
            union.SRID = coordinateSystemId;
            return union;
        }
    }

    public static Geometry? FromWKT(string? wkt, int srid)
    {
        if (string.IsNullOrWhiteSpace(wkt))
            return null;

        var geoReader = new WKTReader();
        var geometry = geoReader.Read(wkt);
        geometry.SRID = srid;
        return geometry;
    }

    public static FeatureCollection MultiPolygonToFeatureCollection(this Geometry potentialMultiPolygon)
    {
        if (potentialMultiPolygon.GeometryType.ToUpper() == "MULTIPOLYGON")
        {
            var featureCollection = new FeatureCollection();

            // Leaflet.Draw does not support multipolgyon editing because its dev team decided it wasn't necessary.
            // Unless https://github.com/Leaflet/Leaflet.draw/issues/268 is resolved, we have to break into separate polys.
            // On an unrelated note, DbGeometry.ElementAt is 1-indexed instead of 0-indexed, which is terrible.
            for (var i = 0; i < potentialMultiPolygon.NumGeometries; i++)
            {
                var geometry = potentialMultiPolygon.GetGeometryN(i);
                // Reduce is SQL Server's implementation of the Douglas–Peucker downsampling algorithm
                featureCollection.Add(new Feature(geometry.MakeValid(), new AttributesTable()));
            }

            return featureCollection;
        }

        return new FeatureCollection() { new Feature(potentialMultiPolygon, new AttributesTable())};
    }

    public static List<Geometry> MakeValidAndExplodeIfNeeded(Geometry geometry)
    {
        var geometries = new List<Geometry>();
        if (!geometry.IsValid)
        {
            var validGeometry = geometry.MakeValid();
            for (var i = 0; i < validGeometry.NumGeometries; i++)
            {
                var geometryPart = validGeometry.GetGeometryN(i);
                if (geometryPart.GeometryType.ToUpper() == "POLYGON")
                {
                    geometries.Add(geometryPart);
                }
            }
        }
        else
        {
            geometries.Add(geometry);
        }

        return geometries;
    }

    public static bool CanParseGeometry(Geometry? geometry)
    {
        return geometry != null && geometry.IsValid && geometry.GeometryType != "GeometryCollection";
    }

    public const int CoordinateSystemId = 4326;
    public const double SquareMetersToAcres = 0.000247105;
    private const double MetersPerFoot = 0.3048;

    public static double FeetToLatDegree(Geometry geometry, double feet)
    {
        var longitude = GetRepresentativeXCoordinate(geometry);
        var latitude = GetRepresentativeYCoordinate(geometry);
        var coordinateSystemId = geometry.SRID == 0 ? CoordinateSystemId : geometry.SRID;

        var geography = GeometryHelper.CreateLocationPointFromLatLong(longitude, latitude - 0.5, coordinateSystemId);

        var dbGeographyOneDegreeLatitude = GeometryHelper.CreateLocationPointFromLatLong(longitude, latitude + 0.5, coordinateSystemId);
        var degreesLatitudePerMeter = geography.Distance(dbGeographyOneDegreeLatitude);

        return (feet * MetersPerFoot) / degreesLatitudePerMeter;
    }

    public static double FeetToLonDegree(Geometry geometry, double feet)
    {
        var longitude = GetRepresentativeXCoordinate(geometry);
        var latitude = GetRepresentativeYCoordinate(geometry);
        var coordinateSystemId = geometry.SRID == 0 ? CoordinateSystemId : geometry.SRID;

        var geography = GeometryHelper.CreateLocationPointFromLatLong(longitude - 0.5, latitude, coordinateSystemId);

        var dbGeographyOneDegreeLongitude = GeometryHelper.CreateLocationPointFromLatLong(longitude + 0.5, latitude, coordinateSystemId);
        var degreesLongitudePerMeter =
            geography.Distance(dbGeographyOneDegreeLongitude);

        return (feet * MetersPerFoot) / degreesLongitudePerMeter;
    }

    private static double GetRepresentativeYCoordinate(Geometry geometry)
    {
        return geometry.Centroid != null ? geometry.Centroid.Coordinate.Y
            : geometry.OgcGeometryType == OgcGeometryType.Point ? geometry.Coordinate.Y : geometry.Envelope.Centroid.Coordinate.Y;
    }

    private static double GetRepresentativeXCoordinate(Geometry geometry)
    {
        return geometry.Centroid != null ? geometry.Centroid.Coordinate.X
            : geometry.OgcGeometryType == OgcGeometryType.Point ? geometry.Coordinate.X : geometry.Envelope.Centroid.Coordinate.X;
    }
}