using System.Text.Json.Serialization;

namespace Equiparts.Models.Catalog;

public class BannerResponse
{
    [JsonPropertyName("bannerId")]
    public int BannerId { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("subtitle")]
    public string? Subtitle { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("position")]
    public string? Position { get; set; }

    [JsonPropertyName("linkType")]
    public string? LinkType { get; set; }
}
