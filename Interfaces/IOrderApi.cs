using Equiparts.Models.Auth;
using Equiparts.Models.Catalog;
using Equiparts.Models.Orders;
using Refit;

namespace Equiparts.Interfaces;

// Authenticated endpoints: registered WITH AuthHandler.
public interface IOrderApi
{
    [Post("/api/orders")]
    Task<ApiResult<OrderResponse>> CreateOrderAsync([Body] CreateOrderRequest request, CancellationToken cancellationToken = default);

    [Post("/api/orders/checkout")]
    Task<ApiResult<OrderResponse>> CheckoutAsync([Body] CheckoutRequest request, CancellationToken cancellationToken = default);

    [Get("/api/orders")]
    Task<ApiResult<PagedResult<OrderResponse>>> GetOrdersAsync(
        [Query] int page = 1,
        [Query] int pageSize = 20,
        CancellationToken cancellationToken = default);

    [Get("/api/orders/{orderId}")]
    Task<ApiResult<OrderResponse>> GetOrderAsync(int orderId, CancellationToken cancellationToken = default);

    [Post("/api/orders/{orderId}/cancel")]
    Task<ApiResult<OrderResponse>> CancelOrderAsync(int orderId, CancellationToken cancellationToken = default);
}
