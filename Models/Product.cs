using CommunityToolkit.Mvvm.ComponentModel;

namespace Equiparts.Models;

public class Product
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public int SubCategoryId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public bool IsBestSeller { get; set; }

    public string Image { get; set; }
}