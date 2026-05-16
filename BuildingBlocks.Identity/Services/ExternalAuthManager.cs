using BuildingBlocks.Identity.Abstractions;

namespace BuildingBlocks.Identity.Services;

public sealed class ExternalAuthManager
{
    private readonly IEnumerable<IExternalAuthProvider> _providers;

    public ExternalAuthManager(IEnumerable<IExternalAuthProvider> providers)
    {
        _providers = providers;
    }

    public async Task<ExternalUserPayload> GetExternalUserInfoAsync(
        string providerName,
        string token,
        CancellationToken cancellationToken = default)
    {
        var provider = _providers.FirstOrDefault(p =>
            p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new NotSupportedException(providerName);

        return await provider.VerifyTokenAsync(token, cancellationToken);
    }
}