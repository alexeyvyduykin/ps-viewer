using NetTopologySuite.Geometries;

namespace webapi.Extensions;

public static class CoordinateExtensions
{
    public static Coordinate ToCoordinate(this (double x, double y) value)
    {
        return new Coordinate(value.x, value.y);
    }

    public static Coordinate[] ToCoordinates(this IEnumerable<(double, double)> values)
    {
        var coordinates = values.Select(s => s.ToCoordinate()).ToArray();

        return coordinates;
    }

    public static Coordinate[] ToClosedCoordinates(this IEnumerable<(double, double)> values)
    {
        var coordinates = values.Select(s => s.ToCoordinate()).ToList();

        var first = coordinates.First();

        if (first != coordinates.Last())
        {
            coordinates.Add(first);
        }

        return [.. coordinates];
    }

    public static Coordinate[] ToClosedCoordinates(this Coordinate[] coordinates)
    {
        var first = coordinates[0];

        var list = coordinates.ToList();

        if (first != list.Last())
        {
            list.Add(first);
        }

        return [.. list];
    }

    public static Coordinate[] ToGreaterThanTwoCoordinates(this IEnumerable<(double, double)> values)
    {
        var coordinates = values.Select(s => s.ToCoordinate()).ToArray();

        if (coordinates.Length >= 2)
        {
            return coordinates;
        }

        if (coordinates.Length == 0)
        {
            return [new Coordinate(0.0, 0.0), new Coordinate(0.0, 0.0)];
        }

        return [coordinates[0], coordinates[0]];
    }

}
