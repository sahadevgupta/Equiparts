using System.Text.Json.Serialization;

namespace Equiparts.Models.Catalog;

// The /similar endpoint returns full product records plus why each one matched.
public class SimilarProductResponse : ProductResponse
{
    [JsonPropertyName("matchType")]
    public string? MatchType { get; set; }

    [JsonPropertyName("matchReason")]
    public string? MatchReason { get; set; }
}
