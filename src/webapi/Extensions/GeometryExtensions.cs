using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using webapi.Extensions;

namespace FootprintViewerWebApi.Extensions;

public static class GeometryExtensions
{
    public static IEnumerable<Feature> ToFeatures(this IEnumerable<Geometry> geometries)
    {
        return geometries.Select(g => g.ToFeature()).ToList();
    }

    public static Feature ToFeature(this Geometry geometry)
    {
        return new Feature() { Geometry = geometry };
    }

    public static Feature ToFeature(this Geometry geometry, IAttributesTable attributes)
    {
        return new Feature(geometry, attributes);
    }

    public static Geometry? InsertCoordinate(this Geometry? geometry, Coordinate coordinate, int segment)
    {
        if (geometry is null)
        {
            return null;
        }

        List<Coordinate> list = geometry.MainCoordinates();
        list.Insert(segment + 1, coordinate);
        if (geometry is Polygon)
        {
            return list.ToPolygon();
        }

        if (geometry is LineString)
        {
            return list.ToLineString();
        }

        throw new NotSupportedException();
    }

    public static Geometry? DeleteCoordinate(this Geometry? geometry, int index)
    {
        if (geometry is null)
        {
            return null;
        }

        List<Coordinate> list = geometry.MainCoordinates();
        list.RemoveAt(index);
        if (geometry is Polygon)
        {
            return list.ToPolygon();
        }

        if (geometry is LineString)
        {
            return list.ToLineString();
        }

        throw new NotSupportedException();
    }

    public static IList<IList<Coordinate>> GetVertexLists(this Geometry geometry)
    {
        var point = geometry as Point;
        if (point is { })
        {
            return
            [
                new List<Coordinate> { point.Coordinate }
            ];
        }

        var lineString = geometry as LineString;
        if (lineString is { })
        {
            return
            [
                new List<Coordinate>(lineString.Coordinates)
            ];
        }

        var polygon = geometry as Polygon;
        if (polygon is { })
        {
#pragma warning disable IDE0028 // Упростите инициализацию коллекции
            var list = new List<IList<Coordinate>>();
#pragma warning restore IDE0028 // Упростите инициализацию коллекции
            list.Add(polygon.ExteriorRing?.Coordinates.ToList() ?? new List<Coordinate>());
            list.AddRange(polygon.InteriorRings.Select((LineString i) => i.Coordinates));
            return list;
        }

        throw new NotImplementedException();
    }

    public static List<Coordinate> MainCoordinates(this Geometry geometry)
    {
        if (geometry is LineString lineString)
        {
            return [.. lineString.Coordinates];
        }

        if (geometry is Polygon polygon)
        {
            return polygon.ExteriorRing?.Coordinates.ToList() ?? new List<Coordinate>();
        }

        if (geometry is Point point)
        {
            return [point.Coordinate];
        }

        throw new NotImplementedException();
    }
}
