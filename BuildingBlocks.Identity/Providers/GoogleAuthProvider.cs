using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using BuildingBlocks.Identity.Options;
using BuildingBlocks.Identity.Abstractions;

namespace BuildingBlocks.Identity.Providers;

public class GoogleAuthProvider : IExternalAuthProvider
{
    public string ProviderName => "Google";

    private readonly GoogleOptions _options;

    // نستخدم IOptions لكي نحصل على القيم بشكل "Strongly Typed"
    public GoogleAuthProvider(IOptions<GoogleOptions> options)
    {
        _options = options.Value;
    }

    public async Task<ExternalUserPayload> VerifyTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_options.ClientId))
            throw new InvalidOperationException("Google ClientId is not configured.");

        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new List<string> { _options.ClientId }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
        return new ExternalUserPayload(payload.Email, payload.Name, ProviderName);
    }
}