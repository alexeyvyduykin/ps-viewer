namespace Shared.Models;

public class PlannedScheduleObject
{
    public required string Name { get; init; }

    public required DateTime DateTime { get; init; }

    public required List<Satellite> Satellites { get; init; }

    public required List<GroundTarget> GroundTargets { get; init; }

    public required List<GroundStation> GroundStations { get; init; }

    public required List<ObservationTask> ObservationTasks { get; init; }

    public required List<CommunicationTask> CommunicationTasks { get; init; }

    public required List<Availability> ObservationWindows { get; init; }

    public required List<Availability> CommunicationWindows { get; init; }

    public required List<ObservationTaskResult> ObservationTaskResults { get; init; }

    public required List<CommunicationTaskResult> CommunicationTaskResults { get; init; }
}