using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Services;
using System.Collections.ObjectModel;

namespace Equiparts.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly IProductService _service;

    public ObservableCollection<Category> Categories { get; } = [];

    public ObservableCollection<Product> BestSellers { get; } = [];

    [ObservableProperty]
    private Category? selectedCategory;

    public HomeViewModel(IProductService service)
    {
        Title = "Home";
        _service = service;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        Categories.Clear();

        var categories = await _service.GetCategoriesAsync();

        foreach (var category in categories)
            Categories.Add(category);

        BestSellers.Clear();

        var products = await _service.GetProductsAsync();

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
    void AddToCart(Product product)
    {
        // Implement later
    }
}