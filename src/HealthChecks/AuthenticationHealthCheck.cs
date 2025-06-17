using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TransitGtfsApi.HealthChecks;

public class AuthenticationHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("Authentication is healthy"));
    }
}