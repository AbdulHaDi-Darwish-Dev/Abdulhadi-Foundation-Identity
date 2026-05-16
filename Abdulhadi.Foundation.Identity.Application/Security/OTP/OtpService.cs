using System.Security.Cryptography;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

namespace Abdulhadi.Foundation.Identity.Application.Security.OTP;

public class OtpService : IOtpService
{
    private readonly ICacheService _cache;

    public OtpService(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<string> GenerateAsync(string key, TimeSpan expiry)
    {
        var otp = GenerateOtp();

        await _cache.SetAsync(key, new { Code = otp }, expiry);

        return otp;
    }

    public async Task<bool> VerifyAsync<T>(string key, string code)
    {
        var stored = await _cache.GetAsync<dynamic>(key);

        if (stored == null)
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