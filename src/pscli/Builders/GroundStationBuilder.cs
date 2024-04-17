﻿using FlatSpace.Models;
using NetTopologySuite.Geometries;
using Shared.Models;

namespace pscli.Builders;

public class GroundStationBuilderResult
{
    public (double lon, double lat) Center { get; init; } = new();

    public double InnerAngle { get; init; }

    public double OuterAngle { get; init; }

    public List<IEnumerable<IEnumerable<(double lon, double lat)>>> Areas { get; init; } = [];

    public List<IEnumerable<(double lon, double lat)>> InnerBorder { get; init; } = [];

    public List<IEnumerable<(double lon, double lat)>> OuterBorder { get; init; } = [];
}

public static class GroundStationBuilder
{
    private static readonly double[] _defaultAngles = [0.0, 10.0];

    private static async Task<T> Start<T>(Func<T> func)
        => await Task.Run(func);

    public static async Task<IList<GroundStation>> CreateDefaultAsync()
        => await Start(() => CreateDefault());

    public static GroundStation CreateRandom()
    {
        return new GroundStation()
        {
            Name = $"London",
            Center = new Point(-0.118092, 51.509865),
            Angles = [12, 18, 22, 26, 30],
        };
    }

    public static IList<GroundStation> CreateDefault()
    {
        return [
            new() { Name = "Tokyo",       Center = new Point(139.41,  35.41), Angles = [0.0, 6, 10, 11] },
            new() { Name = "São Paulo",   Center = new Point(-46.38, -23.33), Angles = [ 0.0, 6, 10, 11 ] },
            new() { Name = "Mexico City", Center = new Point(-99.80,  19.26), Angles = [ 0.0, 6, 10, 11 ] },
            new() { Name = "New York",    Center = new Point(-74.00,  40.42), Angles = [ 0.0, 6, 10, 11 ] },
            new() { Name = "Paris",       Center = new Point(  2.21,  48.51), Angles = [ 0.0, 6, 10, 11 ] },
            new() { Name = "Seoul",       Center = new Point(126.59,  37.33), Angles = [ 0.0, 6, 10, 11 ] },
        ];
    }

    public static GroundStationBuilderResult Create(double lon, double lat, double[] angles)
    {
        var (isHole, anglesRes) = Verify(angles);
        var circles = EarthGeometry.BuildCircles(lon, lat, anglesRes);

        return new GroundStationBuilderResult()
        {
            Center = (lon, lat),
            InnerAngle = (isHole == false) ? 0.0 : anglesRes.First(),
            OuterAngle = anglesRes.Last(),
            Areas = circles.Select(s => s.Areas).ToList(),
            InnerBorder = (isHole == false) ? new List<IEnumerable<(double, double)>>() : circles.First().Borders.ToList(),
            OuterBorder = circles.Last().Borders.ToList(),
        };
    }

    private static (bool isHole, double[] angles) Verify(double[] anglesSource)
    {
        var list = new List<double>(anglesSource);

        list.Sort();

        if (list.Count == 1)
        {
            list.Insert(0, 0.0);
        }

        if (list.Where(s => s != 0.0).Any() == false)
        {
            list.Clear();
            list.AddRange(_defaultAngles);
        }

        var first = list.First();

        var isHole = (first != 0.0);

        var angles = list.Where(s => s != 0.0).ToArray();

        return (isHole, angles);
    }
}
