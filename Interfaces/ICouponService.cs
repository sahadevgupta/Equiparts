using Equiparts.Models;

namespace Equiparts.Interfaces;

public class CouponValidationResult
{
    public bool IsValid { get; set; }

    public string? ErrorMessage { get; set; }

    public Coupon? Coupon { get; set; }
}

public interface ICouponService
{
    Task<CouponValidationResult> ValidateCouponAsync(string code, decimal orderAmount, CancellationToken cancellationToken = default);
}
