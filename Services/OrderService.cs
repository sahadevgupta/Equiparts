using System.Text.Json;
using Equiparts.Models;

namespace Equiparts.Services;

public class OrderService : IOrderService
{
    private readonly List<Order> _orders = [];

    public async Task<List<Order>> GetOrdersAsync()
    {
        if (_orders.Any())
            return _orders;

        using var stream = await FileSystem.OpenAppPackageFileAsync("orders.json");

        using var reader = new StreamReader(stream);

        var json = await reader.ReadToEndAsync();

        var orders = JsonSerializer.Deserialize<List<Order>>(json)!;

        _orders.AddRange(orders);

        return _orders;
    }

    public async Task<Order?> GetOrderAsync(string orderNo)
    {
        await GetOrdersAsync();

        return _orders.FirstOrDefault(x => x.OrderNo == orderNo);
    }

    public async Task PlaceOrderAsync(List<CartItem> cartItems)
    {
        await GetOrdersAsync();

        var order = new Order
        {
            OrderNo = $"EQ{DateTime.Now:yyyyMMddHHmmss}",
            Date = DateTime.Now,
            Status = "Processing",
            Items = cartItems,
            Total = cartItems.Sum(x => x.Total)
        };

        _orders.Insert(0, order);

        // Later this will call REST API
    }

    public async Task CancelOrderAsync(string orderNo)
    {
        await GetOrdersAsync();

        var order = _orders.FirstOrDefault(x => x.OrderNo == orderNo);

        if (order != null)
            order.Status = "Cancelled";
    }
}