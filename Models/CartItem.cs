namespace Equiparts.Models;

public class CartItem
{
    public int CartItemId { get; set; }

    public int ProductId { get; set; }

    public string? PartNumber { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public double GstRatePercent { get; set; }

    public decimal LineSubTotal { get; set; }

    public decimal LineTax { get; set; }

    public decimal LineTotal { get; set; }
}
