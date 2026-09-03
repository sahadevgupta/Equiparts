using System.Text.Json.Serialization;

namespace Equiparts.Models.Auth;

// Example payload for an authenticated endpoint - proves the AuthHandler/token-refresh
// pipeline end-to-end. Replace with real authenticated API models as they are added.
public class UserProfileResponse
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("accountType")]
    public string AccountType { get; set; } = string.Empty;

    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = [];
}
