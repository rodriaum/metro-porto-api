using AspNetCoreRateLimit;
using MetroPortoAPI.Api.Filter;
using MetroPortoAPI.Api.Interfaces;
using MetroPortoAPI.Api.Interfaces.Database;
using MetroPortoAPI.Api.Interfaces.Gtfs;
using MetroPortoAPI.Api.Middleware;
using MetroPortoAPI.Api.Models;
using MetroPortoAPI.Api.Service;
using MetroPortoAPI.Api.Service.Database;
using MetroPortoAPI.Api.Service.Gtfs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

namespace MetroPortoAPI.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureProtectionServices(services);
        ConfigureInfrastructureServices(services);
        ConfigureDatabaseServices(services);
        ConfigureCacheServices(services);
        ConfigureApplicationServices(services);
    }

    private void ConfigureProtectionServices(IServiceCollection services)
    {
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
        services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
        services.AddInMemoryRateLimiting();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }

    private void ConfigureInfrastructureServices(IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
        });

        services.AddControllers(options =>
        {
            options.CacheProfiles.Add("Default", new CacheProfile
            {
                Duration = 60,
                Location = ResponseCacheLocation.Any
            });
        });

        services.AddResponseCaching();

        // Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Metro do Porto API", Version = "v1" });
        });

        services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

        services.AddScoped<TokenAuthFilter>();

        services.AddHttpClient();
    }

    private void ConfigureDatabaseServices(IServiceCollection services)
    {
        // MongoDB
        services.AddSingleton<IMongoClient>(sp =>
        {
            string? connection = Configuration.GetConnectionString("MongoDB");

            if (string.IsNullOrWhiteSpace(connection))
            {
                // dev test
                connection = "mongodb://localhost:27017";
                Console.WriteLine("MongoDB connection string not found in configuration.");
            }

            MongoClientSettings settings = MongoClientSettings.FromConnectionString(connection);

            settings.MaxConnectionPoolSize = 1000;
            settings.MinConnectionPoolSize = 10;
            settings.WaitQueueSize = 10000;

            return new MongoClient(connection);
        });

        services.AddSingleton(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase("metro_porto"));
    }

    private void ConfigureCacheServices(IServiceCollection services)
    {
        // Redis
        services.AddStackExchangeRedisCache(options =>
        {
            string? connection = Configuration.GetConnectionString("Redis");

            if (string.IsNullOrWhiteSpace(connection))
            {
                // dev test
                connection = "localhost:6379,abortConnect=false";
                Console.WriteLine("Redis connection string not found in configuration.");
            }

            options.Configuration = connection;
            options.InstanceName = $"{Constant.Name.Replace(" ", "-").ToLower()}:";
        });

        services.AddSingleton<IRedisService, RedisService>();
    }

    private void ConfigureApplicationServices(IServiceCollection services)
    {
        services.AddSingleton<IGtfsDataService, GtfsDataService>();
        services.AddSingleton<IGtfsFileService, GtfsFileService>();

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
    }

    public void ConfigureSecurityHeaders(IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");

            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");

            await next();
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, ILogger<Startup> logger)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }

        ConfigureSecurityHeaders(app);

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Metro do Porto API v1"));

        app.UseMiddleware<BlacklistMiddleware>();
        app.UseMiddleware<ProtectionMiddleware>();
        app.UseIpRateLimiting();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseResponseCaching();
        app.UseResponseCompression();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        InitializeDatabase(serviceProvider, logger);
    }

    private void InitializeDatabase(IServiceProvider serviceProvider, ILogger<Startup> logger)
    {
        var gtfsDataService = serviceProvider.GetRequiredService<IGtfsDataService>();

        // Bomb code :)
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