using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Services;
using Equiparts.Views;
using System.Collections.ObjectModel;

namespace Equiparts.ViewModels;

public partial class OrdersViewModel : BaseViewModel
{
    private readonly IOrderService _orderService;
    private readonly IDialogService _dialogService;
    private readonly ILoadingPopupService _loadingPopupService;
    private readonly INavigationService _navigationService;

    public ObservableCollection<Order> Orders { get; } = [];

    [ObservableProperty]
    private bool _isListEmpty;

    public OrdersViewModel(IOrderService orderService, IDialogService dialogService, ILoadingPopupService loadingPopupService, INavigationService navigationService)
    {
        Title = "Orders";

        _orderService = orderService;
        _dialogService = dialogService;
        _loadingPopupService = loadingPopupService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    public async Task Load()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        try
        {
            Orders.Clear();

            using (_loadingPopupService.Show())
            {
                var orders = await _orderService.GetOrdersAsync();

                foreach (var order in orders.OrderByDescending(x => x.Date))
                    Orders.Add(order);
            }

            IsListEmpty = !Orders.Any();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task ViewOrder(Order order)
    {
        if (order == null)
            return;

        await _navigationService.NaviagteAsync<OrderDetailsPage>(parameters: new Dictionary<string, object>
        {
            ["Order"] = order
        });
    }

    [RelayCommand]
    async Task CancelOrder(Order order)
    {
        if (order == null)
            return;

        bool confirm = await _dialogService.ShowConfirmAsync(
            "Do you want to cancel this order?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await _orderService.CancelOrderAsync(order.OrderNo);

        await Load();
    }

    public override void LoadDataOnNavigatedTo()
    {
        try
        {
            _ = Load();
        }
        catch (Exception)
        {

        }
    }
}