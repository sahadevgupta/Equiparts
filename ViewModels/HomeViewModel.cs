using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Services;

namespace Equiparts.ViewModels;

public partial class HomeViewModel(ICartService cartService,
    IProductService productService) : BaseViewModel
{

    [ObservableProperty]
    private ObservableCollection<Category>? _categories;

    [ObservableProperty]
    private ObservableCollection<Product>? _bestSellers;

    [ObservableProperty]
    private Category? selectedCategory;


    [ObservableProperty]
    private ObservableCollection<Banner> _banners = [];


    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;


        try
        {
            var categoryTask = productService.GetCategoriesAsync();
            var bannerTask = productService.GetBannersAsync();
            var productTask = productService.GetProductsAsync();

            await Task.WhenAll(categoryTask, bannerTask, productTask);

            var categories = await categoryTask;
            var banners = await bannerTask;
            var products = await productTask;

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Categories = new ObservableCollection<Category>(categories);
                Banners = new ObservableCollection<Banner>(banners);
                BestSellers = new ObservableCollection<Product>(products);
            });
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

    [RelayCommand]
    async Task ProductSelected(Product product)
    {
        if (product == null)
            return;

        // Navigate later
    }

    [RelayCommand]
    async Task AddToCart(Product product)
    {
        if (product == null)
            return;

        cartService.AddToCart(product.Id, 1);

        await Shell.Current.DisplayAlertAsync(
            "Success",
            $"{product.Name} added to cart.",
            "OK");
    }
}