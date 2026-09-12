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

public partial class CategoriesViewModel : BaseViewModel, IQueryAttributable
{
    readonly IProductService _service;
    readonly INavigationService _navigationService;

    int? _pendingCategoryId;

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

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("CategoryId", out var categoryIdValue) && categoryIdValue is int categoryId)
            _pendingCategoryId = categoryId;
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

        // A tap on Home's category grid arrives here as a pending id (set via
        // ApplyQueryAttributes); consumed once so a later plain tab switch
        // still falls back to the first category as before.
        Category? categoryToSelect = null;
        if (_pendingCategoryId is int pendingCategoryId)
        {
            categoryToSelect = Categories.FirstOrDefault(c => c.CategoryId == pendingCategoryId);
            _pendingCategoryId = null;
        }
        categoryToSelect ??= Categories.FirstOrDefault();

        foreach (var category in Categories)
            category.IsSelected = false;

        if (categoryToSelect != null)
            categoryToSelect.IsSelected = true;

        SelectedCategory = categoryToSelect;
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