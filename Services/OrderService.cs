using Equiparts.Configuration.Mapper;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Models.Orders;
using Microsoft.Extensions.Logging;
using Refit;

namespace Equiparts.Services;

public class OrderService(IOrderApi orderApi,
    IConnectivityService connectivityService,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task<List<Order>> GetOrdersAsync()
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync();
            var response = await orderApi.GetOrdersAsync();

            if (response is not { Success: true, Data: not null })
                return [];

            return BackendToAppModelMapper.GetOrders(response.Data);
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to fetch orders ({StatusCode}).", apiEx.StatusCode);
            return [];
        }
    }

    public async Task<Order?> GetOrderAsync(string orderNo)
    {
        var orders = await GetOrdersAsync();

        return orders.FirstOrDefault(x => x.OrderNo == orderNo);
    }

    public async Task PlaceOrderAsync(List<CartItem> cartItems, int addressId, string paymentMethod, string? couponCode = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync();

            var request = new CreateOrderRequest
            {
                AddressId = addressId,
                PaymentMethod = paymentMethod,
                CouponCode = couponCode,
                Items = [.. cartItems.Select(x => new OrderItemRequest
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                })]
            };

            var a = await orderApi.CreateOrderAsync(request);
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to place order ({StatusCode}).", apiEx.StatusCode);
        }
    }

    public async Task<Order?> CheckoutAsync(int addressId, string paymentMethod, string? couponCode = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync(cancellationToken);

            var request = new CheckoutRequest
            {
                AddressId = addressId,
                PaymentMethod = paymentMethod,
                CouponCode = couponCode
            };

            var response = await orderApi.CheckoutAsync(request, cancellationToken);

            if (response is not { Success: true, Data: not null })
                return null;

            return BackendToAppModelMapper.GetOrder(response.Data);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to checkout order ({StatusCode}).", apiEx.StatusCode);
            return null;
        }
    }

    public async Task CancelOrderAsync(string orderNo)
    {
        try
        {
            var order = await GetOrderAsync(orderNo);

            if (order is null)
                return;

            await connectivityService.CheckInternetAccessAsync();
            await orderApi.CancelOrderAsync(order.Id);
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to cancel order ({StatusCode}).", apiEx.StatusCode);
        }
    }
}
