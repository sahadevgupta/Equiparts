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
    private ObservableCollection<Product>? _bestSellers;

    [ObservableProperty]
    private Category? selectedCategory;


    [ObservableProperty]
    private ObservableCollection<Banner> _banners = [];

    [ObservableProperty]
    private bool _isRefreshing;
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
                await UpdateDashboardDataAsync();
            }
            else
            {
                using (loadingPopupService.Show())
                {
                    var categoryTask = productService.GetCategoriesAsync();
                    var bannerTask = productService.GetBannersAsync();
                    var productTask = productService.GetProductsAsync();

                    await Task.WhenAll(categoryTask, bannerTask, productTask);

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
            BestSellers = new ObservableCollection<Product>(products.Take(4));
        });
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
    async Task AddToCart(Product product)
    {
        if (product == null)
            return;

        await cartService.AddToCartAsync(product.Id, 1);

        dialogService.ShowToast($"{product.Name} added to cart.");
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