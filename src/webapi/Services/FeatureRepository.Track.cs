using FlatSpace;
using FlatSpace.Extensions;
using FlatSpace.Utils;
using NetTopologySuite.Features;
using Shared.Models;
using webapi.Extensions;
using webapi.Geometries;
using static FlatSpace.Extensions.OrbitExtensions;

namespace webapi.Services;

public partial class FeatureRepository
{
    private FeatureCollection CreateTrack(string satelliteName, int node, bool isLonLat = true)
    {
        var ps = _dataService.GetPlannedScheduleObject();

        if (ps != null)
        {
            var sat = ps.Satellites.Where(s => string.Equals(s.Name, satelliteName)).Single();

            var tracks = FeatureBuilder.CreateTracks(sat.BuildTracks(), isLonLat);

            foreach (var item in tracks[node])
            {
                item.Attributes = new AttributesTable
                {
                    { "Satellite", satelliteName }
                };
            }

            // TODO: Warning: node change
            return tracks[node].ToFeatureCollection();
        }

        return [];
    }

    private FeatureCollection CreateTrackIntervals(string satelliteName, int node, bool isLonLat = true)
    {
        var res = _dataService.GetPlannedScheduleObject();

        if (res != null)
        {
            var ps = _mapper.Map<PlannedScheduleObject>(res);

            var ivals = ps.BuildObservableIntervals(satelliteName, node);

            var intervalTrack = FeatureBuilder.CreateTrack(ivals, isLonLat);

            intervalTrack.Attributes = new AttributesTable
            {
                { "Satellite", satelliteName }
            };

            return new[] { intervalTrack }.ToFeatureCollection();
        }

        return [];
    }

    private FeatureCollection CreateTrackSegment(string observationTaskName, bool isLonLat = true)
    {
        var dict = new Dictionary<int, FeatureCollection>();

        var res = _dataService.GetPlannedScheduleObject();

        if (res != null)
        {
            var ps = _mapper.Map<PlannedScheduleObject>(res);

            var observationTask = ps.GetObservationTaskResult(observationTaskName);
            var sat = ps.Satellites.Where(s => string.Equals(s.Name, observationTask?.SatelliteName)).SingleOrDefault();

            if (sat != null && observationTask != null)
            {
                var satelliteName = sat.Name;
                var node = observationTask.Node;
                var begin = observationTask.Interval.Begin;
                var duration = observationTask.Interval.Duration;
                var epoch = sat.Epoch;
                var period = sat.Period;
                var begin0 = begin.AddSeconds(-period * (node - 1));
                var t0 = (begin0 - epoch).TotalSeconds;
                var t1 = t0 + duration;

                var orbit = sat.ToOrbit();

                var track = FlatSpaceFactory.CreateGroundTrack(orbit);
                track.CalculateTrackOnTimeInterval(t0 - 60, t1 + 60, 10);
                var res2 = track.GetTrack(node, duration + 2 * 60);
                var baseTrackLines = LonSplitters.Default.Split(res2);

                var baseTrackFeatures = baseTrackLines.Select(s => FeatureBuilder.CreateTrack(s, isLonLat)).ToList();

                foreach (var item in baseTrackFeatures)
                {
                    item.Attributes = new AttributesTable
                    {
                        { "Satellite", satelliteName }
                    };
                }

                return baseTrackFeatures.ToFeatureCollection();
            }
        }

        return [];
    }

    private FeatureCollection CreateIntervalTrackSegment(string observationTaskName, bool isLonLat = true)
    {
        var dict = new Dictionary<int, FeatureCollection>();

        var res = _dataService.GetPlannedScheduleObject();

        if (res != null)
        {
            var ps = _mapper.Map<PlannedScheduleObject>(res);

            var observationTask = ps.GetObservationTaskResult(observationTaskName);
            var sat = ps.Satellites.Where(s => string.Equals(s.Name, observationTask?.SatelliteName)).SingleOrDefault();

            if (sat != null && observationTask != null)
            {
                var satelliteName = sat.Name;
                var node = observationTask.Node;
                var begin = observationTask.Interval.Begin;
                var duration = observationTask.Interval.Duration;
                var epoch = sat.Epoch;
                var period = sat.Period;
                var begin0 = begin.AddSeconds(-period * (node - 1));
                var t0 = (begin0 - epoch).TotalSeconds;
                var t1 = t0 + duration;

                var orbit = sat.ToOrbit();

                var track = FlatSpaceFactory.CreateGroundTrack(orbit);
                track.CalculateTrackOnTimeInterval(t0, t1, 2);
                var res1 = track.GetTrack(node, duration);
                var trackLines = LonSplitters.Default.Split(res1);

                var trackFeatures = trackLines.Select(s => FeatureBuilder.CreateTrack(s, isLonLat)).ToList();

                foreach (var item in trackFeatures)
                {
                    item.Attributes = new AttributesTable
                    {
                        { "Satellite", satelliteName }
                    };
                }

                return trackFeatures.ToFeatureCollection();
            }
        }

        return [];
    }

    private FeatureCollection CreateTaskResultMarkers(string satelliteName, int node, bool isLonLat = true)
    {
        var res = _dataService.GetPlannedScheduleObject();

        if (res != null)
        {
            var ps = _mapper.Map<PlannedScheduleObject>(res);

            var points0 = ps.BuildObservableMarkers(satelliteName, node);

            //-------

            var sat = ps.Satellites.Where(s => string.Equals(s.Name, satelliteName)).Single();

            var orbit = sat.ToOrbit();

            var track = FlatSpaceFactory.CreateGroundTrack(orbit);

            var offset = (track.NodeOffsetDeg + track.EarthRotateOffsetDeg) * (node - 1);

            var points = points0.Select(s => (lonDeg: LonSplitters.Default.Split(s.lonDeg + offset), s.latDeg, s.name, s.u)).ToList();


            //-------

            var markers = FeatureBuilder.CreateMarkers(points.Select(s => (s.lonDeg, s.latDeg)).ToList(), isLonLat);

            for (var i = 0; i < points.Count; i++)
            {
                markers[i].Attributes = new AttributesTable
                {
                    { "Name", points[i].name },
                    { "Satellite", satelliteName }
                };
            }

            return markers.ToFeatureCollection();
        }

        return [];
    }

    private FeatureCollection CreateMarkerTrack(string satelliteName, int node, bool isLonLat = true)
    {
        var res = _dataService.GetPlannedScheduleObject();

        if (res != null)
        {
            var ps = _mapper.Map<PlannedScheduleObject>(res);

            var points = ps.BuildObservableMarkers(satelliteName, node);

            var us = points.Select(s => s.u).ToArray();

            var sat = ps.Satellites.Where(s => string.Equals(s.Name, satelliteName)).Single();

            var orbit = sat.ToOrbit();

            var track = FlatSpaceFactory.CreateGroundTrack(orbit);

            track.CalculateTrackWithLogStep(100, us);

            var dict = new int[] { node }.ToDictionary(s => s, s => LonSplitters.Default.Split(track.GetTrack(s)));

            var tracks = FeatureBuilder.CreateTracks(dict, isLonLat);

            foreach (var item in tracks[node])
            {
                item.Attributes = new AttributesTable
                {
                    { "Satellite", satelliteName }
                };
            }

            // TODO: Warning: node change
            return tracks[node].ToFeatureCollection();
        }

        return [];
    }

}
