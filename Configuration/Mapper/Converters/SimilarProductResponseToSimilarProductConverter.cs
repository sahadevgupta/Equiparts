using Equiparts.Models;
using Equiparts.Models.Catalog;

namespace Equiparts.Configuration.Mapper.Converters;

public class SimilarProductResponseToSimilarProductConverter : ConverterBase<SimilarProductResponse, SimilarProduct>
{
    readonly ProductResponseToProductConverter _productConverter = new();

    protected override SimilarProduct ConvertImpl(SimilarProductResponse source)
    {
        return new SimilarProduct
        {
            Product = _productConverter.Convert(source),
            MatchType = source.MatchType,
            MatchReason = source.MatchReason
        };
    }
}
