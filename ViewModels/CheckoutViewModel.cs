using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Controls;
using Equiparts.Enums;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Services;
using Equiparts.Views;
using Mopups.Interfaces;

namespace Equiparts.ViewModels;

public partial class CheckoutViewModel : BaseViewModel
{
    private const int ItemPreviewCount = 3;

    // Free delivery above a threshold, flat fee otherwise - a placeholder business
    // rule until a real shipping-rate API exists.
    private const decimal FreeDeliveryThreshold = 2000m;
    private const decimal FlatDeliveryCharge = 99m;

    private readonly ICartService _cartService;
    private readonly IProfileService _profileService;
    private readonly IOrderService _orderService;
    private readonly ICouponService _couponService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private readonly ILoadingPopupService _loadingPopupService;
    private readonly IPopupNavigation _popupNavigation;

    private List<Address> _addresses = [];

    public ObservableCollection<CartItem> Items { get; } = [];

    public ObservableCollection<PaymentMethod> PaymentMethods { get; } = [];

    [ObservableProperty]
    private Address? selectedAddress;

    [ObservableProperty]
    private PaymentMethod? selectedPaymentMethod;

    [ObservableProperty]
    private IEnumerable<CartItem> displayedItems = [];

    [ObservableProperty]
    private bool showAllItems;

    [ObservableProperty]
    private bool hasMoreItems;

    [ObservableProperty]
    private string couponCode = string.Empty;

    [ObservableProperty]
    private Coupon? appliedCoupon;

    [ObservableProperty]
    private bool isCouponApplying;

    [ObservableProperty]
    private string? couponMessage;

    [ObservableProperty]
    private bool couponMessageIsError;

    [ObservableProperty]
    private OrderSummary summary = new();

    [ObservableProperty]
    private bool isPlacingOrder;

    public string PayButtonLabel => $"Proceed to Pay · ₹{Summary.GrandTotal:N0}";

    public bool HasAddress => SelectedAddress is not null;

    public bool IsCartEmpty => Items.Count == 0;

    public CheckoutViewModel(
        ICartService cartService,
        IProfileService profileService,
        IOrderService orderService,
        ICouponService couponService,
        INavigationService navigationService,
        IDialogService dialogService,
        ILoadingPopupService loadingPopupService,
        IPopupNavigation popupNavigation)
    {
        Title = "Checkout";

        _cartService = cartService;
        _profileService = profileService;
        _orderService = orderService;
        _couponService = couponService;
        _navigationService = navigationService;
        _dialogService = dialogService;
        _loadingPopupService = loadingPopupService;
        _popupNavigation = popupNavigation;

        _cartService.CartChanged += OnCartChanged;
    }

    public override void LoadDataOnNavigatedTo() => _ = InitializeAsync();

    private async Task InitializeAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        try
        {
            using (_loadingPopupService.Show())
            {
                await _cartService.GetCartAsync();
                RefreshItems();

                await LoadAddressesAsync();
                LoadPaymentMethods();

                RecalculateSummary();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnCartChanged()
    {
        RefreshItems();
        RecalculateSummary();
    }

    private void RefreshItems()
    {
        Items.Clear();

        foreach (var item in _cartService.Items)
            Items.Add(item);

        DisplayedItems = ShowAllItems ? Items : Items.Take(ItemPreviewCount);
        HasMoreItems = Items.Count > ItemPreviewCount;

        OnPropertyChanged(nameof(IsCartEmpty));
    }

    private async Task LoadAddressesAsync()
    {
        _addresses = await _profileService.GetAddressesAsync();

        SelectedAddress = _addresses.FirstOrDefault(a => a.IsDefault) ?? _addresses.FirstOrDefault();
    }

    private void LoadPaymentMethods()
    {
        if (PaymentMethods.Count > 0)
            return;

        foreach (var method in PaymentMethod.GetAvailableMethods().Where(m => m.IsActive))
            PaymentMethods.Add(method);

        SelectedPaymentMethod ??= PaymentMethods.FirstOrDefault();
    }

    private void RecalculateSummary()
    {
        var cart = _cartService.Cart;
        var itemTotal = cart?.SubTotal ?? 0;

        Summary = new OrderSummary
        {
            ItemTotal = itemTotal,
            Discount = cart?.DiscountAmount ?? 0,
            CouponDiscount = AppliedCoupon?.DiscountAmount ?? 0,
            DeliveryCharge = CalculateDeliveryCharge(itemTotal),
            Tax = cart?.TaxTotal ?? 0
        };

        OnPropertyChanged(nameof(PayButtonLabel));
    }

    private static decimal CalculateDeliveryCharge(decimal itemTotal) =>
        itemTotal <= 0 || itemTotal >= FreeDeliveryThreshold ? 0 : FlatDeliveryCharge;

    partial void OnSelectedAddressChanged(Address? value) => OnPropertyChanged(nameof(HasAddress));

    [RelayCommand]
    void ToggleShowAllItems()
    {
        ShowAllItems = !ShowAllItems;
        DisplayedItems = ShowAllItems ? Items : Items.Take(ItemPreviewCount);
    }

    [RelayCommand]
    void SelectPaymentMethod(PaymentMethod method)
    {
        if (method is not null)
            SelectedPaymentMethod = method;
    }

    [RelayCommand]
    async Task ChangeAddress()
    {
        var popup = new AddressSelectorPopup(_addresses, SelectedAddress?.Id ?? 0);
        await _popupNavigation.PushAsync(popup);
        var selection = await popup.Result;

        if (selection is null)
            return;

        if (selection.SelectedAddress is not null)
        {
            SelectedAddress = selection.SelectedAddress;
            return;
        }

        if (selection.IsAddNew)
        {
            await OpenAddressEditorAsync(null);
            return;
        }

        if (selection.EditAddress is not null)
            await OpenAddressEditorAsync(selection.EditAddress);
    }

    [RelayCommand]
    async Task AddNewAddress() => await OpenAddressEditorAsync(null);

    private async Task OpenAddressEditorAsync(Address? existing)
    {
        var popup = new AddressEditorPopup(existing);
        await _popupNavigation.PushAsync(popup);
        var formAddress = await popup.Result;

        if (formAddress is null)
            return;

        using (_loadingPopupService.Show())
        {
            var saved = existing is null
                ? await _profileService.AddAddressAsync(formAddress)
                : await _profileService.UpdateAddressAsync(existing.Id, formAddress);

            if (saved is null)
            {
                await _dialogService.ShowAlertAsync("Failed to save address. Please try again.", AlertType.Error);
                return;
            }

            await LoadAddressesAsync();
            SelectedAddress = _addresses.FirstOrDefault(a => a.Id == saved.Id) ?? saved;
        }
    }

    [RelayCommand]
    async Task ApplyCoupon()
    {
        var code = CouponCode?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(code))
        {
            CouponMessage = "Please enter a coupon code.";
            CouponMessageIsError = true;
            return;
        }

        IsCouponApplying = true;
        CouponMessage = null;

        try
        {
            var result = await _couponService.ValidateCouponAsync(code, Summary.ItemTotal);

            if (!result.IsValid || result.Coupon is null)
            {
                AppliedCoupon = null;
                CouponMessage = result.ErrorMessage ?? "Invalid coupon code.";
                CouponMessageIsError = true;
            }
            else
            {
                AppliedCoupon = result.Coupon;
                CouponMessage = result.Coupon.Description ?? "Coupon applied successfully.";
                CouponMessageIsError = false;
            }

            RecalculateSummary();
        }
        finally
        {
            IsCouponApplying = false;
        }
    }

    [RelayCommand]
    void RemoveCoupon()
    {
        AppliedCoupon = null;
        CouponCode = string.Empty;
        CouponMessage = null;
        CouponMessageIsError = false;
        RecalculateSummary();
    }

    [RelayCommand]
    async Task PlaceOrder()
    {
        if (IsPlacingOrder)
            return;

        if (IsCartEmpty)
        {
            await _dialogService.ShowAlertAsync("Your cart is empty.");
            return;
        }

        if (SelectedAddress is null)
        {
            await _dialogService.ShowAlertAsync("Please select a delivery address.");
            return;
        }

        if (SelectedPaymentMethod is null)
        {
            await _dialogService.ShowAlertAsync("Please select a payment method.");
            return;
        }

        IsPlacingOrder = true;

        try
        {
            using (_loadingPopupService.Show())
            {
                var result = await _orderService.PlaceOrderAsync(displayedItems!.ToList(), SelectedAddress.Id, SelectedPaymentMethod.Id, AppliedCoupon?.Code);

                if (result)
                    await _cartService.ClearCartAsync();
                else
                {
                    await _dialogService.ShowAlertAsync("We couldn't place your order. Please try again.", AlertType.Error);
                    return;
                }
            }

            await _dialogService.ShowAlertAsync("Order placed successfully.", AlertType.Success);

            await Shell.Current.GoToAsync("//app/home");
        }
        finally
        {
            IsPlacingOrder = false;
        }
    }

    [RelayCommand]
    async Task GoBack() => await _navigationService.GoBackAsync();
}
