using FlatSpace.Models;
using FlatSpace.Utils;

namespace FlatSpace;

public static class FlatSpaceFactory
{
    public static Orbit CreateOrbit(double a, double inclDeg)
    {
        double gm = Constants.GM;

        var incl = inclDeg * FlatSpaceMath.DegreesToRadians;

        var period = 2 * Math.PI * Math.Sqrt(a * a * a / gm);

        return new Orbit(a, 0.0, incl, 0.0, 0.0, 0.0, period, new());
    }

    public static Orbit CreateOrbit(double a, double inclDeg, double lonANDeg)
    {
        double gm = Constants.GM;

        var incl = inclDeg * FlatSpaceMath.DegreesToRadians;
        var lonAN = lonANDeg * FlatSpaceMath.DegreesToRadians;

        var period = 2 * Math.PI * Math.Sqrt(a * a * a / gm);

        return new Orbit(a, 0.0, incl, 0.0, lonAN, 0.0, period, new());
    }

    public static Orbit CreateOrbit(
        double a,
        double ecc,
        double inclDeg,
        double argOfPerDeg,
        double lonANDeg,
        double raanDeg,
        double period,
        DateTime epoch
    )
    {
        var incl = inclDeg * FlatSpaceMath.DegreesToRadians;
        var argOfPer = argOfPerDeg * FlatSpaceMath.DegreesToRadians;
        var lonAN = lonANDeg * FlatSpaceMath.DegreesToRadians;
        var raan = raanDeg * FlatSpaceMath.DegreesToRadians;

        return new Orbit(a, ecc, incl, argOfPer, lonAN, raan, period, epoch);
    }

    public static Julian CreateJulianDate(DateTime utc)
    {
        return new Julian(utc);
    }

    public static GroundTrack CreateGroundTrack(Orbit orbit)
    {
        return new GroundTrack(orbit);
    }

    public static GroundTrack CreateGroundTrack(
        Orbit orbit,
        FactorShiftTrack factor,
        double angleDeg,
        TrackDirection direction
    )
    {
        return new GroundTrack(orbit, factor, angleDeg, direction);
    }

    public static Swath CreateSwath(
        Orbit orbit,
        double lookAngleDEG,
        double radarAngleDEG,
        SwathDirection direction
    )
    {
        return new Swath(orbit, lookAngleDEG, radarAngleDEG, direction);
    }
}
