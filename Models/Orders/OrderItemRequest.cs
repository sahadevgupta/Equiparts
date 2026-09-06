using System.Text.Json.Serialization;

namespace Equiparts.Models.Orders;

public class OrderItemRequest
{
    [JsonPropertyName("productId")]
    public int ProductId { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
}
