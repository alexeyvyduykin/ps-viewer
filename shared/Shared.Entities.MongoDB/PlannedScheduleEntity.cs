using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Models;

namespace Shared.Entities.MongoDB;

public class PlannedScheduleEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

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