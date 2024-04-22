using FlatSpace;
using FlatSpace.Extensions;
using FlatSpace.Models;
using Shared.Models;
using static FlatSpace.Extensions.OrbitExtensions;

namespace webapi.Extensions;

public static class SatelliteExtensions
{
    public static Orbit ToOrbit(this Satellite satellite)
    {
        var a = satellite.Semiaxis;
        var ecc = satellite.Eccentricity;
        var inclDeg = satellite.InclinationDeg;
        var argOfPerDeg = satellite.ArgumentOfPerigeeDeg;
        var lonANDeg = satellite.LongitudeAscendingNodeDeg;
        var raanDeg = satellite.RightAscensionAscendingNodeDeg;
        var period = satellite.Period;
        var epoch = satellite.Epoch;

        var orbit = FlatSpaceFactory.CreateOrbit(a, ecc, inclDeg, argOfPerDeg, lonANDeg, raanDeg, period, epoch);

        return orbit;
    }

    public static int NodesOnDay(this Satellite satellite)
    {
        return satellite.ToOrbit().NodesOnDay();
    }

    public static Dictionary<int, List<List<(double lonDeg, double latDeg)>>> BuildSwaths(this Satellite satellite, Shared.Models.SwathDirection direction)
    {
        var orbit = satellite.ToOrbit();

        var dir = direction switch
        {
            Shared.Models.SwathDirection.Left => FlatSpace.Models.SwathDirection.Left,
            Shared.Models.SwathDirection.Right => FlatSpace.Models.SwathDirection.Right,
            _ => throw new NotImplementedException(),
        };

        var swath = FlatSpaceFactory.CreateSwath(orbit, satellite.LookAngleDeg, satellite.RadarAngleDeg, dir);

        var nodes = Enumerable.Range(1, orbit.NodesOnDay()).ToArray();

        var res = swath.BuildSwaths(nodes);

        return res;
    }

    public static List<List<(double lonDeg, double latDeg)>> BuildSwath(this Satellite satellite, int node, Shared.Models.SwathDirection direction)
    {
        var orbit = satellite.ToOrbit();

        var dir = direction switch
        {
            Shared.Models.SwathDirection.Left => FlatSpace.Models.SwathDirection.Left,
            Shared.Models.SwathDirection.Right => FlatSpace.Models.SwathDirection.Right,
            _ => throw new NotImplementedException(),
        };

        var swath = FlatSpaceFactory.CreateSwath(orbit, satellite.LookAngleDeg, satellite.RadarAngleDeg, dir);

        var res = swath.BuildSwaths([node]);

        return res[node];
    }

    public static Dictionary<int, List<List<(double lonDeg, double latDeg)>>> BuildTracks(this Satellite satellite)
    {
        var res = satellite.ToOrbit().BuildTracks();

        return res.Track;
    }
}
