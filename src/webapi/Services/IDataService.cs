using Shared.Models;

namespace webapi.Services;

public interface IDataService
{
    PlannedScheduleObject? GetPlannedScheduleObject();
}
