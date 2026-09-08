using System.Text.Json.Serialization;

namespace Equiparts.Models.Auth;

public class RefreshTokenRequest
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
}
