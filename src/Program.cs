namespace TransitGtfs.Api;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.ConfigureKestrel(options =>
                {
                    options.Limits.MaxConcurrentConnections = 10000;
                    options.Limits.MaxConcurrentUpgradedConnections = 10000;
                    options.Limits.MaxRequestBodySize = 10 * 1024; // 10 KB
                    options.Limits.MinRequestBodyDataRate = null;
                    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);

                    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
                });
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();

                logging.AddFilter("Microsoft.AspNetCore.HttpOverrides", LogLevel.Warning);
                logging.AddFilter("TransitGtfs.Api.Middleware", LogLevel.Warning);
            });
}