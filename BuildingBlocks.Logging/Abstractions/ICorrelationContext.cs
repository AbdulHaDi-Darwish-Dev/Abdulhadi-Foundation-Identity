namespace BuildingBlocks.Logging.Abstractions;

public interface ICorrelationContext
{
    string CorrelationId { get; set; }
}