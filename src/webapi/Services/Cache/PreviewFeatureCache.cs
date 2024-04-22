using Microsoft.Extensions.Caching.Memory;
using NetTopologySuite.Features;

namespace webapi.Services;

public class PreviewFuncParams
{
    public required string ObservationTask { get; init; }

    public required PreviewFeatureType Type { get; init; }

    public required IFeatureServiceOptions Options { get; init; }
}

public class PreviewFeatureCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _relative;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Использовать основной конструктор", Justification = "<Ожидание>")]
    public PreviewFeatureCache(IMemoryCache memoryCache, TimeSpan? relative = null)
    {
        _memoryCache = memoryCache;
        _relative = relative ?? TimeSpan.FromMinutes(5);
    }

    public FeatureMap GetValue(string observationTaskName, PreviewFeatureType types, IFeatureServiceOptions options, Func<PreviewFuncParams, FeatureCollection> creator)
    {
        var dict = new FeatureMap();

        foreach (var type in Enum.GetValues<PreviewFeatureType>().Where(s => types.HasFlag(s)))
        {
            var key = $"{observationTaskName}-{type}";

            _memoryCache.TryGetValue<FeatureCollection>(key, out var value);

            if (value == null)
            {
                var ps = new PreviewFuncParams()
                {
                    ObservationTask = observationTaskName,
                    Type = type,
                    Options = options
                };

                value = creator.Invoke(ps);

                if (value != null)
                {
                    _memoryCache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_relative));
                }
            }

            dict.Add(type.ToString(), value!);
        }

        return dict;
    }
}
