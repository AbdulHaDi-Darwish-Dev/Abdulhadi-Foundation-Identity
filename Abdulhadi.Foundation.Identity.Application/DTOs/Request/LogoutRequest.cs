using System.ComponentModel.DataAnnotations;

namespace Abdulhadi.Foundation.Identity.Application.DTOs.Request;

public sealed class LogoutRequest
{
    [Required(ErrorMessage = "Refresh token is required for logout.")]
    [StringLength(int.MaxValue, MinimumLength = 30, ErrorMessage = "Refresh token format is invalid.")]
    public string RefreshToken { get; set; } = null!;
}