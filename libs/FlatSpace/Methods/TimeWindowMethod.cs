using FlatSpace.Extensions;
using FlatSpace.Models;
using FlatSpace.Utils;

namespace FlatSpace.Methods;

internal static class TimeWindowMethod
{
    public static IList<TimeWindowResult> BuildOnNode(
        Orbit orbit,
        int node,
        double angle1Deg,
        double angle2Deg,
        List<(double lon, double lat, string name)> targets
    )
    {
        return BuildOnNodes(orbit, node, node, angle1Deg, angle2Deg, targets);
    }

    public static IList<TimeWindowResult> BuildOnNodes(
        Orbit orbit,
        int fromNode,
        int toNode,
        double angle1Deg,
        double angle2Deg,
        List<(double lon, double lat, string name)> targets
    )
    {
        var (centralAngleMinDeg, centralAngleMaxDeg) = orbit.GetValidRange(angle1Deg, angle2Deg);

        var list = new List<TimeWindowResult>();

        var dt = 1.0;
        var beginSkip = (int)(10.0 / dt);
        var endSkip = (int)(10.0 / dt);

        var track = new GroundTrack(orbit);

        var factor = new FactorShiftTrack(orbit, angle1Deg, angle2Deg, SwathDirection.Left);

        var nearTrackLeft = new GroundTrack(orbit, factor, angle1Deg, TrackDirection.Left);
        var nearTrackRight = new GroundTrack(orbit, factor, angle1Deg, TrackDirection.Right);

        track.CalculateTrack(dt);
        nearTrackLeft.CalculateTrack(dt);
        nearTrackRight.CalculateTrack(dt);

        foreach (var (lonTargetDeg, latTargetDeg, name) in targets)
        {
            (double lon, double lat) nearLeftSaveDeg = (0.0, 0.0);
            (double lon, double lat) nearRightSaveDeg = (0.0, 0.0);
            double lonSaveDeg = 0.0;
            double latSaveDeg = 0.0;
            double tVis = double.NaN;
            double uVis = double.NaN;
            int nodeVis = int.MinValue;

            double minCenterlAngleDeg = double.MaxValue;

            bool isVisible = false;
            bool isTemp = false;
            bool isMiss = false;
            int lastCounter = int.MinValue;
            int counter = 0;
            double uBeginVisible = double.MinValue;
            double tBeginVisible = double.MinValue;
            double uEndVisible = double.MaxValue;
            double tEndVisible = double.MaxValue;
            int nodeBegin = int.MinValue;
            int nodeEnd = int.MinValue;

            for (int node = fromNode; node <= toNode; node++)
            {
                var fullTrack = track
                    .GetFullTrack(node, LonConverters.Default)
                    .Select((s, index) => (s, index));

                if (node == fromNode)
                {
                    fullTrack = fullTrack.Skip(beginSkip).ToList();
                }

                if (node == toNode)
                {
                    fullTrack = fullTrack.SkipLast(endSkip).ToList();
                }

                foreach (var ((lonDeg, latDeg, u, t), i) in fullTrack)
                {
                    double centralAngleDeg = SpaceMethods.CreateCentralAngle(
                        (lonDeg, latDeg),
                        (lonTargetDeg, latTargetDeg)
                    );

                    if (centralAngleDeg < centralAngleMinDeg)
                    {
                        // для исключение точек попавших в зону между полосами
                        isMiss = true;
                    }

                    isVisible = false;

                    if (
                        centralAngleDeg <= centralAngleMaxDeg
                        && centralAngleDeg >= centralAngleMinDeg
                    )
                    {
                        isVisible = true;
                    }

                    if (isVisible == true)
                    {
                        counter++;

                        isTemp = true;
                        if (centralAngleDeg < minCenterlAngleDeg)
                        {
                            lastCounter = counter;
                            minCenterlAngleDeg = centralAngleDeg;

                            nearLeftSaveDeg = nearTrackLeft.GetTrackOfIndex(
                                i,
                                node,
                                LonConverters.Default
                            );
                            nearRightSaveDeg = nearTrackRight.GetTrackOfIndex(
                                i,
                                node,
                                LonConverters.Default
                            );
                            tVis = t;
                            uVis = u;
                            nodeVis = node;
                            lonSaveDeg = lonDeg;
                            latSaveDeg = latDeg;
                        }

                        if (counter == 1)
                        {
                            tBeginVisible = t;
                            uBeginVisible = u;
                            nodeBegin = node;
                        }

                        tEndVisible = t;
                        uEndVisible = u;
                        nodeEnd = node;
                    }

                    if (isVisible == false && isTemp == true)
                    {
                        if (lastCounter != 1 && isMiss == false)
                        {
                            var nearLeftAngle = SpaceMethods.CreateCentralAngle(
                                nearLeftSaveDeg,
                                (lonTargetDeg, latTargetDeg)
                            );
                            var nearRightAngle = SpaceMethods.CreateCentralAngle(
                                nearRightSaveDeg,
                                (lonTargetDeg, latTargetDeg)
                            );

                            var isLeftSwath = (nearLeftAngle < nearRightAngle);

                            var track33 = new GroundTrack(orbit);
                            track33.CalculateTrack(uBeginVisible, uEndVisible);

                            var interval = track33.GetTrack(node, LonConverters.Default);

                            var direction = new List<(double, double)>()
                            {
                                (lonSaveDeg, latSaveDeg),
                                (lonTargetDeg, latTargetDeg)
                            };

                            list.Add(
                                new()
                                {
                                    Name = name,
                                    BeginTime = tBeginVisible,
                                    EndTime = tEndVisible,
                                    BeginU = uBeginVisible,
                                    EndU = uEndVisible,
                                    Lat = latTargetDeg,
                                    Lon = lonTargetDeg,
                                    Node = nodeVis,
                                    BeginNode = nodeBegin,
                                    EndNode = nodeEnd,
                                    IsLeftSwath = isLeftSwath,
                                    NadirTime = tVis,
                                    NadirU = uVis,
                                    MinAngle = minCenterlAngleDeg,
                                    Interval = LonSplitters.Default.Split(interval),
                                    Direction = LonSplitters.Default.Split(direction)
                                }
                            );
                        }

                        isTemp = false;
                        isMiss = false;
                        counter = 0;
                        lastCounter = int.MinValue;
                        minCenterlAngleDeg = double.MaxValue;
                    }
                }
            }

            if (isVisible == true && isTemp == true)
            {
                if (counter != lastCounter && isMiss == false)
                {
                    var nearLeftAngle = SpaceMethods.CreateCentralAngle(
                        nearLeftSaveDeg,
                        (lonTargetDeg, latTargetDeg)
                    );
                    var nearRightAngle = SpaceMethods.CreateCentralAngle(
                        nearRightSaveDeg,
                        (lonTargetDeg, latTargetDeg)
                    );

                    var isLeftSwath = (nearLeftAngle < nearRightAngle);

                    var track33 = new GroundTrack(orbit);
                    track33.CalculateTrack(uBeginVisible, uEndVisible);

                    var interval = track33.GetTrack(toNode, LonConverters.Default);

                    var direction = new List<(double, double)>()
                    {
                        (lonSaveDeg, latSaveDeg),
                        (lonTargetDeg, latTargetDeg)
                    };

                    list.Add(
                        new()
                        {
                            Name = name,
                            BeginTime = tBeginVisible,
                            EndTime = tEndVisible,
                            BeginU = uBeginVisible,
                            EndU = uEndVisible,
                            Lat = latTargetDeg,
                            Lon = lonTargetDeg,
                            Node = nodeVis,
                            BeginNode = nodeBegin,
                            EndNode = nodeEnd,
                            IsLeftSwath = isLeftSwath,
                            NadirTime = tVis,
                            NadirU = uVis,
                            MinAngle = minCenterlAngleDeg,
                            Interval = LonSplitters.Default.Split(interval),
                            Direction = LonSplitters.Default.Split(direction)
                        }
                    );
                }
            }
        }

        return list;
    }
}
