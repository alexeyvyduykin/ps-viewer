using NetTopologySuite.Features;
using webapi.Extensions;
using webapi.Geometries;

namespace webapi.Services;

public partial class FeatureRepository
{
    private FeatureCollection CreateGroundStation(string name, double[]? angles = null, bool isLonLat = true)
    {
        var ps = _dataService.GetPlannedScheduleObject();

        if (ps != null)
        {
            var gs = ps.GroundStations
                .Where(s => string.Equals(s.Name, name))
                .SingleOrDefault();

            if (gs != null)
            {
                if (angles != null)
                {
                    return FeatureBuilder.CreateGroundStation(gs.Center.X, gs.Center.Y, angles, isLonLat).ToFeatureCollection();
                }

                return FeatureBuilder.CreateGroundStation(gs, isLonLat).ToFeatureCollection();
            }
        }

        return [];
    }
}
