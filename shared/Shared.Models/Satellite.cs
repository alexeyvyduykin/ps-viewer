namespace Shared.Models;

public class Satellite
{
    public required string Name { get; init; }

    public required double Semiaxis { get; init; }

    public required double Eccentricity { get; init; }

    public required double InclinationDeg { get; init; }

    public required double ArgumentOfPerigeeDeg { get; init; }

    public required double LongitudeAscendingNodeDeg { get; init; }

    public required double RightAscensionAscendingNodeDeg { get; init; }

    public required double Period { get; init; }

    public required DateTime Epoch { get; init; }

    public required double LookAngleDeg { get; init; }

    public required double RadarAngleDeg { get; init; }
}