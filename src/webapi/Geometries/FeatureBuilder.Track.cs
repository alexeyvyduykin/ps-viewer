using FootprintViewerWebApi.Extensions;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using webapi.Extensions;
using webapi.Utils;

namespace webapi.Geometries;

public static partial class FeatureBuilder
{
    public static Dictionary<int, List<Feature>> CreateTracks(Dictionary<int, List<List<(double lonDeg, double latDeg)>>> tracks, bool isLonLat = true)
    {
        return tracks.ToDictionary(
            s => s.Key,
            s => s.Value.Select(s => CreateLineString(s, isLonLat)).ToList());
    }

    public static Dictionary<int, List<IFeature>> CreateTracksVertices(Dictionary<int, List<List<(double lonDeg, double latDeg)>>> tracks, bool isLonLat = true)
    {
        return tracks.ToDictionary(
            s => s.Key,
            s => s.Value.SelectMany(s => ToPointsFeatures(s, isLonLat)).ToList());
    }

    public static IFeature CreateTrack(List<(double lonDeg, double latDeg)> list, bool isLonLat = true)
    {
        return CreateLineString(list, isLonLat);
    }

    public static IFeature CreateTrack(List<List<(double lonDeg, double latDeg)>> list, bool isLonLat = true)
    {
        var lineStrings = new List<LineString>();

        foreach (var item in list)
        {
            var vertices = (isLonLat) ? item : item.Select(s => SphericalMercator.FromLonLat(s.lonDeg, s.latDeg));

            var line = new GeometryFactory().CreateLineString(vertices.ToGreaterThanTwoCoordinates());

            lineStrings.Add(line);
        }

        return new MultiLineString([.. lineStrings]).ToFeature();
    }

    public static List<IFeature> CreateMarkers(List<(double lonDeg, double latDeg)> points, bool isLonLat = true)
    {
        return [.. ToPointsFeatures(points, isLonLat)];
    }
}