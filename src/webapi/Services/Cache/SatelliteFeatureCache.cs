using Microsoft.Extensions.Caching.Memory;
using NetTopologySuite.Features;

namespace webapi.Services;

public class SatelliteFuncParams
{
    public required string Satellite { get; init; }

    public required int Node { get; init; }

    public required SatelliteFeatureType Type { get; init; }

    public required IFeatureServiceOptions Options { get; init; }
}

public class SatelliteFeatureCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _relative;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Использовать основной конструктор", Justification = "<Ожидание>")]
    public SatelliteFeatureCache(IMemoryCache memoryCache, TimeSpan? relative = null)
    {
        _memoryCache = memoryCache;
        _relative = relative ?? TimeSpan.FromMinutes(5);
    }

    public NodeFeatureMap GetValue(string satelliteName, SatelliteFeatureType types, int[] nodes, IFeatureServiceOptions options, Func<SatelliteFuncParams, FeatureCollection> creator)
    {
        var dict = new NodeFeatureMap();

        foreach (var node in nodes)
        {
            dict.Add(node, []);

            foreach (var type in Enum.GetValues<SatelliteFeatureType>().Where(s => types.HasFlag(s)))
            {
                var key = $"{satelliteName}-{node}-{type}";

                _memoryCache.TryGetValue<FeatureCollection>(key, out var value);

                if (value == null)
                {
                    var ps = new SatelliteFuncParams()
                    {
                        Satellite = satelliteName,
                        Node = node,
                        Type = type,
                        Options = options
                    };

                    value = creator.Invoke(ps);

                    if (value != null)
                    {
                        _memoryCache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(_relative));
                    }
                }

                dict[node].Add(type.ToString(), value!);
            }
        }

        return dict;
    }

}
