using Equiparts.Interfaces;
using Equiparts.Models;

namespace Equiparts.Services;

// No backend coupon-validation endpoint exists yet (see ICartApi/IOrderApi), so this
// validates against a small local rule set instead of a Refit client. The interface
// boundary already matches the shape a real API-backed implementation would have, so
// swapping one in later only touches this class, not CheckoutViewModel or the page.
public class CouponService(IConnectivityService connectivityService) : ICouponService
{
    private sealed record CouponRule(decimal? FlatDiscount, double? PercentDiscount, decimal MinOrderAmount, string Description, bool IsExpired);

    private static readonly Dictionary<string, CouponRule> KnownCoupons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SAVE10"] = new CouponRule(null, 10, 0, "10% off on your order", false),
        ["FLAT300"] = new CouponRule(300, null, 1000, "Flat ₹300 off on orders above ₹1,000", false),
        ["WELCOME50"] = new CouponRule(50, null, 0, "New customer offer", true)
    };

    public async Task<CouponValidationResult> ValidateCouponAsync(string code, decimal orderAmount, CancellationToken cancellationToken = default)
    {
        code = code?.Trim() ?? string.Empty;

        await connectivityService.CheckInternetAccessAsync(cancellationToken);

        // Simulated latency so the "applying coupon" loading state has something to show.
        await Task.Delay(500, cancellationToken);

        if (string.IsNullOrWhiteSpace(code) || !KnownCoupons.TryGetValue(code, out var rule))
            return new CouponValidationResult { IsValid = false, ErrorMessage = "Invalid coupon code." };

        if (rule.IsExpired)
            return new CouponValidationResult { IsValid = false, ErrorMessage = "This coupon has expired." };

        if (orderAmount < rule.MinOrderAmount)
            return new CouponValidationResult { IsValid = false, ErrorMessage = $"Add items worth ₹{rule.MinOrderAmount:N0} more to use this coupon." };

        var discount = rule.PercentDiscount.HasValue
            ? Math.Round(orderAmount * (decimal)(rule.PercentDiscount.Value / 100.0), 2)
            : rule.FlatDiscount ?? 0;

        return new CouponValidationResult
        {
            IsValid = true,
            Coupon = new Coupon
            {
                Code = code.ToUpperInvariant(),
                Description = rule.Description,
                DiscountAmount = discount
            }
        };
    }
}
