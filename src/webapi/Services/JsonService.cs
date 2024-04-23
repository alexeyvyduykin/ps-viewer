﻿using Microsoft.Extensions.Caching.Memory;
using Shared.Entities.MongoDB;
using System.Text.Json;

namespace webapi.Services;

public class JsonService : IDataService
{
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IMemoryCache _memoryCache;
    private readonly JsonSerializerOptions _serializeOptions;

    public JsonService(IWebHostEnvironment hostingEnvironment, IMemoryCache memoryCache)
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
        //_memoryCache.Remove(CacheKeys.PlannedSchedule);

        _memoryCache.TryGetValue<PlannedScheduleEntity>(CacheKeys.PlannedSchedule, out var value);

        if (value == null)
        {
            value = GetFromJson();
            if (value != null)
            {
                _memoryCache.Set(CacheKeys.PlannedSchedule, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
        }

        return value;
    }

    private PlannedScheduleEntity? GetFromJson()
    {
        var rootPath = _hostingEnvironment.ContentRootPath; //get the root path

        var localPath = "Data/ps_demo.json";

        var fullPath = Path.Combine(rootPath, localPath); //combine the root path with that of our json file inside mydata directory

        using StreamReader file = File.OpenText(fullPath);

        var res = JsonSerializer.Deserialize(file.BaseStream, typeof(PlannedScheduleEntity), _serializeOptions);

        if (res is PlannedScheduleEntity planned)
        {
            return planned;
        }

        return null;
    }
}