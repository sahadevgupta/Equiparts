using Equiparts.Models.Auth;
using Equiparts.Models.Cart;
using Refit;

namespace Equiparts.Interfaces;

// Authenticated endpoints: registered WITH AuthHandler.
public interface ICartApi
{
    [Get("/api/cart")]
    Task<ApiResult<CartResponse>> GetCartAsync(CancellationToken cancellationToken = default);

    [Post("/api/cart/items")]
    Task<ApiResult<CartResponse>> AddCartItemAsync([Body] AddCartItemRequest request, CancellationToken cancellationToken = default);

    [Put("/api/cart/items/{cartItemId}")]
    Task<ApiResult<CartResponse>> UpdateCartItemAsync(int cartItemId, [Body] UpdateCartItemRequest request, CancellationToken cancellationToken = default);

    [Delete("/api/cart/items/{cartItemId}")]
    Task<ApiResult<CartResponse>> RemoveCartItemAsync(int cartItemId, CancellationToken cancellationToken = default);

    [Delete("/api/cart")]
    Task<ApiResult<object?>> ClearCartAsync(CancellationToken cancellationToken = default);
}
