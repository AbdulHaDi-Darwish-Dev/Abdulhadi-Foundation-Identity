namespace Abdulhadi.Foundation.Identity.Application.DTOs.Response;

public class LoginResponse : RefreshTokenResponse
{
    public bool RequiresVerification { get; set; }
    public string? Message { get; set; }
}