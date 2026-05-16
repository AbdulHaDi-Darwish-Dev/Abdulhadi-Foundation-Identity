using System.Security.Claims;

namespace Abdulhadi.Foundation.Identity.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(    
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var start = DateTime.UtcNow;

        var request = context.Request;
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";

        try
        {
            _logger.LogInformation(
                "HTTP {Method} {Path} started | User: {User}",
                request.Method,
                request.Path,
                userId);

            await _next(context);

            var duration = (DateTime.UtcNow - start).TotalMilliseconds;

            _logger.LogInformation(
                "HTTP {Method} {Path} completed | Status: {StatusCode} | Duration: {Duration}ms | User: {User}",
                request.Method,
                request.Path,
                context.Response.StatusCode,
                duration,
                userId);
        }
        catch (Exception ex)
        {
            var duration = (DateTime.UtcNow - start).TotalMilliseconds;

            _logger.LogError(
                ex,
                "HTTP {Method} {Path} failed | Duration: {Duration}ms | User: {User}",
                request.Method,
                request.Path,
                duration,
                userId);

            throw;
        }
    }
}