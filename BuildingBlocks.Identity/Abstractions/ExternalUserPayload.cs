namespace BuildingBlocks.Identity.Abstractions
{
    public record ExternalUserPayload(string Email, string Name, string Provider);
}