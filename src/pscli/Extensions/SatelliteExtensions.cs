using FlatSpace;
using FlatSpace.Extensions;
using FlatSpace.Models;
using Shared.Models;

namespace pscli.Extensions;

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
}
