using CommunityToolkit.Mvvm.ComponentModel;

namespace Equiparts.Models;

public partial class Category : ObservableObject
{

    public int CategoryId { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsFeatured { get; set; }
    public List<SubCategory>? SubCategories { get; set; }

    [ObservableProperty]
    private bool _isSelected;
}