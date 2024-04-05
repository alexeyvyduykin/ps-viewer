using FlatSpace.Utils;

namespace FlatSpace.Models;

public class GroundTrack
{
    private readonly double _earthRotationSec = 86164.09053;
    private readonly FactorShiftTrack _factor;
    private readonly Orbit _orbit;
    private readonly double _angleRad;
    private readonly double _angleDeg;
    private readonly double _period;
    private readonly TrackDirection _trackPointDirection;
    private readonly int _direction;
    private readonly List<(double lonDeg, double latDeg, double u, double t)> _cache = [];

    internal GroundTrack(Orbit orbit)
    {
        _angleDeg = 0.0;
        _angleRad = 0.0;
        _orbit = orbit;
        _factor = new FactorShiftTrack(_orbit, 0.0, 0.0, SwathDirection.Middle);
        _direction = 0;
        _trackPointDirection = TrackDirection.None;
        _period = orbit.Period;
    }

    internal GroundTrack(
        Orbit orbit,
        FactorShiftTrack factor,
        double angleDeg,
        TrackDirection direction
    )
    {
        _angleDeg = angleDeg;
        _angleRad = angleDeg * FlatSpaceMath.DegreesToRadians;
        _orbit = orbit;
        _factor = factor;
        _period = orbit.Period;
        _trackPointDirection = direction;
        _direction = direction switch
        {
            TrackDirection.None => 0,
            TrackDirection.Left => -1,
            TrackDirection.Right => 1,
            _ => 0,
        };
    }

    public List<(double lonDeg, double latDeg, double u, double t)> CacheTrack => _cache;

    public double AngleDeg => _angleDeg;

    public TrackDirection Direction => _trackPointDirection;

    public double NodeOffsetDeg => 360.0 * _factor.Offset;

    public double EarthRotateOffsetDeg => -(_period / _earthRotationSec) * 360.0;

    public void CalculateTrackWithLogStep(int counts)
    {
        _cache.Clear();

        var slices = (int)Math.Ceiling(counts / 4.0);

        // [0 - 90)
        foreach (var item in LogStep(0.0, 90, slices).SkipLast(1))
        {
            var u = item * FlatSpaceMath.DegreesToRadians;

            var (lonDeg, latDeg) = ContinuousTrack(u);

            _cache.Add((lonDeg, latDeg, u, double.NaN));
        }

        // [90 - 180)
        foreach (var item in LogStepReverse(180, 90, slices).SkipLast(1))
        {
            var u = item * FlatSpaceMath.DegreesToRadians;

            var (lonDeg, latDeg) = ContinuousTrack(u);

            _cache.Add((lonDeg, latDeg, u, double.NaN));
        }

        // [180 - 270)
        foreach (var item in LogStep(180, 270, slices).SkipLast(1))
        {
            var u = item * FlatSpaceMath.DegreesToRadians;

            var (lonDeg, latDeg) = ContinuousTrack(u);

            _cache.Add((lonDeg, latDeg, u, double.NaN));
        }

        // [270 - 360]
        foreach (var item in LogStepReverse(360, 270, slices))
        {
            var u = item * FlatSpaceMath.DegreesToRadians;

            var (lonDeg, latDeg) = ContinuousTrack(u);

            _cache.Add((lonDeg, latDeg, u, double.NaN));
        }
        return;
    }

    public void CalculateTrackWithLogStep(int counts, double[] us)
    {
        _cache.Clear();

        var slices = (int)Math.Ceiling(counts / 4.0);

        // [0 - 90)
        foreach (var item in LogStep(0.0, 90, slices).SkipLast(1))
        {
            var u = item * FlatSpaceMath.DegreesToRadians;

            var (lonDeg, latDeg) = ContinuousTrack(u);

            _cache.Add((lonDeg, latDeg, u, double.NaN));
        }

        // [90 - 180)
        foreach (var item in LogStepReverse(180, 90, slices).SkipLast(1))
        {
            var u = item * FlatSpaceMath.DegreesToRadians;

            var (lonDeg, latDeg) = ContinuousTrack(u);

            _cache.Add((lonDeg, latDeg, u, double.NaN));
        }

        // [180 - 270)
        foreach (var item in LogStep(180, 270, slices).SkipLast(1))
        {
            var u = item * FlatSpaceMath.DegreesToRadians;

            var (lonDeg, latDeg) = ContinuousTrack(u);

            _cache.Add((lonDeg, latDeg, u, double.NaN));
        }

        // [270 - 360]
        foreach (var item in LogStepReverse(360, 270, slices))
        {
            var u = item * FlatSpaceMath.DegreesToRadians;

            var (lonDeg, latDeg) = ContinuousTrack(u);

            _cache.Add((lonDeg, latDeg, u, double.NaN));
        }

        foreach (var u in us)
        {
            var (lonDeg, latDeg) = ContinuousTrack(u);

            _cache.Add((lonDeg, latDeg, u, double.NaN));
        }

        _cache.Sort((p1, p2) => p1.u.CompareTo(p2.u));

        return;
    }

    public void CalculateTrack(double dt = 60.0)
    {
        var period = _orbit.Period;

        _cache.Clear();

        for (double t = 0; t < period; t += dt)
        {
            var u = _orbit.Anomalia(t);

            var (lonDeg, latDeg) = ContinuousTrack(u);

            _cache.Add((lonDeg, latDeg, u, t));
        }

        return;
    }

    public (double lonDeg, double latDeg, double u) GetPointOnTrack(double t)
    {
        var u = _orbit.Anomalia(t);
        var (lonDeg, latDeg) = ContinuousTrack(u);
        return (lonDeg, latDeg, u);
    }

    // TODO: make complex solution
    internal bool _isNodeCorrect = false;

    public void CalculateTrackOnTimeInterval(double t0, double t1, double dt = 60.0)
    {
        if (t0 < 0)
        {
            _isNodeCorrect = true;
        }

        var period = _orbit.Period;
        var tBegin = Math.Min(t0, t1);
        var tEnd = Math.Max(t0, t1);

        int n = 0;

        while (tBegin >= period)
        {
            tBegin -= period;
            n++;
        }

        tEnd -= n * period;

        _cache.Clear();

        if (Math.Abs(tBegin - tEnd) >= period)
        {
            // default one node, with start from [tBegin, tEnd)

            for (double t = tBegin; t < tBegin + period; t += dt)
            {
                var u = _orbit.Anomalia(t);

                var (lonDeg, latDeg) = ContinuousTrack(u);

                _cache.Add((lonDeg, latDeg, u, t));
            }

            return;
        }

        for (double t = tBegin; t < tEnd; t += dt)
        {
            var u = _orbit.Anomalia(t);

            var (lonDeg, latDeg) = ContinuousTrack(u);

            _cache.Add((lonDeg, latDeg, u, t));
        }

        var uEnd = _orbit.Anomalia(tEnd);

        var (lonDegEnd, latDegEnd) = ContinuousTrack(uEnd);

        _cache.Add((lonDegEnd, latDegEnd, uEnd, tEnd));

        return;
    }

    public void CalculateTrack(double uBegin, double uEnd, int counts = 10)
    {
        _cache.Clear();

        var num = Math.Max(1, counts);

        if (num == 1)
        {
            var (lonDeg, latDeg) = ContinuousTrack(uBegin);

            _cache.Add((lonDeg, latDeg, uBegin, uBegin * _period / (2 * Math.PI)));

            return;
        }

        var du = (uEnd - uBegin) / (num - 1);

        for (double u = uBegin; u <= uEnd; u += du)
        {
            var (lonDeg, latDeg) = ContinuousTrack(u);

            _cache.Add((lonDeg, latDeg, u, u * _period / (2 * Math.PI)));
        }

        return;
    }

    private static double[] LogStep(double start = 0.0, double end = 1.0, int slices = 10)
    {
        //   List<double> list = new List<double>();

        // I want to create 7 slices in a segment of length = end - start
        // whose extremes are logarithmically distributed:
        //     |         1       |     2    |   3  |  4 | 5 |6 |7|
        //     +-----------------+----------+------+----+---+--+-+
        //   start                                              end

        double scale = (end - start) / Math.Log(1.0 + slices);
        double lower_bound = start;

        double[] arr = new double[slices + 1];

        for (int i = 0; i < slices; ++i)
        {
            // transform to the interval (1,n_slices+1):
            //     1                 2          3      4    5   6  7 8
            //     +-----------------+----------+------+----+---+--+-+
            //   start                                              end

            double upper_bound = start + Math.Log(2.0 + i) * scale;

            // use the extremes in your function
            //my_function(lower_bound, upper_bound);

            //yield return lower_bound;

            //list.Add(lower_bound);
            arr[i] = lower_bound;

            // update
            lower_bound = upper_bound;
        }

        //yield return lower_bound;

        //  list.Add(lower_bound);
        arr[slices] = lower_bound;

        return arr;
    }

    private static double[] LogStepReverse(double start = 0.0, double end = 1.0, int slices = 10)
    {
        //   List<double> list = new List<double>();

        // I want to create 7 slices in a segment of length = end - start
        // whose extremes are logarithmically distributed:
        //     |         1       |     2    |   3  |  4 | 5 |6 |7|
        //     +-----------------+----------+------+----+---+--+-+
        //   start                                              end

        double scale = (end - start) / Math.Log(1.0 + slices);
        double lower_bound = start;

        double[] arr = new double[slices + 1];

        for (int i = 0; i < slices; ++i)
        {
            // transform to the interval (1,n_slices+1):
            //     1                 2          3      4    5   6  7 8
            //     +-----------------+----------+------+----+---+--+-+
            //   start                                              end

            double upper_bound = start + Math.Log(2.0 + i) * scale;

            // use the extremes in your function
            //my_function(lower_bound, upper_bound);

            //yield return lower_bound;

            //list.Add(lower_bound);
            arr[slices - i] = lower_bound;

            // update
            lower_bound = upper_bound;
        }

        //yield return lower_bound;

        //  list.Add(lower_bound);
        arr[0] = lower_bound;

        return arr;
    }

    public (double lonDeg, double latDeg) ContinuousTrack(double u)
    {
        double semi_axis = (_orbit.Eccentricity == 0.0) ? _orbit.SemimajorAxis : _orbit.Semiaxis(u);
        double angle =
            FlatSpaceMath.HALFPI
            - Math.Acos(semi_axis * Math.Sin(_angleRad) / Constants.Re)
            - _angleRad;
        double uTr = (angle == 0.0) ? u : Math.Acos(Math.Cos(angle) * Math.Cos(u));
        double iTr = _orbit.Inclination - Math.Atan2(Math.Tan(angle), Math.Sin(u)) * _direction;

        double lat = Math.Asin(Math.Sin(uTr) * Math.Sin(iTr));
        double asinlon = Math.Tan(lat) / Math.Tan(iTr);

        if (Math.Abs(asinlon) > 1.0)
        {
            asinlon = FlatSpaceMath.Sign(asinlon);
        }

        double lon = Math.Asin(asinlon);

        if (u >= 0.0 && u < FlatSpaceMath.HALFPI)
        {
            lon = 0 + lon;
        }
        else if (u >= FlatSpaceMath.HALFPI && u < 3.0 * FlatSpaceMath.HALFPI)
        {
            lon = Math.PI - lon - _factor.Quart23 * FlatSpaceMath.TWOPI;
        }
        else if (u >= 3.0 * FlatSpaceMath.HALFPI && u <= FlatSpaceMath.TWOPI)
        {
            lon = FlatSpaceMath.TWOPI + lon - _factor.Quart4 * FlatSpaceMath.TWOPI;
        }

        lon = lon - (_period / _earthRotationSec) * u;

        lon = lon + _orbit.LonAscnNode;

        //   lon = lon + SpaceMath.TWOPI * _factor.Offset;
        return (lon * FlatSpaceMath.RadiansToDegrees, lat * FlatSpaceMath.RadiansToDegrees);
    }

    public bool PolisMod(double lat, out double polis_mod)
    {
        polis_mod = double.NaN;

        double t_polis;
        int err;
        if (lat >= 0.0)
        {
            t_polis = _orbit.TimeHalfPi();
            err = 1;
        }
        else
        {
            t_polis = 3.0 * _orbit.TimeHalfPi();
            err = -1;
        }

        //double per = 3.0 * Orbit.Period / 4.0;

        double fi = CentralAngleFromT(
            t_polis + _orbit.ArgumentOfPerigee * _orbit.Period / FlatSpaceMath.TWOPI
        );
        double i = _orbit.Inclination - fi * _direction;

        //double i_deg = Orbit.Inclination * ScienceMath.RadiansToDegrees;
        //double fi_deg = fi * ScienceMath.RadiansToDegrees;

        if (i > FlatSpaceMath.HALFPI)
        {
            i = Math.PI - i;
        }

        if (i <= Math.Abs(lat))
        {
            polis_mod = _orbit.InclinationNormal + fi * _direction * err;
            return true;
        }

        return false;
    }

    private double CentralAngleFromT(double t)
    {
        double u = _orbit.Anomalia(t) + _orbit.ArgumentOfPerigee;
        double a = _orbit.Semiaxis(u);
        return FlatSpaceMath.HALFPI - Math.Acos(a * Math.Sin(_angleRad) / Constants.Re) - _angleRad;
    }
}
