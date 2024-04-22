using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Models;

namespace pscli.Services;

// TODO: use mongoDB entity shared class
public class PsData : PlannedScheduleObject
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
}
