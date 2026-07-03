using Equiparts.Models;

namespace Equiparts.Services;

public interface IOrderService
{
    Task<List<Order>> GetOrdersAsync();

    Task<Order?> GetOrderAsync(string orderNo);

    Task PlaceOrderAsync(List<CartItem> cartItems);

    Task CancelOrderAsync(string orderNo);
}