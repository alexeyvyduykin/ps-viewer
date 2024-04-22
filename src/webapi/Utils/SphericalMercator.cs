using NetTopologySuite.Geometries;
using webapi.Extensions;

namespace webapi.Utils;

public class SphericalMercator
{
#pragma warning disable IDE0051 // Удалите неиспользуемые закрытые члены
    private const double Radius = 6378137.0;

    private const double D2R = Math.PI / 180.0;

    private const double HalfPi = Math.PI / 2.0;
#pragma warning restore IDE0051 // Удалите неиспользуемые закрытые члены

    public static Coordinate FromLonLat(Coordinate point)
    {
        return FromLonLat(point.X, point.Y).ToCoordinate();
    }

    public static (double x, double y) FromLonLat(double lon, double lat)
    {
        double num = Math.PI / 180.0 * lon;
        double num2 = Math.PI / 180.0 * lat;
        double item = 6378137.0 * num;
        double item2 = 6378137.0 * Math.Log(Math.Tan(Math.PI / 4.0 + num2 * 0.5));
        return (item, item2);
    }

    public static Coordinate ToLonLat(Coordinate point)
    {
        return ToLonLat(point.X, point.Y).ToCoordinate();
    }

    public static (double lon, double lat) ToLonLat(double x, double y)
    {
        double d = Math.Exp((0.0 - y) / 6378137.0);
        double num = Math.PI / 2.0 - 2.0 * Math.Atan(d);
        double item = x / 6378137.0 / (Math.PI / 180.0);
        double item2 = num / (Math.PI / 180.0);
        return (item, item2);
    }
}
