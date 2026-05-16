using Abdulhadi.Foundation.Identity.Domain.Enums;

namespace Abdulhadi.Foundation.Identity.Application.Security.OTP;

public static class CacheKeys
{
    public static string Otp(OtpType type, string email)
        => $"otp:{type}:{email.ToLower().Trim()}";

    public static string RateLimit(OtpType type, string email)
        => $"rate:{type}:{email.ToLower().Trim()}";
}