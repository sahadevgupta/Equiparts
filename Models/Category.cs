using CommunityToolkit.Mvvm.ComponentModel;

namespace Equiparts.Models;

public partial class Category : ObservableObject
{
    public int CategoryId { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsFeatured { get; set; }
    public string? ImageUrl { get; set; }
    public List<SubCategory>? SubCategories { get; set; }

    public int SubCategoryCount => SubCategories?.Count ?? 0;

    public bool HasSubCategories => SubCategoryCount > 0;

    [ObservableProperty]
    private bool _isSelected;
}