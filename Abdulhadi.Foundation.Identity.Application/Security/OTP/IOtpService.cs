namespace Abdulhadi.Foundation.Identity.Application.Security.OTP;

public interface IOtpService
{
    Task<string> GenerateAsync(string key, TimeSpan expiry);
    Task<bool> VerifyAsync<T>(string key, string code);
}