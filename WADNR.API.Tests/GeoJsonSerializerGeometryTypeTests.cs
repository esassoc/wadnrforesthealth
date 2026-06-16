using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using WADNR.Common.GeoSpatial;

namespace WADNR.API.Tests;

/// <summary>
/// Covers <see cref="GeoJsonSerializer.GetOgrMultiGeometryTypeToken"/>, which picks the explicit
/// ogr2ogr -nlt token for a layer. The mixed Polygon/MultiPolygon case is the regression that broke
/// the multi-layer GDB export: PROMOTE_TO_MULTI was insufficient and OpenFileGDB rejected wkbUnknown.
/// </summary>
[TestClass]
public class GeoJsonSerializerGeometryTypeTests
{
    private static readonly GeometryFactory Factory = new();

    private static Polygon Square(double x, double y) => Factory.CreatePolygon(new[]
    {
        new Coordinate(x, y),
        new Coordinate(x, y + 1),
        new Coordinate(x + 1, y + 1),
        new Coordinate(x + 1, y),
        new Coordinate(x, y),
    });

    private static MultiPolygon MultiSquare(double x, double y) => Factory.CreateMultiPolygon(new[] { Square(x, y) });

    private static FeatureCollection CollectionOf(params Geometry?[] geometries)
    {
        var collection = new FeatureCollection();
        foreach (var geometry in geometries)
        {
            collection.Add(new Feature(geometry, new AttributesTable()));
        }
        return collection;
    }

    [TestMethod]
    public void GetOgrMultiGeometryTypeToken_ReturnsMultiPolygon_ForUniformPolygons()
    {
        var collection = CollectionOf(Square(0, 0), Square(2, 2));

        Assert.AreEqual("MULTIPOLYGON", GeoJsonSerializer.GetOgrMultiGeometryTypeToken(collection));
    }

    [TestMethod]
    public void GetOgrMultiGeometryTypeToken_ReturnsMultiPolygon_ForMixedPolygonAndMultiPolygon()
    {
        // The regression case: a layer mixing Polygon and MultiPolygon reports as wkbUnknown to OGR,
        // which PROMOTE_TO_MULTI does not resolve. A concrete MULTIPOLYGON must be returned instead.
        var collection = CollectionOf(Square(0, 0), MultiSquare(2, 2));

        Assert.AreEqual("MULTIPOLYGON", GeoJsonSerializer.GetOgrMultiGeometryTypeToken(collection));
    }

    [TestMethod]
    public void GetOgrMultiGeometryTypeToken_ReturnsMultiPoint_ForPoints()
    {
        var collection = CollectionOf(
            Factory.CreatePoint(new Coordinate(-122.1, 47.1)),
            Factory.CreatePoint(new Coordinate(-122.2, 47.2)));

        Assert.AreEqual("MULTIPOINT", GeoJsonSerializer.GetOgrMultiGeometryTypeToken(collection));
    }

    [TestMethod]
    public void GetOgrMultiGeometryTypeToken_ReturnsMultiPoint_ForMixedPointAndMultiPoint()
    {
        var collection = CollectionOf(
            Factory.CreatePoint(new Coordinate(0, 0)),
            Factory.CreateMultiPointFromCoords(new[] { new Coordinate(1, 1), new Coordinate(2, 2) }));

        Assert.AreEqual("MULTIPOINT", GeoJsonSerializer.GetOgrMultiGeometryTypeToken(collection));
    }

    [TestMethod]
    public void GetOgrMultiGeometryTypeToken_ReturnsMultiLineString_ForLineStrings()
    {
        var collection = CollectionOf(
            Factory.CreateLineString(new[] { new Coordinate(0, 0), new Coordinate(1, 1) }));

        Assert.AreEqual("MULTILINESTRING", GeoJsonSerializer.GetOgrMultiGeometryTypeToken(collection));
    }

    [TestMethod]
    public void GetOgrMultiGeometryTypeToken_IgnoresNullGeometries()
    {
        var collection = CollectionOf(Square(0, 0), null, MultiSquare(2, 2));

        Assert.AreEqual("MULTIPOLYGON", GeoJsonSerializer.GetOgrMultiGeometryTypeToken(collection));
    }

    [TestMethod]
    public void GetOgrMultiGeometryTypeToken_FallsBackToPromote_ForMixedDimensions()
    {
        // Points + polygons cannot share a single GDB layer; let ogr2ogr decide rather than forcing a type.
        var collection = CollectionOf(Factory.CreatePoint(new Coordinate(0, 0)), Square(2, 2));

        Assert.AreEqual("PROMOTE_TO_MULTI", GeoJsonSerializer.GetOgrMultiGeometryTypeToken(collection));
    }

    [TestMethod]
    public void GetOgrMultiGeometryTypeToken_FallsBackToPromote_ForEmptyCollection()
    {
        Assert.AreEqual("PROMOTE_TO_MULTI", GeoJsonSerializer.GetOgrMultiGeometryTypeToken(new FeatureCollection()));
    }

    [TestMethod]
    public void GetOgrMultiGeometryTypeToken_FallsBackToPromote_ForUnsupportedGeometryType()
    {
        var collection = CollectionOf(
            Factory.CreateGeometryCollection(new Geometry[] { Factory.CreatePoint(new Coordinate(0, 0)) }));

        Assert.AreEqual("PROMOTE_TO_MULTI", GeoJsonSerializer.GetOgrMultiGeometryTypeToken(collection));
    }
}
