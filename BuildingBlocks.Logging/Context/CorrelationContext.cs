using BuildingBlocks.Logging.Abstractions;

namespace BuildingBlocks.Logging.Context;

public class CorrelationContext : ICorrelationContext
{
    public string CorrelationId { get; set; } = string.Empty;
}