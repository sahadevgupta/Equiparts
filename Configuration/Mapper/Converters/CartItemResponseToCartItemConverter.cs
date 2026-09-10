using Equiparts.Models;
using Equiparts.Models.Cart;

namespace Equiparts.Configuration.Mapper.Converters;

public class CartItemResponseToCartItemConverter : ConverterBase<CartItemResponse, CartItem>
{
    protected override CartItem ConvertImpl(CartItemResponse source)
    {
        return new CartItem
        {
            CartItemId = source.CartItemId,
            ProductId = source.ProductId,
            PartNumber = source.PartNumber,
            Name = source.Name ?? string.Empty,
            ImageUrl = source.ImageUrl,
            Quantity = source.Quantity,
            UnitPrice = (decimal)source.UnitPrice,
            GstRatePercent = source.GstRatePercent,
            LineSubTotal = (decimal)source.LineSubTotal,
            LineTax = (decimal)source.LineTax,
            LineTotal = (decimal)source.LineTotal
        };
    }
}
