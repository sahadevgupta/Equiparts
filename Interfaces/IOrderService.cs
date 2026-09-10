using Equiparts.Models;

namespace Equiparts.Services;

public interface IOrderService
{
    Task<List<Order>> GetOrdersAsync();

    Task<Order?> GetOrderAsync(string orderNo);

    Task PlaceOrderAsync(List<CartItem> cartItems, int addressId, string paymentMethod, string? couponCode = null, CancellationToken cancellationToken = default);

    // Places an order from the server-side cart via IOrderApi.CheckoutAsync, rather
    // than sending items explicitly like PlaceOrderAsync does.
    Task<Order?> CheckoutAsync(int addressId, string paymentMethod, string? couponCode = null, CancellationToken cancellationToken = default);

    Task CancelOrderAsync(string orderNo);
}