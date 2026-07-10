using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Coupons.CreateCoupon;

public sealed class CreateCouponCommandHandler
    : IRequestHandler<CreateCouponCommand, Result<Guid>>
{
    private readonly ICouponRepository _couponRepository;

    public CreateCouponCommandHandler(
        ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public async Task<Result<Guid>> Handle(
        CreateCouponCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await _couponRepository.ExistsByCodeAsync(
            request.Code,
            null,
            cancellationToken);

        if (exists)
        {
            return Result<Guid>.Failure(
                CouponErrors.AlreadyExists);
        }

        var coupon = new Coupon(
            request.Code,
            request.Name,
            request.Description,
            request.DiscountType,
            request.DiscountValue,
            request.MinimumOrderAmount,
            request.MaximumDiscountAmount,
            request.StartsAtUtc,
            request.ExpiresAtUtc,
            request.UsageLimit);

        var added = await _couponRepository.AddOrRestoreAsync(
            coupon,
            cancellationToken);

        if (!added)
        {
            return Result<Guid>.Failure(
                CouponErrors.AlreadyExists);
        }

        await _couponRepository.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(
            coupon.Id,
            "Coupon created successfully.");
    }
}