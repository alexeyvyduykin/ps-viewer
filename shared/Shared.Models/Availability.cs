namespace Shared.Models;

public class Availability
{
    public required string TaskName { get; init; }

    public required string SatelliteName { get; init; }

    public required List<Interval> Windows { get; init; }
}
