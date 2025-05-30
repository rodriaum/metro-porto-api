using Microsoft.Extensions.Caching.Memory;

namespace MetroPortoAPI.Api.Middleware;

public class ProtectionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ProtectionMiddleware> _logger;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _monitoringPeriod;
    private readonly int _requestThreshold;

    public ProtectionMiddleware(RequestDelegate next, ILogger<ProtectionMiddleware> logger, IMemoryCache cache, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _cache = cache;

        int monitoringSeconds = configuration.GetValue("Security:Protection:MonitoringPeriodSeconds", 10);
        _monitoringPeriod = TimeSpan.FromSeconds(monitoringSeconds);

        _requestThreshold = configuration.GetValue("Security:Protection:RequestThreshold", 50);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? remoteIp = context.Connection.RemoteIpAddress?.ToString();
        string path = context.Request.Path.ToString();

        if (remoteIp != null)
        {
            string cacheKey = $"protection:{remoteIp}:{path}";

            if (!_cache.TryGetValue(cacheKey, out int requestCount))
            {
                requestCount = 0;
            }

            requestCount++;

            _cache.Set(cacheKey, requestCount, _monitoringPeriod);

            if (requestCount > _requestThreshold)
            {
                _logger.LogWarning("Possible DDoS attempt detected from IP: {IpAddress} on path: {Path}", remoteIp, path);
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }
        }

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DDoS protection middleware");
            throw;
        }
    }
}