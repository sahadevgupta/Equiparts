using System.Text.Json.Serialization;

namespace Equiparts.Models.Catalog;

public record CategoryResponse : BaseCategoryResponse
{
    [JsonPropertyName("children")]
    public List<SubCategoryResponse> SubCategories { get; set; } = [];
}
