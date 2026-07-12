using FluentAssertions;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Enums;

namespace ShopSphere.UnitTests.Domain.Coupons;

public class CouponTests
{
    [Fact]
    public void Create_Should_Set_Properties()
    {
        var coupon = CreateCoupon();

        coupon.Code.Should().Be("SAVE10");
        coupon.Name.Should().Be("Summer Sale");
        coupon.UsedCount.Should().Be(0);
        coupon.CanBeUsed().Should().BeTrue();
    }

    private static Coupon CreateCoupon()
    {
        return new Coupon(
            "SAVE10",
            "Summer Sale",
            "10% off",
            DiscountType.Percentage,
            10,
            500,
            100,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(10),
            100);
    }

    [Fact]
    public void CalculateDiscount_Should_Return_Percentage()
    {
        var coupon = CreateCoupon();

        var discount = coupon.CalculateDiscount(1000);

        discount.Should().Be(100);
    }

    [Fact]
    public void CalculateDiscount_Should_Return_FixedAmount()
    {
        var coupon = new Coupon(
            "SAVE100",
            "Flat Discount",
            null,
            DiscountType.FixedAmount,
            100,
            500,
            null,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(10),
            100);

        coupon.CalculateDiscount(1000)
            .Should()
            .Be(100);
    }

    [Fact]
    public void CalculateDiscount_Should_Return_Zero_When_Minimum_Not_Met()
    {
        var coupon = CreateCoupon();

        coupon.CalculateDiscount(400)
            .Should()
            .Be(0);
    }

    [Fact]
    public void CalculateDiscount_Should_Not_Exceed_MaximumDiscount()
    {
        var coupon = CreateCoupon();

        coupon.CalculateDiscount(5000)
            .Should()
            .Be(100);
    }

    [Fact]
    public void IncrementUsage_Should_Increase_Count()
    {
        var coupon = CreateCoupon();

        coupon.IncrementUsage();

        coupon.UsedCount.Should().Be(1);
    }

    [Fact]
    public void CanBeUsed_Should_Return_False_When_UsageLimit_Reached()
    {
        var coupon = new Coupon(
            "SAVE10",
            "Sale",
            null,
            DiscountType.Percentage,
            10,
            0,
            null,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(5),
            1);

        coupon.IncrementUsage();

        coupon.CanBeUsed().Should().BeFalse();
    }

    [Fact]
    public void CanBeUsed_Should_Return_False_When_Expired()
    {
        var coupon = new Coupon(
            "SAVE10",
            "Sale",
            null,
            DiscountType.Percentage,
            10,
            0,
            null,
            DateTime.UtcNow.AddDays(-10),
            DateTime.UtcNow.AddDays(-1),
            100);

        coupon.CanBeUsed().Should().BeFalse();
    }
}