using System.Security.Claims;
using Abdulhadi.Foundation.Identity.Domain.Entities;

namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    (string RawToken, RefreshToken RefreshToken) CreateRefreshToken(Guid userId);
}