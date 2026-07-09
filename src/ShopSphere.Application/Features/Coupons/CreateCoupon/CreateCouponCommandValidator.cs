using FluentValidation;
using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Features.Coupons.CreateCoupon;

public sealed class CreateCouponCommandValidator
    : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponCommandValidator()
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