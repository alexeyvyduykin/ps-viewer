using Shared.Models;

namespace pscli.Builders;

public static class PlannedScheduleBuilder
{
    public static async Task<PlannedScheduleObject> CreateAsync(string name, IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations, IList<Footprint> footprints)
        => await Task.Run(() => Create(name, satellites, groundTargets, groundStations, footprints));

    public static async Task<PlannedScheduleObject> CreateAsync(string name, IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations)
        => await Task.Run(() => Create(name, satellites, groundTargets, groundStations));

    public static PlannedScheduleObject Create(string name, IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations, IList<Footprint> footprints)
    {
        var observations = TaskBuilder.CreateObservationTask(groundTargets);
        var communications = TaskBuilder.CreateCommunicationTask(groundStations);

        var observationTasks = TaskResultBuilder.CreateObservations(observations, footprints);
        var observationWindows = TaskAvailabilityBuilder.CreateObservations(footprints, satellites, observations);
        var communicationWindows = TaskAvailabilityBuilder.CreateCommunications(satellites, groundStations, footprints, communications);
        var communicationTasks = TaskResultBuilder.CreateCommunications(communicationWindows);

        return new PlannedScheduleObject()
        {
            Name = name,
            DateTime = DateTime.Now,
            Satellites = [.. satellites],
            GroundTargets = [.. groundTargets],
            GroundStations = [.. groundStations],
            ObservationTasks = [.. observations],
            CommunicationTasks = [.. communications],
            ObservationWindows = [.. observationWindows],
            CommunicationWindows = [.. communicationWindows],
            ObservationTaskResults = [.. observationTasks],
            CommunicationTaskResults = [.. communicationTasks]
        };
    }

    public static PlannedScheduleObject Create(string name, IList<Satellite> satellites, IList<GroundTarget> groundTargets, IList<GroundStation> groundStations)
    {
        var observations = TaskBuilder.CreateObservationTask(groundTargets);
        var communications = TaskBuilder.CreateCommunicationTask(groundStations);

        var timeWindows = TimeWindowBuilder.Create(satellites, groundTargets);
        var observationTasks = TaskResultBuilder.CreateObservations(observations, satellites, timeWindows);
        var observationWindows = TaskAvailabilityBuilder.CreateObservations(observations, satellites, timeWindows);
        // var communicationWindows = TaskAvailabilityBuilder.CreateCommunications(satellites, groundStations, footprints, tasks);
        // var communicationTasks = TaskResultBuilder.CreateCommunications(communicationWindows);

        return new PlannedScheduleObject()
        {
            Name = name,
            DateTime = DateTime.Now,
            Satellites = [.. satellites],
            ObservationTasks = [.. observations],
            CommunicationTasks = [.. communications],
            GroundTargets = [.. groundTargets],
            GroundStations = [.. groundStations],
            ObservationWindows = [.. observationWindows],
            CommunicationWindows = [],
            ObservationTaskResults = [.. observationTasks],
            CommunicationTaskResults = []
        };
    }
}
