using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Interfaces;
using Equiparts.Models;

namespace Equiparts.ViewModels;

public partial class OrderDetailsViewModel : BaseViewModel, IQueryAttributable
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Order? order;

    [ObservableProperty]
    private bool hasError;

    public OrderDetailsViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    // The orders list already fetched the full order record, so this page receives it
    // directly through Shell navigation parameters instead of re-requesting it from the
    // API, mirroring ProductDetailViewModel.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Order", out var value) && value is Order selectedOrder)
        {
            Order = selectedOrder;
            Title = selectedOrder.OrderNo;
            HasError = false;
        }
        else
        {
            HasError = true;
        }
    }

    [RelayCommand]
    async Task GoBack() => await _navigationService.GoBackAsync();
}
