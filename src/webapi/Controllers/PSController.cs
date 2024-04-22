using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using webapi.Services;

namespace webapi.Controllers;

[Route("api/[controller]/[action]")]
public class PSController : BaseController
{
    private readonly IDataService _dataService;
    private readonly IMapper _mapper;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Использовать основной конструктор", Justification = "<Ожидание>")]
    public PSController(IDataService dataService, IMapper mapper)
    {
        _dataService = dataService;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<PlannedScheduleObject> GetPS()
    {
        var res = _dataService.GetPlannedScheduleObject();

        if (res is { })
        {
            return _mapper.Map<PlannedScheduleObject>(res);
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<bool> IsValid(string server, string database, string user, string password)
    {
        return server == "local"
            && database == "PlannedScheduleDatabase"
            && user == "User"
            && password == "User";
    }

    [HttpGet]
    public ActionResult<IEnumerable<Satellite>> GetSatellites()
    {
        var res = _dataService.GetPlannedScheduleObject();

        if (res is { })
        {
            return res.Satellites;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<GroundStation>> GetGSS()
    {
        var res = _dataService.GetPlannedScheduleObject();

        if (res is { })
        {
            return res.GroundStations;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<GroundTarget>> GetGTS()
    {
        var res = _dataService.GetPlannedScheduleObject();

        if (res is { })
        {
            return res.GroundTargets;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<ObservationTask>> GetObservationTasks()
    {
        var res = _dataService.GetPlannedScheduleObject();

        if (res is { })
        {
            return res.ObservationTasks;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommunicationTask>> GetCommunicationTasks()
    {
        var res = _dataService.GetPlannedScheduleObject();

        if (res is { })
        {
            return res.CommunicationTasks;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<Availability>> GetObservationWindows()
    {
        var res = _dataService.GetPlannedScheduleObject();

        if (res is { })
        {
            return res.ObservationWindows;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<Availability>> GetCommunicationWindows()
    {
        var res = _dataService.GetPlannedScheduleObject();

        if (res is { })
        {
            return res.CommunicationWindows;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<ObservationTaskResult>> GetObservationTaskResults()
    {
        var res = _dataService.GetPlannedScheduleObject();

        if (res is { })
        {
            return res.ObservationTaskResults;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommunicationTaskResult>> GetCommunicationTaskResults()
    {
        var res = _dataService.GetPlannedScheduleObject();

        if (res is { })
        {
            return res.CommunicationTaskResults;
        }

        return NotFound();
    }
}
