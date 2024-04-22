using FlatSpace.Extensions;
using FlatSpace.Utils;
using FootprintViewerWebApi.Extensions;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using webapi.Extensions;
using webapi.Geometries;
using webapi.Utils;

namespace webapi.Services;

using Direction = Shared.Models.SwathDirection;

public partial class FeatureService
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Удалите неиспользуемые закрытые члены", Justification = "<Ожидание>")]
    private NodeFeatureMap CreateSwaths(string satelliteName, bool isLonLat = true)
    {
        var dict = new Dictionary<int, FeatureMap>();

        var ps = _dataService.GetPlannedScheduleObject();

        if (ps != null)
        {
            var sat = ps.Satellites.Where(s => string.Equals(s.Name, satelliteName)).Single();

            var leftDict = sat.BuildSwaths(Direction.Left);
            var rightDict = sat.BuildSwaths(Direction.Right);

            var leftFeatures = FeatureBuilder.CreateSwaths(leftDict, isLonLat);
            var rightFeatures = FeatureBuilder.CreateSwaths(rightDict, isLonLat);

            foreach (var key in leftFeatures.Keys)
            {
                foreach (var item in leftFeatures[key])
                {
                    item.Attributes = new AttributesTable
                    {
                        { "Satellite", satelliteName }
                    };
                }

                foreach (var item in rightFeatures[key])
                {
                    item.Attributes = new AttributesTable
                    {
                        { "Satellite", satelliteName }
                    };
                }

                var left = leftFeatures[key].ToFeatureCollection();
                var right = rightFeatures[key].ToFeatureCollection();

                dict.Add(key, new()
                {
                    { "Left", left },
                    { "Right", right }
                });
            }
        }

        return dict;
    }

    private FeatureCollection CreateSwath(string satelliteName, int node, Direction direction, bool isLonLat = true)
    {
        var ps = _dataService.GetPlannedScheduleObject();

        if (ps != null)
        {
            var sat = ps.Satellites.Where(s => string.Equals(s.Name, satelliteName)).Single();
            var arr = sat.BuildSwath(node, direction);
            var features = FeatureBuilder.CreateSwath(arr, isLonLat);

            foreach (var item in features)
            {
                item.Attributes = new AttributesTable
                {
                    { "Satellite", satelliteName }
                };
            }

            return features.ToFeatureCollection();
        }

        return [];
    }

    private FeatureCollection CreateSwathSegment(string observationTaskName, bool isLonLat = true)
    {
        var ps = _dataService.GetPlannedScheduleObject();

        if (ps != null)
        {
            var observationTask = ps.GetObservationTaskResult(observationTaskName);
            var sat = ps.Satellites.Where(s => string.Equals(s.Name, observationTask?.SatelliteName)).SingleOrDefault();

            if (sat != null && observationTask != null)
            {
                var satelliteName = sat!.Name;
                var node = observationTask.Node;
                var begin = observationTask.Interval.Begin;
                var duration = observationTask.Interval.Duration;
                var direction = observationTask.Direction.ToString();
                var epoch = sat.Epoch;
                var period = sat.Period;
                var begin0 = begin.AddSeconds(-period * (node - 1));
                var t0 = (begin0 - epoch).TotalSeconds;
                var t1 = t0 + duration;

                var radarAngle = sat.RadarAngleDeg;
                var lookAngle = sat.LookAngleDeg;
                var orbit = sat.ToOrbit();

                var (baseNear, baseFar) = orbit.BuildSwaths2(node, t0 - 60, t1 + 60, 10, lookAngle, radarAngle, Enum.Parse<FlatSpace.Models.SwathDirection>(direction));
                var baseNear2 = LonSplitters.Default.Split(baseNear);
                var baseFar2 = LonSplitters.Default.Split(baseFar);

                var baseNearSwathFeature = FeatureBuilder.CreateTrack(baseNear2, isLonLat);

                var baseFarSwathFeature = FeatureBuilder.CreateTrack(baseFar2, isLonLat);

                var (near, far) = orbit.BuildSwaths2(node, t0, t1, 2, lookAngle, radarAngle, Enum.Parse<FlatSpace.Models.SwathDirection>(direction));

                var firstNearPoint = near.First();
                var lastNearPoint = near.Last();
                var lastFarPoint = far.Last();
                var firstFarPoint = far.First();

                var res = LonSplitters.Default.SplitArea([firstNearPoint, lastNearPoint, lastFarPoint, firstFarPoint]);

                var geoms = res
                    .Select(s => s.Select(s => (isLonLat) ? (s.lonDeg, s.latDeg) : SphericalMercator.FromLonLat(s.lonDeg, s.latDeg)).ToList())
                    .Select(s => s.ToClosedCoordinates())
                    .Select(s => new LinearRing(s))
                    .Select(s => (Geometry)new Polygon(s));

                var area = new GeometryFactory()
                    .BuildGeometry(geoms)
                    .ToFeature();

                baseNearSwathFeature.Attributes = new AttributesTable
                {
                    { "Feature", "SegmentSwath" },
                    { "Satellite", satelliteName }
                };

                baseFarSwathFeature.Attributes = new AttributesTable
                {
                    { "Feature", "SegmentSwath" },
                    { "Satellite", satelliteName }
                };

                area.Attributes = new AttributesTable
                {
                    { "Feature", "SegmentAreaSwath" },
                    { "Satellite", satelliteName }
                };

                return
                [
                    baseNearSwathFeature,
                    baseFarSwathFeature,
                    area
                ];
            }
        }

        return [];
    }
}
