using Abdulhadi.Foundation.Identity.Domain.Entities;

namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user);
}