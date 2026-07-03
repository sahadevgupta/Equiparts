using Equiparts.Models;

namespace Equiparts.Services;

public interface ICartService
{
    IReadOnlyCollection<CartItem> Items { get; }

    event Action? CartChanged;

    void AddToCart(Product product);

    void RemoveFromCart(Product product);

    void IncreaseQuantity(Product product);

    void DecreaseQuantity(Product product);

    void Clear();

    decimal GetGrandTotal();

    int GetCartCount();
}