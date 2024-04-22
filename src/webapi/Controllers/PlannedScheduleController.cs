using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using webapi.Services;

namespace webapi.Controllers;

[Route("api/ps")]
public class PlannedScheduleController : BaseController
{
    private readonly IDataService _dataService;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Использовать основной конструктор", Justification = "<Ожидание>")]
    public PlannedScheduleController(IDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet]
    public PlannedScheduleObject? GetPlannedSchedule()
    {
        return _dataService.GetPlannedScheduleObject();
    }

    [HttpGet]
    [Route("validation")]
    public bool IsValid(string server, string database, string user, string password)
    {
        return server == "local"
            && database == "PlannedScheduleDatabase"
            && user == "User"
            && password == "User";
    }

    [HttpGet]
    [Route("satellites")]
    public IEnumerable<Satellite>? GetSatellites()
    {
        var res = GetPlannedSchedule();

        if (res != null)
        {
            return res.Satellites;
        }

        return null;
    }

    [HttpGet]
    [Route("gss")]
    public IEnumerable<GroundStation>? GetGroundStations()
    {
        var res = GetPlannedSchedule();

        if (res != null)
        {
            return res.GroundStations;
        }

        return null;
    }

    [HttpGet]
    [Route("gts")]
    public IEnumerable<GroundTarget>? GetGroundTargets()
    {
        var res = GetPlannedSchedule();

        if (res != null)
        {
            return res.GroundTargets;
        }

        return null;
    }

    [HttpGet]
    [Route("observationtasks")]
    public IEnumerable<ObservationTask>? GetObservationTasks()
    {
        var res = GetPlannedSchedule();

        if (res != null)
        {
            return res.ObservationTasks;
        }

        return null;
    }

    [HttpGet]
    [Route("communicationtasks")]
    public IEnumerable<CommunicationTask>? GetCommunicationTasks()
    {
        var res = GetPlannedSchedule();

        if (res != null)
        {
            return res.CommunicationTasks;
        }

        return null;
    }

    [HttpGet]
    [Route("observationwindows")]
    public IEnumerable<Availability>? GetObservationWindows()
    {
        var res = GetPlannedSchedule();

        if (res != null)
        {
            return res.ObservationWindows;
        }

        return null;
    }

    [HttpGet]
    [Route("communicationwindows")]
    public IEnumerable<Availability>? GetCommunicationWindows()
    {
        var res = GetPlannedSchedule();

        if (res != null)
        {
            return res.CommunicationWindows;
        }

        return null;
    }

    [HttpGet]
    [Route("observationtaskresults")]
    public IEnumerable<ObservationTaskResult>? GetObservationTaskResults()
    {
        var res = GetPlannedSchedule();

        return res?.ObservationTaskResults;
    }

    [HttpGet]
    [Route("communicationtaskresults")]
    public IEnumerable<CommunicationTaskResult>? GetCommunicationTaskResults()
    {
        var res = GetPlannedSchedule();

        return res?.CommunicationTaskResults;
    }
}
