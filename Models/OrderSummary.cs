namespace Equiparts.Models;

public class OrderSummary
{
    public decimal ItemTotal { get; set; }

    public decimal Discount { get; set; }

    public decimal CouponDiscount { get; set; }

    public decimal DeliveryCharge { get; set; }

    public decimal Tax { get; set; }

    public decimal GrandTotal =>
        Math.Max(0, ItemTotal - Discount - CouponDiscount + DeliveryCharge + Tax);

    public bool HasDiscount => Discount > 0;

    public bool HasCouponDiscount => CouponDiscount > 0;

    public bool HasDeliveryCharge => DeliveryCharge > 0;

    public bool IsFreeDelivery => !HasDeliveryCharge;
}
