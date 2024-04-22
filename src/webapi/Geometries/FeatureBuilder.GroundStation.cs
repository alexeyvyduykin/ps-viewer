using FootprintViewerWebApi.Extensions;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Shared.Models;
using webapi.Builders;
using webapi.Extensions;
using webapi.Utils;

namespace webapi.Geometries;

public static partial class FeatureBuilder
{
    public static List<Feature> CreateGroundStation(GroundStation groundStation, bool isLonLat = true)
    {
        var list = new List<Feature>();

        var gs = GroundStationBuilder.Create(groundStation.Center.X, groundStation.Center.Y, groundStation.Angles);

        var areaCount = gs.Areas.Count;

        bool isHole = (gs.InnerAngle != 0.0);

        // First area
        if (isHole == false)
        {
            var poligons = gs.Areas.First()
                .Select(s => s.Select(s => isLonLat ? s : SphericalMercator.FromLonLat(s.lon, s.lat)))
                .Select(s => new GeometryFactory().CreatePolygon(s.ToClosedCoordinates()))
                .ToArray();

            var multi = new GeometryFactory().CreateMultiPolygon(poligons);

            var attributes = new AttributesTable()
            {
                { "Count", $"{areaCount}" },
                { "Index", $"{0}" }
            };

            var feature = multi.ToFeature(attributes);

            list.Add(feature);
        }

        // Areas
        if (areaCount > 1)
        {
            for (int i = 1; i < areaCount; i++)
            {
                var poligons = new List<Polygon>();

                var rings = gs.Areas[i - 1].Select(s => s.Reverse().Select(s => isLonLat ? s : SphericalMercator.FromLonLat(s.lon, s.lat))).ToList();

                int index = 0;

                foreach (var points1 in gs.Areas[i])
                {
                    var res = isLonLat ? points1 : points1.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat));

                    if (index < rings.Count)
                    {
                        var interiorRings = (gs.Areas[i].Count() == 1) ? rings : [rings[index++]];

                        var shell = new GeometryFactory().CreateLinearRing(res.ToClosedCoordinates());
                        var holes = interiorRings.Select(s => new GeometryFactory().CreateLinearRing(s.ToClosedCoordinates())).ToArray();

                        var poly = new GeometryFactory().CreatePolygon(shell, holes);

                        poligons.Add(poly);
                    }
                    else
                    {
                        var poly = new GeometryFactory().CreatePolygon(res.ToClosedCoordinates());

                        poligons.Add(poly);
                    }
                }

                var multi = new GeometryFactory().CreateMultiPolygon([.. poligons]);

                var attributes = new AttributesTable()
                {
                    { "Count", $"{(isHole == true ? areaCount - 1 : areaCount)}" },
                    { "Index", $"{(isHole == true ? i - 1 : i)}" }
                };

                var feature = multi.ToFeature(attributes);

                list.Add(feature);
            }
        }

        // Inner border
        if (isHole == true)
        {
            var lineStrings = gs.InnerBorder
                .Select(s => s.Select(s => isLonLat ? s.ToCoordinate() : SphericalMercator.FromLonLat(s.lon, s.lat).ToCoordinate()).ToArray())
                .Select(s => new GeometryFactory().CreateLineString(s))
                .ToArray();

            var multi = new GeometryFactory().CreateMultiLineString(lineStrings);

            var attributes = new AttributesTable()
                {
                    { "Count", $"{(isHole == true ? areaCount - 1 : areaCount)}" },
                    { "InnerBorder", "inner" }
                };

            var feature = multi.ToFeature(attributes);

            list.Add(feature);
        }

        // Outer border
        if (gs.OuterBorder.Count != 0)
        {
            var lineStrings = gs.OuterBorder
                .Select(s => s.Select(s => isLonLat ? s.ToCoordinate() : SphericalMercator.FromLonLat(s.lon, s.lat).ToCoordinate()).ToArray())
                .Select(s => new GeometryFactory().CreateLineString(s))
                .ToArray();

            var multi = new GeometryFactory().CreateMultiLineString(lineStrings);

            var attributes = new AttributesTable()
                {
                    { "Count", $"{(isHole == true ? areaCount - 1 : areaCount)}" },
                    { "OuterBorder", "outer" }
                };

            var feature = multi.ToFeature(attributes);

            list.Add(feature);
        }

        return list;
    }

    public static List<Feature> CreateGroundStation(double x, double y, double[] angles, bool isLonLat = true)
    {
        var list = new List<Feature>();

        var gs = GroundStationBuilder.Create(x, y, angles);

        var areaCount = gs.Areas.Count;

        bool isHole = (gs.InnerAngle != 0.0);

        // First area
        if (isHole == false)
        {
            var poligons = gs.Areas.First()
                .Select(s => s.Select(s => isLonLat ? s : SphericalMercator.FromLonLat(s.lon, s.lat)))
                .Select(s => new GeometryFactory().CreatePolygon(s.ToClosedCoordinates()))
                .ToArray();

            var multi = new GeometryFactory().CreateMultiPolygon(poligons);

            var attributes = new AttributesTable()
            {
                { "Count", $"{areaCount}" },
                { "Index", $"{0}" }
            };

            var feature = multi.ToFeature(attributes);

            list.Add(feature);
        }

        // Areas
        if (areaCount > 1)
        {
            for (int i = 1; i < areaCount; i++)
            {
                var poligons = new List<Polygon>();

                var rings = gs.Areas[i - 1].Select(s => s.Reverse().Select(s => isLonLat ? s : SphericalMercator.FromLonLat(s.lon, s.lat))).ToList();

                int index = 0;

                foreach (var points1 in gs.Areas[i])
                {
                    var res = isLonLat ? points1 : points1.Select(s => SphericalMercator.FromLonLat(s.lon, s.lat));

                    if (index < rings.Count)
                    {
                        var interiorRings = (gs.Areas[i].Count() == 1) ? rings : [rings[index++]];

                        var shell = new GeometryFactory().CreateLinearRing(res.ToClosedCoordinates());
                        var holes = interiorRings.Select(s => new GeometryFactory().CreateLinearRing(s.ToClosedCoordinates())).ToArray();

                        var poly = new GeometryFactory().CreatePolygon(shell, holes);

                        poligons.Add(poly);
                    }
                    else
                    {
                        var poly = new GeometryFactory().CreatePolygon(res.ToClosedCoordinates());

                        poligons.Add(poly);
                    }
                }

                var multi = new GeometryFactory().CreateMultiPolygon([.. poligons]);

                var attributes = new AttributesTable()
                {
                    { "Count", $"{(isHole == true ? areaCount - 1 : areaCount)}" },
                    { "Index", $"{(isHole == true ? i - 1 : i)}" }
                };

                var feature = multi.ToFeature(attributes);

                list.Add(feature);
            }
        }

        // Inner border
        if (isHole == true)
        {
            var lineStrings = gs.InnerBorder
                .Select(s => s.Select(s => isLonLat ? s.ToCoordinate() : SphericalMercator.FromLonLat(s.lon, s.lat).ToCoordinate()).ToArray())
                .Select(s => new GeometryFactory().CreateLineString(s))
                .ToArray();

            var multi = new GeometryFactory().CreateMultiLineString(lineStrings);

            var attributes = new AttributesTable()
                {
                    { "Count", $"{(isHole == true ? areaCount - 1 : areaCount)}" },
                    { "InnerBorder", "inner" }
                };

            var feature = multi.ToFeature(attributes);

            list.Add(feature);
        }

        // Outer border
        if (gs.OuterBorder.Count != 0)
        {
            var lineStrings = gs.OuterBorder
                .Select(s => s.Select(s => isLonLat ? s.ToCoordinate() : SphericalMercator.FromLonLat(s.lon, s.lat).ToCoordinate()).ToArray())
                .Select(s => new GeometryFactory().CreateLineString(s))
                .ToArray();

            var multi = new GeometryFactory().CreateMultiLineString(lineStrings);

            var attributes = new AttributesTable()
                {
                    { "Count", $"{(isHole == true ? areaCount - 1 : areaCount)}" },
                    { "OuterBorder", "outer" }
                };

            var feature = multi.ToFeature(attributes);

            list.Add(feature);
        }

        return list;
    }
}
