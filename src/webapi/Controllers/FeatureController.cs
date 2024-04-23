using Microsoft.AspNetCore.Mvc;
using webapi.Services;

namespace webapi.Controllers;

public class PreviewSearch
{
    public bool? HasFootprint { get; set; }

    public bool? HasPreviewTrack { get; set; }

    public bool? HasPreviewSwath { get; set; }
}

public class TrackSearch
{
    public bool? HasIntervals { get; set; }

    public bool? IsMarkerTrack { get; set; }
}

[Route("api/[controller]/[action]")]
public class FeatureController : BaseController
{
    private readonly IFeatureService _featureService;
    private readonly ILogger<FeatureController> _logger;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Использовать основной конструктор", Justification = "<Ожидание>")]
    public FeatureController(ILogger<FeatureController> logger, IFeatureService featureService)
    {
        _featureService = featureService;
        _logger = logger;

        _logger.LogInformation("FeatureController started.");
    }

    [HttpGet]
    [Route("{observationTaskName}")]
    public ActionResult<FeatureMap> GetPreview(string observationTaskName, [FromQuery] PreviewSearch previewSearch)
    {
        var options = new FeatureServiceOptions()
        {
            IsLonLat = true
        };

        PreviewFeatureType types = default;

        if (previewSearch.HasFootprint == true)
        {
            types |= PreviewFeatureType.Footprint;
        }

        if (previewSearch.HasPreviewTrack == true)
        {
            types |= PreviewFeatureType.PreviewTrack;
            types |= PreviewFeatureType.PreviewIntervalTrack;
        }

        if (previewSearch.HasPreviewSwath == true)
        {
            types |= PreviewFeatureType.PreviewSwath;
        }

        var res = _featureService.GetPreview(observationTaskName, types, options);

        _logger.LogInformation("FeatureController.GetPreview method executing.");

        return Ok(res);
    }

    [HttpGet]
    [Route("{name}/{nodesFormat}")]
    public ActionResult<NodeFeatureMap> GetMarkers(string name, string nodesFormat)
    {
        var options = new FeatureServiceOptions()
        {
            IsLonLat = true
        };

        var nodes = FromNodeFormat(nodesFormat);

        var types = SatelliteFeatureType.PsMarker;

        var res = _featureService.GetSatelliteFeatures(name, types, nodes, options);

        _logger.LogInformation("FeatureController.GetMarkers method executing.");

        return Ok(res);
    }

    [HttpGet]
    [Route("{name}")]
    public ActionResult<FeatureMap> GetGS(string name, [FromQuery] double[]? angles)
    {
        var options = new GSOptions()
        {
            IsLonLat = true,
            Angles = angles,
        };

        var res = _featureService.GetGroundStation(name, options);

        _logger.LogInformation("FeatureController.GetGS method executing.");

        return Ok(res);
    }

    [HttpGet]
    public ActionResult<FeatureMap> GetGTS()
    {
        var res = _featureService.GetGroundTargets();

        _logger.LogInformation("FeatureController.GetGTS method executing.");

        return Ok(res);
    }

    [HttpGet]
    [Route("{name}")]
    public ActionResult<FeatureMap> GetGT(string name)
    {
        var res = _featureService.GetGroundTarget(name);

        _logger.LogInformation("FeatureController.GetGT method executing.");

        return Ok(res);
    }

    [HttpGet]
    [Route("{name}/{nodesFormat}")]
    public ActionResult<NodeFeatureMap> GetTracks(string name, string nodesFormat, [FromQuery] TrackSearch trackSearch)
    {
        var options = new FeatureServiceOptions()
        {
            IsLonLat = true
        };

        var nodes = FromNodeFormat(nodesFormat);

        var types = SatelliteFeatureType.Track;

        if (trackSearch.IsMarkerTrack == true)
        {
            types = SatelliteFeatureType.MarkerTrack;
        }

        if (trackSearch.HasIntervals == true)
        {
            types |= SatelliteFeatureType.IntervalTrack;
        }

        var res = _featureService.GetSatelliteFeatures(name, types, nodes, options);

        _logger.LogInformation("FeatureController.GetTracks method executing.");

        return Ok(res);
    }

    [HttpGet]
    [Route("{name}/{nodesFormat}")]
    public ActionResult<NodeFeatureMap> GetSwaths(string name, string nodesFormat)
    {
        // TODO: ignore upper case
        //var sat = Satellites.Where(s => s.name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToSingle();

        var options = new FeatureServiceOptions()
        {
            IsLonLat = true
        };

        var nodes = FromNodeFormat(nodesFormat);

        var types = SatelliteFeatureType.Left | SatelliteFeatureType.Right;

        var res = _featureService.GetSatelliteFeatures(name, types, nodes, options);

        _logger.LogInformation("FeatureController.GetSwaths method executing.");

        return Ok(res);
    }

    private static int[] FromNodeFormat(string format)
    {
        var arr = format.Split('+').Select(s => int.Parse(s)).ToArray();
        return arr.Length > 1 ? Enumerable.Range(arr[0], arr[1] - arr[0] + 1).ToArray() : arr;
    }
}