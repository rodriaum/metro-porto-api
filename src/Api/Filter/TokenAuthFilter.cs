using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Api.Filter;

public class TokenAuthFilter : IAsyncActionFilter
{
    private readonly AppSettings _appSettings;

    public TokenAuthFilter(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Authorization header is missing" });
            return;
        }

        string headerValue = authHeader.ToString();
        if (!headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Invalid authorization format" });
            return;
        }

        string token = headerValue.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(token) || token != _appSettings.ApiToken)
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Invalid token" });
            return;
        }

        await next();
    }
}