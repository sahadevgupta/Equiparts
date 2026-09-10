using Equiparts.Configuration.Mapper;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Models.Auth;
using Equiparts.Models.Cart;
using Microsoft.Extensions.Logging;
using Refit;

namespace Equiparts.Services;

public class CartService(ICartApi cartApi,
    IConnectivityService connectivityService,
    ILogger<CartService> logger) : ICartService
{
    public CartSummary? Cart { get; private set; }

    public IReadOnlyCollection<CartItem> Items => Cart?.Items ?? [];

    public event Action? CartChanged;

    public async Task<CartSummary?> GetCartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync(cancellationToken);
            var response = await cartApi.GetCartAsync(cancellationToken);
            ApplyCartResponse(response);
            return Cart;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to fetch cart ({StatusCode}).", apiEx.StatusCode);
            return null;
        }
    }

    public async Task<bool> AddToCartAsync(int productId, int quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync(cancellationToken);
            var response = await cartApi.AddCartItemAsync(new AddCartItemRequest { ProductId = productId, Quantity = quantity }, cancellationToken);
            return ApplyCartResponse(response);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to add item to cart ({StatusCode}).", apiEx.StatusCode);
            return false;
        }
    }

    public async Task<bool> UpdateCartItemAsync(int cartItemId, int quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync(cancellationToken);
            var response = await cartApi.UpdateCartItemAsync(cartItemId, new UpdateCartItemRequest { Quantity = quantity }, cancellationToken);
            return ApplyCartResponse(response);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to update cart item ({StatusCode}).", apiEx.StatusCode);
            return false;
        }
    }

    public async Task<bool> RemoveFromCartAsync(int cartItemId, CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync(cancellationToken);
            var response = await cartApi.RemoveCartItemAsync(cartItemId, cancellationToken);
            return ApplyCartResponse(response);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to remove cart item ({StatusCode}).", apiEx.StatusCode);
            return false;
        }
    }

    public async Task<bool> ClearCartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync(cancellationToken);
            var response = await cartApi.ClearCartAsync(cancellationToken);

            if (response is not { Success: true })
                return false;

            Cart = null;
            CartChanged?.Invoke();
            return true;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to clear cart ({StatusCode}).", apiEx.StatusCode);
            return false;
        }
    }

    public decimal GetGrandTotal()
    {
        return Cart?.GrandTotal ?? 0;
    }

    public int GetCartCount()
    {
        return Cart?.TotalItems ?? 0;
    }

    private bool ApplyCartResponse(ApiResult<CartResponse>? response)
    {
        if (response is not { Success: true, Data: not null })
            return false;

        Cart = BackendToAppModelMapper.GetCartSummary(response.Data);
        CartChanged?.Invoke();
        return true;
    }
}
