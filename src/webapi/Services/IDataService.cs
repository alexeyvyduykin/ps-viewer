using Shared.Entities.MongoDB;

namespace webapi.Services;

public interface IDataService
{
    PlannedScheduleEntity? GetPlannedScheduleObject();
}
