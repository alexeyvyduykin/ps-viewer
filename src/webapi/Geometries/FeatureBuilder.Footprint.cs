using FootprintViewerWebApi.Extensions;
using NetTopologySuite.Features;
using Shared.Models;

namespace webapi.Geometries;

public static partial class FeatureBuilder
{
    public static List<Feature> CreateFootprints(IEnumerable<Footprint> footprints, bool isLonLat = true)
    {
        return footprints.Select(s => CreateFootprint(s, isLonLat)).ToList();
    }

    // TODO: footprint border is nullable ???
    public static Feature CreateFootprint(Footprint footprint, bool isLonLat = true)
    {
        var poly = AreaCutting(footprint.Border?.Coordinates ?? [], isLonLat);

        var feature = poly.ToFeature();

        return feature;
    }
}
