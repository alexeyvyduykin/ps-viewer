using FlatSpace;
using FlatSpace.Utils;
using Shared.Models;

namespace pscli;

public static class Factory
{
    public static Satellite CreateSatellite(string name, double semiaxis, double ecc, double incl, double lan, double period, DateTime epoch, double lookAngle, double radarAngle)
    {
        var raan = GetRAAN(epoch, 0.0, lan);

        return new Satellite()
        {
            Name = name,
            Semiaxis = semiaxis,
            Eccentricity = ecc,
            InclinationDeg = incl,
            ArgumentOfPerigeeDeg = 0.0,
            LongitudeAscendingNodeDeg = lan,
            RightAscensionAscendingNodeDeg = FlatSpaceMath.LongitudeNormalization(raan),
            Period = period,
            Epoch = epoch,
            LookAngleDeg = lookAngle,
            RadarAngleDeg = radarAngle
        };
    }

    private static double GetRAAN(DateTime epoch, double tAN, double lonAN)
    {
        var jd = FlatSpaceFactory.CreateJulianDate(epoch);
        double S = jd.ToGmst();
        //double S = orbitState.SiderealTime();       
        return (tAN * Constants.Omega + S) * FlatSpaceMath.RadiansToDegrees + lonAN;
    }
}
