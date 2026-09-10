namespace Equiparts.Models;

public class Coupon
{
    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal DiscountAmount { get; set; }
}
