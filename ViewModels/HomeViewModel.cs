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

    public ObservableCollection<Category> Categories { get; } = [];

    public ObservableCollection<Product> BestSellers { get; } = [];

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

        await Task.Delay(50);
        Categories.Clear();


        Banners = new ObservableCollection<Banner>
        {
            new Banner{ ImageUrl = "banner1.png"},
            new Banner{ ImageUrl = "banner2.png"},
            new Banner{ ImageUrl = "banner3.png"},
            new Banner{ ImageUrl = "banner4.png"},
            new Banner{ ImageUrl = "banner5.png"},
            new Banner{ ImageUrl = "banner6.png"},
            new Banner{ ImageUrl = "banner7.png"}
        };


        var categories = await productService.GetCategoriesAsync();

        foreach (var category in categories)
            Categories.Add(category);

        BestSellers.Clear();

        var products = await productService.GetProductsAsync();

        foreach (var product in products.Where(x => x.IsBestSeller))
            BestSellers.Add(product);

        IsBusy = false;
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