using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Services;

namespace Equiparts.ViewModels;

public partial class ProductDetailViewModel : BaseViewModel, IQueryAttributable
{
    readonly ICartService _cartService;
    readonly INavigationService _navigationService;

    [ObservableProperty]
    private Product? product;

    [ObservableProperty]
    private bool hasError;

    public ProductDetailViewModel(ICartService cartService, INavigationService navigationService)
    {
        _cartService = cartService;
        _navigationService = navigationService;
    }

    // The product listing already fetched the full product record, so the detail
    // page receives it directly through Shell navigation parameters instead of
    // re-requesting it from the API - there is no GET /api/products/{id} endpoint,
    // and this also avoids a duplicate round trip for data we already have.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Product", out var value) && value is Product selectedProduct)
        {
            Product = selectedProduct;
            Title = selectedProduct.Name;
            HasError = false;
        }
        else
        {
            HasError = true;
        }
    }

    [RelayCommand]
    async Task AddToCart()
    {
        if (Product is null || !Product.CanAddToCart)
            return;

        await _cartService.AddToCart(Product.Id, 1);

        await Shell.Current.DisplayAlertAsync(
            "Success",
            $"{Product.Name} added to cart.",
            "OK");
    }

    [RelayCommand]
    async Task Enquire()
    {
        if (Product is null)
            return;

        await Shell.Current.DisplayAlertAsync(
            "Enquiry Sent",
            $"Our team will get back to you with a quote for {Product.Name} shortly.",
            "OK");
    }

    [RelayCommand]
    async Task GoBack()
    {
        await _navigationService.GoBackAsync();
    }
}
