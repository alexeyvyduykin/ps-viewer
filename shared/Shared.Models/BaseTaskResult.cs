namespace Shared.Models;

public abstract class BaseTaskResult
{
    public required string Name { get; init; }

    public required string TaskName { get; init; }

    public required string SatelliteName { get; init; }

    public required Interval Interval { get; init; }

    public required int Node { get; init; }

    public Interval? Transition { get; init; }
}