using Resend;
using Microsoft.Extensions.Options;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Services.Email;

public sealed class ResendEmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly ResendOptions _options;

    public ResendEmailService(IResend resend, IOptions<ResendOptions> options)
    {
        _resend = resend;
        _options = options.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new EmailMessage
        {
            From = _options.FromEmail,
            Subject = subject,
            HtmlBody = body,
        };

        message.To.Add(to);

        await _resend.EmailSendAsync(message);
    }

    public Task SendVerificationOtpAsync(string to, string code) =>
        SendEmailAsync(
            to,
            subject: "رمز التحقق",
            body: $"""
                   <h2>رمز التحقق</h2>
                   <p>رمزك هو: <strong>{code}</strong></p>
                   <p>صالح لمدة 10 دقائق.</p>
                   """
        );

    public Task SendVerificationLinkAsync(string to, string link) =>
        SendEmailAsync(
            to,
            subject: "تأكيد البريد الإلكتروني",
            body: $"""
                   <h2>تأكيد البريد</h2>
                   
                   <p>اضغط الرابط:</p>
                   <a href="{link}">تأكيد البريد</a>
                   
                   <p>الرابط صالح لمدة 10 دقائق.</p>
                   """
        );

    public Task SendPasswordResetAsync(string to, string link, string? code = null) =>
        SendEmailAsync(
            to,
            subject: "إعادة تعيين كلمة المرور",
            body: $"""
                   <h2>إعادة تعيين كلمة المرور</h2>
                   
                   <p>اضغط الرابط:</p>
                   <a href="{link}">إعادة التعيين</a>
                   
                   {(code is not null ? $"<p>أو استخدم الكود:</p><h1>{code}</h1>" : "")}
                   
                   <p>صالح لمدة 10 دقائق.</p>
                   """
        );
}