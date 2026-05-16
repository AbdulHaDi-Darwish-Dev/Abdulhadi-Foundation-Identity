using Abdulhadi.Foundation.Identity.Domain.Enums;
using Abdulhadi.Foundation.Identity.Domain.Entities;

namespace Abdulhadi.Foundation.Identity.Application.Security.OTP;

public interface ISecurityCodeService
{
    Task SendOtpAsync(ApplicationUser user, OtpType type);
    Task<bool> VerifyOtpAsync(string email, string code, OtpType type);
}