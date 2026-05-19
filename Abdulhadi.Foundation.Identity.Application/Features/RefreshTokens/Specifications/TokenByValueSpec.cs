using Abdulhadi.Foundation.Identity.Domain.Entities;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Persistence;

namespace Abdulhadi.Foundation.Identity.Application.Features.RefreshTokens.Specifications;

public sealed class TokenByValueSpec : Specification<RefreshToken>
{
    public TokenByValueSpec(string incomingTokenHash)
    {
        Criteria = r => r.Token == incomingTokenHash;

        AsTracking = true;
    }
}