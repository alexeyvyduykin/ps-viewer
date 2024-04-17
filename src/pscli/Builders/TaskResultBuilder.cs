using FlatSpace.Methods;
using NetTopologySuite.Geometries;
using Shared.Models;

namespace pscli.Builders;

public static class TaskResultBuilder
{
    private static readonly Random _random = new();

    public static async Task<List<ObservationTaskResult>> CreateObservationsAsync(IList<ObservationTask> tasks, IList<Footprint> footprints)
        => await Task.Run(() => CreateObservations(tasks, footprints));

    public static async Task<List<CommunicationTaskResult>> CreateCommunicationsAsync(List<Availability> availabilities)
        => await Task.Run(() => CreateCommunications(availabilities));

    public static List<ObservationTaskResult> CreateObservations(IList<ObservationTask> tasks, IList<Satellite> satellites, IList<(string satName, IList<TimeWindowResult> windows)> windows)
    {
        int durationMin = 30;// 10;
        int durationMax = 45;// 30;

        var random = new Random();

        var listTaskResults = new List<ObservationTaskResult>();

        int index = 0;

        foreach (var task in tasks)
        {
            var taskName = task.Name;
            var gtName = task.GroundTargetName;

            var res = windows.Select(s => (s.satName, windows: s.windows.Where(t => Equals(t.Name, gtName)).ToList())).ToList();

            var i = random.Next(0, res.Count);

            if (res[i].windows.Count == 0)
            {
                continue;
            }

            var j = random.Next(0, res[i].windows.Count);

            var satName = res[i].satName;

            var sat = satellites.Where(s => Equals(s.Name, satName)).Single();

            var period = sat.Period;

            var epoch = sat.Epoch;

            var selectRes = res[i].windows[j];

            var duration = random.Next(durationMin, durationMax + 1);
            var timeVisible = selectRes.NadirTime;
            var begin = timeVisible - duration / 2.0;

            var lonDeg = selectRes.Lon;
            var latDeg = selectRes.Lat;

            var node = selectRes.Node;
            var direction = (selectRes.IsLeftSwath == true) ? SwathDirection.Left : SwathDirection.Right;

            var taskResult = new ObservationTaskResult()
            {
                Name = $"Observation{++index:0000}",
                TargetName = gtName,
                TaskName = taskName,
                Node = node,
                Direction = direction,
                SatelliteName = satName,
                Interval = new Interval
                {
                    Begin = epoch.AddSeconds(begin + node * period),
                    Duration = duration
                },
                Geometry = new FootprintGeometry
                {
                    Center = new Point(lonDeg, latDeg),
                    Border = FootprintBuilder.CreateFootprintBorder(lonDeg, latDeg)
                },
                Transition = null
            };

            listTaskResults.Add(taskResult);
        }

        return listTaskResults;
    }

    public static List<ObservationTaskResult> CreateObservations(IList<ObservationTask> tasks, IList<Footprint> footprints)
    {
        return tasks
            .SelectMany(s =>
                footprints
                    .Where(f => Equals(f.TargetName, s.GroundTargetName))
                    .Select(f => CreateObservation(s.Name, f)))
            .ToList();
    }

    // TODO: null available for footprint -> targetName and footprint -> border ???
    public static ObservationTaskResult CreateObservation(string taskName, Footprint footprint)
    {
        var begin = footprint.Begin;
        var duration = footprint.Duration;

        var taskResult = new ObservationTaskResult()
        {
            Name = footprint.Name,
            TargetName = footprint.TargetName ?? "ErrorName",
            TaskName = taskName,
            SatelliteName = footprint.SatelliteName ?? "SatelliteDefault",
            Interval = new Interval { Begin = begin, Duration = duration },
            Node = footprint.Node,
            Direction = footprint.Direction,
            Geometry = new FootprintGeometry { Center = footprint.Center, Border = footprint.Border ?? new LineString([]) },
            Transition = null
        };

        return taskResult;
    }

    // TODO: CommunicationTaskResult must have Node property -> using dummy value
    public static List<CommunicationTaskResult> CreateCommunications(List<Availability> availabilities)
    {
        var taskResults = new List<CommunicationTaskResult>();

        foreach (var item in availabilities)
        {
            var count = item.Windows.Count;

            var indexUplink = _random.Next(0, count);
            var indexDownlink = _random.Next(0, count);
            if (indexUplink == indexDownlink)
            {
                indexDownlink++;
            }

            int index = 0;

            for (int i = 0; i < count; i++)
            {
                var ival = item.Windows[i];

                if (i == indexUplink)
                {
                    var begin = ival.Begin;
                    var duration = ival.Duration;
                    var newDuration = _random.Next((int)(duration / 3), (int)(duration / 2) + 1);
                    var start = _random.Next(0, (int)(duration - newDuration));

                    var taskResult = new CommunicationTaskResult()
                    {
                        Node = 1, // Dummy
                        Name = $"Communication{++index:0000}",
                        TaskName = item.TaskName,
                        SatelliteName = item.SatelliteName,
                        Interval = new() { Begin = begin.AddSeconds(start), Duration = newDuration },
                        Type = CommunicationType.Uplink,
                    };

                    taskResults.Add(taskResult);
                }

                if (i == indexDownlink)
                {
                    var begin = ival.Begin;
                    var duration = ival.Duration;
                    var newDuration = _random.Next((int)(duration / 3), (int)(duration / 2) + 1);
                    var start = _random.Next(0, (int)(duration - newDuration));

                    var taskResult = new CommunicationTaskResult()
                    {
                        Node = 1, // Dummy
                        Name = $"Communication{++index:0000}",
                        TaskName = item.TaskName,
                        SatelliteName = item.SatelliteName,
                        Interval = new() { Begin = begin.AddSeconds(start), Duration = newDuration },
                        Type = CommunicationType.Downlink,
                    };

                    taskResults.Add(taskResult);
                }
            }
        }

        return taskResults;
    }
}
