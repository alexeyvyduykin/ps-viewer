using FootprintViewerWebApi.Extensions;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using webapi.Extensions;
using webapi.Utils;

namespace webapi.Geometries;

public static partial class FeatureBuilder
{
    public static Dictionary<int, List<Feature>> CreateSwaths(Dictionary<int, List<List<(double lonDeg, double latDeg)>>> swaths, bool isLonLat = true)
    {
        return swaths.ToDictionary(
            s => s.Key,
            s => s.Value.Select(s => ToPolygonFeature(s, isLonLat)).ToList());
    }

    public static List<Feature> CreateSwath(List<List<(double lonDeg, double latDeg)>> swaths, bool isLonLat = true)
    {
        return swaths.Select(s => ToPolygonFeature(s, isLonLat)).ToList();
    }

    private static Feature ToPolygonFeature(List<(double lonDeg, double latDeg)> list, bool isLonLat = true)
    {
        var vertices = (isLonLat) ? list : list.Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg));

        var poly = new GeometryFactory().CreatePolygon(vertices.ToClosedCoordinates());

        var feature = poly.ToFeature();

        return feature;
    }

    public static Dictionary<int, List<IFeature>> CreateSwathsVertices(Dictionary<int, List<List<(double lonDeg, double latDeg)>>> swaths, bool isLonLat = true)
    {
        return swaths.ToDictionary(
            s => s.Key,
            s => s.Value.SelectMany(s => ToPointsFeatures(s, isLonLat)).ToList());
    }

    private static List<IFeature> ToPointsFeatures(List<(double lonDeg, double latDeg)> list, bool isLonLat = true)
    {
        return list
            .Select(s => (isLonLat) ? s : SphericalMercator.FromLonLat(s.lonDeg, s.latDeg))
            .ToCoordinates()
            .Select(s => new GeometryFactory().CreatePoint(s))
            .Select(s => (IFeature)s.ToFeature())
            .ToList();
    }
}
