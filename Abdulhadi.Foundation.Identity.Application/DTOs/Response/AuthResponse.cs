using System.Text.Json.Serialization;

namespace Abdulhadi.Foundation.Identity.Application.DTOs.Response;

public class AuthResponse : RefreshTokenResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? RequiresVerification { get; set; } = null;


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; set; } = null;
}