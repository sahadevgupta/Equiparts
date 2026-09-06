using System.Text.Json.Serialization;

namespace Equiparts.Models.Cart;

public class CartResponse
{
    [JsonPropertyName("items")]
    public List<CartItemResponse> Items { get; set; } = [];

    [JsonPropertyName("cartId")]
    public int CartId { get; set; }

    [JsonPropertyName("totalItems")]
    public int TotalItems { get; set; }

    [JsonPropertyName("subTotal")]
    public double SubTotal { get; set; }

    [JsonPropertyName("taxTotal")]
    public double TaxTotal { get; set; }

    [JsonPropertyName("grandTotal")]
    public double GrandTotal { get; set; }

    [JsonPropertyName("discountAmount")]
    public double DiscountAmount { get; set; }
}
