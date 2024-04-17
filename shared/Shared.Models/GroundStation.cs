using NetTopologySuite.Geometries;

namespace Shared.Models;

public class GroundStation
{
    public required string Name { get; init; }

    public required Point Center { get; init; }

    public required double[] Angles { get; init; }
}
