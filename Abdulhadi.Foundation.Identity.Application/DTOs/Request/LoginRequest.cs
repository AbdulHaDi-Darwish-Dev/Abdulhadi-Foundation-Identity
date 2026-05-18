using System.ComponentModel.DataAnnotations;

namespace Abdulhadi.Foundation.Identity.Application.DTOs.Request;

public class LoginRequest
{
    [Required(ErrorMessage = "Email or Username is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Email or Username must be between 3 and 100 characters.")]
    public string Identifier { get; set; } = default!;


    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = default!;
}