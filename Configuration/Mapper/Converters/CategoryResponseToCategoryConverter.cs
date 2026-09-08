using Equiparts.Models;
using Equiparts.Models.Catalog;

namespace Equiparts.Configuration.Mapper.Converters;

public class CategoryResponseToCategoryConverter : ConverterBase<CategoryResponse, Category>
{
    private readonly SubCategoryResponseToSubCategoryConverter _subCategoryConverter = new();

    protected override Category ConvertImpl(CategoryResponse source)
    {
        return new Category
        {
            CategoryId = source.CategoryId,
            Name = source.Name,
            Slug = source.Slug,
            DisplayOrder = source.DisplayOrder,
            IsFeatured = source.IsFeatured,
            ImageUrl = source.ImageUrl,
            SubCategories = source.SubCategories.Select(_subCategoryConverter.Convert).ToList()
        };
    }
}
