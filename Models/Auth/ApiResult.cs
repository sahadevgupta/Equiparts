using System.Text.Json.Serialization;

namespace Equiparts.Models.Auth;

// Named ApiResult (not ApiResponse) to avoid colliding with Refit's own ApiResponse<T>.
public class ApiResult<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }
}
