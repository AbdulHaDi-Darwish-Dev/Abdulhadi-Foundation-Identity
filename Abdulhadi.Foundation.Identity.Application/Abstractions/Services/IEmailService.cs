namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

public interface IEmailService
{
    Task SendVerificationOtpAsync(string to, string code);

    Task SendVerificationLinkAsync(string to, string link);

    Task SendPasswordResetAsync(string to, string link, string? code = null);

    Task SendEmailAsync(string to, string subject, string body);
}