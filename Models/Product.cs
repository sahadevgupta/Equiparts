namespace Equiparts.Models;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Sku { get; set; }

    public string? PartNumber { get; set; }

    public decimal Price { get; set; }

    public decimal Mrp { get; set; }

    public double DiscountPercent { get; set; }

    public bool IsBestSeller { get; set; }

    public string? Image { get; set; }

    public string? StockStatus { get; set; }

    public double AverageRating { get; set; }

    public int ReviewCount { get; set; }

    public string? CategoryName { get; set; }

    public string? BrandName { get; set; }

    public bool HasDiscount => DiscountPercent > 0;
}
