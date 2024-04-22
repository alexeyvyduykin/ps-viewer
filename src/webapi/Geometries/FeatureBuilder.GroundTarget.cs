using FootprintViewerWebApi.Extensions;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Shared.Models;
using webapi.Extensions;
using webapi.Utils;

namespace webapi.Geometries;

public static partial class FeatureBuilder
{
    public static List<Feature> CreateGroundTargets(IEnumerable<GroundTarget> groundTargets)
    {
        return groundTargets.Select(s => CreateGroundTarget(s)).ToList();
    }

    public static Feature CreateGroundTarget(GroundTarget groundTarget, bool isLonLat = true)
    {
        var geometry = groundTarget.Type switch
        {
            GroundTargetType.Point => isLonLat ? groundTarget.Points! : new Point(SphericalMercator.FromLonLat(((Point)groundTarget.Points!).X, ((Point)groundTarget.Points!).Y).ToCoordinate()),
            GroundTargetType.Route => RouteCutting(groundTarget.Points!.Coordinates.Select(s => new Point(s.X, s.Y)).ToList(), isLonLat),
            GroundTargetType.Area => AreaCutting(groundTarget.Points!.Coordinates.Select(s => new Point(s.X, s.Y)).ToList(), isLonLat),
            _ => throw new Exception()
        };

        //var geometry = groundTarget.Center!;

        var feature = geometry.ToFeature();

        return feature;
    }
}
