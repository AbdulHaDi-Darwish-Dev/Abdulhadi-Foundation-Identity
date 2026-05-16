namespace Abdulhadi.Foundation.Identity.Infrastructure.Services.Email;

public sealed class ResendOptions
{
    public string FromEmail { get; set; } = null!;
    public string BaseUrl { get; set; } = null!;
}