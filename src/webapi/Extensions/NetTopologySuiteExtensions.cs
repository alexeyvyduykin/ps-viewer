using FootprintViewerWebApi.Extensions;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace webapi.Extensions;

public static class NetTopologySuiteExtensions
{
    public static Polygon ToLinearPolygon(this LineString lineString)
    {
        var points = lineString.MainCoordinates().SkipLast(1);
        var reversePoints = lineString.MainCoordinates();

        reversePoints.Reverse();
        reversePoints = reversePoints.SkipLast(1).ToList();

        var linearRing = points.Concat(reversePoints).ToLinearRing();
        return new Polygon(linearRing);
    }

    public static Point ToPoint(this Coordinate coordinate)
    {
        return new Point(coordinate.X, coordinate.Y);
    }

    public static LineString ToLineString(this IEnumerable<Coordinate> coordinates)
    {
        List<Coordinate> list = coordinates.ToList();
        if (list.Count == 1)
        {
            list.Add(list[0].Copy());
        }

        return new LineString([.. list]);
    }

    public static LinearRing ToLinearRing(this IEnumerable<Coordinate> coordinates)
    {
        List<Coordinate> list = coordinates.ToList();
        if (list.Count == 1)
        {
            list.Add(list[0].Copy());
        }

        if (list.Count == 2)
        {
            list.Add(list[0].Copy());
        }

        if (list.Count > 2 && !list.First().Equals2D(list.Last()))
        {
            list.Add(list[0].Copy());
        }

        return new LinearRing([.. list]);
    }

    public static Polygon ToPolygon(this IEnumerable<Coordinate> coordinates, IEnumerable<IEnumerable<Coordinate>>? holes = null)
    {
        if (holes == null || !holes.Any())
        {
            return new Polygon(coordinates.ToLinearRing());
        }

        return new Polygon(coordinates.ToLinearRing(), holes.Select((IEnumerable<Coordinate> h) => h.ToLinearRing()).ToArray());
    }

    public static Feature ToFeatureEx(this Geometry geometry, string? name = null)
    {
        if (string.IsNullOrEmpty(name) == false)
        {
            AttributesTable attributes = new()
            {
                { "Name", name }
            };

            return geometry.ToFeature(attributes);
        }

        return geometry.ToFeature();
    }

    public static FeatureCollection ToFeatureCollection(this IEnumerable<IFeature> features)
    {
        var fc = new FeatureCollection();

        foreach (var item in features)
        {
            fc.Add(item);
        }

        return fc;
    }

    public static void AddRange(this FeatureCollection fc, IFeature[] features)
    {
        foreach (var item in features)
        {
            fc.Add(item);
        }
    }
}