using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Shared.Models;

namespace webapi.Services;

public class FeatureServiceOptions : IFeatureServiceOptions
{
    public bool IsLonLat { get; init; } = true;
}

public class GSOptions : FeatureServiceOptions, IGSOptions
{
    public double[]? Angles { get; init; }
}

public partial class FeatureRepository : IFeatureRepository
{
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);
    private readonly IDataRepository _dataService;
    private readonly IMapper _mapper;

    private readonly SatelliteFeatureCache _satelliteCache;
    private readonly PreviewFeatureCache _previewCache;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Использовать основной конструктор", Justification = "<Ожидание>")]
    public FeatureRepository(IDataRepository dataService, IMemoryCache memoryCache, IMapper mapper)
    {
        _memoryCache = memoryCache;
        _dataService = dataService;
        _mapper = mapper;

        _satelliteCache = new SatelliteFeatureCache(memoryCache);
        _previewCache = new PreviewFeatureCache(memoryCache);
    }

    private T GetFromCache<T>(object key, Func<T> creator)
    {
        _memoryCache.TryGetValue<T>(key, out var value);

        if (value == null)
        {
            value = creator.Invoke();
            if (value != null)
            {
                // Sliding Expiration
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(_cacheExpiration);

                _memoryCache.Set(key, value, cacheEntryOptions);
            }
        }

        return value!;
    }

    public NodeFeatureMap GetSatelliteFeatures(string satelliteName, SatelliteFeatureType types, int[] nodes, IFeatureServiceOptions? options = null)
    {
        options ??= new FeatureServiceOptions();

        return _satelliteCache.GetValue(satelliteName, types, nodes, options, (ps) =>
        {
            return ps.Type switch
            {
                SatelliteFeatureType.Track => CreateTrack(ps.Satellite, ps.Node, ps.Options.IsLonLat),
                SatelliteFeatureType.IntervalTrack => CreateTrackIntervals(ps.Satellite, ps.Node, ps.Options.IsLonLat),
                SatelliteFeatureType.Left => CreateSwath(ps.Satellite, ps.Node, SwathDirection.Left, ps.Options.IsLonLat),
                SatelliteFeatureType.Right => CreateSwath(ps.Satellite, ps.Node, SwathDirection.Right, ps.Options.IsLonLat),
                SatelliteFeatureType.PsMarker => CreateTaskResultMarkers(ps.Satellite, ps.Node, ps.Options.IsLonLat),
                SatelliteFeatureType.MarkerTrack => CreateMarkerTrack(ps.Satellite, ps.Node, ps.Options.IsLonLat),
                _ => []
            };
        });
    }

    public FeatureMap GetGroundStation(string name, IGSOptions? options = null)
    {
        options ??= new GSOptions();

        if (options.Angles == null)
        {
            var fc = GetFromCache($"GS_{name}", () => CreateGroundStation(name, null, options.IsLonLat));
            return new() { { "GS", fc } };
        }

        return new() { { "GS", CreateGroundStation(name, options.Angles, options.IsLonLat) } };
    }

    public FeatureMap GetGroundTargets(IFeatureServiceOptions? options = null)
    {
        options ??= new FeatureServiceOptions();

        return new() { { "GT", GetFromCache("GTS", () => CreateGroundTargets(options.IsLonLat)) } };
    }

    public FeatureMap GetGroundTarget(string name, IFeatureServiceOptions? options = null)
    {
        options ??= new FeatureServiceOptions();

        return new() { { "GT", GetFromCache($"GT_{name}", () => CreateGroundTarget(name, options.IsLonLat)) } };
    }

    public FeatureMap GetPreview(string observationTaskName, PreviewFeatureType types, IFeatureServiceOptions? options = null)
    {
        options ??= new FeatureServiceOptions();

        return _previewCache.GetValue(observationTaskName, types, options, (ps) =>
        {
            return ps.Type switch
            {
                PreviewFeatureType.Footprint => CreatePreviewFootprint(ps.ObservationTask, ps.Options.IsLonLat),
                PreviewFeatureType.PreviewTrack => CreateTrackSegment(ps.ObservationTask, ps.Options.IsLonLat),
                PreviewFeatureType.PreviewIntervalTrack => CreateIntervalTrackSegment(ps.ObservationTask, ps.Options.IsLonLat),
                PreviewFeatureType.PreviewSwath => CreateSwathSegment(ps.ObservationTask, ps.Options.IsLonLat),
                _ => []
            };
        });
    }
}
