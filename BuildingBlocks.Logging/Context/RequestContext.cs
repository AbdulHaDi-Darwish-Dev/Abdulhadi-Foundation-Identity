using BuildingBlocks.Logging.Abstractions;

namespace BuildingBlocks.Logging.Context;

public class RequestContext : IRequestContext
{
    public string CorrelationId { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
}