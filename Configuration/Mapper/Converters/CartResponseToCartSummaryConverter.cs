using Equiparts.Models;
using Equiparts.Models.Cart;

namespace Equiparts.Configuration.Mapper.Converters;

public class CartResponseToCartSummaryConverter : ConverterBase<CartResponse, CartSummary>
{
    private readonly CartItemResponseToCartItemConverter _itemConverter = new();

    protected override CartSummary ConvertImpl(CartResponse source)
    {
        return new CartSummary
        {
            CartId = source.CartId,
            Items = source.Items.Select(_itemConverter.Convert).ToList(),
            TotalItems = source.TotalItems,
            SubTotal = (decimal)source.SubTotal,
            TaxTotal = (decimal)source.TaxTotal,
            GrandTotal = (decimal)source.GrandTotal,
            DiscountAmount = (decimal)source.DiscountAmount
        };
    }
}
