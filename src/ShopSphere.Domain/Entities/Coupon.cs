using ShopSphere.Domain.Enums;

namespace ShopSphere.Domain.Entities;

public sealed class Coupon : AuditableEntity
{
    private Coupon()
    {
    }

    public Coupon(
        string code,
        string name,
        string? description,
        DiscountType discountType,
        decimal discountValue,
        decimal minimumOrderAmount,
        decimal? maximumDiscountAmount,
        DateTime startsAtUtc,
        DateTime expiresAtUtc,
        int usageLimit)
    {
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = description?.Trim();

        DiscountType = discountType;
        DiscountValue = discountValue;

        MinimumOrderAmount = minimumOrderAmount;
        MaximumDiscountAmount = maximumDiscountAmount;

        StartsAtUtc = startsAtUtc;
        ExpiresAtUtc = expiresAtUtc;

        UsageLimit = usageLimit;
    }

    public string Code { get; private set; } = default!;

    public string Name { get; private set; } = default!;

    public string? Description { get; private set; }

    public DiscountType DiscountType { get; private set; }

    public decimal DiscountValue { get; private set; }

    public decimal MinimumOrderAmount { get; private set; }

    public decimal? MaximumDiscountAmount { get; private set; }

    public DateTime StartsAtUtc { get; private set; }

    public DateTime ExpiresAtUtc { get; private set; }

    public int UsageLimit { get; private set; }

    public int UsedCount { get; private set; }

    public bool IsExpired =>
        DateTime.UtcNow > ExpiresAtUtc;

    public bool CanBeUsed() =>
        !IsExpired &&
        UsedCount < UsageLimit;

    public void IncrementUsage()
    {
        if (!CanBeUsed())
        {
            throw new InvalidOperationException(
                "Coupon usage limit reached.");
        }

        UsedCount++;
    }

    public decimal CalculateDiscount(
        decimal orderAmount)
    {
        if (orderAmount < MinimumOrderAmount)
        {
            return 0;
        }

        decimal discount =
            DiscountType == DiscountType.Percentage
                ? orderAmount * DiscountValue / 100m
                : DiscountValue;

        if (MaximumDiscountAmount.HasValue)
        {
            discount = Math.Min(
                discount,
                MaximumDiscountAmount.Value);
        }

        return Math.Min(
            discount,
            orderAmount);
    }

    public void Update(
    string code,
    string name,
    string? description,
    DiscountType discountType,
    decimal discountValue,
    decimal minimumOrderAmount,
    decimal? maximumDiscountAmount,
    DateTime startsAtUtc,
    DateTime expiresAtUtc,
    int usageLimit,
    bool isActive)
    {
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = description?.Trim();

        DiscountType = discountType;
        DiscountValue = discountValue;

        MinimumOrderAmount = minimumOrderAmount;
        MaximumDiscountAmount = maximumDiscountAmount;

        StartsAtUtc = startsAtUtc;
        ExpiresAtUtc = expiresAtUtc;

        UsageLimit = usageLimit;
        IsActive = isActive;
    }
}