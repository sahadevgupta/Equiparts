using System.Text.Json.Serialization;

namespace Equiparts.Models.Orders;

// Places an order from the items already in the server-side cart (see ICartApi),
// as opposed to CreateOrderRequest which supplies items explicitly.
public class CheckoutRequest
{
    [JsonPropertyName("addressId")]
    public int AddressId { get; set; }

    [JsonPropertyName("paymentMethod")]
    public string? PaymentMethod { get; set; }
}
