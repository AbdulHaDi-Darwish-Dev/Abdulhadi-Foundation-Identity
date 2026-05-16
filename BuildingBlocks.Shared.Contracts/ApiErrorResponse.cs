namespace BuildingBlocks.Shared.Contracts
{
    public sealed class ApiErrorResponse
    {
        public string? Code { get; init; }
        public string? Message { get; init; }
    }
}