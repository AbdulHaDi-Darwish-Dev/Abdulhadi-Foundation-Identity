using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Abdulhadi.Foundation.Identity.Domain.Entities;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Authentication;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Authentication.Jwt;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;

    private readonly UserManager<ApplicationUser> _userManager;

    public JwtProvider(IOptions<JwtOptions> options, UserManager<ApplicationUser> userManager)
    {
        _options = options.Value;

        _userManager = userManager;
    }

    public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName!),

            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            new(ClaimTypes.NameIdentifier, user.Id.ToString()),

            new(ClaimTypes.Name, user.UserName!),

            new(ClaimTypes.Email, user.Email!),

            new("IsActive", user.IsActive.ToString()),

            new("SecurityStamp", user.SecurityStamp ?? string.Empty)
        };

        claims.AddRange(
            roles.Select(role =>
                new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpirationInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}