using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Equiparts.Interfaces;
using Equiparts.Models;
using Equiparts.Services;
using System.Collections.ObjectModel;

namespace Equiparts.ViewModels;

public partial class CategoriesViewModel : BaseViewModel
{
    readonly IProductService _service;

    public ObservableCollection<Category> Categories { get; } = [];

    public ObservableCollection<SubCategory> SubCategories { get; } = [];

    [ObservableProperty]
    Category? selectedCategory;

    partial void OnSelectedCategoryChanged(Category? value)
    {
        LoadSubCategories(value);
    }

    public CategoriesViewModel(IProductService service)
    {
        Title = "Categories";
        _service = service;
    }

    [RelayCommand]
    async Task LoadAsync()
    {
        Categories.Clear();

        var categories = await _service.GetCategoriesAsync();

        foreach (var category in categories)
            Categories.Add(category);

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
        // Navigate to product listing
    }
}