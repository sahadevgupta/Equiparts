using System.Text.Json;
using System.Text.Json.Serialization;

namespace Equiparts.Models.Catalog;

public class ProductResponse
{
    [JsonPropertyName("hsnCode")]
    public string? HsnCode { get; set; }

    [JsonPropertyName("gstRatePercent")]
    public double GstRatePercent { get; set; }

    [JsonPropertyName("stockQuantity")]
    public int StockQuantity { get; set; }

    [JsonPropertyName("unitOfMeasure")]
    public string? UnitOfMeasure { get; set; }

    [JsonPropertyName("images")]
    public List<string>? Images { get; set; }

    [JsonPropertyName("alternatePartNumbers")]
    public List<string>? AlternatePartNumbers { get; set; }

    [JsonPropertyName("attributes")]
    public List<JsonElement>? Attributes { get; set; }

    [JsonPropertyName("fitsMachines")]
    public List<JsonElement>? FitsMachines { get; set; }

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

    [JsonPropertyName("gallery")]
    public List<ProductImageResponse>? Gallery { get; set; }
}

public class ProductImageResponse
{
    [JsonPropertyName("imageId")]
    public int ImageId { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("isPrimary")]
    public bool IsPrimary { get; set; }

    [JsonPropertyName("displayOrder")]
    public int DisplayOrder { get; set; }
}
