using BuildingBlocks.Shared.Core;
using Abdulhadi.Foundation.Identity.Domain.Enums;
using Abdulhadi.Foundation.Identity.Domain.Entities;
using Abdulhadi.Foundation.Identity.Application.Security.OTP;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Services;

public class SecurityCodeService : ISecurityCodeService
{
    private readonly ICacheService _cache;

    private readonly IEmailService _email;

    private readonly IOtpService _otpService;

    public SecurityCodeService(ICacheService cache, IEmailService email, IOtpService otpService)
    {
        _cache = cache;
        _email = email;
        _otpService = otpService;
    }

    public async Task SendOtpAsync(ApplicationUser user, OtpType type)
    {
        var email = user.Email!.ToLower().Trim();

        var rateKey = CacheKeys.RateLimit(type, email);
        var otpKey = CacheKeys.Otp(type, email);

        // 🧠 Rate limit (anti abuse)
        var isBlocked = await _cache.GetAsync<bool?>(rateKey);

        if (isBlocked == true)
            throw new InfrastructureException("Too many requests", ErrorCodes.TooManyRequests);

        await _cache.SetAsync(rateKey, true, TimeSpan.FromMinutes(2));

        // 🔐 Generate OTP
        var otp = await _otpService.GenerateAsync(otpKey, TimeSpan.FromMinutes(10));

        await _cache.SetAsync(
            otpKey,
            new OtpData(otp),
            TimeSpan.FromMinutes(10));

        // 📩 Send email
        await _email.SendVerificationOtpAsync(user.Email, otp);
    }

    public async Task<bool> VerifyOtpAsync(string email, string code, OtpType type)
    {
        email = email.ToLower().Trim();

        var key = CacheKeys.Otp(type, email);

        return await _otpService.VerifyAsync(key, code);
    }
}