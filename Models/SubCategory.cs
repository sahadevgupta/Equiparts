using CommunityToolkit.Mvvm.ComponentModel;

namespace Equiparts.Models;

public partial class SubCategory : ObservableObject
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; }

    public string Image { get; set; }
    public bool ComingSoon { get; set; }

    [ObservableProperty]
    private List<Product> _products = new();
}