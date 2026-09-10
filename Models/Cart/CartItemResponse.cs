using System.Text.Json.Serialization;

namespace Equiparts.Models.Cart;

public class CartItemResponse
{
    [JsonPropertyName("cartItemId")]
    public int CartItemId { get; set; }

    [JsonPropertyName("productId")]
    public int ProductId { get; set; }

    [JsonPropertyName("partNumber")]
    public string? PartNumber { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public double UnitPrice { get; set; }

    [JsonPropertyName("gstRatePercent")]
    public double GstRatePercent { get; set; }

    [JsonPropertyName("lineSubTotal")]
    public double LineSubTotal { get; set; }

    [JsonPropertyName("lineTax")]
    public double LineTax { get; set; }

    [JsonPropertyName("lineTotal")]
    public double LineTotal { get; set; }
}
