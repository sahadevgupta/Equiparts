using System.Text.Json.Serialization;

namespace Equiparts.Models.Catalog;

public class ProductResponse
{
    [JsonPropertyName("productId")]
    public int ProductId { get; set; }

    [JsonPropertyName("sku")]
    public string? Sku { get; set; }

    [JsonPropertyName("partNumber")]
    public string? PartNumber { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    [JsonPropertyName("mrp")]
    public double Mrp { get; set; }

    [JsonPropertyName("sellingPrice")]
    public double SellingPrice { get; set; }

    [JsonPropertyName("discountPercent")]
    public double DiscountPercent { get; set; }

    [JsonPropertyName("stockStatus")]
    public string? StockStatus { get; set; }

    [JsonPropertyName("salesStatus")]
    public string? SalesStatus { get; set; }

    [JsonPropertyName("rank")]
    public int Rank { get; set; }

    [JsonPropertyName("isFeatured")]
    public bool IsFeatured { get; set; }

    [JsonPropertyName("averageRating")]
    public double AverageRating { get; set; }

    [JsonPropertyName("reviewCount")]
    public int ReviewCount { get; set; }

    [JsonPropertyName("categoryName")]
    public string? CategoryName { get; set; }

    [JsonPropertyName("brandName")]
    public string? BrandName { get; set; }

    [JsonPropertyName("tierName")]
    public string? TierName { get; set; }

    [JsonPropertyName("primaryImageUrl")]
    public string? PrimaryImageUrl { get; set; }
}
