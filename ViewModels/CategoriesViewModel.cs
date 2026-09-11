using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Configuration;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Services;
using Equiparts.Views;
using System.Collections.ObjectModel;
using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;

namespace Equiparts.ViewModels;

public partial class CategoriesViewModel : BaseViewModel
{
    readonly IProductService _service;
    readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<Category> _categories = [];

    [ObservableProperty]
    private ObservableCollection<SubCategory> _subCategories = [];

    [ObservableProperty]
    Category? selectedCategory;

    partial void OnSelectedCategoryChanged(Category? value)
    {
        LoadSubCategories(value);
    }

    public CategoriesViewModel(IProductService service, INavigationService navigationService)
    {
        Title = "Categories";
        _service = service;
        _navigationService = navigationService;
    }

    [RelayCommand]
    async Task LoadAsync()
    {
        Categories.Clear();

        if (AppConfiguration.Categories != null && AppConfiguration.Categories.Any())
        {
            Categories = new ObservableCollection<Category>(AppConfiguration.Categories);
        }
        else
        {
            var categories = await _service.GetCategoriesAsync();

            foreach (var category in categories)
                Categories.Add(category);
        }
        Categories.First().IsSelected = true;
        SelectedCategory = Categories.FirstOrDefault();
    }

    [RelayCommand]
    private void CategorySelected(Category category)
    {
        if (category.Name == SelectedCategory?.Name)
            return;

        var alreadySelectedCategory = Categories.FirstOrDefault(c => c.IsSelected);
        if (alreadySelectedCategory is not null)
        {
            alreadySelectedCategory.IsSelected = false;
        }
        SelectedCategory = category;
        category.IsSelected = true;

    }

    void LoadSubCategories(Category? category)
    {
        SubCategories.Clear();

        if (category == null)
            return;

        foreach (var item in category.SubCategories)
            SubCategories.Add(item);
    }

    [RelayCommand]
    async Task OpenSubCategory(SubCategory subCategory)
    {
        if (subCategory == null)
            return;

        await _navigationService.NaviagteAsync<ProductPage>(parameters: new Dictionary<string, object>
        {
            ["CategoryId"] = subCategory.CategoryId,
            ["Title"] = subCategory.Name ?? "Products"
        });
    }
}