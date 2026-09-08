using CommunityToolkit.Mvvm.ComponentModel;

namespace Equiparts.Models;

public partial class SubCategory : ObservableObject
{
    public int CategoryId { get; set; }
    public int ParentCategoryId { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsFeatured { get; set; }
    public string? ImageUrl { get; set; }
    public List<object>? Children { get; set; }
}