using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Models;
using Equiparts.Services;
using System.Collections.ObjectModel;

namespace Equiparts.ViewModels;

public partial class OrdersViewModel : BaseViewModel
{
    private readonly IOrderService _orderService;

    public ObservableCollection<Order> Orders { get; } = [];

    public OrdersViewModel(IOrderService orderService)
    {
        Title = "Orders";

        _orderService = orderService;
    }

    [RelayCommand]
    public async Task Load()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        Orders.Clear();

        var orders = await _orderService.GetOrdersAsync();

        foreach (var order in orders.OrderByDescending(x => x.Date))
            Orders.Add(order);

        IsBusy = false;
    }

    [RelayCommand]
    async Task ViewOrder(Order order)
    {
        if (order == null)
            return;

        // await Shell.Current.GoToAsync(
        //     $"{nameof(OrderDetailsPage)}",
        //     new Dictionary<string, object>
        //     {
        //         ["Order"] = order
        //     });
    }

    [RelayCommand]
    async Task CancelOrder(Order order)
    {
        if (order == null)
            return;

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Cancel Order",
            "Do you want to cancel this order?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await _orderService.CancelOrderAsync(order.OrderNo);

        await Load();
    }
}