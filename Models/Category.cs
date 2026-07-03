using CommunityToolkit.Mvvm.ComponentModel;

namespace Equiparts.Models;

public partial class Category : ObservableObject
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Image { get; set; }

    public List<SubCategory> SubCategories { get; set; } = [];

    [ObservableProperty]
    private bool _isSelected;
}