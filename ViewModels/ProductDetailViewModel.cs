using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Equiparts.Enums;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Services;
using Equiparts.Views;

using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace Equiparts.ViewModels;

public partial class ProductDetailViewModel : BaseViewModel, IQueryAttributable
{
    readonly ICartService _cartService;
    readonly INavigationService _navigationService;
    readonly IDialogService _dialogService;
    readonly IProductService _productService;

    const int SimilarProductsTake = 10;

    [ObservableProperty]
    private Product? product;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasCartItems))]
    private int cartItemCount;

    public bool HasCartItems => CartItemCount > 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSimilarProducts))]
    private IReadOnlyList<SimilarProduct> similarProducts = [];

    [ObservableProperty]
    private bool isLoadingSimilarProducts;

    public bool HasSimilarProducts => SimilarProducts.Count > 0;

    public ProductDetailViewModel(ICartService cartService, INavigationService navigationService, IDialogService dialogService, IProductService productService)
    {
        _cartService = cartService;
        _navigationService = navigationService;
        _dialogService = dialogService;
        _productService = productService;
    }

    // Runs only on push/replace (not when returning from a similar product's page),
    // so the list isn't refetched every time the user comes back.
    public override void LoadDataOnNavigatedTo() => _ = LoadSimilarProductsAsync();

    async Task LoadSimilarProductsAsync()
    {
        if (Product is null || IsLoadingSimilarProducts)
            return;

        IsLoadingSimilarProducts = true;
        try
        {
            var productId = Product.Id;
            var results = await _productService.GetSimilarProductsAsync(productId, SimilarProductsTake);

            // Defensive: never suggest the product the user is already looking at.
            SimilarProducts = results.Where(s => s.Product.Id != productId).ToList();
        }
        catch (Exception)
        {
            // Similar products are supplementary - a failure (e.g. offline) just hides the section.
            SimilarProducts = [];
        }
        finally
        {
            IsLoadingSimilarProducts = false;
        }
    }

    [RelayCommand]
    async Task OpenSimilarProduct(SimilarProduct similarProduct)
    {
        if (similarProduct is null)
            return;

        await _navigationService.NaviagteAsync<ProductDetailPage>(parameters: new Dictionary<string, object>
        {
            ["Product"] = similarProduct.Product
        });
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

        CartItemCount = _cartService.GetCartCount();
    }

    [RelayCommand]
    async Task AddToCart()
    {
        if (Product is null || !Product.CanAddToCart)
            return;

        await _cartService.AddToCartAsync(Product.Id, 1);
        CartItemCount = _cartService.GetCartCount();

        _dialogService.ShowToast($"{Product.Name} added to cart.");
    }

    [RelayCommand]
    async Task Enquire()
    {
        if (Product is null)
            return;

        await _dialogService.ShowAlertAsync(
            $"Our team will get back to you with a quote for {Product.Name} shortly.",
            AlertType.Success);
    }

    [RelayCommand]
    async Task GoBack()
    {
        await _navigationService.GoBackAsync();
    }

    [RelayCommand]
    async Task Share()
    {
        if (Product is null)
            return;

        await Microsoft.Maui.ApplicationModel.DataTransfer.Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = "Share Product",
            Text = $"{Product.Name} (SKU: {Product.Sku})"
        });
    }

    [RelayCommand]
    async Task NotifyStock()
    {
        if (Product is null)
            return;

        await _dialogService.ShowAlertAsync(
            $"We'll let you know as soon as {Product.Name} is back in stock.",
            AlertType.Success);
    }
}
