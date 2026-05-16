namespace BuildingBlocks.Logging.Abstractions;

public interface IRequestContext
{
    string CorrelationId { get; set; }
    string? UserId { get; set; }
    string? IpAddress { get; set; }
}