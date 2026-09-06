using Equiparts.Models;
using Equiparts.Models.Orders;

namespace Equiparts.Configuration.Mapper.Converters;

public class OrderItemResponseToOrderItemConverter : ConverterBase<OrderItemResponse, OrderItem>
{
    protected override OrderItem ConvertImpl(OrderItemResponse source)
    {
        return new OrderItem
        {
            ProductId = source.ProductId,
            ProductName = source.ProductName,
            ImageUrl = source.ImageUrl,
            UnitPrice = source.UnitPrice,
            Quantity = source.Quantity,
            LineTotal = source.LineTotal
        };
    }
}
