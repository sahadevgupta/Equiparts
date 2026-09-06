using System.Text.Json.Serialization;
using Equiparts.Models.Profile;

namespace Equiparts.Models.Orders;

public class OrderResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("orderNumber")]
    public string OrderNumber { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("orderDate")]
    public DateTime OrderDate { get; set; }

    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    [JsonPropertyName("shippingAddress")]
    public AddressResponse? ShippingAddress { get; set; }

    [JsonPropertyName("items")]
    public List<OrderItemResponse> Items { get; set; } = [];
}
