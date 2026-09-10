namespace Equiparts.Models;

public class CartSummary
{
    public int CartId { get; set; }

    public List<CartItem> Items { get; set; } = [];

    public int TotalItems { get; set; }

    public decimal SubTotal { get; set; }

    public decimal TaxTotal { get; set; }

    public decimal GrandTotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public bool HasDiscount => DiscountAmount > 0;

    public bool IsEmpty => Items.Count == 0;
}
