using Equiparts.Models;
using Equiparts.Models.Orders;

namespace Equiparts.Configuration.Mapper.Converters;

public class OrderItemResponseToOrderItemConverter : ConverterBase<OrderItemResponse, OrderItem>
{
    protected override OrderItem ConvertImpl(OrderItemResponse source)
    {
        return new OrderItem
        {
            PartNumber = source.PartNumber,
            ProductName = source.ProductName ?? string.Empty,
            UnitPrice = (decimal)source.UnitPrice,
            Quantity = source.Quantity,
            LineTotal = (decimal)source.LineTotal
        };
    }
}
