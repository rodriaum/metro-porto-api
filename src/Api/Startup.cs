using Api.Filter;
using Api.Interfaces;
using Api.Service;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

namespace Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Metro do Porto API", Version = "v1" });
        });

        services.AddScoped<TokenAuthFilter>();

        // MongoDB configuration
        services.AddSingleton<IMongoClient>(sp =>
            new MongoClient(Configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017"));

        services.AddSingleton<IMongoDatabase>(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase("metro_porto"));

        // Add Data Services
        services.AddSingleton<IGtfsDataService, GtfsDataService>();
        services.AddSingleton<IAgencyService, AgencyService>();
        services.AddSingleton<ICalendarService, CalendarService>();
        services.AddSingleton<ICalendarDatesService, CalendarDatesService>();
        services.AddSingleton<IFareAttributesService, FareAttributesService>();
        services.AddSingleton<IFareRulesService, FareRulesService>();
        services.AddSingleton<IRoutesService, RoutesService>();
        services.AddSingleton<IShapesService, ShapesService>();
        services.AddSingleton<IStopsService, StopsService>();
        services.AddSingleton<IStopTimesService, StopTimesService>();
        services.AddSingleton<ITransfersService, TransfersService>();
        services.AddSingleton<ITripsService, TripsService>();

        // Add File Download Service
        services.AddSingleton<IGtfsFileService, GtfsFileService>();

        // Add HttpClient for downloading GTFS data
        services.AddHttpClient();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, ILogger<Startup> logger)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Metro do Porto API v1"));

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // Initialize database
        var gtfsDataService = serviceProvider.GetRequiredService<IGtfsDataService>();

        try
        {
            gtfsDataService.InitializeAsync().Wait();
        }
        catch (Exception ex)
        {
            logger.LogError($"Unable to configure GTFS data service. Maybe the data is being loaded!\n -> {ex.Message}");
            Task.Delay(5000).Wait();
            Environment.Exit(0);
        }
    }
}