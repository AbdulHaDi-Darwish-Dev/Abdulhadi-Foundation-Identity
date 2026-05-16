using Microsoft.AspNetCore.Http;
using BuildingBlocks.Logging.Constants;
using BuildingBlocks.Logging.Abstractions;

namespace BuildingBlocks.Logging.Middleware;

public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICorrelationContext correlationContext)
    {
        var correlationId =
            context.Request.Headers[CorrelationConstants.Header].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        correlationContext.CorrelationId = correlationId;

        context.Items[CorrelationConstants.Header] = correlationId;

        context.Response.Headers[CorrelationConstants.Header] = correlationId;

        await _next(context);
    }
}