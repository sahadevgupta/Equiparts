using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Models;
using Equiparts.Services;
using System.Collections.ObjectModel;

namespace Equiparts.ViewModels;

public partial class ProductListViewModel : BaseViewModel
{
    readonly ProductService _service;

    public ObservableCollection<Product> Products { get; } = [];

    [ObservableProperty]
    private SubCategory? selectedSubCategory;

    public ProductListViewModel(ProductService service)
    {
        _service = service;
    }

    [RelayCommand]
    async Task LoadProducts(SubCategory subCategory)
    {
        SelectedSubCategory = subCategory;

        Products.Clear();

        var products = await _service.GetProductsAsync();

        foreach (var product in products.Where(x => x.SubCategoryId == subCategory.Id))
            Products.Add(product);
    }

    [RelayCommand]
    void AddToCart(Product product)
    {
        // Implement later
    }

    [RelayCommand]
    async Task OpenProduct(Product product)
    {
        // Navigate
    }
}