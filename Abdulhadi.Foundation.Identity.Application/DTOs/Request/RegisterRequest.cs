using BuildingBlocks.Shared.Core;
using System.ComponentModel.DataAnnotations;

namespace Abdulhadi.Foundation.Identity.Application.DTOs.Request;

public sealed class RegisterRequest
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Username can only contain letters, numbers, hyphens, and underscores.")]
    public string Username { get; set; }


    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
    [RegularExpression(ValidationPatterns.StrongPassword, ErrorMessage = ValidationMessages.StrongPassword)]
    public string Password { get; set; }


    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters.")]
    public string Email { get; set; }

    public RegisterRequest(string username, string password, string email)
    {
        Email = email;
        Username = username;
        Password = password;
    }
}