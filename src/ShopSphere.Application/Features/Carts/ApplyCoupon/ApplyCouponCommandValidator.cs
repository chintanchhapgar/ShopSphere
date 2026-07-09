using FluentValidation;

namespace ShopSphere.Application.Features.Carts.ApplyCoupon;

public sealed class ApplyCouponCommandValidator
    : AbstractValidator<ApplyCouponCommand>
{
    public ApplyCouponCommandValidator()
    {
        RuleFor(x => x.CouponCode)
            .NotEmpty()
            .MaximumLength(50);
    }
}