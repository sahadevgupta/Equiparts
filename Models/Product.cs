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

    public string? SalesStatus { get; set; }

    public double AverageRating { get; set; }

    public int ReviewCount { get; set; }

    public string? CategoryName { get; set; }

    public string? BrandName { get; set; }

    public string? TierName { get; set; }

    public bool HasDiscount => DiscountPercent > 0;

    public bool HasValidPrice => Price > 0;

    public bool HasRating => ReviewCount > 0 && AverageRating > 0;

    public bool HasBrandInfo => !string.IsNullOrWhiteSpace(TierName) || !string.IsNullOrWhiteSpace(BrandName);

    public bool IsOutOfStock => string.Equals(StockStatus, "OutOfStock", StringComparison.OrdinalIgnoreCase);

    public bool IsLowStock => string.Equals(StockStatus, "LowStock", StringComparison.OrdinalIgnoreCase);

    public bool IsInStock => !string.IsNullOrEmpty(StockStatus) && !IsOutOfStock && !IsLowStock;

    // "NotSold" products (e.g. quote-only lubricants) never have a purchasable price,
    // so the UI must fall back to an enquiry flow instead of Add To Cart.
    public bool RequiresEnquiry => !HasValidPrice || string.Equals(SalesStatus, "NotSold", StringComparison.OrdinalIgnoreCase);

    public bool CanAddToCart => !IsOutOfStock && !RequiresEnquiry;

    public string StockStatusDisplay => StockStatus switch
    {
        "OutOfStock" => "Out of Stock",
        "InStock" => "In Stock",
        "LowStock" => "Low Stock",
        null or "" => string.Empty,
        _ => StockStatus
    };
}
