using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Coupons.UpdateCoupon;

public sealed class UpdateCouponCommandHandler
    : IRequestHandler<UpdateCouponCommand, Result>
{
    private readonly ICouponRepository _couponRepository;

    public UpdateCouponCommandHandler(
        ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public async Task<Result> Handle(
        UpdateCouponCommand request,
        CancellationToken cancellationToken)
    {
        var coupon = await _couponRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (coupon is null)
        {
            return Result.Failure(
                CouponErrors.NotFound);
        }

        var exists = await _couponRepository.ExistsByCodeAsync(
            request.Code,
            request.Id,
            cancellationToken);

        if (exists)
        {
            return Result.Failure(
                CouponErrors.AlreadyExists);
        }

        coupon.Update(
            request.Code,
            request.Name,
            request.Description,
            request.DiscountType,
            request.DiscountValue,
            request.MinimumOrderAmount,
            request.MaximumDiscountAmount,
            request.StartsAtUtc,
            request.ExpiresAtUtc,
            request.UsageLimit,
            request.IsActive);

        await _couponRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Coupon updated successfully.");
    }
}