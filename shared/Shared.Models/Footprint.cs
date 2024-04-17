using NetTopologySuite.Geometries;

namespace Shared.Models;

public class Footprint
{
    public required string Name { get; init; }

    public required string SatelliteName { get; init; }

    public string? TargetName { get; set; }

    public required Point Center { get; init; }

    public LineString? Border { get; set; }

    public required DateTime Begin { get; init; }

    public required double Duration { get; init; }

    public required int Node { get; init; }

    public required SwathDirection Direction { get; init; }
}