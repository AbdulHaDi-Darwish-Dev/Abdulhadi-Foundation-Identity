using System.Text.Json.Serialization;

namespace Abdulhadi.Foundation.Identity.Application.DTOs.Response;

public class RefreshTokenResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AccessToken { get; set; } = null;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? AccessTokenExpiresAt { get; set; } = null;


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RefreshToken { get; set; } = null;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? RefreshTokenExpiresAt { get; set; } = null;
}