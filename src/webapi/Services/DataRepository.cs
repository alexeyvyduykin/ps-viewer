using Microsoft.Extensions.Caching.Memory;
using Shared.Entities.MongoDB;
using System.Text.Json;

namespace webapi.Services;

public class DataRepository : IDataRepository
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IMemoryCache _memoryCache;
    private readonly JsonSerializerOptions _serializeOptions;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);
    private readonly string _localPath = "Data/ps_demo.json";

    public DataRepository(IWebHostEnvironment hostingEnvironment, IMemoryCache memoryCache)
    {
        _hostingEnvironment = hostingEnvironment;
        _memoryCache = memoryCache;

        _serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new NetTopologySuite.IO.Converters.GeoJsonConverterFactory(),
            }
        };
    }

    public PlannedScheduleEntity? GetPlannedScheduleObject()
    {
        var cacheKey = CacheKeys.PlannedSchedule;

        _memoryCache.TryGetValue<PlannedScheduleEntity>(cacheKey, out var value);

        if (value == null)
        {
            value = GetFromJson();
            if (value != null)
            {
                // Sliding Expiration
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(_cacheExpiration);

                _memoryCache.Set(CacheKeys.PlannedSchedule, value, cacheEntryOptions);
            }
        }

        return value;
    }

    private PlannedScheduleEntity? GetFromJson()
    {
        var rootPath = _hostingEnvironment.ContentRootPath; //get the root path

        var fullPath = Path.Combine(rootPath, _localPath); //combine the root path with that of our json file inside mydata directory

        using StreamReader file = File.OpenText(fullPath);

        var res = JsonSerializer.Deserialize(file.BaseStream, typeof(PlannedScheduleEntity), _serializeOptions);

        if (res is PlannedScheduleEntity planned)
        {
            return planned;
        }

        return null;
    }
}
