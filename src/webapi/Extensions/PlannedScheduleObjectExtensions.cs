using FlatSpace;
using FlatSpace.Extensions;
using FlatSpace.Utils;
using Shared.Models;

namespace webapi.Extensions;

public static class PlannedScheduleObjectExtensions
{
    public static List<ObservationTaskResult> GetObservationTaskResults(this PlannedScheduleObject ps)
    {
        return [.. ps.ObservationTaskResults];
    }

    public static ObservationTaskResult? GetObservationTaskResult(this PlannedScheduleObject ps, string observationTaskName)
    {
        return ps.ObservationTaskResults
            .Where(s => string.Equals(s.TaskName, observationTaskName))
            .SingleOrDefault();
    }

    public static List<Footprint> GetFootprints(this PlannedScheduleObject ps)
    {
        return ps.GetObservationTaskResults()
            .Select(s => s.ToFootprint())
            .ToList();
    }

    public static Footprint? GetFootprint(this PlannedScheduleObject ps, string observationTaskName)
    {
        return ps.GetObservationTaskResult(observationTaskName)?.ToFootprint();
    }

    public static Dictionary<int, List<List<(double lonDeg, double latDeg)>>> BuildObservableIntervals(this PlannedScheduleObject ps, string satelliteName)
    {
        var sat = ps.Satellites.Where(s => string.Equals(s.Name, satelliteName)).Single();

        var nodes = sat.NodesOnDay();
        var name = sat.Name;

        var dict = new Dictionary<int, List<List<(double lonDeg, double latDeg)>>>();

        for (int i = 0; i < nodes; i++)
        {
            var observationTasks = ps.ObservationTaskResults
                 .Where(s => s.Node == i)
                 .Where(s => Equals(s.SatelliteName, name));

            var orbit = sat.ToOrbit();
            var epoch = sat.Epoch;
            var period = sat.Period;

            dict.Add(i, []);

            foreach (var taskResult in observationTasks)
            {
                var begin = taskResult.Interval.Begin;
                var duration = taskResult.Interval.Duration;

                var track = FlatSpaceFactory.CreateGroundTrack(orbit);
                var begin0 = begin.AddSeconds(-period * i);

                var t0 = (begin0 - epoch).TotalSeconds;
                var t1 = t0 + duration;

                track.CalculateTrackOnTimeInterval(t0, t1, 2);

                var res = track.GetTrack(i, duration);

                var trackLine = LonSplitters.Default.Split(res);

                dict[i].AddRange(trackLine);
            }
        }

        return dict;
    }

    public static List<List<(double lonDeg, double latDeg)>> BuildObservableIntervals(this PlannedScheduleObject ps, string satelliteName, int node)
    {
        var sat = ps.Satellites.Where(s => string.Equals(s.Name, satelliteName)).Single();

        var nodes = sat.NodesOnDay();
        var name = sat.Name;

        var list = new List<List<(double lonDeg, double latDeg)>>();

        var observationTasks = ps.ObservationTaskResults
             .Where(s => s.Node == node)
             .Where(s => Equals(s.SatelliteName, name));

        var orbit = sat.ToOrbit();
        var epoch = sat.Epoch;
        var period = sat.Period;

        foreach (var taskResult in observationTasks)
        {
            var begin = taskResult.Interval.Begin;
            var duration = taskResult.Interval.Duration;

            var track = FlatSpaceFactory.CreateGroundTrack(orbit);
            var begin0 = begin.AddSeconds(-period * node);

            var t0 = (begin0 - epoch).TotalSeconds;
            var t1 = t0 + duration;

            track.CalculateTrackOnTimeInterval(t0, t1, 2);

            var res = track.GetTrack(node, duration);

            var trackLine = LonSplitters.Default.Split(res);

            list.AddRange(trackLine);
        }

        return list;
    }

    public static List<(double lonDeg, double latDeg, string name, double u)> BuildObservableMarkers(this PlannedScheduleObject ps, string satelliteName, int node)
    {
        var sat = ps.Satellites.Where(s => string.Equals(s.Name, satelliteName)).Single();

        var nodes = sat.NodesOnDay();

        var list = new List<(double lonDeg, double latDeg, string name, double u)>();

        var observationTasks = ps.ObservationTaskResults
             .Where(s => s.Node == node)
             .Where(s => Equals(s.SatelliteName, satelliteName));

        var orbit = sat.ToOrbit();
        var epoch = sat.Epoch;
        var period = sat.Period;

        foreach (var taskResult in observationTasks)
        {
            var name = taskResult.Name;
            var begin = taskResult.Interval.Begin;
            var duration = taskResult.Interval.Duration;

            var track = FlatSpaceFactory.CreateGroundTrack(orbit);
            var begin0 = begin.AddSeconds(-period * node);

            var t0 = (begin0 - epoch).TotalSeconds;
            var t = t0 + duration / 2.0;

            var (lonDeg, latDeg, u) = track.GetPointOnTrack(t);

            var (lonDeg2, latDeg2) = LonSplitters.Default.Split((lonDeg, latDeg));

            list.Add((lonDeg2, latDeg2, name, u));
        }

        return list;
    }
}
