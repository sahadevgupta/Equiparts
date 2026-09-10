using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Enums;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Services;
using Equiparts.Views;
using System.Collections.ObjectModel;

namespace Equiparts.ViewModels;

public partial class CartViewModel : BaseViewModel
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly ILoadingPopupService _loadingPopupService;
    private readonly IDialogService _dialogService;

    public ObservableCollection<CartItem> Items { get; } = [];

    public decimal SubTotal => _cartService.Cart?.SubTotal ?? 0;

    public decimal TaxTotal => _cartService.Cart?.TaxTotal ?? 0;

    public decimal DiscountAmount => _cartService.Cart?.DiscountAmount ?? 0;

    public bool HasDiscount => _cartService.Cart?.HasDiscount ?? false;

    public decimal GrandTotal => _cartService.Cart?.GrandTotal ?? 0;

    public string ItemCountLabel => Items.Count == 1 ? "1 Item" : $"{Items.Count} Items";

    [ObservableProperty]
    private bool _isListEmpty;

    public CartViewModel(ICartService cartService, IOrderService orderService, ILoadingPopupService loadingPopupService, IDialogService dialogService)
    {
        Title = "My Cart";

        _cartService = cartService;
        _orderService = orderService;
        _loadingPopupService = loadingPopupService;
        _dialogService = dialogService;

        _cartService.CartChanged += RefreshCart;
    }

    private async Task InitializeDataAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        try
        {
            using (_loadingPopupService.Show())
            {
                await _cartService.GetCartAsync();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void RefreshCart()
    {
        Items.Clear();

        foreach (var item in _cartService.Items)
            Items.Add(item);

        OnPropertyChanged(nameof(SubTotal));
        OnPropertyChanged(nameof(TaxTotal));
        OnPropertyChanged(nameof(DiscountAmount));
        OnPropertyChanged(nameof(HasDiscount));
        OnPropertyChanged(nameof(GrandTotal));
        OnPropertyChanged(nameof(ItemCountLabel));

        IsListEmpty = !Items.Any();
    }

    [RelayCommand]
    async Task Increase(CartItem item)
    {
        if (item is null)
            return;

        await _cartService.UpdateCartItemAsync(item.CartItemId, item.Quantity + 1);
    }

    [RelayCommand]
    async Task Decrease(CartItem item)
    {
        if (item is null)
            return;

        if (item.Quantity > 1)
            await _cartService.UpdateCartItemAsync(item.CartItemId, item.Quantity - 1);
        else
            await _cartService.RemoveFromCartAsync(item.CartItemId);
    }

    [RelayCommand]
    async Task Remove(CartItem item)
    {
        if (item is null)
            return;

        await _cartService.RemoveFromCartAsync(item.CartItemId);
    }

    [RelayCommand]
    async Task ClearCart()
    {
        if (!Items.Any())
            return;

        bool confirmed = await _dialogService.ShowConfirmAsync(
            "Remove all items from your cart?",
            "Clear",
            "Cancel");

        if (!confirmed)
            return;

        await _cartService.ClearCartAsync();
    }

    [RelayCommand]
    async Task Checkout()
    {
        if (!_cartService.Items.Any())
        {
            await _dialogService.ShowAlertAsync("Your cart is empty.");

            return;
        }

        await _orderService.PlaceOrderAsync(_cartService.Items.ToList());

        await _cartService.ClearCartAsync();

        await _dialogService.ShowAlertAsync("Order placed successfully.", AlertType.Success);

        await Shell.Current.GoToAsync(nameof(OrdersPage));
    }

    #region [ Override Methods ]

    public override void LoadDataOnNavigatedTo()
    {
        _ = InitializeDataAsync();
    }

    #endregion
}
