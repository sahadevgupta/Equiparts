using Equiparts.Models;
using Equiparts.Models.Catalog;

namespace Equiparts.Configuration.Mapper.Converters;

public class BannerResponseToBannerConverter : ConverterBase<BannerResponse, Banner>
{
    protected override Banner ConvertImpl(BannerResponse source)
    {
        return new Banner
        {
            Id = source.BannerId,
            Title = source.Title,
            Subtitle = source.Subtitle,
            ImageUrl = source.ImageUrl,
            Position = source.Position,
            LinkType = source.LinkType
        };
    }
}
