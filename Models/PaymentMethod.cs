using Equiparts.Extensions;

namespace Equiparts.Models;

public class PaymentMethod
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string IconGlyph { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    // Static for now since there's no backend endpoint listing supported payment
    // methods; swap this for an API-backed IPaymentMethodService call if/when one
    // exists without touching CheckoutViewModel's usage of PaymentMethod itself.
    public static List<PaymentMethod> GetAvailableMethods() =>
    [
        new()
        {
            Id = "UPI",
            Name = "UPI",
            Description = "Pay instantly using any UPI app",
            IconGlyph = FontAwesomeIcons.MobileAlt,
            IsActive = false
        },
        new()
        {
            Id = "CARD",
            Name = "Credit / Debit Card",
            Description = "Visa, Mastercard, RuPay & more",
            IconGlyph = FontAwesomeIcons.CreditCard,
            IsActive = false
        },
        new()
        {
            Id = "NETBANKING",
            Name = "Net Banking",
            Description = "All major banks supported",
            IconGlyph = FontAwesomeIcons.University,
            IsActive = false
        },
        new()
        {
            Id = "COD",
            Name = "Cash on Delivery",
            Description = "Pay in cash when your order arrives",
            IconGlyph = FontAwesomeIcons.MoneyBillWave,
            IsActive = true
        }
    ];
}
