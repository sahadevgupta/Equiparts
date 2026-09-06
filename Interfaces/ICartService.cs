using Equiparts.Models;

namespace Equiparts.Services;

public interface ICartService
{
    IReadOnlyCollection<CartItem> Items { get; }

    event Action? CartChanged;

    Task<IEnumerable<Category>> GetCartAsync(CancellationToken cancellationToken = default);

    Task<bool> AddToCart(int productId, int quantity, CancellationToken cancellationToken = default);

    Task<bool> UpdateCartItemAsync(int cartId, int quantity, CancellationToken cancellationToken = default);

    Task<bool> RemoveFromCartAsync(int cartId, CancellationToken cancellationToken = default);

    Task<bool> ClearCartAsync(CancellationToken cancellationToken = default);

    void IncreaseQuantity(Product product);

    void DecreaseQuantity(Product product);

    void RemoveFromCart(Product product);

    void Clear();

    decimal GetGrandTotal();

    int GetCartCount();
}