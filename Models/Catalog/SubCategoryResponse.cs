using System.Text.Json.Serialization;

namespace Equiparts.Models.Catalog;

public record SubCategoryResponse : BaseCategoryResponse
{
    [JsonPropertyName("parentCategoryId")]
    public int ParentCategoryId { get; set; }

    [JsonPropertyName("children")]
    public List<object> SubCategories { get; set; } = [];
}
