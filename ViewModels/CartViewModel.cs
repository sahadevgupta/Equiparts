using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Services;
using Equiparts.Views;
using System.Collections.ObjectModel;
using System.Linq;

namespace Equiparts.ViewModels;

public partial class CartViewModel : BaseViewModel
{
    private readonly ICartService _cartService;
    private readonly INavigationService _navigationService;
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

    public CartViewModel(ICartService cartService, INavigationService navigationService, ILoadingPopupService loadingPopupService, IDialogService dialogService)
    {
        Title = "My Cart";

        _cartService = cartService;
        _navigationService = navigationService;
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
        var latest = _cartService.Items;

        // Merge in place (rather than Clear()+re-add) so the CollectionView doesn't
        // fully re-render/flicker on every stepper tap and loses scroll position.
        for (int i = Items.Count - 1; i >= 0; i--)
        {
            if (latest.All(l => l.CartItemId != Items[i].CartItemId))
                Items.RemoveAt(i);
        }

        foreach (var item in latest)
        {
            var existing = Items.FirstOrDefault(i => i.CartItemId == item.CartItemId);

            if (existing is null)
                Items.Add(item);
            else
                existing.CopyMutableFieldsFrom(item);
        }

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
        if (item is null || item.IsUpdating)
            return;

        item.IsUpdating = true;

        try
        {
            await _cartService.UpdateCartItemAsync(item.CartItemId, item.Quantity + 1);
        }
        finally
        {
            item.IsUpdating = false;
        }
    }

    [RelayCommand]
    async Task Decrease(CartItem item)
    {
        if (item is null || item.IsUpdating)
            return;

        item.IsUpdating = true;

        try
        {
            if (item.Quantity > 1)
                await _cartService.UpdateCartItemAsync(item.CartItemId, item.Quantity - 1);
            else
                await _cartService.RemoveFromCartAsync(item.CartItemId);
        }
        finally
        {
            item.IsUpdating = false;
        }
    }

    [RelayCommand]
    async Task Remove(CartItem item)
    {
        if (item is null || item.IsUpdating)
            return;

        item.IsUpdating = true;

        try
        {
            await _cartService.RemoveFromCartAsync(item.CartItemId);
        }
        finally
        {
            item.IsUpdating = false;
        }
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

        await _navigationService.NaviagteAsync<CheckoutPage>();
    }

    #region [ Override Methods ]

    public override void LoadDataOnNavigatedTo()
    {
        _ = InitializeDataAsync();
    }

    #endregion
}
