namespace BuildingBlocks.Identity.Abstractions;

public interface IExternalAuthProvider
{
    string ProviderName { get; }
    Task<ExternalUserPayload> VerifyTokenAsync(string token, CancellationToken cancellationToken = default);
}