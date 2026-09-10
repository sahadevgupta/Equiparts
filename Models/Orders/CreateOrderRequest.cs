using System.Text.Json.Serialization;

namespace Equiparts.Models.Orders;

public class CreateOrderRequest
{
    [JsonPropertyName("shippingAddressId")]
    public int AddressId { get; set; }

    [JsonPropertyName("paymentMethod")]
    public string? PaymentMethod { get; set; }

    [JsonPropertyName("couponCode")]
    public string? CouponCode { get; set; }

    [JsonPropertyName("customerNote")]
    public string? CustomerNote { get; set; }

    [JsonPropertyName("items")]
    public List<OrderItemRequest> Items { get; set; } = [];
}
