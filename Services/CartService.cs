using System.Net;
using Equiparts.Configuration.Mapper;
using Equiparts.Interfaces;
using Equiparts.Models;
using Microsoft.Extensions.Logging;
using Refit;

namespace Equiparts.Services;

public class CartService(ICartApi cartApi,
    ILogger<CartService> logger) : ICartService
{
    private readonly List<CartItem> _items = [];

    public IReadOnlyCollection<CartItem> Items => _items;

    public event Action? CartChanged;

    public async Task<IEnumerable<Category>> GetCartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            //await connectivityService.CheckInternetAccessAsync();
            var response = await cartApi.GetCartAsync(cancellationToken);
            if (response is not { Success: true, Data: not null })
                return Enumerable.Empty<Category>();

            return BackendToAppModelMapper.GetCategories(response.Data);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Login rejected by server ({StatusCode}).", apiEx.StatusCode);
            var message = apiEx.StatusCode == HttpStatusCode.Unauthorized
                ? "Invalid email or password."
                : "Login failed. Please try again.";
            return Enumerable.Empty<Category>();
        }
    }

    public async Task<bool> AddToCart(int productId, int quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            //await connectivityService.CheckInternetAccessAsync();
            var response = await cartApi.AddCartItemAsync(new Models.Cart.AddCartItemRequest { ProductId = productId, Quantity = quantity }, cancellationToken);

            var existing = _items.FirstOrDefault(x => x.Product.Id == productId);

            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                _items.Add(new CartItem
                {
                    Product = existing?.Product ?? new(),
                    Quantity = 1
                });
            }

            CartChanged?.Invoke();
            return true;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Login rejected by server ({StatusCode}).", apiEx.StatusCode);
            var message = apiEx.StatusCode == HttpStatusCode.Unauthorized
                ? "Invalid email or password."
                : "Login failed. Please try again.";
            return false;
        }
    }

    public async Task<bool> UpdateCartItemAsync(int cartId, int quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            //await connectivityService.CheckInternetAccessAsync();
            var response = await cartApi.UpdateCartItemAsync(cartId, new Models.Cart.UpdateCartItemRequest { Quantity = quantity }, cancellationToken);
            return true;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Login rejected by server ({StatusCode}).", apiEx.StatusCode);
            var message = apiEx.StatusCode == HttpStatusCode.Unauthorized
                ? "Invalid email or password."
                : "Login failed. Please try again.";
            return false;
        }
    }

    public async Task<bool> RemoveFromCartAsync(int cartId, CancellationToken cancellationToken = default)
    {
        try
        {
            //await connectivityService.CheckInternetAccessAsync();
            var response = await cartApi.RemoveCartItemAsync(cartId, cancellationToken);
            return true;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Login rejected by server ({StatusCode}).", apiEx.StatusCode);
            var message = apiEx.StatusCode == HttpStatusCode.Unauthorized
                ? "Invalid email or password."
                : "Login failed. Please try again.";
            return false;
        }
    }

    public async Task<bool> ClearCartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            //await connectivityService.CheckInternetAccessAsync();
            var response = await cartApi.ClearCartAsync(cancellationToken);
            return true;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Login rejected by server ({StatusCode}).", apiEx.StatusCode);
            var message = apiEx.StatusCode == HttpStatusCode.Unauthorized
                ? "Invalid email or password."
                : "Login failed. Please try again.";
            return false;
        }
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

    public void RemoveFromCart(Product product)
    {
        var item = _items.FirstOrDefault(x => x.Product.Id == product.Id);

        if (item == null)
            return;

        _items.Remove(item);

        CartChanged?.Invoke();
    }

    public void Clear()
    {
        _items.Clear();

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

}