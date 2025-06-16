namespace TransitGtfsApi.Middleware;

public class BlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BlacklistMiddleware> _logger;
    private readonly List<string> _blacklistedIps;

    public BlacklistMiddleware(RequestDelegate next, ILogger<BlacklistMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _blacklistedIps = configuration.GetSection("Security:BlacklistedIps").Get<List<string>>() ?? new List<string>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? remoteIp = context.Connection.RemoteIpAddress?.ToString();

        if (remoteIp != null && _blacklistedIps.Contains(remoteIp))
        {
            _logger.LogWarning("Access blocked for blacklisted IP: {IpAddress}", remoteIp);
            
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Access denied.");
            return;
        }

        await _next(context);
    }
}