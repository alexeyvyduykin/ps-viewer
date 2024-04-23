using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using webapi.Services;

namespace webapi.Controllers;

[Route("api/[controller]/[action]")]
public class PSController : BaseController
{
    private readonly IDataRepository _dataRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PSController> _logger;

    public PSController(ILogger<PSController> logger, IDataRepository dataRepository, IMapper mapper)
    {
        _dataRepository = dataRepository;
        _mapper = mapper;
        _logger = logger;

        _logger.LogInformation("PSController started.");
    }

    [HttpGet]
    public ActionResult<PlannedScheduleObject> GetPS()
    {
        var res = _dataRepository.GetPlannedScheduleObject();

        if (res is { })
        {
            var res1 = _mapper.Map<PlannedScheduleObject>(res);

            _logger.LogInformation("PSController.GetPS method executing.");

            return Ok(res1);
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
        var res = _dataRepository.GetPlannedScheduleObject();

        if (res is { })
        {
            _logger.LogInformation("PSController.GetSatellites method executing.");

            return res.Satellites;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<GroundStation>> GetGSS()
    {
        var res = _dataRepository.GetPlannedScheduleObject();

        if (res is { })
        {
            _logger.LogInformation("PSController.GetGSS method executing.");

            return res.GroundStations;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<GroundTarget>> GetGTS()
    {
        var res = _dataRepository.GetPlannedScheduleObject();

        if (res is { })
        {
            _logger.LogInformation("PSController.GetGTS method executing.");

            return res.GroundTargets;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<ObservationTask>> GetObservationTasks()
    {
        var res = _dataRepository.GetPlannedScheduleObject();

        if (res is { })
        {
            _logger.LogInformation("PSController.GetObservationTasks method executing.");

            return res.ObservationTasks;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommunicationTask>> GetCommunicationTasks()
    {
        var res = _dataRepository.GetPlannedScheduleObject();

        if (res is { })
        {
            _logger.LogInformation("PSController.GetCommunicationTasks method executing.");

            return res.CommunicationTasks;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<Availability>> GetObservationWindows()
    {
        var res = _dataRepository.GetPlannedScheduleObject();

        if (res is { })
        {
            _logger.LogInformation("PSController.GetObservationWindows method executing.");

            return res.ObservationWindows;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<Availability>> GetCommunicationWindows()
    {
        var res = _dataRepository.GetPlannedScheduleObject();

        if (res is { })
        {
            _logger.LogInformation("PSController.GetCommunicationWindows method executing.");

            return res.CommunicationWindows;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<ObservationTaskResult>> GetObservationTaskResults()
    {
        var res = _dataRepository.GetPlannedScheduleObject();

        if (res is { })
        {
            _logger.LogInformation("PSController.GetObservationTaskResults method executing.");

            return res.ObservationTaskResults;
        }

        return NotFound();
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommunicationTaskResult>> GetCommunicationTaskResults()
    {
        var res = _dataRepository.GetPlannedScheduleObject();

        if (res is { })
        {
            _logger.LogInformation("PSController.GetCommunicationTaskResults method executing.");

            return res.CommunicationTaskResults;
        }

        return NotFound();
    }
}
