using NetTopologySuite.Geometries;

namespace Shared.Models;

public enum SwathDirection
{
    Left,
    Right
}

public class FootprintGeometry
{
    public required Point Center { get; init; }

    public required LineString Border { get; init; }
}

public class ObservationTaskResult : BaseTaskResult
{
    public required string TargetName { get; init; }

    public required FootprintGeometry Geometry { get; init; }

    public required SwathDirection Direction { get; init; }
}
