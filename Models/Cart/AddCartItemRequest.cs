using System.Text.Json.Serialization;

namespace Equiparts.Models.Cart;

public class AddCartItemRequest
{
    [JsonPropertyName("productId")]
    public int ProductId { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}
