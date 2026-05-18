namespace Abdulhadi.Foundation.Identity.Application.DTOs.Response;

public class AuthResponse : RefreshTokenResponse
{
    public bool RequiresVerification { get; set; } = false;

    public string? Message { get; set; } = null;
}