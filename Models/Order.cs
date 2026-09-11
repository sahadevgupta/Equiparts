using Equiparts.Extensions;

namespace Equiparts.Models;

public class Order
{
    public int Id { get; set; }

    public string OrderNo { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public decimal Total { get; set; }

    public string Status { get; set; } = string.Empty;

    public string PaymentStatus { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;

    public decimal SubTotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal ShippingCharge { get; set; }

    public List<OrderItem> Items { get; set; } = [];

    public bool HasDiscount => DiscountAmount > 0;

    public bool IsFreeShipping => ShippingCharge <= 0;

    public bool IsDelivered => string.Equals(Status, "Delivered", StringComparison.OrdinalIgnoreCase)
        || string.Equals(Status, "Completed", StringComparison.OrdinalIgnoreCase);

    public bool IsCancelled => string.Equals(Status, "Cancelled", StringComparison.OrdinalIgnoreCase)
        || string.Equals(Status, "Rejected", StringComparison.OrdinalIgnoreCase);

    public bool IsInProgress => !IsDelivered && !IsCancelled;

    public string StatusIconGlyph => Status switch
    {
        _ when IsDelivered => FontAwesomeIcons.CheckCircle,
        _ when IsCancelled => FontAwesomeIcons.TimesCircle,
        "Shipped" or "OutForDelivery" => FontAwesomeIcons.TruckLoading,
        "Processing" or "Confirmed" => FontAwesomeIcons.Boxes,
        _ => FontAwesomeIcons.Clock
    };

    public bool IsPaymentPaid => string.Equals(PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase);

    public bool IsPaymentFailed => string.Equals(PaymentStatus, "Failed", StringComparison.OrdinalIgnoreCase);

    public bool IsPaymentPending => !IsPaymentPaid && !IsPaymentFailed;

    public string PaymentMethodDisplay => PaymentMethod switch
    {
        "COD" => "Cash on Delivery",
        "UPI" => "UPI",
        "CARD" => "Credit / Debit Card",
        "NETBANKING" => "Net Banking",
        null or "" => "-",
        _ => PaymentMethod
    };

    public string PaymentMethodIconGlyph => PaymentMethod switch
    {
        "COD" => FontAwesomeIcons.MoneyBillWave,
        "CARD" => FontAwesomeIcons.CreditCard,
        "NETBANKING" => FontAwesomeIcons.University,
        "UPI" => FontAwesomeIcons.MobileAlt,
        _ => FontAwesomeIcons.MoneyBillWave
    };
}