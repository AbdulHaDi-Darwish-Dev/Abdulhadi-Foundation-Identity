using System.ComponentModel.DataAnnotations;

namespace Abdulhadi.Foundation.Identity.Application.DTOs.Request;

public class ResendCodeRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = default!;
}