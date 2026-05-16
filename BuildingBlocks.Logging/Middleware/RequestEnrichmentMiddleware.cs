using Serilog.Context;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using BuildingBlocks.Logging.Abstractions;

namespace BuildingBlocks.Logging.Middleware;

public class RequestEnrichmentMiddleware
{
    private readonly RequestDelegate _next;

    public RequestEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IRequestContext requestContext)
    {
        requestContext.CorrelationId =
            context.Items["X-Correlation-Id"]?.ToString() ?? string.Empty;

        requestContext.UserId =
            context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        requestContext.IpAddress =
            context.Connection.RemoteIpAddress?.ToString();

        using (LogContext.PushProperty("CorrelationId", requestContext.CorrelationId))
        using (LogContext.PushProperty("UserId", requestContext.UserId ?? "Anonymous"))
        using (LogContext.PushProperty("IpAddress", requestContext.IpAddress ?? "Unknown"))
        {
            await _next(context);
        }
    }
}