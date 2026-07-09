using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Coupons.DeleteCoupon;

public sealed class DeleteCouponCommandHandler
    : IRequestHandler<DeleteCouponCommand, Result>
{
    private readonly ICouponRepository _couponRepository;

    public DeleteCouponCommandHandler(
        ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public async Task<Result> Handle(
        DeleteCouponCommand request,
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

        _couponRepository.Remove(coupon);

        await _couponRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Coupon deleted successfully.");
    }
}