using System.ComponentModel.DataAnnotations;

namespace Abdulhadi.Foundation.Identity.Application.DTOs.Request;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Access token is required.")]
    public string AccessToken { get; set; } = default!;


    [Required(ErrorMessage = "Refresh token is required.")]
    public string RefreshToken { get; set; } = default!;
}