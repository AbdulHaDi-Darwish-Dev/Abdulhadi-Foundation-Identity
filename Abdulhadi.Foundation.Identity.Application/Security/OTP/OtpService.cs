using System.Text;
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
        // 1. توليد الرمز الصريح (مثلاً: "123456")
        var otp = GenerateOtp();

        // 2. تحويل الرمز إلى هاش (مثلاً: "A3F2...")
        var hashedOtp = HashOtp(otp);

        // 3. تخزين الهاش في الكاش وليس الرمز الصريح
        await _cache.SetAsync(key, hashedOtp, expiry);

        // 4. إرجاع الرمز الصريح فقط لكي يتم إرساله عبر الإيميل للمستخدم
        return otp;
    }

    public async Task<bool> VerifyAsync(string key, string code)
    {
        // 1. جلب الهاش المخزن من الكاش
        var storedHash = await _cache.GetAsync<string>(key);

        if (string.IsNullOrEmpty(storedHash))
            return false;

        // 2. تحويل الكود المدخل من المستخدم إلى هاش لمقارنته بالهاش المخزن
        var inputHash = HashOtp(code);

        // 3. المقارنة بين الهاشين
        if (storedHash != inputHash)
            return false;

        // 4. مسح الرمز من الكاش بعد التحقق الناجح لمنع إعادة استخدامه
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

    private string HashOtp(string otp)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(otp));
        return Convert.ToHexString(bytes);
    }
}