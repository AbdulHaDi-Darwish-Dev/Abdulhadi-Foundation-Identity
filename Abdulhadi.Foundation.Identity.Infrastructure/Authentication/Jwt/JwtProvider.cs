using System.Text;
using System.Security.Claims;
using BuildingBlocks.Shared.Core;
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

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            ValidateLifetime = false, // 🛑 هذه هي النقطة السحرية: نتجاهل كون التوكن منتهي الصلاحية لقراءة الـ Claims منه
            ValidIssuer = _options.Issuer,
            ValidAudience = _options.Audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            // التأكد من أن التوكن مشفر بخوارزمية الـ JWT الصحيحة (مثلاً HmacSha256) لمنع هجمات التلاعب
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }

        catch
        {
            return null; // إذا كان التوكن تالفاً أو غير صحيح نهائياً
        }
    }

    public (string RawToken, RefreshToken RefreshToken) CreateRefreshToken(Guid userId)
    {
        // 1. توليد التوكن الصريح الآمن (الذي سيرسل للمستخدم)
        var rawToken = CryptoHelper.GenerateSecureRandomString();

        // 2. تحويل التوكن الصريح إلى هاش فوراً لحفظه بأمان
        var hashedToken = CryptoHelper.HashText(rawToken);

        // 3. إنشاء كائن الـ RefreshToken بالهاش المشفر ليُحفظ في الداتابيز
        var refreshToken = RefreshToken.Create(userId, hashedToken, expiryDays: 7);

        return (rawToken, refreshToken);
    }
}