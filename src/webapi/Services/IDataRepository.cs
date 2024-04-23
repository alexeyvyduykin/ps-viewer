using Shared.Entities.MongoDB;

namespace webapi.Services;

public interface IDataRepository
{
    PlannedScheduleEntity? GetPlannedScheduleObject();
}
