using NetTopologySuite.Geometries;

namespace Shared.Models;

public enum GroundTargetType
{
    Point,
    Route,
    Area
}

public class GroundTarget
{
    public required string Name { get; init; }

    public required GroundTargetType Type { get; init; }

    public Point? Center { get; init; }

    public Geometry? Points { get; init; }
}
