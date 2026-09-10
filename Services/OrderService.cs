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

            return BackendToAppModelMapper.GetOrders(response.Data.Items);
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

    public async Task PlaceOrderAsync(List<CartItem> cartItems)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync();

            var request = new CreateOrderRequest
            {
                Items = [.. cartItems.Select(x => new OrderItemRequest
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                })]
            };

            await orderApi.CreateOrderAsync(request);
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to place order ({StatusCode}).", apiEx.StatusCode);
        }
    }

    public async Task CheckOutAsync(List<CartItem> cartItems)
    {
        try
        {
            await connectivityService.CheckInternetAccessAsync();

            var request = new CreateOrderRequest
            {
                Items = [.. cartItems.Select(x => new OrderItemRequest
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                })]
            };

            await orderApi.CreateOrderAsync(request);
        }
        catch (ApiException apiEx)
        {
            logger.LogWarning(apiEx, "Failed to place order ({StatusCode}).", apiEx.StatusCode);
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
