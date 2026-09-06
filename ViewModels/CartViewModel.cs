using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Models;
using Equiparts.Services;
using System.Collections.ObjectModel;

namespace Equiparts.ViewModels;

public partial class CartViewModel : BaseViewModel
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;

    public ObservableCollection<CartItem> Items { get; } = [];

    public decimal GrandTotal => _cartService?.GetGrandTotal() ?? 0;

    public CartViewModel(ICartService cartService, IOrderService orderService)
    {
        _cartService = cartService;
        _orderService = orderService;

        //RefreshCart();

        //_cartService.CartChanged += RefreshCart;
    }

    private async Task InitializeDataAsync()
    {
        try
        {
            await _cartService.GetCartAsync();
        }
        catch (Exception ex)
        {

        }
    }

    private void RefreshCart()
    {
        Items.Clear();

        foreach (var item in _cartService.Items)
            Items.Add(item);

        OnPropertyChanged(nameof(GrandTotal));
    }

    [RelayCommand]
    void Increase(CartItem item)
    {
        _cartService.IncreaseQuantity(item.Product);

        OnPropertyChanged(nameof(GrandTotal));
    }

    [RelayCommand]
    void Decrease(CartItem item)
    {
        if (item.Quantity > 1)
        {
            _cartService.DecreaseQuantity(item.Product);
        }


        OnPropertyChanged(nameof(GrandTotal));
    }

    [RelayCommand]
    void Remove(CartItem item)
    {
        _cartService.RemoveFromCart(item.Product);

        OnPropertyChanged(nameof(GrandTotal));
    }

    [RelayCommand]
    async Task Checkout()
    {
        if (!_cartService.Items.Any())
        {
            await Shell.Current.DisplayAlertAsync(
                "Cart",
                "Your cart is empty.",
                "OK");

            return;
        }

        await _orderService.PlaceOrderAsync(_cartService.Items.ToList());

        _cartService.Clear();

        await Shell.Current.DisplayAlertAsync(
            "Success",
            "Order placed successfully.",
            "OK");

        await Shell.Current.GoToAsync("//orders");
    }

    #region [ Override Methods ]

    public override void LoadDataOnNavigatedTo()
    {
        InitializeDataAsync();
    }

    #endregion
}