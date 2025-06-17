using AspNetCoreRateLimit;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using TransitGtfsApi.Filter;
using TransitGtfsApi.HealthChecks;
using TransitGtfsApi.Interfaces;
using TransitGtfsApi.Interfaces.Database;
using TransitGtfsApi.Interfaces.Gtfs;
using TransitGtfsApi.Service;
using TransitGtfsApi.Service.Database;
using TransitGtfsApi.Service.Gtfs;

namespace TransitGtfsApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string envPath = Path.Combine(baseDirectory, ".env");

        Console.WriteLine($"Trying to load .env file from path: {envPath}");

        if (File.Exists(envPath))
        {
            Console.WriteLine(".env file found!");
            Env.Load(envPath);
            Console.WriteLine(".env file loaded successfully!");
        }
        else
        {
            Console.WriteLine(".env file not found in bin directory!");

            string rootPath = Path.Combine(baseDirectory, "..", "..", "..", "..", ".env");
            Console.WriteLine($"Trying to load from root directory: {rootPath}");

            if (File.Exists(rootPath))
            {
                Console.WriteLine(".env file found in root directory!");
                Env.Load(rootPath);
                Console.WriteLine(".env file loaded successfully!");
            }
            else
            {
                Console.WriteLine("ERROR: .env file not found in any location!");
                Console.WriteLine("Please create a .env file in the project root");
                Environment.Exit(1);
            }
        }

        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    private void ValidateEnvironmentVariables(ILogger<Startup> logger)
    {
        List<string> missingVars = new List<string>();

        foreach (var envVar in Constant.RequiredEnvVars)
        {
            string? value = Environment.GetEnvironmentVariable(envVar);

            if (string.IsNullOrWhiteSpace(value))
            {
                missingVars.Add(envVar);
                logger.LogError($"Required environment variable not found: {envVar}");
            }
            else
            {
                logger.LogInformation($"Loaded environment variable: {envVar}");
            }
        }

        if (missingVars.Any())
        {
            logger.LogError("Application terminated due to missing environment variables.");
            Environment.Exit(0);
        }
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureProtectionServices(services);
        ConfigureInfrastructureServices(services);
        ConfigureDatabaseServices(services);
        ConfigureCacheServices(services);
        ConfigureApplicationServices(services);
        ConfigureSecurityServices(services);
        ConfigureLogging(services);
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
            options.Providers.Add<BrotliCompressionProvider>();
        });

        services.AddControllers(options =>
        {
            options.Filters.Add<ValidateModelStateFilter>();
            options.Filters.Add<SanitizeInputFilter>();
            options.CacheProfiles.Add("Default", new CacheProfile
            {
                Duration = 60,
                Location = ResponseCacheLocation.Any
            });
        });

        services.AddResponseCaching();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = $"{Constant.Name} API", Version = Constant.Version });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        services.AddScoped<TokenAuthFilter>();
        services.AddHttpClient();
    }

    private void ConfigureSecurityServices(IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins(Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
                       .WithMethods(Configuration.GetSection("Cors:AllowedMethods").Get<string[]>() ?? Array.Empty<string>())
                       .WithHeaders(Configuration.GetSection("Cors:AllowedHeaders").Get<string[]>() ?? Array.Empty<string>())
                       .WithExposedHeaders(Configuration.GetSection("Cors:ExposedHeaders").Get<string[]>() ?? Array.Empty<string>())
                       .SetPreflightMaxAge(TimeSpan.FromSeconds(Configuration.GetValue<int>("Cors:MaxAge", 3600)));
            });
        });

        services.AddHealthChecks()
            .AddCheck<SecurityHealthCheck>("security")
            .AddCheck<RateLimitHealthCheck>("rate_limit")
            .AddCheck<AuthenticationHealthCheck>("auth");
    }

    private void ConfigureLogging(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddSerilog(new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger());
        });
    }

    private void ConfigureDatabaseServices(IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            string? connection = Environment.GetEnvironmentVariable("MONGODB_CONNECTION");
            ILogger<Startup> logger = sp.GetRequiredService<ILogger<Startup>>();

            logger.LogInformation("Setting up MongoDB connection...");

            MongoClientSettings settings = MongoClientSettings.FromConnectionString(connection!);
            settings.MaxConnectionPoolSize = 1000;
            settings.MinConnectionPoolSize = 10;
            settings.WaitQueueSize = 10000;

            return new MongoClient(settings);
        });

        services.AddSingleton(sp =>
        {
            string? dbName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME")?.ToLower();
            ILogger<Startup> logger = sp.GetRequiredService<ILogger<Startup>>();

            logger.LogInformation($"Using MongoDB database: {dbName}");

            return sp.GetRequiredService<IMongoClient>().GetDatabase(dbName!);
        });
    }

    private void ConfigureCacheServices(IServiceCollection services)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            string? connection = Environment.GetEnvironmentVariable("REDIS_CONNECTION");
            string? instanceName = Environment.GetEnvironmentVariable("REDIS_INSTANCE_NAME")?.ToLower();

            ILogger<Startup> logger = services.BuildServiceProvider().GetRequiredService<ILogger<Startup>>();

            logger.LogInformation("Configuring Redis connection...");
            logger.LogInformation($"Redis Instance Name: {instanceName}");

            options.Configuration = connection;
            options.InstanceName = $"{instanceName}:";
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
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
            context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
            context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", "none");

            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");

            await next();
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, ILogger<Startup> logger)
    {
        ValidateEnvironmentVariables(logger);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }

        app.UseSerilogRequestLogging();
        
        ConfigureSecurityHeaders(app);

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Constant.Name} API {Constant.Version}"));

        app.UseIpRateLimiting();

        app.UseHttpsRedirection();
        app.UseRouting();
        
        app.UseCors();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseResponseCaching();
        app.UseResponseCompression();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });

        InitializeDatabase(serviceProvider, logger);
    }

    private void InitializeDatabase(IServiceProvider serviceProvider, ILogger<Startup> logger)
    {
        IGtfsDataService gtfsDataService = serviceProvider.GetRequiredService<IGtfsDataService>();

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