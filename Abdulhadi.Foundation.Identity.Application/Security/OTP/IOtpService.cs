namespace Abdulhadi.Foundation.Identity.Application.Security.OTP;

public interface IOtpService
{
    Task<string> GenerateAsync(string key, TimeSpan expiry);
    Task<bool> VerifyAsync(string key, string code);
}