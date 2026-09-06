using System.Text.Json.Serialization;

namespace Equiparts.Models.Cart;

public class UpdateCartItemRequest
{
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}
