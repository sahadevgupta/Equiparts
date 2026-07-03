using Equiparts.Models;

namespace Equiparts.Services;

public class CartService : ICartService
{
    private readonly List<CartItem> _items = [];

    public IReadOnlyCollection<CartItem> Items => _items;

    public event Action? CartChanged;

    public void AddToCart(Product product)
    {
        var existing = _items.FirstOrDefault(x => x.Product.Id == product.Id);

        if (existing != null)
        {
            existing.Quantity++;
        }
        else
        {
            _items.Add(new CartItem
            {
                Product = product,
                Quantity = 1
            });
        }

        CartChanged?.Invoke();
    }

    public void RemoveFromCart(Product product)
    {
        var item = _items.FirstOrDefault(x => x.Product.Id == product.Id);

        if (item == null)
            return;

        _items.Remove(item);

        CartChanged?.Invoke();
    }

    public void IncreaseQuantity(Product product)
    {
        var item = _items.FirstOrDefault(x => x.Product.Id == product.Id);

        if (item == null)
            return;

        item.Quantity++;

        CartChanged?.Invoke();
    }

    public void DecreaseQuantity(Product product)
    {
        var item = _items.FirstOrDefault(x => x.Product.Id == product.Id);

        if (item == null)
            return;

        if (item.Quantity > 1)
            item.Quantity--;
        else
            _items.Remove(item);

        CartChanged?.Invoke();
    }

    public decimal GetGrandTotal()
    {
        return _items.Sum(x => x.Total);
    }

    public int GetCartCount()
    {
        return _items.Sum(x => x.Quantity);
    }

    public void Clear()
    {
        _items.Clear();

        CartChanged?.Invoke();
    }
}