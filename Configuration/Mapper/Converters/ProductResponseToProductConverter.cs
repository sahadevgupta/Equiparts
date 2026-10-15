using Equiparts.Models;
using Equiparts.Models.Catalog;

namespace Equiparts.Configuration.Mapper.Converters;

public class ProductResponseToProductConverter : ConverterBase<ProductResponse, Product>
{
    protected override Product ConvertImpl(ProductResponse source)
    {
        var images = BuildImages(source);

        return new Product
        {
            Id = source.ProductId,
            Name = source.Name ?? string.Empty,
            Sku = source.Sku,
            PartNumber = source.PartNumber,
            Price = (decimal)source.SellingPrice,
            Mrp = (decimal)source.Mrp,
            DiscountPercent = source.DiscountPercent,
            IsBestSeller = source.IsFeatured,
            Image = source.PrimaryImageUrl ?? images.FirstOrDefault(),
            Images = images,
            HsnCode = source.HsnCode,
            GstRatePercent = source.GstRatePercent,
            StockQuantity = source.StockQuantity,
            UnitOfMeasure = source.UnitOfMeasure,
            AlternatePartNumbers = source.AlternatePartNumbers ?? [],
            StockStatus = source.StockStatus,
            SalesStatus = source.SalesStatus,
            AverageRating = source.AverageRating,
            ReviewCount = source.ReviewCount,
            CategoryName = source.CategoryName,
            BrandName = source.BrandName,
            TierName = source.TierName
        };
    }

    // Prefers the ordered gallery (primary first), falling back to the plain
    // images list and finally the primary image so older payloads still work.
    static List<string> BuildImages(ProductResponse source)
    {
        var images = (source.Gallery ?? [])
            .Where(g => !string.IsNullOrWhiteSpace(g.ImageUrl))
            .OrderByDescending(g => g.IsPrimary)
            .ThenBy(g => g.DisplayOrder)
            .Select(g => g.ImageUrl!)
            .ToList();

        if (images.Count == 0 && source.Images is { Count: > 0 })
            images = source.Images.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();

        if (images.Count == 0 && !string.IsNullOrWhiteSpace(source.PrimaryImageUrl))
            images.Add(source.PrimaryImageUrl);

        return images.Distinct().ToList();
    }
}
