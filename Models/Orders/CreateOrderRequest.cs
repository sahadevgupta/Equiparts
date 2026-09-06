using System.Text.Json.Serialization;

namespace Equiparts.Models.Orders;

public class CreateOrderRequest
{
    [JsonPropertyName("addressId")]
    public int AddressId { get; set; }

    [JsonPropertyName("paymentMethod")]
    public string? PaymentMethod { get; set; }

    [JsonPropertyName("items")]
    public List<OrderItemRequest> Items { get; set; } = [];
}
