﻿using FlatSpace.Extensions;
using FlatSpace.Methods;
using FlatSpace.Utils;
using pscli.Extensions;
using Shared.Models;

namespace pscli.Builders;

public static class TimeWindowBuilder
{
    public static IList<TimeWindowResult> Create(Satellite satellite, IList<GroundTarget> gts)
    {
        var targets = gts.Select(s => ToTarget(s)).ToList();

        var orbit = satellite.ToOrbit();
        var nodes = orbit.NodesOnDay();
        var gam1Deg = satellite.LookAngleDeg - satellite.RadarAngleDeg / 2.0;
        var gam2Deg = satellite.LookAngleDeg + satellite.RadarAngleDeg / 2.0;

        return SpaceMethods.ObservationGroundTargets(orbit, 0, nodes - 1, gam1Deg, gam2Deg, targets);
    }

    public static IList<(string satName, IList<TimeWindowResult> windows)> Create(IList<Satellite> satellites, IList<GroundTarget> gts)
    {
        var targets = gts.Select(s => ToTarget(s)).ToList();

        var list = new List<(string satName, IList<TimeWindowResult> windows)>();

        foreach (var sat in satellites)
        {
            var orbit = sat.ToOrbit();
            var nodes = orbit.NodesOnDay();
            var gam1Deg = sat.LookAngleDeg - sat.RadarAngleDeg / 2.0;
            var gam2Deg = sat.LookAngleDeg + sat.RadarAngleDeg / 2.0;

            var res = SpaceMethods.ObservationGroundTargets(orbit, 0, nodes - 1, gam1Deg, gam2Deg, targets);

            list.Add((sat.Name!, res));
        }

        return list;
    }

    // TODO: gt.Center is null ???
    private static (double lonDeg, double latDeg, string name) ToTarget(GroundTarget gt)
    {
        var coord = gt.Center ?? new(0, 0);
        var name = gt.Name ?? "";
        return (coord.X, coord.Y, name);
    }
}
