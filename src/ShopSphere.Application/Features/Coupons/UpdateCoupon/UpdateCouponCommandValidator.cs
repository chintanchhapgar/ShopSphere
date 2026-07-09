using FluentValidation;
using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Features.Coupons.UpdateCoupon;

public sealed class UpdateCouponCommandValidator
    : AbstractValidator<UpdateCouponCommand>
{
    public UpdateCouponCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.DiscountValue)
            .GreaterThan(0);

        RuleFor(x => x.MinimumOrderAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.UsageLimit)
            .GreaterThan(0);

        RuleFor(x => x.ExpiresAtUtc)
            .GreaterThan(x => x.StartsAtUtc);

        RuleFor(x => x.MaximumDiscountAmount)
            .GreaterThan(0)
            .When(x =>
                x.DiscountType == DiscountType.Percentage &&
                x.MaximumDiscountAmount.HasValue);
    }
}