using Equiparts.Models;
using Equiparts.Models.Catalog;

namespace Equiparts.Configuration.Mapper.Converters;

public class ProductResponseToProductConverter : ConverterBase<ProductResponse, Product>
{
    protected override Product ConvertImpl(ProductResponse source)
    {
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
            Image = source.PrimaryImageUrl,
            StockStatus = source.StockStatus,
            AverageRating = source.AverageRating,
            ReviewCount = source.ReviewCount,
            CategoryName = source.CategoryName,
            BrandName = source.BrandName
        };
    }
}
