using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Shared.Models;
using webapi.Extensions;
using webapi.Utils;

namespace webapi.Services;

public partial class FeatureService
{
    private FeatureCollection CreatePreviewFootprint(string observationTaskName, bool isLonLat = true)
    {
        var fc = new FeatureCollection();

        var res = _dataService.GetPlannedScheduleObject();

        if (res != null)
        {
            var ps = _mapper.Map<PlannedScheduleObject>(res);

            var footprint = ps.GetFootprint(observationTaskName);

            if (footprint != null)
            {
                var feature = CreateFootprint(footprint, isLonLat);
                return feature != null ? (new[] { feature }).ToFeatureCollection() : [];
            }
        }

        return fc;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Пометьте члены как статические", Justification = "<Ожидание>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Удалите неиспользуемые закрытые члены", Justification = "<Ожидание>")]
    private Polygon CreateArrow(double x11, double y11, double x22, double y22)
    {
        var (x1, y1) = SphericalMercator.FromLonLat(x11, y11);
        var (x2, y2) = SphericalMercator.FromLonLat(x22, y22);

        // Backward direction vector
        var (dx, dy) = (x1 - x2, y1 - y2);

        // Length of it:
        var norm = Math.Sqrt(dx * dx + dy * dy);

        // Normalize it: uD =

        var (udx, udy) = (dx / norm, dy / norm);

        // To form "wings" of arrow, rotate uD by needed angle. For example, I use angle Pi / 6 with Cos(Pi/ 6) = Sqrt(3) / 2 and Sin(Pi/ 6) = 1 / 2

        var angle = Math.PI / 3;// Math.PI / 6;
        var cosa = Math.Cos(angle);
        var sina = Math.Sin(angle);

        var ax = udx * cosa - udy * sina;
        var ay = udx * sina + udy * cosa;
        var bx = udx * cosa + udy * sina;
        var by = -udx * sina + udy * cosa;

        // Points for head with wing length L = 20:
        double len = 50000;// 0.5;
        var (a, b) = (x1 + len * ax, y1 + len * ay);
        var (c, d) = (x1 + len * bx, y1 + len * by);

        var list = new (double x, double y)[]
        {
            (x2, y2),
            (a, b),
            (c, d),
            (x2, y2),
        };

        var coordinates = list
            // .Select(s => SphericalMercator.FromLonLat(s.Item1, s.Item2))
            .Select(s => new Coordinate(s.x, s.y))
            .ToArray();

        var gf = new GeometryFactory();
        var poly = gf.CreatePolygon(coordinates);

        return poly!;
    }
}
