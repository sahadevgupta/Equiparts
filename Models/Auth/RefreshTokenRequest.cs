using System.Text.Json.Serialization;

namespace Equiparts.Models.Auth;

public class RefreshTokenRequest
{
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
}
