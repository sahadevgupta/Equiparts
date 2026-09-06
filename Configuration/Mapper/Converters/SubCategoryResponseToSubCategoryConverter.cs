using Equiparts.Models;
using Equiparts.Models.Catalog;

namespace Equiparts.Configuration.Mapper.Converters;

public class SubCategoryResponseToSubCategoryConverter : ConverterBase<SubCategoryResponse, SubCategory>
{
    protected override SubCategory ConvertImpl(SubCategoryResponse source)
    {
        return new SubCategory
        {
            CategoryId = source.CategoryId,
            ParentCategoryId = source.ParentCategoryId,
            Name = source.Name,
            Slug = source.Slug,
            DisplayOrder = source.DisplayOrder,
            IsFeatured = source.IsFeatured,
            Children = source.SubCategories
        };
    }
}
