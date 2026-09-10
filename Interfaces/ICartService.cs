using Equiparts.Models;

namespace Equiparts.Services;

public interface ICartService
{
    CartSummary? Cart { get; }

    IReadOnlyCollection<CartItem> Items { get; }

    event Action? CartChanged;

    Task<CartSummary?> GetCartAsync(CancellationToken cancellationToken = default);

    Task<bool> AddToCartAsync(int productId, int quantity, CancellationToken cancellationToken = default);

    Task<bool> UpdateCartItemAsync(int cartItemId, int quantity, CancellationToken cancellationToken = default);

    Task<bool> RemoveFromCartAsync(int cartItemId, CancellationToken cancellationToken = default);

    Task<bool> ClearCartAsync(CancellationToken cancellationToken = default);

    decimal GetGrandTotal();

    int GetCartCount();
}
