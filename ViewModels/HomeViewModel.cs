using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Configuration;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Services;
using Equiparts.Views;

namespace Equiparts.ViewModels;

public partial class HomeViewModel(ICartService cartService,
    IProductService productService,
    ILoadingPopupService loadingPopupService,
    INavigationService navigationService,
    IDialogService dialogService) : BaseViewModel
{

    [ObservableProperty]
    private ObservableCollection<Category>? _categories;

    [ObservableProperty]
    private ObservableCollection<BestSellerItem>? _bestSellers;

    [ObservableProperty]
    private ObservableCollection<Banner> _banners = [];

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private string? _searchQuery;

    private IEnumerable<Category> categories;
    private IEnumerable<Banner> banners;
    private IEnumerable<Product> products;

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;


        try
        {
            if (categories != null || banners != null || products != null)
            {
                // Cart contents can change elsewhere (Cart tab, product detail) between
                // visits to Home, so it's refreshed here even on the cached fast-path
                // to keep each best-seller's Add-to-Cart/stepper state correct.
                await cartService.GetCartAsync();
                await UpdateDashboardDataAsync();
            }
            else
            {
                using (loadingPopupService.Show())
                {
                    var categoryTask = productService.GetCategoriesAsync();
                    var bannerTask = productService.GetBannersAsync();
                    var productTask = productService.GetProductsAsync();
                    var cartTask = cartService.GetCartAsync();

                    await Task.WhenAll(categoryTask, bannerTask, productTask, cartTask);

                    categories = await categoryTask;
                    banners = await bannerTask;
                    products = await productTask;
                    await UpdateDashboardDataAsync();
                }
            }
        }
        catch (Exception ex)
        {
            // Log exception
            // App.Logger?.LogError(ex, "Failed to load product data");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task UpdateDashboardDataAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            AppConfiguration.Categories = categories;
            Categories = new ObservableCollection<Category>(categories);
            Banners = new ObservableCollection<Banner>(banners);
            BestSellers = new ObservableCollection<BestSellerItem>(products
                .Where(p => p.IsBestSeller && p.IsInStock)
                .Select(CreateBestSellerItem));
        });
    }

    private BestSellerItem CreateBestSellerItem(Product product)
    {
        var item = new BestSellerItem(product);

        var cartItem = cartService.Items.FirstOrDefault(i => i.ProductId == product.Id);
        if (cartItem != null)
        {
            item.CartItemId = cartItem.CartItemId;
            item.Quantity = cartItem.Quantity;
            item.IsInCart = true;
        }

        return item;
    }

    [RelayCommand]
    async Task ProductSelected(Product product)
    {
        if (product == null)
            return;

        await navigationService.NaviagteAsync<ProductDetailPage>(parameters: new Dictionary<string, object>
        {
            ["Product"] = product
        });
    }

    [RelayCommand]
    async Task ViewAllProducts()
    {
        await navigationService.NaviagteAsync<ProductPage>(parameters: new Dictionary<string, object>
        {
            ["Title"] = "All Products"
        });
    }

    [RelayCommand]
    async Task AddToCart(BestSellerItem item)
    {
        if (item == null || item.IsInCart)
            return;

        var success = await cartService.AddToCartAsync(item.Product.Id, 1);
        if (!success)
            return;

        var cartItem = cartService.Items.FirstOrDefault(i => i.ProductId == item.Product.Id);
        item.CartItemId = cartItem?.CartItemId;
        item.Quantity = cartItem?.Quantity ?? 1;
        item.IsInCart = true;

        dialogService.ShowToast($"{item.Product.Name} added to cart.");
    }

    [RelayCommand]
    async Task IncreaseQuantity(BestSellerItem item)
    {
        if (item?.CartItemId is not int cartItemId)
            return;

        if (await cartService.UpdateCartItemAsync(cartItemId, item.Quantity + 1))
            item.Quantity++;
    }

    [RelayCommand]
    async Task DecreaseQuantity(BestSellerItem item)
    {
        if (item?.CartItemId is not int cartItemId)
            return;

        if (item.Quantity > 1)
        {
            if (await cartService.UpdateCartItemAsync(cartItemId, item.Quantity - 1))
                item.Quantity--;
        }
        else if (await cartService.RemoveFromCartAsync(cartItemId))
        {
            item.IsInCart = false;
            item.Quantity = 1;
            item.CartItemId = null;
        }
    }

    [RelayCommand]
    async Task CategorySelected(Category category)
    {
        if (category == null)
            return;

        await Shell.Current.GoToAsync("//app/catalog", new Dictionary<string, object>
        {
            ["CategoryId"] = category.CategoryId
        });
    }

    [RelayCommand]
    async Task Search()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            await ViewAllProducts();
            return;
        }

        await navigationService.NaviagteAsync<ProductPage>(parameters: new Dictionary<string, object>
        {
            ["Title"] = "Search Results",
            ["SearchText"] = SearchQuery
        });
    }

    [RelayCommand]
    async Task Refresh()
    {
        IsRefreshing = true;

        try
        {
            await LoadAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }
}

// Mirrors this product's cart state (in cart? at what quantity/cart-item id?)
// for the best-seller card's Add-to-Cart/stepper toggle, since Product itself
// carries no cart-specific state.
public partial class BestSellerItem(Product product) : ObservableObject
{
    public Product Product => product;

    [ObservableProperty]
    private int _quantity = 1;

    [ObservableProperty]
    private bool _isInCart;

    public int? CartItemId { get; set; }
}