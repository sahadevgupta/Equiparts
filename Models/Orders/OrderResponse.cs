using System.Text.Json.Serialization;
using Equiparts.Models.Profile;

namespace Equiparts.Models.Orders;

public class OrderResponse
{
    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }

    [JsonPropertyName("orderNumber")]
    public string OrderNumber { get; set; }

    [JsonPropertyName("orderStatus")]
    public string OrderStatus { get; set; }

    [JsonPropertyName("paymentStatus")]
    public string PaymentStatus { get; set; }

    [JsonPropertyName("paymentMethod")]
    public string PaymentMethod { get; set; }

    [JsonPropertyName("subTotal")]
    public double SubTotal { get; set; }

    [JsonPropertyName("discountAmount")]
    public double DiscountAmount { get; set; }

    [JsonPropertyName("taxAmount")]
    public double TaxAmount { get; set; }

    [JsonPropertyName("shippingCharge")]
    public double ShippingCharge { get; set; }

    [JsonPropertyName("totalAmount")]
    public double TotalAmount { get; set; }

    [JsonPropertyName("placedAtUtc")]
    public DateTime PlacedAtUtc { get; set; }

    [JsonPropertyName("items")]
    public List<OrderItemResponse> Items { get; set; } = [];
}
