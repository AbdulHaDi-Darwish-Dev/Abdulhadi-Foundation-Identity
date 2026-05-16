using System.Security.Cryptography;
using Abdulhadi.Foundation.Identity.Domain.Enums;
using Abdulhadi.Foundation.Identity.Domain.Entities;
using Abdulhadi.Foundation.Identity.Application.Security.OTP;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Services;

public class SecurityCodeService : ISecurityCodeService
{
    private readonly ICacheService _cache;
    private readonly IEmailService _email;

    public SecurityCodeService(ICacheService cache, IEmailService email)
    {
        _cache = cache;
        _email = email;
    }

    public async Task SendOtpAsync(ApplicationUser user, OtpType type)
    {
        var email = user.Email!.ToLower().Trim();

        var rateKey = CacheKeys.RateLimit(type, email);
        var otpKey = CacheKeys.Otp(type, email);

        // 🧠 Rate limit (anti abuse)
        var isBlocked = await _cache.GetAsync<bool?>(rateKey);

        if (isBlocked == true)
            throw new Exception("Too many requests");

        await _cache.SetAsync(rateKey, true, TimeSpan.FromMinutes(2));

        // 🔐 Generate OTP
        var otp = GenerateOtp();

        await _cache.SetAsync(
            otpKey,
            new OtpData(otp),
            TimeSpan.FromMinutes(10));

        // 📩 Send email
        await _email.SendVerificationOtpAsync(email, otp);
    }

    public async Task<bool> VerifyOtpAsync(string email, string code, OtpType type)
    {
        email = email.ToLower().Trim();

        var key = CacheKeys.Otp(type, email);

        var stored = await _cache.GetAsync<OtpData>(key);

        if (stored is null)
            return false;

        if (stored.Code != code)
            return false;

        await _cache.RemoveAsync(key);

        return true;
    }
 
    private string GenerateOtp()
    {
        var bytes = new byte[4];
        RandomNumberGenerator.Fill(bytes);

        var value = BitConverter.ToUInt32(bytes, 0) % 900000 + 100000;
        return value.ToString();
    }
}