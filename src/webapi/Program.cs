using NetTopologySuite;
using webapi.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddScoped<IDataRepository, DataRepository>();
        builder.Services.AddScoped<IFeatureRepository, FeatureRepository>();

        builder.Services.AddAutoMapper(typeof(Program).Assembly);

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory());
            //options.JsonSerializerOptions.Converters.Add(new InterfaceConverter<ITask>());
            //options.JsonSerializerOptions.Converters.Add(new InterfaceConverter<ITaskResult>());
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });

        // nothing to do with NTS.IO.GeoJSON4STJ specifically, but a recommended
        // best-practice is to inject this instead of using the global variable:
        builder.Services.AddSingleton(NtsGeometryServices.Instance);

        builder.Services.AddMemoryCache();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}