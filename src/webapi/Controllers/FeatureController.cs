using Microsoft.AspNetCore.Mvc;
using webapi.Services;

namespace webapi.Controllers;

[Route("api/features")]
public class FeatureController : BaseController
{
    private readonly IFeatureService _featureService;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Использовать основной конструктор", Justification = "<Ожидание>")]
    public FeatureController(IFeatureService featureService)
    {
        _featureService = featureService;
    }

    [HttpGet]
    [Route("preview/{observationTaskName}")]
    public FeatureMap GetPreview(string observationTaskName, [FromQuery] bool? hasFootprint, [FromQuery] bool? hasPreviewTrack, [FromQuery] bool? hasPreviewSwath)
    {
        var options = new FeatureServiceOptions()
        {
            IsLonLat = true
        };

        PreviewFeatureType types = default;

        if (hasFootprint == true)
        {
            types |= PreviewFeatureType.Footprint;
        }

        if (hasPreviewTrack == true)
        {
            types |= PreviewFeatureType.PreviewTrack;
            types |= PreviewFeatureType.PreviewIntervalTrack;
        }

        if (hasPreviewSwath == true)
        {
            types |= PreviewFeatureType.PreviewSwath;
        }

        return _featureService.GetPreview(observationTaskName, types, options);
    }

    [HttpGet]
    [Route("markers/{name}/{nodesFormat}")]
    public NodeFeatureMap GetMarkers(string name, string nodesFormat)
    {
        var options = new FeatureServiceOptions()
        {
            IsLonLat = true
        };

        var nodes = FromNodeFormat(nodesFormat);

        var types = SatelliteFeatureType.PsMarker;

        return _featureService.GetSatelliteFeatures(name, types, nodes, options);
    }

    [HttpGet]
    [Route("gss/{name}")]
    public FeatureMap GetGroundStation(string name, [FromQuery] double[]? angles)
    {
        var options = new GSOptions()
        {
            IsLonLat = true,
            Angles = angles,
        };

        return _featureService.GetGroundStation(name, options);
    }

    [HttpGet]
    [Route("gts")]
    public FeatureMap GetGroundTargets()
    {
        return _featureService.GetGroundTargets();
    }

    [HttpGet]
    [Route("gts/{name}")]
    public FeatureMap GetGroundTarget(string name)
    {
        return _featureService.GetGroundTarget(name);
    }

    [HttpGet]
    [Route("tracks/{name}/{nodesFormat}")]
    public NodeFeatureMap GetTracks(string name, string nodesFormat, [FromQuery] bool? hasIntervals, [FromQuery] bool? isMarkerTrack)
    {
        var options = new FeatureServiceOptions()
        {
            IsLonLat = true
        };

        var nodes = FromNodeFormat(nodesFormat);

        var types = SatelliteFeatureType.Track;

        if (isMarkerTrack == true)
        {
            types = SatelliteFeatureType.MarkerTrack;
        }

        if (hasIntervals == true)
        {
            types |= SatelliteFeatureType.IntervalTrack;
        }

        return _featureService.GetSatelliteFeatures(name, types, nodes, options);
    }

    [HttpGet]
    [Route("swaths/{name}/{nodesFormat}")]
    public NodeFeatureMap GetSwaths(string name, string nodesFormat)
    {
        var options = new FeatureServiceOptions()
        {
            IsLonLat = true
        };

        var nodes = FromNodeFormat(nodesFormat);

        var types = SatelliteFeatureType.Left | SatelliteFeatureType.Right;

        return _featureService.GetSatelliteFeatures(name, types, nodes, options);
    }

    private static int[] FromNodeFormat(string format)
    {
        var arr = format.Split('+').Select(s => int.Parse(s)).ToArray();
        return arr.Length > 1 ? Enumerable.Range(arr[0], arr[1] - arr[0] + 1).ToArray() : arr;
    }
}
