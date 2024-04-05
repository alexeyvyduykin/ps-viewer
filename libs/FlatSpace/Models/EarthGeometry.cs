using FlatSpace.Utils;

namespace FlatSpace.Models;

public class EarthCircle
{
    public bool IsNorthPoleCover { get; init; }

    public bool IsSouthPoleCover { get; init; }

    public double Angle { get; set; }

    public IEnumerable<IEnumerable<(double lon, double lat)>> Borders { get; init; } =
        new List<List<(double, double)>>();

    // Areas count: 1 or 2
    public IEnumerable<IEnumerable<(double lon, double lat)>> Areas { get; init; } =
        new List<List<(double, double)>>();
}

public static class EarthGeometry
{
    public static double Bottom { get; set; } = -89.0;

    public static double Rearth { get; set; } = 6371.0;

    public static int SegmentDelta { get; set; } = 4;

    public static IEnumerable<EarthCircle> BuildCircles(double lon, double lat, double[] angles)
    {
        foreach (var angle in angles)
        {
            yield return BuildCircle(lon, lat, angle);
        }
    }

    public static EarthCircle BuildCircle(double lon, double lat, double angle)
    {
        ZonaOb(
            lon,
            lat,
            angle,
            out double[] lons,
            out double[] lats,
            out bool isNorth,
            out bool isSouth
        );

        var count = lons.Length;

        List<List<(double, double)>> borders = [];
        List<List<(double, double)>> area = [];

        if (isNorth == false && isSouth == false)
        {
            List<(double, double)> temp = [];
            List<(double, double)>[] areas =
            [
                [],
                []
            ];
            int index = 0;

            double begin = lons[0];
            (double lon, double lat) end = (lons[0], lats[0]);
            for (int i = 0; i < count; i++)
            {
                double lonn = lons[i] - 180;
                if (lonn < -180.0)
                {
                    lonn += 360.0;
                }

                begin = lonn;

                if (Math.Abs(end.lon - begin) > 180.0)
                {
                    var cutLat = LinearInterpDiscontLat(begin, lats[i], end.lon, end.lat);

                    if (end.lon - begin >= 0)
                    {
                        temp.Add((180, cutLat));

                        borders.Add(temp);
                        areas[index].AddRange(temp);

                        temp = [(-180, cutLat)];
                    }
                    else
                    {
                        temp.Add((-180, cutLat));

                        borders.Add(temp);
                        areas[index].AddRange(temp);

                        temp = [(180, cutLat)];
                    }

                    index = Check(index);
                }

                temp.Add((lonn, lats[i]));

                end = (begin, lats[i]);
            }

            borders.Add(temp);
            areas[index].AddRange(temp);

            if (areas[1].Count == 0)
            {
                area = [areas[0]];
            }
            else
            {
                area = [areas[0], areas[1]];
            }
        }
        else if (isNorth == true)
        {
            List<(double, double)> area1 = [];
            List<(double, double)> temp = [];

            double begin = lons[0];
            (double lon, double lat) end = (lons[0], lats[0]);
            for (int i = 0; i < count; i++)
            {
                double lonn = lons[i] - 180;
                if (lonn < -180.0)
                {
                    lonn += 360.0;
                }

                begin = lonn;

                if (Math.Abs(end.lon - begin) > 180.0)
                {
                    var cutLat = LinearInterpDiscontLat(begin, lats[i], end.lon, end.lat);

                    if (end.lon - begin >= 0)
                    {
                        temp.Add((180, cutLat));
                        area1.Add((180, cutLat));
                        borders.Add(temp);

                        area1.Add((+180, 90));
                        area1.Add((-180, 90));

                        temp = [(-180, cutLat)];
                        area1.Add((-180, cutLat));
                    }
                    else
                    {
                        temp.Add((-180, cutLat));
                        area1.Add((-180, cutLat));

                        borders.Add(temp);

                        area1.Add((-180, 90));
                        area1.Add((+180, 90));

                        temp = [(180, cutLat)];
                        area1.Add((180, cutLat));
                    }
                }

                temp.Add((lonn, lats[i]));
                area1.Add((lonn, lats[i]));

                end = (begin, lats[i]);
            }

            borders.Add(temp);
            area = [area1];
        }
        else if (isSouth == true)
        {
            List<(double, double)> area1 = [];
            List<(double, double)> temp = [];

            double begin = lons[0];
            (double lon, double lat) end = (lons[0], lats[0]);
            for (int i = 0; i < count; i++)
            {
                double lonn = lons[i] - 180;
                if (lonn < -180.0)
                {
                    lonn += 360.0;
                }

                begin = lonn;

                if (Math.Abs(end.lon - begin) > 180.0)
                {
                    var cutLat = LinearInterpDiscontLat(begin, lats[i], end.lon, end.lat);

                    if (end.lon - begin >= 0)
                    {
                        temp.Add((180, cutLat));
                        area1.Add((180, cutLat));
                        borders.Add(temp);

                        area1.Add((+180, Bottom));
                        area1.Add((-180, Bottom));

                        temp = [(-180, cutLat)];
                        area1.Add((-180, cutLat));
                    }
                    else
                    {
                        temp.Add((-180, cutLat));
                        area1.Add((-180, cutLat));

                        borders.Add(temp);

                        area1.Add((+180, Bottom));
                        area1.Add((-180, Bottom));

                        temp = [(180, cutLat)];
                        area1.Add((180, cutLat));
                    }
                }

                temp.Add((lonn, lats[i]));
                area1.Add((lonn, lats[i]));

                end = (begin, lats[i]);
            }

            borders.Add(temp);
            area = [area1];
        }

        return new EarthCircle()
        {
            IsNorthPoleCover = isNorth,
            IsSouthPoleCover = isSouth,
            Angle = angle,
            Borders = borders,
            Areas = area.Select(s => s.Select(s => s)),
        };
    }

    private static void ZonaOb(
        double lonDeg,
        double latDeg,
        double angleDeg,
        out double[] lons,
        out double[] lats,
        out bool isNorthPoleCover,
        out bool isSouthPoleCover
    )
    {
        double lat = latDeg * FlatSpaceMath.DegreesToRadians;
        double lon = lonDeg * FlatSpaceMath.DegreesToRadians + Math.PI;
        double angle = angleDeg * FlatSpaceMath.DegreesToRadians;

        double xs = Rearth * Math.Cos(angle);
        double zs = Rearth * Math.Sin(angle);

        var segmentCount = 360 / SegmentDelta + 1;

        lats = new double[segmentCount];
        lons = new double[segmentCount];

        for (int i = 0; i <= 360; i += SegmentDelta)
        {
            int j = i / 4;
            double g = i * FlatSpaceMath.DegreesToRadians;
            double xg =
                xs * Math.Cos(lat) * Math.Cos(-lon)
                + zs
                    * (
                        -Math.Sin(lat) * Math.Cos(g) * Math.Cos(-lon) + Math.Sin(g) * Math.Sin(-lon)
                    );
            double yg =
                xs * (-Math.Sin(-lon) * Math.Cos(lat))
                + zs
                    * (Math.Sin(-lon) * Math.Sin(lat) * Math.Cos(g) + Math.Sin(g) * Math.Cos(-lon));
            double zg = xs * Math.Sin(lat) + zs * Math.Cos(lat) * Math.Cos(g);

            var lon1 = Math.Atan2(yg, xg);
            var lat1 = Math.Asin(zg / Rearth);

            if (lon1 < 0)
            {
                lon1 += 2 * Math.PI;
            }

            if (lon1 > 2 * Math.PI)
            {
                lon1 -= 2 * Math.PI;
            }

            lon1 *= FlatSpaceMath.RadiansToDegrees;
            lat1 *= FlatSpaceMath.RadiansToDegrees;

            if (lon1 > 270)
            {
                lon1 -= 360;
            }

            lons[j] = lon1;
            lats[j] = lat1;
        }

        isNorthPoleCover = latDeg + angleDeg > 90;
        isSouthPoleCover = latDeg - angleDeg < -90;
    }

    private static int Check(int index)
    {
        if (index == 0)
        {
            return 1;
        }
        if (index == 1)
        {
            return 0;
        }

        throw new Exception();
    }

    private static double LinearInterpDiscontLat(
        double lonRad1,
        double latRad1,
        double lonRad2,
        double latRad2
    )
    {
        // one longitude should be negative one positive, make them both positive
        if (lonRad1 > lonRad2)
        {
            lonRad2 += 2 * Math.PI; // in radians
        }
        else
        {
            lonRad1 += 2 * Math.PI;
        }

        return latRad1 + (Math.PI - lonRad1) * (latRad2 - latRad1) / (lonRad2 - lonRad1);
    }
}
