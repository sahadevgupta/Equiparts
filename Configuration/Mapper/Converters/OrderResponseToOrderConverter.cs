using Equiparts.Models;
using Equiparts.Models.Orders;

namespace Equiparts.Configuration.Mapper.Converters;

public class OrderResponseToOrderConverter : ConverterBase<OrderResponse, Order>
{
    private readonly OrderItemResponseToOrderItemConverter _itemConverter = new();

    protected override Order ConvertImpl(OrderResponse source)
    {
        return new Order
        {
            Id = source.OrderId,
            OrderNo = source.OrderNumber,
            Date = source.PlacedAtUtc,
            Total = (decimal)source.TotalAmount,
            Status = source.OrderStatus,
            Items = source.Items.Select(_itemConverter.Convert).ToList()
        };
    }
}
