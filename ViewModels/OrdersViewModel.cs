using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Models;
using Equiparts.Services;
using System.Collections.ObjectModel;

namespace Equiparts.ViewModels;

public partial class OrdersViewModel : BaseViewModel
{
    readonly ProductService _service;

    public ObservableCollection<Order> Orders { get; } = [];

    public OrdersViewModel(ProductService service)
    {
        Title = "Orders";
        _service = service;
    }

    [RelayCommand]
    async Task LoadAsync()
    {
        Orders.Clear();

        var orders = await _service.GetOrdersAsync();

        foreach (var order in orders)
            Orders.Add(order);
    }

    [RelayCommand]
    async Task OpenOrder(Order order)
    {
        // Navigate to order detail
    }
}